using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using GenericParsing;
using Shun;

namespace Zeus
{
    /// <summary>
    /// Class for products to be used in the inventory and point of sale system
    /// </summary>
    public class RetailInventory: IInventory, ISqLDataBase
    {

        #region Fields
        private DataTable _dictOfData;
        private string _filePath;
        public static IInventory _inventory = null;
        public static ISystemConfiguration _systemConfig = null;
        public EnumerableRowCollection<DataRow> allFields { get; set; }

        public List<string> _dbColumns = new List<string>()
        {
            "Id",
            "Codigo",
            "CodigoAlterno",
            "ProveedorProductoId",
            "Descripcion",
            "Proveedor",
            "Categoria",
            "UltimoPedidoFecha",
            "Costo",
            "CostoMoneda",
            "Precio",
            "PrecioMoneda",
            "CantidadInternoHistorial",
            "CantidadVendidoHistorial",
            "VendidoHistorial",
            "CantidadLocal",
            "CantidadDisponibleTotal",
            "CantidadMinima",
            "UltimaTransaccionFecha",
            "Imagen"
        };

        #endregion

        #region Properties

        #region ISqlDatabase Properties
        public string Server { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string SqlDataBase { get; set; }
        public string Table { get; set; }
        public MySqlDatabase MySqlData { get; set; }
        #endregion

        /// <summary>
        /// Contains system configuration to customize the funcionality
        /// </summary>
        public ISystemConfiguration SystemConfig
        {
            get { return _systemConfig; }
            set { _systemConfig = value; }
        }

        public List<string> DbColumns
        {
            get { return _dbColumns; }
            set { _dbColumns = value; }
        }

        public DataTable DictOfData
        {
            get { return _dictOfData; }
            set { _dictOfData = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        #endregion

        #region Constructors
        //Singleton pattern
        protected RetailInventory(string filePath, MySqlDatabase mySqlDb, ISystemConfiguration systemConfig)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            //Read inventory CSV format
            FilePath = filePath;
            SystemConfig = systemConfig;
            //LoadCsvToDataTable();
            if (mySqlDb != null)
            {
                MySqlData = mySqlDb;
            }
            else
            {
                LoadCsvToDataTable();
            }
        }
        public static IInventory GetInstance(string filePath, MySqlDatabase mySqlDb, ISystemConfiguration systemConfig)
        {
            if (_inventory == null)
                _inventory = new RetailInventory(filePath, mySqlDb, systemConfig);
            return _inventory;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add new product to inventory datatable
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool AddNewProductToTable(IProduct product)
        {
            long id = 0;
            if (product is RetailItem item)
            {              
                //Add product to inventory database
                if (MySqlData != null && SystemConfig.CloudInventory)
                {
                    var productColValPairs = new List<Tuple<string, string>>()
                    {
                        //0 is Id
                        new Tuple<string, string>(DbColumns[1], item.Code),
                        new Tuple<string, string>(DbColumns[2], item.AlternativeCode),
                        new Tuple<string, string>(DbColumns[3], item.ProviderProductId),
                        new Tuple<string, string>(DbColumns[4], item.Description),
                        new Tuple<string, string>(DbColumns[5], item.Provider),
                        new Tuple<string, string>(DbColumns[6], item.Category),
                        new Tuple<string, string>(DbColumns[7], item.LastPurchaseDateString),
                        new Tuple<string, string>(DbColumns[8], item.Cost.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[9], item.CostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[10], item.Price.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[11], item.PriceCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[12], item.InternalQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[13], item.QuantitySold.ToString()),
                        new Tuple<string, string>(DbColumns[14], item.AmountSold.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[15], item.LocalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[16], item.TotalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[17], item.MinimumStockQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[18], item.LastSaleDateString),
                        new Tuple<string, string>(DbColumns[19], item.ImageName)
                    };
                    MySqlData.Insert(productColValPairs);
                    //Get Id for local database
                    MySqlData.Read("Codigo", item.Code, out var readData);
                    id = long.Parse(readData[0].Item2[0].Item2);
                }
                //Add product to datatable 
                if (!SystemConfig.LocalInventory) return true;

                if (id != 0) item.Id = Int32.Parse(id.ToString());

                DictOfData.Rows.Add();
                var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
                row["Id"] = item.Id.ToString();
                row["Codigo"] = item.Code;
                row["CodigoAlterno"] = item.AlternativeCode;
                row["ProveedorProductoId"] = item.ProviderProductId;
                row["Descripcion"] = item.Description;
                row["Proveedor"] = item.Provider;
                row["Categoria"] = item.Category;
                row["UltimoPedidoFecha"] = item.LastPurchaseDate.ToString();
                row["Costo"] = item.Cost.ToString(CultureInfo.InvariantCulture);
                row["CostoMoneda"] = item.CostCurrency;
                row["Precio"] = item.Price.ToString();
                row["PrecioMoneda"] = item.PriceCurrency;
                row["CantidadInternoHistorial"] = item.InternalQuantity.ToString();
                row["CantidadVendidoHistorial"] = item.QuantitySold.ToString();
                row["VendidoHistorial"] = item.AmountSold.ToString(CultureInfo.InvariantCulture);
                row["CantidadLocal"] = item.LocalQuantityAvailable.ToString();
                row["CantidadDisponibleTotal"] = item.TotalQuantityAvailable.ToString();
                row["CantidadMinima"] = item.MinimumStockQuantity.ToString();
                row["UltimaTransaccionFecha"] = item.LastSaleDate.ToString();
                row["Imagen"] = item.ImageName;
                return true;
            }
            else
            {
                return false;
            }

        }

        public void DeleteItemInDataTable(string inputSearch, string columnName)
        {
            if (MySqlData != null && SystemConfig.CloudInventory)
            {
                MySqlData.Delete(columnName, inputSearch);
            }

            if (!SystemConfig.LocalInventory) return;

            for (int index = 0; index < DictOfData.Rows.Count; index++)
            {
                var row = DictOfData.Rows[index];
                if (row[columnName].ToString() == inputSearch)
                {
                    DictOfData.Rows[index].Delete();
                    return;
                }
            }
        }

        public int GetLastItemNumber()
        {
            ///TODO: need to implement for DB
            if (DictOfData.Rows.Count == 0)
                return 0;
            var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
            return Int32.Parse(row["Id"].ToString());
        }

        public IProduct GetProduct(string code)
        {
            try
            {
                if (MySqlData != null && SystemConfig.CloudInventory)
                {
                    MySqlData.Read("Codigo", code, out var foundData);
                    if (foundData.Count < 1)
                    {
                        return new ProductBase() { Description = "", Category = "", Cost = 0M };
                    }
                    else
                    {
                        var data = foundData.First().Item2;
                        return new RetailItem()
                        {
                            Id = Int32.Parse(foundData.First().Item2[0].Item2),
                            Code = foundData.First().Item2[1].Item2,
                            AlternativeCode = foundData.First().Item2[2].Item2,
                            ProviderProductId = foundData.First().Item2[3].Item2,
                            Description = foundData.First().Item2[4].Item2,
                            Provider = foundData.First().Item2[5].Item2,
                            Category = foundData.First().Item2[6].Item2,
                            LastPurchaseDate = Convert.ToDateTime(foundData.First().Item2[7].Item2),
                            Cost = Decimal.Parse(foundData.First().Item2[8].Item2),
                            CostCurrency = foundData.First().Item2[9].Item2 == "USD"
                                    ? CurrencyTypeEnum.USD
                                    : CurrencyTypeEnum.MXN,
                            Price = decimal.Parse(foundData.First().Item2[10].Item2),
                            PriceCurrency = foundData.First().Item2[11].Item2 == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN,
                            InternalQuantity = Int32.Parse(foundData.First().Item2[12].Item2),
                            QuantitySold = Int32.Parse(foundData.First().Item2[13].Item2),
                            AmountSold = decimal.Parse(foundData.First().Item2[14].Item2),
                            LocalQuantityAvailable = Int32.Parse(foundData.First().Item2[15].Item2),
                            TotalQuantityAvailable = Int32.Parse(foundData.First().Item2[16].Item2),
                            MinimumStockQuantity = Int32.Parse(foundData.First().Item2[17].Item2),
                            LastSaleDate = Convert.ToDateTime(foundData.First().Item2[18].Item2),
                            ImageName = foundData.First().Item2[19].Item2
                        };
                    }
                }

                if (SystemConfig.LocalInventory)
                {
                    for (int index = 0; index < DictOfData.Rows.Count; index++)
                    {
                        var row = DictOfData.Rows[index];
                        if (row["Codigo"].ToString() == code)
                        {
                            return new RetailItem()
                            {
                                Id = Int32.Parse(row["Id"].ToString()),
                                Code = row["Codigo"].ToString(),
                                AlternativeCode = row["CodigoAlterno"].ToString(),
                                ProviderProductId = row["ProveedorProductoId"].ToString(),
                                Description = row["Descripcion"].ToString(),
                                Provider = row["Proveedor"].ToString(),
                                Category = row["Categoria"].ToString(),
                                LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                                Cost = Decimal.Parse(row["Costo"].ToString()),
                                CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN,
                                Price = decimal.Parse(row["Precio"].ToString()),
                                PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN,
                                InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                                QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                                AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                                LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                                TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                                MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                                LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                                ImageName = row["Imagen"].ToString(),
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error en el Codigo", "Error");
            }

            return new ProductBase() { Description = "", Category = "", Cost = 0M };
        }

        ///TODO: Remove InventoryBase
        public List<IProduct> GetProductList(string filePath, out string listName)
        {
            var productList = new List<IProduct>();

            if (RetailInventory._inventory != null)
            {
                //Get codes from product lists
                var list = CategoryCatalog.GetList(filePath);
                var codeList = new List<string>();
                //Skip first line, which is title of the list
                for (int i = 1; i < list.Count; ++i)
                {
                    codeList.Add(list[i]);
                   // productList.Add(RetailInventory._inventory.GetProduct(list[i]));
                }

                //New section
                productList = this.Search(codeList);
                //End of new section
                
                listName = list.First();
                return productList;
            }
            else
            {
                listName = "Lista";
                return productList;
            }
        }

        public void LoadCsvToDataTable()
        {
            using (var parser = new GenericParserAdapter(FilePath))
            {
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = true;
                parser.SkipStartingDataRows = 0;
                parser.SkipEmptyRows = true;
                parser.MaxBufferSize = 4096;
                parser.MaxRows = 8000;

                DictOfData = parser.GetDataTable();
            }
        }

        public void SaveDataTableToCsv()
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = DictOfData.Columns.Cast<DataColumn>().
                Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in DictOfData.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            File.WriteAllText(FilePath, sb.ToString());
        }

        public List<IProduct> Search(string input, bool updateFromDataBase = true)
        {
            var products = new List<IProduct>();

            //Return empty list if invalid inputs are entered for the search
            if (string.IsNullOrWhiteSpace(input) || input == "x")
                return products;

            if (MySqlData != null && SystemConfig.CloudInventory)
            {
                if(updateFromDataBase || allFields == null) allFields = MySqlData.Select(DbColumns).AsEnumerable();
                
                if (input == "*")
                {
                    var allProducts = allFields;
                    foreach (var row in allProducts)
                    {

                        var product = new RetailItem()
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Code = row["Codigo"].ToString(),
                            AlternativeCode = row["CodigoAlterno"].ToString(),
                            ProviderProductId = row["ProveedorProductoId"].ToString(),
                            Description = row["Descripcion"].ToString(),
                            Provider = row["Proveedor"].ToString(),
                            Category = row["Categoria"].ToString(),
//                          LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                            Cost = Decimal.Parse(row["Costo"].ToString()),
                            Price = decimal.Parse(row["Precio"].ToString()),
                            InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                            QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                            AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                            LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                            TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                            MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
//                          LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                            ImageName = row["Imagen"].ToString(),
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        products.Add(product);
                    }
                }

                var descriptionFilter = allFields.Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                var codeFilter = allFields.Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                foreach (var row in codeFilter)
                {
                    var product = new RetailItem()
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Code = row["Codigo"].ToString(),
                        AlternativeCode = row["CodigoAlterno"].ToString(),
                        ProviderProductId = row["ProveedorProductoId"].ToString(),
                        Description = row["Descripcion"].ToString(),
                        Provider = row["Proveedor"].ToString(),
                        Category = row["Categoria"].ToString(),
//                        LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                        Cost = Decimal.Parse(row["Costo"].ToString()),
                        Price = decimal.Parse(row["Precio"].ToString()),
                        InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                        QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                        AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                        LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                        TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                        MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
   //                     LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                        ImageName = row["Imagen"].ToString(),
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    products.Add(product);
                }

                foreach (var row in descriptionFilter)
                {
                    var product = new RetailItem()
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Code = row["Codigo"].ToString(),
                        AlternativeCode = row["CodigoAlterno"].ToString(),
                        ProviderProductId = row["ProveedorProductoId"].ToString(),
                        Description = row["Descripcion"].ToString(),
                        Provider = row["Proveedor"].ToString(),
                        Category = row["Categoria"].ToString(),
 //                       LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                        Cost = Decimal.Parse(row["Costo"].ToString()),
                        Price = decimal.Parse(row["Precio"].ToString()),
                        InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                        QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                        AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                        LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                        TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                        MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
   //                     LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                        ImageName = row["Imagen"].ToString(),
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    //Add if it does not exist already
                    if (!products.Exists(x => x.Code == product.Code))
                        products.Add(product);
                }

                return products;
            }
            else
            {
                if (input == "*")
                {
                    var allProducts = DictOfData.AsEnumerable();
                    foreach (var row in allProducts)
                    {

                        var product = new RetailItem()
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Code = row["Codigo"].ToString(),
                            AlternativeCode = row["CodigoAlterno"].ToString(),
                            ProviderProductId = row["ProveedorProductoId"].ToString(),
                            Description = row["Descripcion"].ToString(),
                            Provider = row["Proveedor"].ToString(),
                            Category = row["Categoria"].ToString(),
                            LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                            Cost = Decimal.Parse(row["Costo"].ToString()),
                            Price = decimal.Parse(row["Precio"].ToString()),
                            InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                            QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                            AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                            LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                            TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                            MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                            LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                            ImageName = row["Imagen"].ToString(),
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;

                        products.Add(product);
                    }
                }

                var descriptionFilter = DictOfData.AsEnumerable()
                    .Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                var codeFilter = DictOfData.AsEnumerable()
                    .Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                foreach (var row in codeFilter)
                {
                    var product = new RetailItem()
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Code = row["Codigo"].ToString(),
                        AlternativeCode = row["CodigoAlterno"].ToString(),
                        ProviderProductId = row["ProveedorProductoId"].ToString(),
                        Description = row["Descripcion"].ToString(),
                        Provider = row["Proveedor"].ToString(),
                        Category = row["Categoria"].ToString(),
                        LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                        Cost = Decimal.Parse(row["Costo"].ToString()),
                        Price = decimal.Parse(row["Precio"].ToString()),
                        InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                        QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                        AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                        LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                        TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                        MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                        LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                        ImageName = row["Imagen"].ToString(),
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
 
                    products.Add(product);
                }

                foreach (var row in descriptionFilter)
                {
                    var product = new RetailItem()
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Code = row["Codigo"].ToString(),
                        AlternativeCode = row["CodigoAlterno"].ToString(),
                        ProviderProductId = row["ProveedorProductoId"].ToString(),
                        Description = row["Descripcion"].ToString(),
                        Provider = row["Proveedor"].ToString(),
                        Category = row["Categoria"].ToString(),
                        LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                        Cost = Decimal.Parse(row["Costo"].ToString()),
                        Price = decimal.Parse(row["Precio"].ToString()),
                        InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                        QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                        AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                        LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                        TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                        MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                        LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                        ImageName = row["Imagen"].ToString(),
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    //Add if it does not exist already
                    if (!products.Exists(x => x.Code == product.Code))
                        products.Add(product);
                }

                return products;
            }
        }

        public List<IProduct> Search(List<string> inputs, bool updateFromDataBase = true)
        {
            var products = new List<IProduct>();
            
            if (MySqlData != null && SystemConfig.CloudInventory)
            {
                if (updateFromDataBase || allFields == null) allFields = MySqlData.Select(DbColumns).AsEnumerable();

                foreach (var item in inputs)
                {
                    var input = item.ToLower();
                    //Return empty list if invalid inputs are entered for the search
                    if (string.IsNullOrWhiteSpace(input) || input == "x") continue;

                    if (input == "*")
                    {
                        var allProducts = allFields;
                        foreach (var row in allProducts)
                        {
                            var product = new RetailItem()
                            {
                                Id = Int32.Parse(row["Id"].ToString()),
                                Code = row["Codigo"].ToString(),
                                AlternativeCode = row["CodigoAlterno"].ToString(),
                                ProviderProductId = row["ProveedorProductoId"].ToString(),
                                Description = row["Descripcion"].ToString(),
                                Provider = row["Proveedor"].ToString(),
                                Category = row["Categoria"].ToString(),
                                //                          LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                                Cost = Decimal.Parse(row["Costo"].ToString()),
                                Price = decimal.Parse(row["Precio"].ToString()),
                                InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                                QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                                AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                                LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                                TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                                MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                                //                          LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                                ImageName = row["Imagen"].ToString(),
                            };

                            product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN;
                            product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN;
                            products.Add(product);
                        }
                    }

                    var descriptionFilter = allFields.Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                    var codeFilter = allFields.Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                    foreach (var row in codeFilter)
                    {
                        var product = new RetailItem()
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Code = row["Codigo"].ToString(),
                            AlternativeCode = row["CodigoAlterno"].ToString(),
                            ProviderProductId = row["ProveedorProductoId"].ToString(),
                            Description = row["Descripcion"].ToString(),
                            Provider = row["Proveedor"].ToString(),
                            Category = row["Categoria"].ToString(),
                            //                        LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                            Cost = Decimal.Parse(row["Costo"].ToString()),
                            Price = decimal.Parse(row["Precio"].ToString()),
                            InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                            QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                            AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                            LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                            TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                            MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                            //                     LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                            ImageName = row["Imagen"].ToString(),
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;

                        products.Add(product);
                    }

                    foreach (var row in descriptionFilter)
                    {
                        var product = new RetailItem()
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Code = row["Codigo"].ToString(),
                            AlternativeCode = row["CodigoAlterno"].ToString(),
                            ProviderProductId = row["ProveedorProductoId"].ToString(),
                            Description = row["Descripcion"].ToString(),
                            Provider = row["Proveedor"].ToString(),
                            Category = row["Categoria"].ToString(),
                            //                       LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                            Cost = Decimal.Parse(row["Costo"].ToString()),
                            Price = decimal.Parse(row["Precio"].ToString()),
                            InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                            QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                            AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                            LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                            TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                            MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                            //                     LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                            ImageName = row["Imagen"].ToString(),
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;

                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == product.Code))
                            products.Add(product);
                    }
                }

                return products;
            }
            else
            {
                foreach (var item in inputs)
                {
                    var input = item.ToLower();
                    //Return empty list if invalid inputs are entered for the search
                    if (string.IsNullOrWhiteSpace(input) || input == "x") continue;

                    if (input == "*")
                    {
                        var allProducts = DictOfData.AsEnumerable();
                        foreach (var row in allProducts)
                        {

                            var product = new RetailItem()
                            {
                                Id = Int32.Parse(row["Id"].ToString()),
                                Code = row["Codigo"].ToString(),
                                AlternativeCode = row["CodigoAlterno"].ToString(),
                                ProviderProductId = row["ProveedorProductoId"].ToString(),
                                Description = row["Descripcion"].ToString(),
                                Provider = row["Proveedor"].ToString(),
                                Category = row["Categoria"].ToString(),
                                LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                                Cost = Decimal.Parse(row["Costo"].ToString()),
                                Price = decimal.Parse(row["Precio"].ToString()),
                                InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                                QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                                AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                                LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                                TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                                MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                                LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                                ImageName = row["Imagen"].ToString(),
                            };

                            product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN;
                            product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN;

                            products.Add(product);
                        }
                    }

                    var descriptionFilter = DictOfData.AsEnumerable()
                        .Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                    var codeFilter = DictOfData.AsEnumerable()
                        .Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                    foreach (var row in codeFilter)
                    {
                        var product = new RetailItem()
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Code = row["Codigo"].ToString(),
                            AlternativeCode = row["CodigoAlterno"].ToString(),
                            ProviderProductId = row["ProveedorProductoId"].ToString(),
                            Description = row["Descripcion"].ToString(),
                            Provider = row["Proveedor"].ToString(),
                            Category = row["Categoria"].ToString(),
                            LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                            Cost = Decimal.Parse(row["Costo"].ToString()),
                            Price = decimal.Parse(row["Precio"].ToString()),
                            InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                            QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                            AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                            LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                            TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                            MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                            LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                            ImageName = row["Imagen"].ToString(),
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;

                        products.Add(product);
                    }

                    foreach (var row in descriptionFilter)
                    {
                        var product = new RetailItem()
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Code = row["Codigo"].ToString(),
                            AlternativeCode = row["CodigoAlterno"].ToString(),
                            ProviderProductId = row["ProveedorProductoId"].ToString(),
                            Description = row["Descripcion"].ToString(),
                            Provider = row["Proveedor"].ToString(),
                            Category = row["Categoria"].ToString(),
                            LastPurchaseDate = Convert.ToDateTime(row["UltimoPedidoFecha"].ToString()),
                            Cost = Decimal.Parse(row["Costo"].ToString()),
                            Price = decimal.Parse(row["Precio"].ToString()),
                            InternalQuantity = Int32.Parse(row["CantidadInternoHistorial"].ToString()),
                            QuantitySold = Int32.Parse(row["CantidadVendidoHistorial"].ToString()),
                            AmountSold = decimal.Parse(row["VendidoHistorial"].ToString()),
                            LocalQuantityAvailable = Int32.Parse(row["CantidadLocal"].ToString()),
                            TotalQuantityAvailable = Int32.Parse(row["CantidadDisponibleTotal"].ToString()),
                            MinimumStockQuantity = Int32.Parse(row["CantidadMinima"].ToString()),
                            LastSaleDate = Convert.ToDateTime(row["UltimaTransaccionFecha"].ToString()),
                            ImageName = row["Imagen"].ToString(),
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;

                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == product.Code))
                            products.Add(product);
                    }
                }
                return products;
            }
        }

        public bool UpdateProductToTable(IProduct product)
        {
            if (product is RetailItem item)
            {
                if (MySqlData != null)
                {
                    var data = new List<Tuple<string, string>>()
                    {
                        new Tuple<string, string>(DbColumns[1], item.Code),
                        new Tuple<string, string>(DbColumns[2], item.AlternativeCode),
                        new Tuple<string, string>(DbColumns[3], item.ProviderProductId),
                        new Tuple<string, string>(DbColumns[4], item.Description),
                        new Tuple<string, string>(DbColumns[5], item.Provider),
                        new Tuple<string, string>(DbColumns[6], item.Category),
                        new Tuple<string, string>(DbColumns[7], item.LastPurchaseDateString),
                        new Tuple<string, string>(DbColumns[8], item.Cost.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[9], item.CostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[10], item.Price.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[11], item.PriceCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[12], item.InternalQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[13], item.QuantitySold.ToString()),
                        new Tuple<string, string>(DbColumns[14], item.AmountSold.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[15], item.LocalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[16], item.TotalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[17], item.MinimumStockQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[18], item.LastSaleDateString),
                        new Tuple<string, string>(DbColumns[19], item.ImageName)
                    };    
                    MySqlData.Update("Codigo", product.Code, data);
                }

                if (!SystemConfig.LocalInventory) return false;

                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == item.Code)
                    {
                        row["Id"] = item.Id.ToString();
                        row["Codigo"] = item.Code;
                        row["CodigoAlterno"] = item.AlternativeCode;
                        row["ProveedorProductoId"] = item.ProviderProductId;
                        row["Descripcion"] = item.Description;
                        row["Proveedor"] = item.Provider;
                        row["Categoria"] = item.Category;
                        row["UltimoPedidoFecha"] = item.LastPurchaseDate.ToString();
                        row["Costo"] = item.Cost.ToString(CultureInfo.InvariantCulture);
                        row["CostoMoneda"] = item.CostCurrency;
                        row["Precio"] = item.Price.ToString();
                        row["PrecioMoneda"] = item.PriceCurrency;
                        row["CantidadInternoHistorial"] = item.InternalQuantity.ToString();
                        row["CantidadVendidoHistorial"] = item.QuantitySold.ToString();
                        row["VendidoHistorial"] = item.AmountSold.ToString(CultureInfo.InvariantCulture);
                        row["CantidadLocal"] = item.LocalQuantityAvailable.ToString();
                        row["CantidadDisponibleTotal"] = item.TotalQuantityAvailable.ToString();
                        row["CantidadMinima"] = item.MinimumStockQuantity.ToString();
                        row["UltimaTransaccionFecha"] = item.LastSaleDate.ToString();
                        row["Imagen"] = item.ImageName;
                    }
                }

                return true;
            }

            return false;
        }
    }
    #endregion
}

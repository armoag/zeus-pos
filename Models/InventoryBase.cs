using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zeus;
using GenericParsing;
using Shun;

namespace Zeus
{
    public class InventoryBase : IInventory, ISqLDataBase
    {
        #region Fields

        public static InventoryBase _inventory = null;
        public static ISystemConfiguration _systemConfig = null;

        DataTable _dictofdata;
        private string _filePath;

        #endregion

        #region Properties
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
            get { return _dictofdata; }
            set { _dictofdata = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        #endregion

        #region Constructors
        //Singleton pattern
        protected InventoryBase(string filePath, MySqlDatabase mySqlDb, ISystemConfiguration systemConfig)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            //Read inventory CSV format
            _filePath = filePath;
            SystemConfig = systemConfig;
            LoadCsvToDataTable();
            if (mySqlDb != null)
            {
                MySqlData = mySqlDb;
            }
        }

        public static IInventory GetInstance(string filePath, MySqlDatabase mySqlDb, ISystemConfiguration systemConfig)
        {
            if (_inventory == null)
                _inventory = new InventoryBase(filePath, mySqlDb, systemConfig);
            return _inventory;
        }

        public InventoryBase()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Load CSV database into a datatable object
        /// </summary>
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

        /// <summary>
        /// Save datatable object in a CSV file
        /// </summary>
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
        
        /// <summary>
        /// Get the last item number in the inventory
        /// </summary>
        /// <returns></returns>
        public int GetLastItemNumber()
        {
            if (SystemConfig.LocalInventory)
            {
                if (DictOfData.Rows.Count == 0)
                    return 0;
                var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
                return Int32.Parse(row["Id"].ToString());
            }

            return -1;
        }

        /// <summary>
        /// Removes a full entry in the inventory
        /// </summary>
        /// <param name="inputSearch"></param>
        /// <param name="columnName"></param>
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

        /// <summary>
        /// Get product based on a code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual IProduct GetProduct(string code)
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
                        return new ProductBase()
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


                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == code)
                    {
                        return new ProductBase()
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
                            ImageName = row["Imagen"].ToString()                        
                        };
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error en el Codigo", "Error");

            }

            return new ProductBase() { Description = "", Category = "", Cost = 0M };
        }

        /// <summary>
        /// Update product in the datatable
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual bool UpdateProductToTable(IProduct product)
        {
            if (product is ProductBase productBase)
            {
                if (MySqlData != null && SystemConfig.CloudInventory)
                {
                    var data = new List<Tuple<string, string>>()
                    {

                        new Tuple<string, string>(DbColumns[1], productBase.Code),
                        new Tuple<string, string>(DbColumns[2], productBase.AlternativeCode),
                        new Tuple<string, string>(DbColumns[3], productBase.ProviderProductId),
                        new Tuple<string, string>(DbColumns[4], productBase.Code),
                        new Tuple<string, string>(DbColumns[5], productBase.Description),
                        new Tuple<string, string>(DbColumns[6], productBase.Provider),
                        new Tuple<string, string>(DbColumns[7], productBase.Category),
                        new Tuple<string, string>(DbColumns[8], productBase.LastPurchaseDateString),
                        new Tuple<string, string>(DbColumns[9], productBase.Cost.ToString()),
                        new Tuple<string, string>(DbColumns[10], productBase.CostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[11], productBase.Price.ToString()),
                        new Tuple<string, string>(DbColumns[12], productBase.PriceCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[13], productBase.InternalQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[14], productBase.AmountSold.ToString()),
                        new Tuple<string, string>(DbColumns[15], productBase.LocalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[16], productBase.TotalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[17], productBase.MinimumStockQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[18], productBase.LastSaleDateString),
                        new Tuple<string, string>(DbColumns[19], productBase.ImageName)
                    };
                    MySqlData.Update("Codigo", product.Code, data);
                }

                if (!SystemConfig.LocalInventory) return false;

                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == product.Code)
                    {
                        row["Id"] = product.Id.ToString();
                        row["CodigoAlterno"] = product.AlternativeCode;
                        row["ProveedorProductoId"] = product.ProviderProductId;
                        row["Descripcion"] = product.Description;
                        row["Proveedor"] = product.Provider;
                        row["Categoria"] = product.Category;
                        row["Costo"] = product.Cost.ToString(CultureInfo.InvariantCulture);
                        row["CostoMoneda"] = product.CostCurrency;
                        row["Precio"] = product.Price.ToString();
                        row["PrecioMoneda"] = product.PriceCurrency.ToString();
                        row["CantidadInternoHistorial"] = product.InternalQuantity.ToString();
                        row["CantidadVendidoHistorial"] = product.QuantitySold.ToString();
                        row["CantidadLocal"] = product.LocalQuantityAvailable.ToString();
                        row["VendidoHistorial"] = product.AmountSold.ToString();
                        row["CantidadDisponibleTotal"] = product.TotalQuantityAvailable.ToString();
                        row["CantidadMinima"] = product.MinimumStockQuantity.ToString();
                        row["UltimoPedidoFecha"] = product.LastPurchaseDate.ToString();
                        row["UltimaTransaccionFecha"] = product.LastSaleDate.ToString();
                        row["Imagen"] = product.ImageName;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Add new product to data table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual bool AddNewProductToTable(IProduct product)
        {
            long id = 0;
            if (product is ProductBase)
            {
                //Add product to inventory database
                if (MySqlData != null)
                {
                    var productColValPairs = new List<Tuple<string, string>>()
                    {
                        new Tuple<string, string>(DbColumns[1], product.Code),
                        new Tuple<string, string>(DbColumns[2], product.AlternativeCode),
                        new Tuple<string, string>(DbColumns[3], product.ProviderProductId),
                        new Tuple<string, string>(DbColumns[4], product.Description),
                        new Tuple<string, string>(DbColumns[5], product.Category),
                        new Tuple<string, string>(DbColumns[6], product.LastPurchaseDateString),
                        new Tuple<string, string>(DbColumns[7], product.Cost.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[8], product.CostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[9], product.Price.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[10], product.PriceCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[11], product.InternalQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[12], product.QuantitySold.ToString()),
                        new Tuple<string, string>(DbColumns[13], product.AmountSold.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[14], product.LocalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[15], product.TotalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[16], product.MinimumStockQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[17], product.LastSaleDateString),
                        new Tuple<string, string>(DbColumns[18], product.ImageName)
                    };
                    MySqlData.Insert(productColValPairs);
                    //Get Id for local database
                    MySqlData.Read("Codigo", product.Code, out var readData);
                    id = long.Parse(readData[0].Item2[0].Item2);
                }

                //Add product to datatable 
                if (SystemConfig.LocalInventory)
                {
                    DictOfData.Rows.Add();
                    var row = _dictofdata.Rows[_dictofdata.Rows.Count - 1];
                    row["Id"] = product.Id.ToString();
                    row["Codigo"] = product.Code;
                    row["CodigoAlterno"] = product.AlternativeCode;
                    row["ProveedorProductoId"] = product.ProviderProductId;
                    row["Descripcion"] = product.Description;
                    row["Proveedor"] = product.Provider;
                    row["Categoria"] = product.Category;
                    row["Costo"] = product.Cost.ToString(CultureInfo.InvariantCulture);
                    row["CostoMoneda"] = product.CostCurrency;
                    row["Precio"] = product.Price.ToString();
                    row["PrecioMoneda"] = product.PriceCurrency;
                    row["CantidadInternoHistorial"] = product.InternalQuantity.ToString();
                    row["CantidadVendidoHistorial"] = product.QuantitySold.ToString();
                    row["CantidadLocal"] = product.LocalQuantityAvailable.ToString();
                    row["CantidadDisponibleTotal"] = product.TotalQuantityAvailable.ToString();
                    row["VendidoHistorial"] = product.AmountSold.ToString();
                    row["CantidadMinima"] = product.MinimumStockQuantity.ToString();
                    row["UltimoPedidoFecha"] = product.LastPurchaseDate.ToString();
                    row["UltimaTransaccionFecha"] = product.LastSaleDate.ToString();
                    row["Imagen"] = product.ImageName;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Search and returns a list of products
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual List<IProduct> Search(string input, bool updateFromDataBase = true)
        {
            var products = new List<IProduct>();

            //Return empty list if invalid inputs are entered for the search
            if (string.IsNullOrWhiteSpace(input) || input == "x")
                return products;

            if (MySqlData != null && SystemConfig.CloudInventory)
            {
                if (updateFromDataBase || allFields == null) allFields = MySqlData.Select(_dbColumns).AsEnumerable();

                if (input == "*")
                {
                    var allProducts = allFields;
                    foreach (var row in allProducts)
                    {

                        var product = new ProductBase()
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
                            ImageName = row["Imagen"].ToString()
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
                    var product = new ProductBase()
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
                        ImageName = row["Imagen"].ToString()
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
                    var product = new ProductBase()
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
                        ImageName = row["Imagen"].ToString()
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
            else
            {
                if (input == "*")
                {
                    var allProducts = DictOfData.AsEnumerable();
                    foreach (var row in allProducts)
                    {
                        var product = new ProductBase()
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
                            ImageName = row["Imagen"].ToString()
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                        products.Add(product);
                    }
                }
                return products;
            }

            var descriptionFilterLocal = DictOfData.AsEnumerable().Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
            var codeFilterLocal = DictOfData.AsEnumerable().Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

            foreach (var row in codeFilterLocal)
            {
                var product = new ProductBase()
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
                    ImageName = row["Imagen"].ToString()
                };

                product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                products.Add(product);
            }

            foreach(var row in descriptionFilterLocal)
            {
                var product = new ProductBase()
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
                    ImageName = row["Imagen"].ToString()
                };

                product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                //Add if it does not exist already
                if (!products.Exists(x => x.Code == product.Code))
                    products.Add(product);
            }

            return products;
        }

        public virtual List<IProduct> Search(List<string> inputs, bool updateFromDataBase = true)
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

                            var product = new ProductBase()
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
                                ImageName = row["Imagen"].ToString()
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

                    var descriptionFilter =
                        allFields.Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                    var codeFilter = allFields.Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                    foreach (var row in codeFilter)
                    {
                        var product = new ProductBase()
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
                            ImageName = row["Imagen"].ToString()
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
                        var product = new ProductBase()
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
                            ImageName = row["Imagen"].ToString()
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

                    if (input == "*")
                    {
                        var allProducts = DictOfData.AsEnumerable();
                        foreach (var row in allProducts)
                        {
                            var product = new ProductBase()
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
                                ImageName = row["Imagen"].ToString()
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

                    var descriptionFilterLocal = DictOfData.AsEnumerable()
                        .Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                    var codeFilterLocal = DictOfData.AsEnumerable()
                        .Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                    foreach (var row in codeFilterLocal)
                    {
                        var product = new ProductBase()
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
                            ImageName = row["Imagen"].ToString()
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;

                        products.Add(product);
                    }

                    foreach (var row in descriptionFilterLocal)
                    {
                        var product = new ProductBase()
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
                            ImageName = row["Imagen"].ToString()
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

        public List<IProduct> GetProductList(string filePath, out string listName)
        {
            var productList = new List<IProduct>();

            if (InventoryBase._inventory != null)
            {
                //Get codes from product lists
                var list = CategoryCatalog.GetList(filePath);
                //Skip first line, which is title of the list
                for (int i = 1; i < list.Count; ++i)
                {
                    productList.Add(InventoryBase._inventory.GetProduct(list[i]));
                }
                listName = list.First();
                return productList;
            }
            else
            {
                listName = "Lista";
                return productList;
            }
        }

        #endregion  
    }
}

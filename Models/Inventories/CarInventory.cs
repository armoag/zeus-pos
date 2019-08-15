using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using GenericParsing;
using Shun;

namespace Zeus
{
    /// <summary>
    /// Class for products to be used in the inventory and point of sale system
    /// </summary>
    public class CarInventory: IInventory, ISqLDataBase
    {

        #region Fields
        private DataTable _dictOfData;
        private string _filePath;
        public static IInventory _inventory = null;
        public static ISystemConfiguration _systemConfig = null;
        public List<string> _dbColumns = new List<string>()
        {
            "Id",
            "Codigo",
            "CodigoAlterno",
            "ProveedorProductoId",
            "Descripcion",
            "VIN",
            "Marca",
            "Modelo",
            "Anho",
            "Transmision",
            "Motor",
            "Color",
            "Proveedor",
            "Categoria",
            "UltimoPedidoFecha",
            "Costo",
            "CostoMoneda",
            "CostoImportacion",
            "CostoImportacionMoneda",
            "Precio",
            "PrecioMoneda",
            "Ubicacion",
            "Pasillo",
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
        protected CarInventory(string filePath, MySqlDatabase mySqlDb, ISystemConfiguration systemConfig)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            //Read inventory CSV format
            FilePath = filePath;
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
                _inventory = new CarInventory(filePath, mySqlDb, systemConfig);
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
            if (product is CarPart carPart)
            {              
                //Add product to inventory database
                if (MySqlData != null && SystemConfig.CloudInventory)
                {
                    var productColValPairs = new List<Tuple<string, string>>()
                    {
                        new Tuple<string, string>(DbColumns[1], carPart.Code),
                        new Tuple<string, string>(DbColumns[2], carPart.AlternativeCode),
                        new Tuple<string, string>(DbColumns[3], carPart.ProviderProductId),
                        new Tuple<string, string>(DbColumns[4], carPart.Description),
                        new Tuple<string, string>(DbColumns[5], carPart.Vin),
                        new Tuple<string, string>(DbColumns[6], carPart.Make),
                        new Tuple<string, string>(DbColumns[7], carPart.Model),
                        new Tuple<string, string>(DbColumns[8], carPart.Year.ToString()),
                        new Tuple<string, string>(DbColumns[9], carPart.Transmission),
                        new Tuple<string, string>(DbColumns[10], carPart.Motor),
                        new Tuple<string, string>(DbColumns[11], carPart.Color),
                        new Tuple<string, string>(DbColumns[12], carPart.Provider),
                        new Tuple<string, string>(DbColumns[13], carPart.Category),
                        new Tuple<string, string>(DbColumns[14], carPart.LastPurchaseDateString),
                        new Tuple<string, string>(DbColumns[15], carPart.Cost.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[16], carPart.CostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[17], carPart.ImportCost.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[18], carPart.ImportCostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[19], carPart.Price.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[20], carPart.PriceCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[21], carPart.Location),
                        new Tuple<string, string>(DbColumns[22], carPart.SpecificLocation),
                        new Tuple<string, string>(DbColumns[23], carPart.InternalQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[24], carPart.QuantitySold.ToString()),
                        new Tuple<string, string>(DbColumns[25], carPart.AmountSold.ToString(CultureInfo.InvariantCulture)),
                        new Tuple<string, string>(DbColumns[26], carPart.LocalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[27], carPart.TotalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[28], carPart.MinimumStockQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[29], carPart.LastSaleDateString),
                        new Tuple<string, string>(DbColumns[30], carPart.ImageName)
                    };
                    MySqlData.Insert(productColValPairs);
                    //Get Id for local database
                    MySqlData.Read("Codigo", carPart.Code, out var readData);
                    id = long.Parse(readData[0].Item2[0].Item2);
                }
                //Add product to datatable 
                if (!SystemConfig.LocalInventory) return true;

                if (id != 0) carPart.Id = Int32.Parse(id.ToString());

                DictOfData.Rows.Add();
                var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
                row["Id"] = carPart.Id.ToString();
                row["Codigo"] = carPart.Code;
                row["CodigoAlterno"] = carPart.AlternativeCode;
                row["ProveedorProductoId"] = carPart.ProviderProductId;
                row["Descripcion"] = carPart.Description;
                row["VIN"] = carPart.Vin;
                row["Marca"] = carPart.Make;
                row["Modelo"] = carPart.Model;
                row["Anho"] = carPart.Year.ToString();
                row["Transmision"] = carPart.Transmission;
                row["Motor"] = carPart.Motor;
                row["Color"] = carPart.Color;
                row["Proveedor"] = carPart.Provider;
                row["Categoria"] = carPart.Category;
                row["Costo"] = carPart.Cost.ToString(CultureInfo.InvariantCulture);
                row["CostoMoneda"] = carPart.CostCurrency;
                row["CostoImportacion"] = carPart.ImportCost.ToString(CultureInfo.InvariantCulture);
                row["CostoImportacionMoneda"] = carPart.ImportCostCurrency;
                row["Precio"] = carPart.Price.ToString();
                row["PrecioMoneda"] = carPart.PriceCurrency;
                row["Ubicacion"] = carPart.Location;
                row["Pasillo"] = carPart.SpecificLocation;
                row["CantidadInternoHistorial"] = carPart.InternalQuantity.ToString();
                row["CantidadVendidoHistorial"] = carPart.QuantitySold.ToString();
                row["CantidadLocal"] = carPart.LocalQuantityAvailable.ToString();
                row["CantidadDisponibleTotal"] = carPart.TotalQuantityAvailable.ToString();
                row["VendidoHistorial"] = carPart.AmountSold.ToString();
                row["CantidadMinima"] = carPart.MinimumStockQuantity.ToString();
                row["UltimoPedidoFecha"] = carPart.LastPurchaseDate.ToString();
                row["UltimaTransaccionFecha"] = carPart.LastSaleDate.ToString();
                row["Imagen"] = carPart.ImageName;
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Deletes an item from the database
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
                        return new CarPart()
                        {
                            Id = Int32.Parse(foundData.First().Item2[0].Item2),
                            Code = foundData.First().Item2[1].Item2,
                            AlternativeCode = foundData.First().Item2[2].Item2,
                            ProviderProductId = foundData.First().Item2[3].Item2,
                            Description = foundData.First().Item2[4].Item2,
                            Vin = foundData.First().Item2[5].Item2,
                            Make = foundData.First().Item2[6].Item2,
                            Model = foundData.First().Item2[7].Item2,
                            Year = Int32.Parse(foundData.First().Item2[8].Item2),
                            Transmission = foundData.First().Item2[9].Item2,
                            Motor = foundData.First().Item2[10].Item2,
                            Color = foundData.First().Item2[11].Item2,
                            Provider = foundData.First().Item2[12].Item2,
                            Category = foundData.First().Item2[13].Item2,
                            LastPurchaseDate = Convert.ToDateTime(foundData.First().Item2[14].Item2),
                            Cost = Decimal.Parse(foundData.First().Item2[15].Item2),
                            CostCurrency = foundData.First().Item2[16].Item2 == "USD"
                                    ? CurrencyTypeEnum.USD
                                    : CurrencyTypeEnum.MXN,
                            ImportCost = decimal.Parse(foundData.First().Item2[17].Item2),
                            ImportCostCurrency = foundData.First().Item2[18].Item2 == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN,
                            Price = decimal.Parse(foundData.First().Item2[19].Item2),
                            PriceCurrency = foundData.First().Item2[20].Item2 == "USD"
                                ? CurrencyTypeEnum.USD
                                : CurrencyTypeEnum.MXN,
                            Location = foundData.First().Item2[21].Item2,
                            SpecificLocation = foundData.First().Item2[22].Item2,
                            InternalQuantity = Int32.Parse(foundData.First().Item2[23].Item2),
                            QuantitySold = Int32.Parse(foundData.First().Item2[24].Item2),
                            AmountSold = decimal.Parse(foundData.First().Item2[25].Item2),
                            LocalQuantityAvailable = Int32.Parse(foundData.First().Item2[26].Item2),
                            TotalQuantityAvailable = Int32.Parse(foundData.First().Item2[27].Item2),
                            MinimumStockQuantity = Int32.Parse(foundData.First().Item2[28].Item2),
                            LastSaleDate = Convert.ToDateTime(foundData.First().Item2[29].Item2),
                            ImageName = foundData.First().Item2[30].Item2
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
                            return new CarPart()
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
                                Vin = row["VIN"].ToString(),
                                Make = row["Marca"].ToString(),
                                Model = row["Modelo"].ToString(),
                                Year = Int32.Parse(row["Anho"].ToString()),
                                Transmission = row["Transmision"].ToString(),
                                Motor = row["Motor"].ToString(),
                                Color = row["Color"].ToString(),
                                ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                                ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN,
                                Location = row["Ubicacion"].ToString(),
                                SpecificLocation = row["Pasillo"].ToString()
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

            if (CarInventory._inventory != null)
            {
                //Get codes from product lists
                var list = CategoryCatalog.GetList(filePath);
                //Skip first line, which is title of the list
                for (int i = 1; i < list.Count; ++i)
                {
                    productList.Add(CarInventory._inventory.GetProduct(list[i]));
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

        public List<IProduct> Search(string input)
        {
            var products = new List<IProduct>();

            //Return empty list if invalid inputs are entered for the search
            if (string.IsNullOrWhiteSpace(input) || input == "x")
                return products;

            if (MySqlData != null && SystemConfig.CloudInventory)
            {
                var allFields = MySqlData.SelectAll(DbColumns).AsEnumerable();
                if (input == "*")
                {
                    var allProducts = allFields;
                    foreach (var row in allProducts)
                    {

                        var product = new CarPart()
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
                            Vin = row["VIN"].ToString(),
                            Make = row["Marca"].ToString(),
                            Model = row["Modelo"].ToString(),
                            Year = Int32.Parse(row["Anho"].ToString()),
                            Transmission = row["Transmision"].ToString(),
                            Motor = row["Motor"].ToString(),
                            Color = row["Color"].ToString(),
                            ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                            Location = row["Ubicacion"].ToString(),
                            SpecificLocation = row["Pasillo"].ToString()
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        products.Add(product);
                    }
                }

                var descriptionFilter = allFields.Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                var codeFilter = allFields.Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

                foreach (var row in codeFilter)
                {
                    var product = new CarPart()
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
                        Vin = row["VIN"].ToString(),
                        Make = row["Marca"].ToString(),
                        Model = row["Modelo"].ToString(),
                        Year = Int32.Parse(row["Anho"].ToString()),
                        Transmission = row["Transmision"].ToString(),
                        Motor = row["Motor"].ToString(),
                        Color = row["Color"].ToString(),
                        ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                        Location = row["Ubicacion"].ToString(),
                        SpecificLocation = row["Pasillo"].ToString()
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    products.Add(product);
                }

                foreach (var row in descriptionFilter)
                {
                    var product = new CarPart()
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
                        Vin = row["VIN"].ToString(),
                        Make = row["Marca"].ToString(),
                        Model = row["Modelo"].ToString(),
                        Year = Int32.Parse(row["Anho"].ToString()),
                        Transmission = row["Transmision"].ToString(),
                        Motor = row["Motor"].ToString(),
                        Color = row["Color"].ToString(),
                        ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                        Location = row["Ubicacion"].ToString(),
                        SpecificLocation = row["Pasillo"].ToString()
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    //Add if it does not exist already
                    if (!products.Exists(x => x.Code == product.Code))
                        products.Add(product);
                }

                return products;
            }
            else //If it is local db
            {
                if (input == "*")
                {
                    var allProducts = DictOfData.AsEnumerable();
                    foreach (var row in allProducts)
                    {

                        var product = new CarPart()
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
                            Vin = row["VIN"].ToString(),
                            Make = row["Marca"].ToString(),
                            Model = row["Modelo"].ToString(),
                            Year = Int32.Parse(row["Anho"].ToString()),
                            Transmission = row["Transmision"].ToString(),
                            Motor = row["Motor"].ToString(),
                            Color = row["Color"].ToString(),
                            ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                            Location = row["Ubicacion"].ToString(),
                            SpecificLocation = row["Pasillo"].ToString()
                        };

                        product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                            ? CurrencyTypeEnum.USD
                            : CurrencyTypeEnum.MXN;
                        product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD"
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
                    var product = new CarPart()
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
                        Vin = row["VIN"].ToString(),
                        Make = row["Marca"].ToString(),
                        Model = row["Modelo"].ToString(),
                        Year = Int32.Parse(row["Anho"].ToString()),
                        Transmission = row["Transmision"].ToString(),
                        Motor = row["Motor"].ToString(),
                        Color = row["Color"].ToString(),
                        ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                        Location = row["Ubicacion"].ToString(),
                        SpecificLocation = row["Pasillo"].ToString()
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    products.Add(product);
                }

                foreach (var row in descriptionFilter)
                {
                    var product = new CarPart()
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
                        Vin = row["VIN"].ToString(),
                        Make = row["Marca"].ToString(),
                        Model = row["Modelo"].ToString(),
                        Year = Int32.Parse(row["Anho"].ToString()),
                        Transmission = row["Transmision"].ToString(),
                        Motor = row["Motor"].ToString(),
                        Color = row["Color"].ToString(),
                        ImportCost = decimal.Parse(row["CostoImportacion"].ToString()),
                        Location = row["Ubicacion"].ToString(),
                        SpecificLocation = row["Pasillo"].ToString()
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;
                    product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD"
                        ? CurrencyTypeEnum.USD
                        : CurrencyTypeEnum.MXN;

                    //Add if it does not exist already
                    if (!products.Exists(x => x.Code == product.Code))
                        products.Add(product);
                }

                return products;
            }
        }

        //public void UpdateItem(string code, string columnName, string newData)
        //{
        //    ///TODO: Depricate Soon
        //    for (int index = 0; index < DictOfData.Rows.Count; index++)
        //    {
        //        var row = DictOfData.Rows[index];
        //        if (row["Codigo"].ToString() == code)
        //        {
        //            row[columnName] = newData;
        //            return;
        //        }
        //    }
        //}

        public bool UpdateProductToTable(IProduct product)
        {
            if (product is CarPart carPart)
            {
                if (MySqlData != null && SystemConfig.CloudInventory)
                {
                    var data = new List<Tuple<string, string>>()
                    {  
                        new Tuple<string, string>(DbColumns[1], carPart.Code),
                        new Tuple<string, string>(DbColumns[2], carPart.AlternativeCode),
                        new Tuple<string, string>(DbColumns[3], carPart.ProviderProductId),
                        new Tuple<string, string>(DbColumns[4], carPart.Code),
                        new Tuple<string, string>(DbColumns[5], carPart.Description),
                        new Tuple<string, string>(DbColumns[6], carPart.Vin),
                        new Tuple<string, string>(DbColumns[7], carPart.Make),
                        new Tuple<string, string>(DbColumns[8], carPart.Model),
                        new Tuple<string, string>(DbColumns[9], carPart.Year.ToString()),
                        new Tuple<string, string>(DbColumns[10], carPart.Transmission),
                        new Tuple<string, string>(DbColumns[11], carPart.Model),
                        new Tuple<string, string>(DbColumns[12], carPart.Provider),
                        new Tuple<string, string>(DbColumns[13], carPart.Category),
                        new Tuple<string, string>(DbColumns[14], carPart.LastPurchaseDateString),
                        new Tuple<string, string>(DbColumns[15], carPart.Cost.ToString()),
                        new Tuple<string, string>(DbColumns[16], carPart.CostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[17], carPart.ImportCost.ToString()),
                        new Tuple<string, string>(DbColumns[18], carPart.ImportCostCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[19], carPart.Price.ToString()),
                        new Tuple<string, string>(DbColumns[20], carPart.PriceCurrency.ToString()),
                        new Tuple<string, string>(DbColumns[21], carPart.Location),
                        new Tuple<string, string>(DbColumns[22], carPart.SpecificLocation),
                        new Tuple<string, string>(DbColumns[23], carPart.InternalQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[24], carPart.AmountSold.ToString()),
                        new Tuple<string, string>(DbColumns[25], carPart.LocalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[26], carPart.TotalQuantityAvailable.ToString()),
                        new Tuple<string, string>(DbColumns[27], carPart.MinimumStockQuantity.ToString()),
                        new Tuple<string, string>(DbColumns[28], carPart.LastSaleDateString),
                        new Tuple<string, string>(DbColumns[29], carPart.ImageName)
                    };    
                    MySqlData.Update("Codigo", product.Code, data);
                }

                if (!SystemConfig.LocalInventory) return false;

                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == carPart.Code)
                    {
                        row["Id"] = carPart.Id.ToString();
                        row["CodigoAlterno"] = carPart.AlternativeCode;
                        row["ProveedorProductoId"] = carPart.ProviderProductId;
                        row["Descripcion"] = carPart.Description;
                        row["Proveedor"] = carPart.Provider;
                        row["Categoria"] = carPart.Category;
                        row["Costo"] = carPart.Cost.ToString(CultureInfo.InvariantCulture);
                        row["CostoMoneda"] = carPart.CostCurrency;
                        row["Precio"] = carPart.Price.ToString();
                        row["PrecioMoneda"] = carPart.PriceCurrency.ToString();
                        row["CantidadInternoHistorial"] = carPart.InternalQuantity.ToString();
                        row["CantidadVendidoHistorial"] = carPart.QuantitySold.ToString();
                        row["CantidadLocal"] = carPart.LocalQuantityAvailable.ToString();
                        row["VendidoHistorial"] = carPart.AmountSold.ToString();
                        row["CantidadDisponibleTotal"] = carPart.TotalQuantityAvailable.ToString();
                        row["CantidadMinima"] = carPart.MinimumStockQuantity.ToString();
                        row["UltimoPedidoFecha"] = carPart.LastPurchaseDate.ToString();
                        row["UltimaTransaccionFecha"] = carPart.LastSaleDate.ToString();
                        row["Imagen"] = carPart.ImageName;
                        row["VIN"] = carPart.Vin;
                        row["Marca"] = carPart.Make;
                        row["Modelo"] = carPart.Model;
                        row["Anho"] = carPart.Year.ToString();
                        row["Transmision"] = carPart.Transmission;
                        row["Motor"] = carPart.Motor;
                        row["Color"] = carPart.Color;
                        row["CostoImportacion"] = carPart.ImportCost.ToString(CultureInfo.InvariantCulture);
                        row["CostoImportacionMoneda"] = carPart.ImportCostCurrency;
                        row["Ubicacion"] = carPart.Location;
                        row["Pasillo"] = carPart.SpecificLocation;
                    }
                }

                return true;
            }

            return false;
        }

        //public void UpdateSoldItemQuantity(string code, int unitsSold)
        //{
        //    for (int index = 0; index < DictOfData.Rows.Count; index++)
        //    {
        //        var row = DictOfData.Rows[index];
        //        if (row["Codigo"].ToString() == code)
        //        {
        //            int quantity = Int32.Parse(row["CantidadLocal"].ToString());
        //            row["CantidadLocal"] = (quantity - unitsSold).ToString();
        //            return;
        //        }
        //    }
        //}

        //public bool UpdateSoldProductToTable(IProduct product)
        //{
        //    ///TODO: Depricate Soon
        //    for (int index = 0; index < DictOfData.Rows.Count; index++)
        //    {
        //        var row = DictOfData.Rows[index];
        //        if (row["Codigo"].ToString() == product.Code)
        //        {
        //            row["CantidadDisponibleTotal"] = product.TotalQuantityAvailable.ToString();
        //            row["Precio"] = product.Price.ToString();
        //            row["CantidadVendidoHistorial"] = product.QuantitySold.ToString();
        //            row["VendidoHistorial"] = product.AmountSold.ToString();
        //            row["CantidadInternoHistorial"] = product.InternalQuantity.ToString();
        //            row["CantidadLocal"] = product.LocalQuantityAvailable.ToString();
        //            row["UltimaTransaccionFecha"] = product.LastSaleDate.ToString();
        //        }
        //    }

        //    return true;
        //}
    }
    #endregion
}

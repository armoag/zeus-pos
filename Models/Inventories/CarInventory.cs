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

namespace Zeus
{
    /// <summary>
    /// Class for products to be used in the inventory and point of sale system
    /// </summary>
    public class CarInventory: IInventory
    {
        private DataTable _dictOfData;
        private string _filePath;

        public static IInventory _inventory = null;
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors

        //Singleton pattern
        protected CarInventory(string filePath)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            //Read inventory CSV format
            FilePath = filePath;
            LoadCsvToDataTable();
        }

        public static IInventory GetInstance(string filePath)
        {
            if (_inventory == null)
                _inventory = new CarInventory(filePath);
            return _inventory;
        }

        #endregion

        #region Methods

        #endregion

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

        public bool AddNewProductToTable(IProduct product)
        {
            if (product is CarPart carPart)
            {
                DictOfData.Rows.Add();
                var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
                row["NumeroProducto"] = carPart.Id.ToString();
                row["Codigo"] = carPart.Code;
                row["CodigoAlterno"] = carPart.AlternativeCode;
                row["ProveedorProductoId"] = carPart.ProviderProductId;
                row["Descripcion"] = carPart.Description;
                row["Proveedor"] = carPart.Provider;
                row["Categoria"] = carPart.Category;
                row["Costo"] = carPart.Cost.ToString(CultureInfo.InvariantCulture);
                row["CostoMoneda"] = carPart.CostCurrency;
                row["Precio"] = carPart.Price.ToString();
                row["PrecioMoneda"] = carPart.PriceCurrency;
                row["CantidadInternoHistorial"] = carPart.InternalQuantity.ToString();
                row["CantidadVendidoHistorial"] = carPart.QuantitySold.ToString();
                row["CantidadLocal"] = carPart.LocalQuantityAvailable.ToString();
                row["CantidadDisponibleTotal"] = carPart.TotalQuantityAvailable.ToString();
                row["VendidoHistorial"] = carPart.AmountSold.ToString();
                row["CantidadMinima"] = carPart.MinimumStockQuantity.ToString();
                row["UltimoPedidoFecha"] = carPart.LastPurchaseDate.ToString();
                row["UltimaTransaccionFecha"] = carPart.LastSaleDate.ToString();
                row["Imagen"] = carPart.ImageName;

                //specific data
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
                
                return true;
            }
            else
            {
                return false;
            }

        }

        public void DeleteItemInDataTable(string inputSearch, string columnName)
        {
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
            if (DictOfData.Rows.Count == 0)
                return 0;
            var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
            return Int32.Parse(row["NumeroProducto"].ToString());
        }

        public IProduct GetProduct(string code)
        {
            try
            {
                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == code)
                    {
                        return new CarPart()
                        {
                            Id = Int32.Parse(row["NumeroProducto"].ToString()),
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
            catch (Exception e)
            {
                MessageBox.Show("Error en el Codigo", "Error");

            }

            return new ProductBase() { Description = "", Category = "", Cost = 0M };
        }

        public IProduct GetProductFromDescription(string description)
        {
            try
            {
                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Descripcion"].ToString() == description)
                    {
                        return new CarPart()
                        {
                            Id = Int32.Parse(row["NumeroProducto"].ToString()),
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

        public string QueryDataFromCode(string code, string columnName)
        {
            for (int index = 0; index < DictOfData.Rows.Count; index++)
            {
                var row = DictOfData.Rows[index];
                if (row["Codigo"].ToString() == code)
                {
                    return row[columnName].ToString();
                }
            }
            return string.Format("No se encontro el codigo {0}", code);
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

            if (input == "*")
            {
                var allProducts = DictOfData.AsEnumerable();
                foreach (var row in allProducts)
                {

                    var product = new CarPart()
                    {
                        Id = Int32.Parse(row["NumeroProducto"].ToString()),
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

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                    product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                    products.Add(product);
                }

                return products;
            }

            var descriptionFilter = DictOfData.AsEnumerable().Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
            var codeFilter = DictOfData.AsEnumerable().Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

            foreach (var row in codeFilter)
            {
                var product = new CarPart()
                {
                    Id = Int32.Parse(row["NumeroProducto"].ToString()),
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

                product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                products.Add(product);
            }

            foreach (var row in descriptionFilter)
            {
                var product = new CarPart()
                {
                    Id = Int32.Parse(row["NumeroProducto"].ToString()),
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

                product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.ImportCostCurrency = row["CostoImportacionMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                //Add if it does not exist already
                if (!products.Exists(x => x.Code == product.Code))
                    products.Add(product);
            }

            return products;
        }

        public void UpdateItem(string code, string columnName, string newData)
        {
            for (int index = 0; index < DictOfData.Rows.Count; index++)
            {
                var row = DictOfData.Rows[index];
                if (row["Codigo"].ToString() == code)
                {
                    row[columnName] = newData;
                    return;
                }
            }
        }

        public bool UpdateProductToTable(IProduct product)
        {
            if (product is CarPart carPart)
            {
                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == carPart.Code)
                    {
                        row["NumeroProducto"] = carPart.Id.ToString();
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

        public void UpdateSoldItemQuantity(string code, int unitsSold)
        {
            for (int index = 0; index < DictOfData.Rows.Count; index++)
            {
                var row = DictOfData.Rows[index];
                if (row["Codigo"].ToString() == code)
                {
                    int quantity = Int32.Parse(row["CantidadLocal"].ToString());
                    row["CantidadLocal"] = (quantity - unitsSold).ToString();
                    return;
                }
            }
        }

        public bool UpdateSoldProductToTable(IProduct product)
        {
            for (int index = 0; index < DictOfData.Rows.Count; index++)
            {
                var row = DictOfData.Rows[index];
                if (row["Codigo"].ToString() == product.Code)
                {
                    row["CantidadDisponibleTotal"] = product.TotalQuantityAvailable.ToString();
                    row["Precio"] = product.Price.ToString();
                    row["CantidadVendidoHistorial"] = product.QuantitySold.ToString();
                    row["VendidoHistorial"] = product.AmountSold.ToString();
                    row["CantidadInternoHistorial"] = product.InternalQuantity.ToString();
                    row["CantidadLocal"] = product.LocalQuantityAvailable.ToString();
                    row["UltimaTransaccionFecha"] = product.LastSaleDate.ToString();
                }
            }

            return true;
        }
    }
}

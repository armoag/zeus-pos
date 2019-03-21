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

namespace Zeus
{
    public class InventoryBase : IInventory
    {
        #region Fields

        public static InventoryBase _inventory = null; 

        DataTable _dictofdata;
        private string _filePath;

        #endregion

        #region Properties

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
        protected InventoryBase(string filePath)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            //Read inventory CSV format
            _filePath = filePath;
            LoadCsvToDataTable();
        }

        public static IInventory GetInstance(string filePath)
        {
            if (_inventory == null)
                _inventory = new InventoryBase(filePath);
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
        /// Query any item data from the code
        /// </summary>
        /// <param name="code">Code to find item</param>
        /// <param name="columnName">Column header to retrive the data</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Get the last item number in the inventory
        /// </summary>
        /// <returns></returns>
        public int GetLastItemNumber()
        {
            if (DictOfData.Rows.Count == 0)
                return 0;
            var row = DictOfData.Rows[DictOfData.Rows.Count - 1];
            return Int32.Parse(row["NumeroProducto"].ToString());
        }

        /// <summary>
        /// Add new data to a specific item column name based on the code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="columnName"></param>
        /// <param name="newData"></param>
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

        /// <summary>
        /// Removes a full entry in the inventory
        /// </summary>
        /// <param name="inputSearch"></param>
        /// <param name="columnName"></param>
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

        /// <summary>
        /// Update the number of items sold
        /// </summary>
        /// <param name="code"></param>
        /// <param name="unitsSold"></param>
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

        /// <summary>
        /// Get product based on a code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual IProduct GetProduct(string code)
        {
            try
            {
                for (int index = 0; index < DictOfData.Rows.Count; index++)
                {
                    var row = DictOfData.Rows[index];
                    if (row["Codigo"].ToString() == code)
                    {
                        return new ProductBase()
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
        /// Get product based on the description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public virtual IProduct GetProductFromDescription(string description)
        {
            try
            {
                for (int index = 0; index < _dictofdata.Rows.Count; index++)
                {
                    var row = _dictofdata.Rows[index];
                    if (row["Descripcion"].ToString() == description)
                    {
                        return new ProductBase()
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
        /// Update the sold product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update product in the datatable
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual bool UpdateProductToTable(IProduct product)
        {
            for (int index = 0; index < DictOfData.Rows.Count; index++)
            {
                var row = DictOfData.Rows[index];
                if (row["Codigo"].ToString() == product.Code)
                {
                    row["NumeroProducto"] = product.Id.ToString();
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

            return true;
        }

        /// <summary>
        /// Add new product to data table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual bool AddNewProductToTable(IProduct product)
        {
            DictOfData.Rows.Add();
            var row = _dictofdata.Rows[_dictofdata.Rows.Count - 1];
            row["NumeroProducto"] = product.Id.ToString();
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

            return true;
        }
        
        //TODO: Need to remove it frmo inventory for cleaning up
        /// <summary>
        /// Create a backup copy of the inventory file
        /// </summary>
        /// <param name="filePath"></param>
        public static void InventoryBackUp(string filePath)
        {
            //Set date format
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            var currentTime = DateTime.Now;
            //Load inventory csv file and create a backup copy
            var inventoryFileBackUpCopyName = Constants.DataFolderPath + Constants.InventoryBackupFolderPath + "Inventario" + 
                                                 currentTime.Day.ToString("00") + currentTime.Month.ToString("00") + currentTime.Year.ToString("0000") +
                                                 currentTime.Hour.ToString("00") + currentTime.Minute.ToString("00") + currentTime.Second.ToString("00") + ".csv";

            File.Copy(filePath, inventoryFileBackUpCopyName);
        }

        /// <summary>
        /// Search and returns a list of products
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual List<IProduct> Search(string input)
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
                    
                    var product = new ProductBase()
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
                        ImageName = row["Imagen"].ToString()
                    };

                    product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                    product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                    products.Add(product);
                }

                return products;
            }

            var descriptionFilter = DictOfData.AsEnumerable().Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
            var codeFilter = DictOfData.AsEnumerable().Where(r => r.Field<string>("Codigo").ToLower().Contains(input));

            foreach (var row in codeFilter)
            {
                var product = new ProductBase()
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
                    ImageName = row["Imagen"].ToString()
                };

                product.CostCurrency = row["CostoMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;
                product.PriceCurrency = row["PrecioMoneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN;

                products.Add(product);
            }

            foreach(var row in descriptionFilter)
            {
                var product = new ProductBase()
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

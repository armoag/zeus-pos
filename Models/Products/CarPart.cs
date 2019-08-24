using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Zeus
{
    /// <summary>
    /// Class for products to be used in the inventory and point of sale system
    /// </summary>
    public class CarPart : IProduct
    {
        #region Fields

        private string _alternativeCode;
        private decimal _amountSold;
        private string _brand;
        private string _category;
        private string _code;
        private decimal _cost;
        private CurrencyTypeEnum _costCurrency;
        private string _description;
        private int _id;
        private BitmapImage _image;
        private string _imageName;
        private int _internalQuantity;
        private decimal _lastAmountSold;
        private DateTime _lastPurchaseDate;
        private string _lastPurchaseDateString;
        private int _lastQuantitySold;
        private DateTime _lastSaleDate;
        private string _lastSaleDateString;
        private int _localQuantityAvailable;
        private int _minimumStockQuantity;
        private string _name;
        private decimal _price;
        private CurrencyTypeEnum _priceCurrency;
        private string _provider;
        private string _providerProductId;
        private int _quantitySold;
        private int _totalQuantityAvailable;

        //Not in interface
        private string _vin;
        private string _model;
        private string _color;
        private string _make;
        private string _transmission;
        private string _motor;
        private int _year;
        private decimal _importCost;
        private CurrencyTypeEnum _importCostCurrency;
        private string _location;
        private string _specificLocation;
        private bool _valid;
        #endregion

        #region Properties


        #endregion

        #region Constructors

        public CarPart()
        {

        }

        public CarPart(IProduct product)
        {
            //Basic
            this.Id = product.Id;
            this.Code = product.Code;
            this.AlternativeCode = product.AlternativeCode;
            this.Description = product.Description;
            this.Brand = product.Brand;
            this.Category = product.Category;
            this.Provider = product.Provider;
            this.ProviderProductId = product.ProviderProductId;
            this.ImageName = product.ImageName;
            this.Image = product.Image;
            //Amounts
            this.AmountSold = product.AmountSold;
            this.Cost = product.Cost;
            this.CostCurrency = product.CostCurrency;
            this.Price = product.Price;
            this.PriceCurrency = product.PriceCurrency;
            //Quantities
            this.LastQuantitySold = product.LastQuantitySold;
            this.TotalQuantityAvailable = product.TotalQuantityAvailable;
            this.LocalQuantityAvailable = product.LocalQuantityAvailable;
            this.MinimumStockQuantity = product.MinimumStockQuantity;
            this.InternalQuantity = product.InternalQuantity;
            this.QuantitySold = product.QuantitySold;
            //dates
            this.LastPurchaseDate = product.LastPurchaseDate;
            this.LastSaleDate = product.LastSaleDate;
            
        }
        #endregion

        #region Methods

        #endregion

        public string AlternativeCode
        {
            get { return _alternativeCode; }
            set { _alternativeCode = value; }
        }

        public decimal AmountSold
        {
            get { return _amountSold; }
            set { _amountSold = value; }
        }

        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public decimal Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public CurrencyTypeEnum CostCurrency
        {
            get { return _costCurrency; }
            set { _costCurrency = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public BitmapImage Image
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                if (ImageName != null)
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(@"C:\Projects\seiya-pos\Data\Images\" + ImageName);
                    bitmap.EndInit();
                    _image = bitmap;
                }
                return bitmap;
            }
            set
            {
                _image = value;
            }
        }

        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; }
        }

        public int InternalQuantity
        {
            get { return _internalQuantity; }
            set { _internalQuantity = value; }
        }

        public decimal LastAmountSold
        {
            get { return _lastAmountSold; }
        }

        public DateTime LastPurchaseDate
        {
            get { return _lastPurchaseDate; }
            set { _lastPurchaseDate = value; }
        }

        public string LastPurchaseDateString
        {
            get { return Utilities.FormatDateForMySql(_lastPurchaseDate); }
        }

        public int LastQuantitySold
        {
            get { return _lastQuantitySold; }
            set { _lastQuantitySold = value; }
        }

        public DateTime LastSaleDate
        {
            get { return _lastSaleDate; }
            set { _lastSaleDate = value; }
        }

        public string LastSaleDateString
        {
            get { return Utilities.FormatDateForMySql(_lastSaleDate); }
        }

        public int LocalQuantityAvailable
        {
            get { return _localQuantityAvailable; }
            set { _localQuantityAvailable = value; }
        }

        public int MinimumStockQuantity
        {
            get { return _minimumStockQuantity; }
            set { _minimumStockQuantity = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public CurrencyTypeEnum PriceCurrency
        {
            get { return _priceCurrency; }
            set { _priceCurrency = value; }
        }

        public string Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public string ProviderProductId
        {
            get { return _providerProductId; }
            set { _providerProductId = value; }
        }

        public int QuantitySold
        {
            get { return _quantitySold; }
            set { _quantitySold = value; }
        }

        public int TotalQuantityAvailable
        {
            get { return _totalQuantityAvailable; }
            set { _totalQuantityAvailable = value; }
        }

        //Specific properties not in the interface

        public string Vin
        {
            get { return _vin; }
            set { _vin = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string Make
        {
            get { return _make; }
            set { _make = value; }
        }

        public string Transmission
        {
            get { return _transmission; }
            set { _transmission = value; }
        }

        public string Motor
        {
            get { return _motor; }
            set { _motor = value; }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public decimal ImportCost
        {
            get { return _importCost; }
            set { _importCost = value; }
        }

        public CurrencyTypeEnum ImportCostCurrency
        {
            get { return _importCostCurrency; }
            set { _importCostCurrency = value; }
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public string SpecificLocation
        {
            get { return _specificLocation; }
            set { _specificLocation = value; }
        }

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        //Methods not in interface
        //Create a basic product with minimal information for manual transactions
        public static IProduct Add(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return new CarPart()
            {
                Description = description,
                Category = category,
                Price = soldPrice,
                LastQuantitySold = lastQuantitySold
            };
        }

        public void UpdateProductListFile(string filePath, List<IProduct> products, string listName)
        {
            //Creates or overwrites file
            StreamWriter writer = File.CreateText(filePath);
            //Write list name
            writer.WriteLine(listName);
            //Write code for each item
            foreach (var product in products)
            {
                writer.WriteLine(product.Code);
            }
            writer.Close();
            writer.Dispose();

            return;
        }

        public IProduct CreateNewItem(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return new CarPart()
            {
                Description = description,
                Category = category,
                Price = soldPrice,
                LastQuantitySold = lastQuantitySold
            };
        }

        public decimal GetMargin(decimal exchangeRate)
        {
            if (Price > 0)
            {
                return 100M * this.GetProfit(exchangeRate) / Price;
            }
            return -1;
        }

        public decimal GetProfit(decimal exchangeRate)
        {
            if (CostCurrency == CurrencyTypeEnum.USD)
            {
                return Price - Cost * exchangeRate;
            }
            return Price - Cost;
        }

        //Not in interface
        public override string ToString()
        {
            return string.Format("{0,-8}", LastQuantitySold) + Category.PadRight(10) + string.Format("{0,-11:c}", Price) + string.Format("{0,-11:c}", Price * LastQuantitySold);
        }

        public string ToString(bool detail)
        {
            string trimmedDescription = Description;
            if (Description.Length > 16)
            {
                trimmedDescription = Description.Substring(0, 16);
            }
            return string.Format("{0,-4}", LastQuantitySold) + trimmedDescription.PadRight(18) + string.Format("{0,-10:c}", Price) + string.Format("{0,-10:c}", Price * LastQuantitySold);
        }

        public string ToString(ReceiptType receiptType)
        {
            return string.Format("{0,-5}", LastQuantitySold) + Category.PadRight(15) + string.Format("{0,-11:c}", Price);
        }

        IProduct IProduct.Add(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return Add(description, category, soldPrice, lastQuantitySold);
        }

        public static List<CarPart> CreateCarParts(CarPart car, List<Tuple<string, string, int, decimal, CurrencyTypeEnum>> parts)
        {
            var list = new List<CarPart>();

            foreach (var part in parts)
            {
                var partialVin = car.Vin.Substring(car.Vin.Length - 5);
                var description = partialVin + " " + car.Model + " " + part.Item1;
                var category = part.Item2;
                var quantity = part.Item3;
                var price = part.Item4;
                var currency = part.Item5;
                var index = parts.IndexOf(part) + 1;
                
                var newPart = new CarPart()
                {
                    Code = partialVin + index,
                    AlternativeCode = "NA",
                    ProviderProductId = "NA",
                    Description = description,
                    Vin = car.Vin,
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Transmission = car.Transmission,
                    Motor = car.Motor,
                    Color = car.Color,
                    Provider = car.Provider,
                    Category = category,
                    LastPurchaseDate = car.LastPurchaseDate,
                    Cost = 0M,
                    CostCurrency = CurrencyTypeEnum.USD,
                    ImportCost = 0M,
                    ImportCostCurrency = CurrencyTypeEnum.USD,
                    Price = price,
                    PriceCurrency = currency,
                    Location = car.Location,
                    SpecificLocation = car.SpecificLocation,
                    InternalQuantity = 0,
                    QuantitySold = 0,
                    AmountSold = 0,
                    LocalQuantityAvailable = quantity,
                    TotalQuantityAvailable = quantity,
                    MinimumStockQuantity = 0,
                    LastSaleDate = car.LastPurchaseDate,
                    ImageName = car.ImageName,
                    Valid = true
                };

                list.Add(newPart);
            }
            return list;
        }

        public static List<Tuple<string, string, int, decimal, CurrencyTypeEnum>> ReadPartsFile(string fullFilePath)
        {
            var partsList = new List<Tuple<string, string, int, decimal, CurrencyTypeEnum>>();
            var db = new DataBase(fullFilePath);        
            db.LoadCsvToDataTable();
            for (int index = 0; index < db.DataTable.Rows.Count; index++)
            {
                var row = db.DataTable.Rows[index];
                var newPartData = new Tuple<string, string, int, decimal, CurrencyTypeEnum>(row["Descripcion"].ToString(), 
                    row["Categoria"].ToString(), Int32.Parse(row["Cantidad"].ToString()), decimal.Parse(row["Precio"].ToString()),
                        row["Moneda"].ToString().ToUpper() == "USD" ? CurrencyTypeEnum.USD : CurrencyTypeEnum.MXN);

                partsList.Add(newPartData);
            }

            return partsList;
        }

        public static void WritePartsFile(string fullFilePath , List<Tuple<string, string, int, decimal, CurrencyTypeEnum>> parts)
        {
            File.WriteAllText(fullFilePath, "Descripcion,Categoria,Cantidad,Precio,Moneda" + Environment.NewLine);

            foreach (var part in parts)
            {
                string data = string.Format("{0},{1},{2},{3},{4}", part.Item1, part.Item2, part.Item3.ToString(), part.Item4.ToString(), part.Item5.ToString())
                              + Environment.NewLine;

                File.AppendAllText(fullFilePath, data);
            }
        }
    }
}

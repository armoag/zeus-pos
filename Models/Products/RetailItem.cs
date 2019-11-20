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
    public class RetailItem : IProduct
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
        //private int _storeOneQuantityAvailable;
        //private int _storeTwoQuantityAvailable;
        //private int _storeThreeQuantityAvailable;
        //private int _storeOneQuantitySold;
        //private int _storeTwoQuantitySold;
        //private int _storeThreeQuantitySold;

        //private string _vin;
        //private string _model;
        //private string _color;
        //private string _make;
        //private string _transmission;
        //private string _motor;
        //private int _year;
        //private decimal _importCost;
        //private CurrencyTypeEnum _importCostCurrency;
        //private string _location;
        //private string _specificLocation;
        private bool _valid;
        #endregion

        #region Properties


        #endregion

        #region Constructors

        public RetailItem()
        {

        }

        public RetailItem(IProduct product)
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
                    if (File.Exists(Constants.DataFolderPath + Constants.ImagesFolderPath + ImageName))
                    {
                        bitmap.UriSource = new Uri(Constants.DataFolderPath + Constants.ImagesFolderPath + ImageName);
                    }
                    else
                    {
                        bitmap.UriSource = new Uri(Constants.DataFolderPath + Constants.ImagesFolderPath + "NA.jpg");
                    }
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

        //public int StoreOneQuantityAvailable
        //{
        //    get { return _storeOneQuantityAvailable; }
        //    set { _storeOneQuantityAvailable = value; }
        //}

        //public int StoreTwoQuantityAvailable
        //{
        //    get { return _storeTwoQuantityAvailable; }
        //    set { _storeTwoQuantityAvailable = value; }
        //}

        //public int StoreThreeQuantityAvailable
        //{
        //    get { return _storeThreeQuantityAvailable; }
        //    set { _storeThreeQuantityAvailable = value; }
        //}

        //public int StoreOneQuantitySold
        //{
        //    get { return _storeOneQuantitySold; }
        //    set { _storeOneQuantitySold = value; }
        //}

        //public int StoreTwoQuantitySold
        //{
        //    get { return _storeTwoQuantitySold; }
        //    set { _storeTwoQuantitySold = value; }
        //}

        //public int StoreThreeQuantitySold
        //{
        //    get { return _storeThreeQuantitySold; }
        //    set { _storeThreeQuantitySold = value; }
        //}

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        //Methods not in interface
        //Create a basic product with minimal information for manual transactions
        public static IProduct Add(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return new RetailItem()
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
            return new RetailItem()
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
    }
}

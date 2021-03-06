﻿using System;
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
    public class ProductBase : IProduct
    {
        #region Fields
        private BitmapImage _image;
        private DateTime _lastPurchaseDate;
        private DateTime _lastSaleDate;
        private decimal _lastAmountSold;
        #endregion

        #region Properties

        public string Name { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string AlternativeCode { get; set; }
        public string Provider { get; set; }
        public string ProviderProductId { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal Cost { get; set; }
        public CurrencyTypeEnum CostCurrency { get; set; }
        public decimal Price { get; set; }
        public CurrencyTypeEnum PriceCurrency { get; set; }
        public int MinimumStockQuantity { get; set; }
        public decimal AmountSold { get; set; }
        public int InternalQuantity { get; set; }
        public int QuantitySold { get; set; } 
        public int LocalQuantityAvailable { get; set; }
        public int TotalQuantityAvailable { get; set; }
        public string ImageName { get; set; }

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

        public DateTime LastPurchaseDate
        {
            get { return _lastPurchaseDate; }
            set { _lastPurchaseDate = value; }
        }

        public DateTime LastSaleDate
        {
            get { return _lastSaleDate; }
            set { _lastSaleDate = value; }
        }

        public string LastPurchaseDateString
        {
            get { return _lastPurchaseDate.ToString("d"); }
        }

        public string LastSaleDateString
        {
            get { return _lastSaleDate.ToString("d"); }
        }

        public int LastQuantitySold { get; set; }

        //public decimal LastAmountSold
        //{
        //    get { return Price * LastQuantitySold; }
        //    set { LastAmountSold = value; }
        //}
        public decimal LastAmountSold
        {
            get { return _lastAmountSold; }
            set { _lastAmountSold = value; }
        }
        public string Seller { get; set; }

        #endregion

        #region Constructors

        public ProductBase()
        {

        }

        public ProductBase(IProduct product)
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
            //TODO: Check
            this.LastAmountSold = this.Price * this.LastQuantitySold;
            this.Seller = product.Seller;
        }

        #endregion

        #region Methods

        //Create a basic product with minimal information for manual transactions
        public static IProduct Add(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return new ProductBase()
            {
                Description = description,
                Category = category,
                Price = soldPrice,
                LastQuantitySold = lastQuantitySold
            };
        }

        //Format for transaction log and display with category
        public override string ToString()
        {
            return string.Format("{0,-8}", LastQuantitySold) + Category.PadRight(10) + string.Format("{0,-11:c}", Price) + string.Format("{0,-11:c}", Price * LastQuantitySold);
        }

        //Format for transaction log and display with description
        IProduct IProduct.Add(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return Add(description, category, soldPrice, lastQuantitySold);
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

        //Format for receipts with basic information
        public string ToString(ReceiptType receiptType)
        {
            return string.Format("{0,-5}", LastQuantitySold) + Category.PadRight(15) + string.Format("{0,-11:c}", Price);
        }

        //Calculate product margin
        public decimal GetMargin(decimal exchangeRate)
        {
            if (Price > 0)
            {
                return 100M * this.GetProfit(exchangeRate) / Price;
            }
            return -1;
        }

        //Calculate product margin
        public decimal GetProfit(decimal exchangeRate)
        {
            if (CostCurrency == CurrencyTypeEnum.USD)
            {
                return Price - Cost * exchangeRate;
            }
            return Price - Cost;
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

        public virtual IProduct CreateNewItem(string description, string category, decimal soldPrice, int lastQuantitySold)
        {
            return new ProductBase()
            {
                Description = description,
                Category = category,
                Price = soldPrice,
                LastQuantitySold = lastQuantitySold
            };
        }

        #endregion

    }
}

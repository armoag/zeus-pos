using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Zeus
{
    public interface IProduct
    {
        #region Properties

        string Name { get; set; }
        int Id { get; set; }
        string Code { get; set; }
        string AlternativeCode { get; set; }
        string Provider { get; set; }
        string ProviderProductId { get; set; }
        string Description { get; set; }
        string Brand { get; set; }
        string Category { get; set; }
        decimal Cost { get; set; }
        CurrencyTypeEnum CostCurrency { get; set; }
        decimal Price { get; set; }
        CurrencyTypeEnum PriceCurrency { get; set; }
        int MinimumStockQuantity { get; set; }
        decimal AmountSold { get; set; }
        int InternalQuantity { get; set; }
        int QuantitySold { get; set; }
        int LocalQuantityAvailable { get; set; }
        int TotalQuantityAvailable { get; set; }
        string ImageName { get; set; }
        BitmapImage Image { get; set; }

        DateTime LastPurchaseDate { get; set; }
        DateTime LastSaleDate { get; set; }

        string LastPurchaseDateString { get; }
        string LastSaleDateString { get; }
        int LastQuantitySold { get; set; }
        decimal LastAmountSold { get; }

        #endregion

        #region Methods

        //Create a basic product with minimal information for manual transactions
        IProduct Add(string description, string category, decimal soldPrice, int lastQuantitySold);

        //Format for transaction log and display with description
        string ToString(bool detail);

        //Format for receipts with basic information
        string ToString(ReceiptType receiptType);

        //Calculate product margin
        decimal GetMargin(decimal exchangeRate);

        //Calculate product margin
        decimal GetProfit(decimal exchangeRate);

        /// <summary>
        /// Update products list file with new changes
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool UpdateProductListFile(string filePath, List<IProduct> products, string listName);

        #endregion
    }
}

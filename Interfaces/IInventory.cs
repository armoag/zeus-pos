using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GenericParsing;

namespace Zeus
{
    public interface IInventory
    {
        #region Properties

        DataTable DictOfData { get; set; }

        #endregion

        #region Constructors
        //Singleton pattern
        IInventory GetInstance(string filePath);

        #endregion

        #region Methods

        /// <summary>
        /// Load CSV database into a datatable object
        /// </summary>
        void LoadCsvToDataTable();

        /// <summary>
        /// Save datatable object in a CSV file
        /// </summary>
        void SaveDataTableToCsv();

        /// <summary>
        /// Query any item data from the code
        /// </summary>
        /// <param name="code">Code to find item</param>
        /// <param name="columnName">Column header to retrive the data</param>
        /// <returns></returns>
        string QueryDataFromCode(string code, string columnName);

        /// <summary>
        /// Get the last item number in the inventory
        /// </summary>
        /// <returns></returns>
        int GetLastItemNumber();

        /// <summary>
        /// Add new data to a specific item column name based on the code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="columnName"></param>
        /// <param name="newData"></param>
        void UpdateItem(string code, string columnName, string newData);

        /// <summary>
        /// Removes a full entry in the inventory
        /// </summary>
        /// <param name="inputSearch"></param>
        /// <param name="columnName"></param>
        void DeleteItemInDataTable(string inputSearch, string columnName);

        /// <summary>
        /// Update the number of items sold
        /// </summary>
        /// <param name="code"></param>
        /// <param name="unitsSold"></param>
        void UpdateSoldItemQuantity(string code, int unitsSold);

        /// <summary>
        /// Get product based on a code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IProduct GetProduct(string code);

        /// <summary>
        /// Get product based on the description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        IProduct GetProductFromDescription(string description);

        /// <summary>
        /// Update the sold product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        bool UpdateSoldProductToTable(IProduct product);

        /// <summary>
        /// Update product in the datatable
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        bool UpdateProductToTable(IProduct product);

        /// <summary>
        /// Add new product to data table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        bool AddNewProductToTable(IProduct product);

        /// <summary>
        /// Create a backup copy of the inventory file
        /// </summary>
        /// <param name="filePath"></param>
        void InventoryBackUp(string filePath);

        /// <summary>
        /// Search and returns a list of products
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<IProduct> Search(string input);

        //Inventory
        List<IProduct> GetProductList(string filePath, out string listName);
        
        #endregion
    }
}

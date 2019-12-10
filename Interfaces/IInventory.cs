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
        DataTable DictOfData { get; set; }
        string FilePath { get; set; }
        List<string> DbColumns { get; set; }

        bool AddNewProductToTable(IProduct product);
        void DeleteItemInDataTable(string inputSearch, string columnName);
        int GetLastItemNumber();
        IProduct GetProduct(string code);
   //     IProduct GetProductFromDescription(string description);
        List<IProduct> GetProductList(string filePath, out string listName);
        void LoadCsvToDataTable();
   //     string QueryDataFromCode(string code, string columnName);
        void SaveDataTableToCsv();
        List<IProduct> Search(string input, bool updateFromDataBase = true);
    //    void UpdateItem(string code, string columnName, string newData);
        bool UpdateProductToTable(IProduct product);

    }
}

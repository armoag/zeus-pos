using System.Collections.Generic;
using System.Data;

namespace Zeus
{
    public interface IDataBase
    {
        bool AddNewItemToTable(object product);
        object GetProduct(string searchInput);
    }
}
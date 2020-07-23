using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus
{
    public interface ISystemConfiguration
    {
        string Server { get; set; }
        string DataBaseName { get; set; }
        string UserID { get; set; }
        string Password { get; set; }
        string CustomerTableName { get; set; }
        string InventoryTableName { get; set; }
        string QueueTableName { get; set; }
        //Optional Features
        bool EmailTransactionsFileAfterEndSalesReport { get; set; }
        bool IntFlag { get; set; }
        bool LocalCustomers { get; set; }
        bool CloudCustomers { get; set; }
        bool LocalInventory { get; set; }
        bool CloudInventory { get; set; }
        bool IndirectPrice { get; set; }
        
        SystemTypeEnum SystemType { get; set; }
        //db type for inventory and customers only
        DatabaseTypeEnum DbType { get; set; }

    }
}

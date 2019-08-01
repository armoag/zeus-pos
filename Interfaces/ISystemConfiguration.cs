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
    }
}

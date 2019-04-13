using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericParsing;
using System.Data;
using System.IO;
using Shun;

namespace Zeus
{
    public interface ISqlDataBase
    {
        #region Properties
        string Server { get; set; }
        string SqlDataBase { get; set; }
        string Table { get; set; }
        string UserID { get; set; }
        string Password { get; set; }
        MySqlDatabase MySqlDataBase { get; set; }
        #endregion
    }
}

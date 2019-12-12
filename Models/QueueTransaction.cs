using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Shun;

namespace Zeus
{
    public class QueueTransaction : DataBase
    {
        #region DB Properties
        //DB Columns
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategory { get; set; }
        public string ProductDescription { get; set; }
        public decimal PriceSold { get; set; }
        public int UnitsSold { get; set; }
        public decimal TotalAmountSold { get; set; }
        public DateTime Date { get; set; }
        public string Customer { get; set; }
        public string Seller { get; set; }
        public bool FiscalReceiptRequired { get; set; }
        public int OrderNumber { get; set; }
        #endregion

        #region Properties
        public string DateString
        {
            get { return Utilities.FormatDateForMySql(Date); }
        }

        public string FiscalReceiptRequiredString
        {
            get
            {
                if (FiscalReceiptRequired) return "Si";
                return "No";
            }
        }
        public List<string> DbColumns
        {
            get
            {
                return new List<string>()
                {
                    "Id",
                    "Nombre",
                    "Codigo",
                    "Email",
                    "Telefono",
                    "FechaRegistro",
                    "RFC",
                    "PuntosDisponibles",
                    "PuntosUsados",
                    "TotalVisitas",
                    "TotalVendido",
                    "UltimaVisitaFecha"
                };
            }
        }
        private string DbPath
        {
            get
            {
                return _dbPath;
            }
            set
            {
                _dbPath = value;
            }
        }

        public MySqlDatabase MySqlData
        {
            get { return _mySqlData; }
            set { _mySqlData = value; }
        }
        #endregion

        #region Fields

        private MySqlDatabase _mySqlData;
        private string _dbPath;

        #endregion

        #region Constructors

        public QueueTransaction(string dbPath, MySqlDatabase mySqlDb) : base(dbPath)
        {
            //TODO: Check if path exists
            DbPath = dbPath;
            FilePath = dbPath;
            MySqlData = mySqlDb;
        }

        #endregion


        /// <summary>
        /// Register queue transaction
        /// </summary>
        public void Insert()
        {
            var colValPairs = new List<Tuple<string, string>>();
            colValPairs.Add(new Tuple<string, string>("Codigo", ProductCode));
            colValPairs.Add(new Tuple<string, string>("CategoriaProducto", ProductCategory));
            colValPairs.Add(new Tuple<string, string>("Descripcion", ProductCategory));
            colValPairs.Add(new Tuple<string, string>("PrecioVendido", PriceSold.ToString()));
            colValPairs.Add(new Tuple<string, string>("UnidadesVendidas", UnitsSold.ToString()));
            colValPairs.Add(new Tuple<string, string>("TotalVendido", PriceSold.ToString()));
            colValPairs.Add(new Tuple<string, string>("FechaVenta", Utilities.FormatDateForMySql(DateTime.Now)));
            colValPairs.Add(new Tuple<string, string>("Cliente", Customer));
            colValPairs.Add(new Tuple<string, string>("Vendedor", Seller));
            colValPairs.Add(new Tuple<string, string>("FacturaRequerida", FiscalReceiptRequiredString));
            colValPairs.Add(new Tuple<string, string>("NumeroPedido", OrderNumber.ToString()));
            MySqlData.Insert(colValPairs);
        }

        /// <summary>
        /// Delete queue transaction
        /// </summary>
        public void Delete(int id)
        { 
            MySqlData.Delete("Id", id.ToString());
        }

        public void DeleteOrder()
        {
            MySqlData.Delete("NumeroPedido", OrderNumber.ToString());
        }

        public void Update(string item, string parameter, string newData)
        {
            var updateData = new List<Tuple<string, string>>();
            updateData.Add(new Tuple<string, string>(parameter, newData));
            MySqlData.Update(parameter, item, updateData);
        }

        public List<QueueTransaction> Search(string searchInput)
         {
            var transactions = new List<QueueTransaction>();
            var columns = new List<string>
            {
                "Id",
                "Codigo",
                "CategoriaProducto",
                "Descripcion",
                "PrecioVendido",
                "UnidadesVendidas",
                "TotalVendido",
                "FechaVenta",
                "Cliente",
                "Vendedor",
                "FacturaRequerida",
                "NumeroPedido"
            };
            //Return empty list if invalid inputs are entered for the search
            if (string.IsNullOrWhiteSpace(searchInput) || searchInput == "x")
                return transactions;

            var allFields = MySqlData.SelectAll(columns).AsEnumerable();
            if (searchInput == "*")
            {
                foreach (var row in allFields)
                {
                    var transaction = new QueueTransaction(base.FilePath, MySqlData)
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        ProductCode = row["Codigo"].ToString(),
                        ProductCategory = row["CategoriaProducto"].ToString(),
                        ProductDescription = row["Descripcion"].ToString(),
                        PriceSold = Decimal.Parse(row["PrecioVendido"].ToString()),
                        UnitsSold = Int32.Parse(row["UnidadesVendidas"].ToString()),
                        TotalAmountSold = Decimal.Parse(row["TotalVendido"].ToString()),
                        Date = Convert.ToDateTime(row["FechaVenta"].ToString()),
                        Customer = row["Cliente"].ToString(),
                        Seller = row["Vendedor"].ToString(),
                        OrderNumber = Int32.Parse(row["NumeroPedido"].ToString())
                    };
                    FiscalReceiptRequired = row["FacturaRequerida"].ToString() == "Si";
                    transactions.Add(transaction);
                }
                return transactions;
            }
            return transactions;
        }

        public bool FullUpdate()
        {
            var columns = new List<string>
            {
                "Id",
                "Codigo",
                "CategoriaProducto",
                "Descripcion",
                "PrecioVendido",
                "UnidadesVendidas",
                "TotalVendido",
                "FechaVenta",
                "Cliente",
                "Vendedor",
                "FacturaRequerida",
                "NumeroPedido"
            };
            if (MySqlData != null)
            {
                var data = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(columns[1], this.ProductCode),
                    new Tuple<string, string>(columns[2], this.ProductCategory),
                    new Tuple<string, string>(columns[3], this.ProductDescription),
                    new Tuple<string, string>(columns[4], this.PriceSold.ToString()),
                    new Tuple<string, string>(columns[5], this.UnitsSold.ToString()),
                    new Tuple<string, string>(columns[6], this.TotalAmountSold.ToString()),
                    new Tuple<string, string>(columns[7], Utilities.FormatDateForMySql(this.Date)),
                    new Tuple<string, string>(columns[8], this.Customer),
                    new Tuple<string, string>(columns[9], this.Seller),
                    new Tuple<string, string>(columns[10], this.FiscalReceiptRequiredString),
                    new Tuple<string, string>(columns[11], this.OrderNumber.ToString())
                };
                MySqlData.Update("Id", this.Id.ToString(), data);
            }

            return true;
        }
    }
}

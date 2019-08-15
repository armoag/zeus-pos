using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Shun;

namespace Zeus
{
    public class Customer : DataBase, IBasicEntityInfo, ISqLDataBase
    {
        #region Fields
        private string _dbPath;

        private string _name;
        private string _code;
        private int _id;
        private string _email;
        private long _phone;
        private DateTime _registrationDate;

        private string _rfc;
        private double _pointsAvailable;
        private double _pointsUsed;
        private int _totalVisits;
        private decimal _totalSpent;
        private DateTime _lastVisitDate;
        private string _server;
        private string _userId;
        private string _password;
        private string _sqlDataBase;
        private string _table;
        private MySqlDatabase _mySqlData;

        #endregion

        #region Properties

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

        public string Name { get => _name; set => _name = value; }
        public int Id { get => _id; set => _id = value; }

        public string Code { get => _code; set => _code = value; }
        public string Email { get => _email; set => _email = value; }
        public long Phone { get => _phone; set => _phone = value; }
        public DateTime RegistrationDate { get => _registrationDate; set => _registrationDate = value; }

        public string Rfc { get => _rfc; set => _rfc = value; }
        public double PointsAvailable { get => _pointsAvailable; set => _pointsAvailable = value; }
        public double PointsUsed { get => _pointsUsed; set => _pointsUsed = value; }
        public int TotalVisits { get => _totalVisits; set => _totalVisits = value; }
        public decimal TotalSpent { get => _totalSpent; set => _totalSpent = value; }
        public DateTime LastVisitDate { get => _lastVisitDate; set => _lastVisitDate = value; }

        #region MySQL Properties

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string SqlDataBase
        {
            get { return _sqlDataBase; }
            set { _sqlDataBase = value; }
        }

        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public MySqlDatabase MySqlData
        {
            get { return _mySqlData; }
            set { _mySqlData = value; }
        }

        #endregion

        #endregion

        #region Constructors

        public Customer(string dbPath, MySqlDatabase mySqlDb) : base(dbPath)
        {
            //TODO: Check if path exists
            DbPath = dbPath;
            FilePath = dbPath;
            if (mySqlDb != null)
            {
                MySqlData = mySqlDb;
            }
            LoadCsvToDataTable();
        }

        public Customer(string dbPath, MySqlDatabase mySqlDb, string name, string code, string email, long phone, int id, string rfc, double pointsAvailable,
            double pointsUsed, int totalVisits, decimal totalSpent) : base(dbPath)
        {
            //TODO: Check if path exists
            DbPath = dbPath;
            Id = id;
            Name = name;
            Code = code;
            Email = email;
            Phone = phone;
            Rfc = rfc;
            PointsAvailable = pointsAvailable;
            PointsUsed = pointsUsed;
            TotalVisits = totalVisits;
            TotalSpent = totalSpent;
            if (mySqlDb != null)
            {
                MySqlData = mySqlDb;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register user to both local and remote databases
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mySqlDb"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="id"></param>
        /// <param name="rfc"></param>
        /// <param name="pointsAvailable"></param>
        /// <param name="pointsUsed"></param>
        /// <param name="totalVisits"></param>
        /// <param name="totalSpent"></param>
        public static void RegisterUser(string filePath, MySqlDatabase mySqlDb, string name, string code, string email, long phone, string id,
            string rfc, double pointsAvailable, double pointsUsed, int totalVisits, decimal totalSpent)
        {
            string readCode = string.Empty;
            if (mySqlDb != null)
            {
                var colValPairs = new List<Tuple<string, string>>();
                colValPairs.Add(new Tuple<string, string>("Nombre", name));
                colValPairs.Add(new Tuple<string, string>("Codigo", code));
                colValPairs.Add(new Tuple<string, string>("Email", email));
                colValPairs.Add(new Tuple<string, string>("FechaRegistro", Utilities.FormatDateForMySql(DateTime.Now)));
                colValPairs.Add(new Tuple<string, string>("Telefono", phone.ToString()));
                colValPairs.Add(new Tuple<string, string>("RFC", rfc));
                colValPairs.Add(new Tuple<string, string>("PuntosDisponibles", pointsAvailable.ToString()));
                colValPairs.Add(new Tuple<string, string>("PuntosUsados", pointsUsed.ToString()));
                colValPairs.Add(new Tuple<string, string>("TotalVisitas", totalVisits.ToString()));
                colValPairs.Add(new Tuple<string, string>("TotalVendido", totalSpent.ToString()));
                colValPairs.Add(new Tuple<string, string>("UltimaVisitaFecha", Utilities.FormatDateForMySql(DateTime.Now)));
                mySqlDb.Insert(colValPairs);
                //Get id for local record
                var readData = new List<Tuple<string, List<Tuple<string, string>>>>();
                mySqlDb.Read("Telefono", phone.ToString(), out readData);
                if (readData != null) readCode = (readData[0].Item2)[0].Item2;
            }

            if (readCode != string.Empty)
            {
                int readId = 0;
                Int32.TryParse(readCode, out readId);
                if (readId != 0) id = readId.ToString();
            }

            //TODO: Check if username already exists
            //TODO: Implement feature
            string data = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", id, name, code,
                              email, phone.ToString(), DateTime.Now.ToString(), rfc, pointsAvailable.ToString(), pointsUsed.ToString(),
                              totalVisits.ToString(), totalSpent.ToString(), DateTime.Now.ToString())
                          + Environment.NewLine;

            File.AppendAllText(filePath, data);
        }

        /// <summary>
        /// Register user to both local and remote database
        /// </summary>
        public void Register()
        {
            string readCode = string.Empty;
            if (MySqlData != null)
            {
                var colValPairs = new List<Tuple<string, string>>();
                colValPairs.Add(new Tuple<string, string>("Nombre", Name));
                colValPairs.Add(new Tuple<string, string>("Codigo", Code));
                colValPairs.Add(new Tuple<string, string>("Email", Email));
                colValPairs.Add(new Tuple<string, string>("FechaRegistro", Utilities.FormatDateForMySql(DateTime.Now)));
                colValPairs.Add(new Tuple<string, string>("Telefono", Phone.ToString()));
                colValPairs.Add(new Tuple<string, string>("RFC", Rfc));
                colValPairs.Add(new Tuple<string, string>("PuntosDisponibles", PointsAvailable.ToString()));
                colValPairs.Add(new Tuple<string, string>("PuntosUsados", PointsUsed.ToString()));
                colValPairs.Add(new Tuple<string, string>("TotalVisitas", TotalVisits.ToString()));
                colValPairs.Add(new Tuple<string, string>("TotalVendido", TotalSpent.ToString()));
                colValPairs.Add(new Tuple<string, string>("UltimaVisitaFecha", Utilities.FormatDateForMySql(DateTime.Now)));
                MySqlData.Insert(colValPairs);
                //Get id for local record
                var readData = new List<Tuple<string, List<Tuple<string, string>>>>();
                MySqlData.Read("Telefono", Phone.ToString(), out readData);
                if (readData != null) readCode = (readData[0].Item2)[0].Item2;
            }

            if (readCode != string.Empty)
            {
                int id = 0;
                Int32.TryParse(readCode, out id);
                if (id != 0) Id = id;
            }

            string data = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", Id, Name, Code,
                 Email, Phone.ToString(), DateTime.Now.ToString(), Rfc, PointsAvailable.ToString(), PointsUsed.ToString(),
                 TotalVisits.ToString(), TotalSpent.ToString(), DateTime.Now.ToString())
                 + Environment.NewLine;

            File.AppendAllText(DbPath, data);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        public void Delete()
        {
            base.RemoveEntryInDataTable(this.Id.ToString(), "Id");
            if (MySqlData != null)
            {
                MySqlData.Delete("Id", Id.ToString());
            }
        }

        public void FullUpdate(string filePath, string name, string email, long phone, string id,
            string rfc, double pointsAvailable, double pointsUsed, int totalVisits, decimal totalSpent)
        {
            //TODO: Check if exists, and update if valid
        }

        //public Customer GetByPhoneNumber()
        //{
        //    //Look for user by username
        //    return new Customer("a");
        //}

        //public Customer GetByID()
        //{
        //    //Look for user by username
        //    return new Customer("a");
        //}

        //public Customer GetByName()
        //{
        //    //Look for user by username
        //    return new Customer("a");
        //}

        public void Update(string item, string parameter, string newData)
        {
            base.UpdateDataFieldInDataTable(item, parameter, newData);
            if (MySqlData != null)
            {
                var updateData = new List<Tuple<string, string>>();
                updateData.Add(new Tuple<string, string>(parameter, newData));
                MySqlData.Update(parameter, item, updateData);
            }
        }

        public void Get(string item)
        {
            ///TODO Needed???
        }

        public int GetLastItemNumber()
        {
            if (MySqlData != null)
            {
                //TODO Check ID of the last entry
            }

            if (DataTable.Rows.Count == 0)
                return 0;
            var row = DataTable.Rows[DataTable.Rows.Count - 1];
            return Int32.Parse(row["Id"].ToString());
        }

        public List<Customer> Search(string searchInput)
        {
            var customers = new List<Customer>();
            var columns = new List<string>
            {
                "Id",
                "Nombre",
                "Email",
                "Telefono",
                "FechaRegistro",
                "RFC",
                "PuntosDisponibles",
                "PuntosUsados",
                "TotalVisitas",
                "TotalVendido",
                "UltimaVisitaFecha",
                "Codigo"
            };
            //Return empty list if invalid inputs are entered for the search
            if (string.IsNullOrWhiteSpace(searchInput) || searchInput == "x")
                return customers;

            if (MySqlData != null)
            {
                var allFields = MySqlData.SelectAll(columns).AsEnumerable();
                if (searchInput == "*")
                {
                    foreach (var row in allFields)
                    {
                        var customer = new Customer(base.FilePath, MySqlData)
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Name = row["Nombre"].ToString(),
                            Code = row["Codigo"].ToString(),
                            Email = row["Email"].ToString(),
                            Phone = long.Parse(row["Telefono"].ToString()),
                            RegistrationDate = Convert.ToDateTime(row["FechaRegistro"].ToString()),
                            Rfc = row["RFC"].ToString(),
                            PointsAvailable = double.Parse(row["PuntosDisponibles"].ToString()),
                            PointsUsed = double.Parse(row["PuntosUsados"].ToString()),
                            TotalVisits = Int32.Parse(row["TotalVisitas"].ToString()),
                            TotalSpent = Decimal.Parse(row["TotalVendido"].ToString()),
                            LastVisitDate = Convert.ToDateTime(row["UltimaVisitaFecha"].ToString())
                        };
                        customers.Add(customer);
                    }
                    return customers;
                }

                var phoneFilter = allFields.Where(r => r.Field<string>("Telefono").ToLower().Contains(searchInput));
                var nameFilter = allFields.Where(r => r.Field<string>("Nombre").ToLower().Contains(searchInput));

                foreach (var row in phoneFilter)
                {
                    var customer = new Customer(base.FilePath, MySqlData)
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Name = row["Nombre"].ToString(),
                        Code = row["Codigo"].ToString(),
                        Email = row["Email"].ToString(),
                        Phone = long.Parse(row["Telefono"].ToString()),
                        RegistrationDate = Convert.ToDateTime(row["FechaRegistro"].ToString()),
                        Rfc = row["RFC"].ToString(),
                        PointsAvailable = double.Parse(row["PuntosDisponibles"].ToString()),
                        PointsUsed = double.Parse(row["PuntosUsados"].ToString()),
                        TotalVisits = Int32.Parse(row["TotalVisitas"].ToString()),
                        TotalSpent = Decimal.Parse(row["TotalVendido"].ToString()),
                        LastVisitDate = Convert.ToDateTime(row["UltimaVisitaFecha"].ToString())
                    };

                    customers.Add(customer);
                }

                foreach (var row in nameFilter)
                {
                    var customer = new Customer(base.FilePath, MySqlData)
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Name = row["Nombre"].ToString(),
                        Code = row["Codigo"].ToString(),
                        Email = row["Email"].ToString(),
                        Phone = long.Parse(row["Telefono"].ToString()),
                        RegistrationDate = Convert.ToDateTime(row["FechaRegistro"].ToString()),
                        Rfc = row["RFC"].ToString(),
                        PointsAvailable = double.Parse(row["PuntosDisponibles"].ToString()),
                        PointsUsed = double.Parse(row["PuntosUsados"].ToString()),
                        TotalVisits = Int32.Parse(row["TotalVisitas"].ToString()),
                        TotalSpent = Decimal.Parse(row["TotalVendido"].ToString()),
                        LastVisitDate = Convert.ToDateTime(row["UltimaVisitaFecha"].ToString())
                    };

                    //Add if it does not exist already
                    if (!customers.Exists(x => x.Phone == customer.Phone))
                        customers.Add(customer);
                }
            }
            else
            {
                if (searchInput == "*")
                {
                    var allFields = base.DataTable.AsEnumerable();
                    foreach (var row in allFields)
                    {
                        var customer = new Customer(base.FilePath, MySqlData)
                        {
                            Id = Int32.Parse(row["Id"].ToString()),
                            Name = row["Nombre"].ToString(),
                            Code = row["Codigo"].ToString(),
                            Email = row["Email"].ToString(),
                            Phone = long.Parse(row["Telefono"].ToString()),
                            RegistrationDate = Convert.ToDateTime(row["FechaRegistro"].ToString()),
                            Rfc = row["RFC"].ToString(),
                            PointsAvailable = double.Parse(row["PuntosDisponibles"].ToString()),
                            PointsUsed = double.Parse(row["PuntosUsados"].ToString()),
                            TotalVisits = Int32.Parse(row["TotalVisitas"].ToString()),
                            TotalSpent = Decimal.Parse(row["TotalVendido"].ToString()),
                            LastVisitDate = Convert.ToDateTime(row["UltimaVisitaFecha"].ToString())
                        };
                        customers.Add(customer);
                    }
                    return customers;
                }

                var phoneFilter = base.DataTable.AsEnumerable().Where(r => r.Field<string>("Telefono").ToLower().Contains(searchInput));
                var nameFilter = base.DataTable.AsEnumerable().Where(r => r.Field<string>("Nombre").ToLower().Contains(searchInput));

                foreach (var row in phoneFilter)
                {
                    var customer = new Customer(base.FilePath, MySqlData)
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Name = row["Nombre"].ToString(),
                        Code = row["Codigo"].ToString(),
                        Email = row["Email"].ToString(),
                        Phone = long.Parse(row["Telefono"].ToString()),
                        RegistrationDate = Convert.ToDateTime(row["FechaRegistro"].ToString()),
                        Rfc = row["RFC"].ToString(),
                        PointsAvailable = double.Parse(row["PuntosDisponibles"].ToString()),
                        PointsUsed = double.Parse(row["PuntosUsados"].ToString()),
                        TotalVisits = Int32.Parse(row["TotalVisitas"].ToString()),
                        TotalSpent = Decimal.Parse(row["TotalVendido"].ToString()),
                        LastVisitDate = Convert.ToDateTime(row["UltimaVisitaFecha"].ToString())
                    };

                    customers.Add(customer);
                }

                foreach (var row in nameFilter)
                {
                    var customer = new Customer(base.FilePath, MySqlData)
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        Name = row["Nombre"].ToString(),
                        Code = row["Codigo"].ToString(),
                        Email = row["Email"].ToString(),
                        Phone = long.Parse(row["Telefono"].ToString()),
                        RegistrationDate = Convert.ToDateTime(row["FechaRegistro"].ToString()),
                        Rfc = row["RFC"].ToString(),
                        PointsAvailable = double.Parse(row["PuntosDisponibles"].ToString()),
                        PointsUsed = double.Parse(row["PuntosUsados"].ToString()),
                        TotalVisits = Int32.Parse(row["TotalVisitas"].ToString()),
                        TotalSpent = Decimal.Parse(row["TotalVendido"].ToString()),
                        LastVisitDate = Convert.ToDateTime(row["UltimaVisitaFecha"].ToString())
                    };

                    //Add if it does not exist already
                    if (!customers.Exists(x => x.Phone == customer.Phone))
                        customers.Add(customer);
                }
            }

            return customers;
        }

        /// <summary>
        /// Update customer in the datatable
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdateUserToTable(Customer customer)
        {
            var columns = new List<string>
            {
                "Id",
                "Nombre",
                "Email",
                "Telefono",
                "FechaRegistro",
                "RFC",
                "PuntosDisponibles",
                "PuntosUsados",
                "TotalVisitas",
                "TotalVendido",
                "UltimaVisitaFecha",
                "Codigo"
            };
            if (MySqlData != null)
            {
                var data = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(columns[1], customer.Name),
                    new Tuple<string, string>(columns[2], customer.Email),
                    new Tuple<string, string>(columns[3], customer.Phone.ToString()),
                    new Tuple<string, string>(columns[4], Utilities.FormatDateForMySql(customer.RegistrationDate)),
                    new Tuple<string, string>(columns[5], customer.Rfc),
                    new Tuple<string, string>(columns[6], customer.PointsAvailable.ToString()),
                    new Tuple<string, string>(columns[7], customer.PointsUsed.ToString()),
                    new Tuple<string, string>(columns[8], customer.TotalVisits.ToString()),
                    new Tuple<string, string>(columns[9], customer.TotalSpent.ToString()),
                    new Tuple<string, string>(columns[10], Utilities.FormatDateForMySql(customer.LastVisitDate)),
                    new Tuple<string, string>(columns[11], customer.Code)
                };
                MySqlData.Update("Id", customer.Id.ToString(), data);
            }

            for (int index = 0; index < DataTable.Rows.Count; index++)
            {
                var row = DataTable.Rows[index];
                if (row["Id"].ToString() == customer.Id.ToString())
                {
                    row["Nombre"] = customer.Name;
                    row["Codigo"] = customer.Code;
                    row["Email"] = customer.Email;
                    row["Telefono"] = customer.Phone;
                    row["FechaRegistro"] = customer.RegistrationDate;
                    row["RFC"] = customer.Rfc;
                    row["PuntosDisponibles"] = customer.PointsAvailable.ToString();
                    row["PuntosUsados"] = customer.PointsUsed.ToString();
                    row["TotalVisitas"] = customer.TotalVisits.ToString();
                    row["TotalVendido"] = customer.TotalSpent.ToString();
                    row["UltimaVisitaFecha"] = customer.LastVisitDate.ToString();
                }
            }

            return true;
        }

        /// <summary>
        /// Update customer in the datatable
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdateUserToTable()
        {
            var columns = new List<string>
            {
                "Id",
                "Nombre",
                "Email",
                "Telefono",
                "FechaRegistro",
                "RFC",
                "PuntosDisponibles",
                "PuntosUsados",
                "TotalVisitas",
                "TotalVendido",
                "UltimaVisitaFecha",
                "Codigo"
            };
            if (MySqlData != null)
            {
                var data = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(columns[1], this.Name),
                    new Tuple<string, string>(columns[2], this.Email),
                    new Tuple<string, string>(columns[3], this.Phone.ToString()),
                    new Tuple<string, string>(columns[4], Utilities.FormatDateForMySql(this.RegistrationDate)),
                    new Tuple<string, string>(columns[5], this.Rfc),
                    new Tuple<string, string>(columns[6], this.PointsAvailable.ToString()),
                    new Tuple<string, string>(columns[7], this.PointsUsed.ToString()),
                    new Tuple<string, string>(columns[8], this.TotalVisits.ToString()),
                    new Tuple<string, string>(columns[9], this.TotalSpent.ToString()),
                    new Tuple<string, string>(columns[10], Utilities.FormatDateForMySql(this.LastVisitDate)),
                    new Tuple<string, string>(columns[11], this.Code)
                };
                MySqlData.Update("Id", this.Id.ToString(), data);
            }

            for (int index = 0; index < DataTable.Rows.Count; index++)
            {
                var row = DataTable.Rows[index];
                if (row["Id"].ToString() == this.Id.ToString())
                {
                    row["Nombre"] = this.Name;
                    row["Codigo"] = this.Code;
                    row["Email"] = this.Email;
                    row["Telefono"] = this.Phone;
                    row["FechaRegistro"] = this.RegistrationDate;
                    row["RFC"] = this.Rfc;
                    row["PuntosDisponibles"] = this.PointsAvailable.ToString();
                    row["PuntosUsados"] = this.PointsUsed.ToString();
                    row["TotalVisitas"] = this.TotalVisits.ToString();
                    row["TotalVendido"] = this.TotalSpent.ToString();
                    row["UltimaVisitaFecha"] = this.LastVisitDate.ToString();
                }
            }
            return true;
        }


        /// <summary>
        /// Add new product to data table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool AddUserToTable(Customer customer)
        {
            ///TODO:Depricate in the future rev
            DataTable.Rows.Add();
            var row = DataTable.Rows[DataTable.Rows.Count - 1];
            row["Id"] = customer.GetLastItemNumber() + 1;
            row["Nombre"] = customer.Name;
            row["Codigo"] = customer.Code;
            row["Email"] = customer.Email;
            row["FechaRegistro"] = customer.RegistrationDate.ToString();
            row["Telefono"] = customer.Phone;
            row["RFC"] = customer.Rfc;
            row["PuntosDisponibles"] = customer.PointsAvailable.ToString();
            row["PuntosUsados"] = customer.PointsUsed.ToString();
            row["TotalVisitas"] = customer.TotalVisits.ToString();
            row["TotalVendido"] = customer.TotalSpent.ToString();
            row["UltimaVisitaFecha"] = customer.LastVisitDate.ToString();
            return true;
        }
        #endregion
    }
}

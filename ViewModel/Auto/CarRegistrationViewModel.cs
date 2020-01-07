using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Zeus.WpfBindingUtilities;

namespace Zeus
{
    public class CarRegistrationViewModel : ObservableObject
    {
        #region Fields
        private static CarRegistrationViewModel _carRegistrationInstance = null;

        private ObservableCollection<CarPart> _carPartsSearchedEntries;
        private ObservableCollection<CarPart> _carParts;
        private CarPart _car;
        private ObservableCollection<string> _carBrandsList;
        private ObservableCollection<string> _locationsList;
        private ObservableCollection<string> _transmissionsList;
        private string _currentPage;
        private IProduct _selectedCarPart;
        private string _partsSearchText;
        #endregion

        #region Constructors

        public static CarRegistrationViewModel GetInstance()
        {
            if (_carRegistrationInstance == null)
                _carRegistrationInstance = new CarRegistrationViewModel();
            else
            {
                _carRegistrationInstance.CurrentPage =
                    Constants.CarRegistrationMainPage;  //"\\View\\CarRegistrationInfoPage.xaml";
            }
            return _carRegistrationInstance;
        }

        public CarRegistrationViewModel()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");

            _carPartsSearchedEntries = new ObservableCollection<CarPart>();
            //_carPartsSearchedEntries.Add(new CarPart()
            //{
            //    Id = 1,
            //    Category = "interior",
            //    Code = "123x",
            //    Description = "Puerta",
            //    Model = "Honda 1998",
            //    Vin = "FDG43BDBSG1435",
            //    Price = 100M,
            //    PriceCurrency = CurrencyTypeEnum.USD,
            //    Enabled = true,
            //    TotalQuantityAvailable = 4
            //});
            //_carPartsSearchedEntries.Add(new CarPart()
            //{
            //    Id = 1,
            //    Category = "interior",
            //    Code = "12343x",
            //    Description = "Cofre",
            //    Model = "Honda 1998",
            //    Vin = "FDG43BDBSG1435",
            //    Price = 150M,
            //    PriceCurrency = CurrencyTypeEnum.USD,
            //    Enabled = true,
            //    TotalQuantityAvailable = 1
            //});
            var carList = FileIO.GetListFromFile(Constants.DataFolderPath + Constants.CarBrandListFileName);
            var transList = FileIO.GetListFromFile(Constants.DataFolderPath + Constants.TransmissiondListFileName);
            var locationList = FileIO.GetListFromFile(Constants.DataFolderPath + Constants.LocationListFileName);

            CarBrandsList = new ObservableCollection<string>(carList);
            TransmissionsList = new ObservableCollection<string>(transList);
            LocationsList = new ObservableCollection<string>(locationList);

            //Set default states
            Car = new CarPart()
            {
                CostCurrency = CurrencyTypeEnum.USD,
                ImportCostCurrency = CurrencyTypeEnum.USD,
                LastPurchaseDate = DateTime.Now,
                Make = carList[0],
                Transmission = transList[1],
                Location = locationList[0],
            };

            CurrentPage = Constants.CarRegistrationMainPage; //"\\View\\CarRegistrationInfoPage.xaml";
        }

        #endregion

        #region Observable Properties

        public ObservableCollection<CarPart> CarPartsSearchedEntries
        {
            get { return _carPartsSearchedEntries; }
            set
            {
                _carPartsSearchedEntries = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CarPart> CarParts
        {
            get { return _carParts; }
            set
            {
                _carParts = value;
                OnPropertyChanged();
            }
        }

        public CarPart Car
        {
            get { return _car; }
            set
            {
                _car = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> CarBrandsList
        {
            get { return _carBrandsList; }
            set
            {
                _carBrandsList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> LocationsList
        {
            get { return _locationsList; }
            set
            {
                _locationsList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> TransmissionsList
        {
            get { return _transmissionsList; }
            set
            {
                _transmissionsList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Holds current page
        /// </summary>
        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private IList<CurrencyTypeEnum> _currencyTypes;
        public IList<CurrencyTypeEnum> CurrencyTypes
        {
            get { return Enum.GetValues(typeof(CurrencyTypeEnum)).Cast<CurrencyTypeEnum>().ToList(); }
            set
            {
                _currencyTypes = value;
                OnPropertyChanged();
            }
        }

        public IProduct SelectedCarPart
        {
            get { return _selectedCarPart; }
            set
            {
                _selectedCarPart = value;
                OnPropertyChanged();
            }
        }

        public string PartsSearchText
        {
            get { return _partsSearchText; }
            set { _partsSearchText = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Commands

        #region RegisterCarCommand
        public ICommand RegisterCarCommand { get { return _registerCarCommand ?? (_registerCarCommand = new DelegateCommand(Execute_RegisterCarCommand, CanExecute_RegisterCarCommand)); } }
        private ICommand _registerCarCommand;

        internal void Execute_RegisterCarCommand(object parameter)
        {
            foreach (var carPart in CarParts)
            {
                if (carPart.Valid)
                {
                    if(MainWindowViewModel.SystemConfig.LocalInventory) carPart.Id = MainWindowViewModel.InventoryInstance.GetLastItemNumber() + 1;
                    MainWindowViewModel.InventoryInstance.AddNewProductToTable(carPart);
                }
            }
            if (MainWindowViewModel.SystemConfig.LocalInventory) MainWindowViewModel.InventoryInstance.SaveDataTableToCsv();

            MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.PosGeneralPage;
        }

        internal bool CanExecute_RegisterCarCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region StartCarRegistrationCommand
        public ICommand StartCarRegistrationCommand { get { return _startCarRegistrationCommand ?? (_startCarRegistrationCommand = new DelegateCommand(Execute_StartCarRegistrationCommand, CanExecute_StartCarRegistrationCommand)); } }
        private ICommand _startCarRegistrationCommand;

        internal void Execute_StartCarRegistrationCommand(object parameter)
        {
            var parts = CarPart.ReadPartsFile(Constants.DataFolderPath + Constants.DefaultPartsListFileName);
            CarParts = new ObservableCollection<CarPart>(CarPart.CreateCarParts(Car, parts));
            CarPartsSearchedEntries = CarParts;
            MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.CarRegistrationListPage;
        }

        internal bool CanExecute_StartCarRegistrationCommand(object parameter)
        {
            return Car.Vin != "" && Car.Make != null && Car.Model != "" && Car.Motor != "" && Car.Cost != 0M &&
                   Car.Year != 0 && Car.ImportCost != 0M &&
                   Car.Color != "" && Car.Transmission != null && Car.Provider != "" && Car.Location != null &&
                   Car.SpecificLocation != "";
        }
        #endregion

        #region ImportExportCarCommand
        public ICommand ImportExportCarCommand { get { return _importExportCarCommand ?? (_importExportCarCommand = new DelegateCommand(Execute_ImportExportCarCommand, CanExecute_ImportExportCarCommand)); } }
        private ICommand _importExportCarCommand;

        internal void Execute_ImportExportCarCommand(object parameter)
        {
            if ((string) parameter == "import")
            {
                var openFileDialog = new OpenFileDialog()
                {
                    Filter = ".csv files (*.csv)|*.csv",
                    Title = "Selecciona archivo de partes de carros",
                    InitialDirectory = @"C:\"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var fileName = openFileDialog.FileName;
                    var parts = CarPart.ReadPartsFile(fileName);
                    //var car = new CarPart()
                    //{
                    //    Vin = "10000",
                    //    Make = "Honda",
                    //    Model = "Civic",
                    //    Year = 2010,
                    //    Color = "Azul",
                    //    Transmission = "Std",
                    //    Motor = "1.8T"
                    //};
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(CarPart.CreateCarParts(Car, parts));
                    CarParts = new ObservableCollection<CarPart>(CarPart.CreateCarParts(Car, parts));
                }
            }
            else if ((string) parameter == "export")
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = ".csv files (*.csv)|*.csv",
                    Title = "Guarda archivo de partes de carro",
                    InitialDirectory = @"C:\"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var carParts = new List<Tuple<string, string, int, decimal, CurrencyTypeEnum, bool>>();
                    foreach (var carPart in CarParts)
                    {
                        //remove detailed information to create default list
                        //remove model
                        var desc = carPart.Description.Replace(Car.Model, "");
                        //remove year
                        desc = desc.Replace(Car.Year.ToString(), "");
                        //remove vin
                        var firstSpaceIndex = desc.IndexOf(" ", StringComparison.Ordinal);
                        desc = desc.Remove(0, firstSpaceIndex + 1);
                        //remove spaces
                        desc = desc.TrimStart(' ');

                        carParts.Add(new Tuple<string, string, int, decimal, CurrencyTypeEnum, bool>(desc, carPart.Category,
                            carPart.TotalQuantityAvailable, carPart.Price, carPart.PriceCurrency, carPart.Valid));
                    }
                    CarPart.WritePartsFile(saveFileDialog.FileName, carParts);
                }
            }
        }

        internal bool CanExecute_ImportExportCarCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region ViewDetailsPartCommand
        public ICommand ViewDetailsPartCommand { get { return _viewDetailsPartCommand ?? (_viewDetailsPartCommand = new DelegateCommand(Execute_ViewDetailsPartCommand, CanExecute_ViewDetailsPartCommand)); } }
        private ICommand _viewDetailsPartCommand;

        internal void Execute_ViewDetailsPartCommand(object parameter)
        {
            MainWindowViewModel.GetInstance(null, null).InventoryTemporalItem = SelectedCarPart;
            MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.InventoryItemPage;
        }

        internal bool CanExecute_ViewDetailsPartCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region ChangePageCommand
        public ICommand ChangePageCommand { get { return _changePageCommand ?? (_changePageCommand = new DelegateCommand(Execute_ChangePageCommand, CanExecute_ChangePageCommand)); } }
        private ICommand _changePageCommand;

        internal void Execute_ChangePageCommand(object parameter)
        {
            switch ((string) parameter)
            {
                case "cancel":
                    MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.PosGeneralPage;
                    break;
                case "part_list":
                    MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.CarRegistrationListPage;
                    break;
                case "car_main":
                    MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.CarRegistrationMainPage;
                    break;
            }
        }

        internal bool CanExecute_ChangePageCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region SearchListCommand

        public ICommand SearchListCommand { get { return _searchListCommand ?? (_searchListCommand = new DelegateCommand(Execute_SearchListCommand, CanExecute_SearchListCommand)); } }
        private ICommand _searchListCommand;

        internal void Execute_SearchListCommand(object parameter)
        {
            IEnumerable<CarPart> catFilter;
            var products = new List<CarPart>();
            //parameter is the button name
            switch ((string)parameter)
            {
                    
                case "freesearch":
                    //var descriptionFilter = DictOfData.AsEnumerable().Where(r => r.Field<string>("Descripcion").ToLower().Contains(input));
                    //var codeFilter = DictOfData.AsEnumerable().Where(r => r.Field<string>("Codigo").ToLower().Contains(input));
                    if (PartsSearchText == "*" || PartsSearchText == " " || PartsSearchText == null)
                    {
                        CarPartsSearchedEntries = CarParts;
                        break;
                    }

                    var descriptionFilter = CarParts.AsEnumerable().Where(r => r.Description.ToLower().Contains(PartsSearchText.ToLower()));

                    foreach (var carPart in descriptionFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    var categoryFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains(PartsSearchText.ToLower()));

                    foreach (var carPart in categoryFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }


                    var priceFilter = CarParts.AsEnumerable().Where(r => r.Price.ToString().Contains(PartsSearchText.ToLower()));

                    foreach (var carPart in priceFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "a":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Carroceria".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "b":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Suspension D".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "c":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Suspension T".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "d":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Interior".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "e":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Exterior".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "f":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Motor".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "g":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Transmision".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "h":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Cajuela".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;

                case "i":

                    catFilter = CarParts.AsEnumerable().Where(r => r.Category.ToLower().Contains("Otros".ToLower()));

                    foreach (var carPart in catFilter)
                    {
                        //Add if it does not exist already
                        if (!products.Exists(x => x.Code == carPart.Code))
                            products.Add(carPart);
                    }

                    CarPartsSearchedEntries = null;
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(products);

                    break;
            }
        }

        internal bool CanExecute_SearchListCommand(object parameter)
        {
            return true;
        }
        #endregion

        #endregion
    }
}

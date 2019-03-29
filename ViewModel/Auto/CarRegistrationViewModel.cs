using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private CarPart _car;
        private ObservableCollection<string> _carBrandsList;
        private ObservableCollection<string> _locationsList;
        private ObservableCollection<string> _transmissionsList;
        private string _currentPage;
        private IProduct _selectedCarPart;
        #endregion

        #region Constructors

        public static CarRegistrationViewModel GetInstance()
        {
            if (_carRegistrationInstance == null)
                _carRegistrationInstance = new CarRegistrationViewModel();
            else
            {
                _carRegistrationInstance.CurrentPage = "\\View\\CarRegistrationInfoPage.xaml";
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

                //Temportal Items
                Vin = "15641",
                Cost = 10M,
                ImportCost = 1M,
                Make = carList[0],
                Transmission = transList[0],
                Location = locationList[0],
                SpecificLocation = "1A",
                Motor = "2.0",
                Color = "Blanco",
                Provider = "SDParts",
                Year = 2012,
                Model = "Civic"
            };

            CurrentPage = "\\View\\CarRegistrationInfoPage.xaml";
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

        #endregion

        #region Methods

        #endregion

        #region Commands

        #region RegisterCarCommand
        public ICommand RegisterCarCommand { get { return _registerCarCommand ?? (_registerCarCommand = new DelegateCommand(Execute_RegisterCarCommand, CanExecute_RegisterCarCommand)); } }
        private ICommand _registerCarCommand;

        internal void Execute_RegisterCarCommand(object parameter)
        {
            foreach (var carPart in CarPartsSearchedEntries)
            {
                MainWindowViewModel.InventoryInstance.AddNewProductToTable(carPart);
            }
            MainWindowViewModel.InventoryInstance.SaveDataTableToCsv();
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
            CarPartsSearchedEntries = new ObservableCollection<CarPart>(CarPart.CreateCarParts(Car, parts)); ;
            MainWindowViewModel.GetInstance(null, null).CurrentPage = "\\View\\CarRegistrationListPage.xaml";
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
                    InitialDirectory = @"C:\Projects\"
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
                    CarPartsSearchedEntries = new ObservableCollection<CarPart>(CarPart.CreateCarParts(Car, parts));;
                }
            }
            else if ((string) parameter == "export")
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = ".csv files (*.csv)|*.csv",
                    Title = "Guarda archivo de partes de carro",
                    InitialDirectory = @"C:\Projects\"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var carParts = new List<Tuple<string, string, int, decimal, CurrencyTypeEnum>>();
                    foreach (var carPart in CarPartsSearchedEntries)
                    {
                        carParts.Add(new Tuple<string, string, int, decimal, CurrencyTypeEnum>(carPart.Description, carPart.Category,
                            carPart.TotalQuantityAvailable, carPart.Price, carPart.PriceCurrency));
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
            MainWindowViewModel.GetInstance(null, null).CurrentPage = "\\View\\InventoryItemPage.xaml";
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
                    MainWindowViewModel.GetInstance(null, null).CurrentPage = "\\View\\PosGeneralPage.xaml";
                    break;
                case "part_list":
                    MainWindowViewModel.GetInstance(null, null).CurrentPage = "\\View\\CarRegistrationListPage.xaml";
                    break;
                case "car_main":
                    MainWindowViewModel.GetInstance(null, null).CurrentPage = "\\View\\CarRegistrationMainPage.xaml";
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
            //parameter is the button name
            switch ((string)parameter)
            {
                case "a":
                    break;
                case "b":
                    break;
                case "c":
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

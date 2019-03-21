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
using Zeus.WpfBindingUtilities;

namespace Zeus
{
    public class CarRegistrationViewModel : ObservableObject
    {
        #region Fields
        private static CarRegistrationViewModel _carRegistrationInstance = null;

        private ObservableCollection<CarPart> _carPartsSearchedEntries;
        private CarPart _selectedCarPart;
        private string _currentPage;
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

        public CarPart SelectedCarPart
        {
            get { return _selectedCarPart; }
            set
            {
                _selectedCarPart = value;
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
        #endregion

        #region Methods

        #endregion

        #region Commands

        #region RegisterCarCommand
        public ICommand RegisterCarCommand { get { return _registerCarCommand ?? (_registerCarCommand = new DelegateCommand(Execute_RegisterCarCommand, CanExecute_RegisterCarCommand)); } }
        private ICommand _registerCarCommand;

        internal void Execute_RegisterCarCommand(object parameter)
        {
            var x = 1;
            var y = CarPartsSearchedEntries;
        }

        internal bool CanExecute_RegisterCarCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region ImportExportCarCommand
        public ICommand ImportExportCarCommand { get { return _importExportCarCommand ?? (_importExportCarCommand = new DelegateCommand(Execute_ImportExportCarCommand, CanExecute_ImportExportCarCommand)); } }
        private ICommand _importExportCarCommand;

        internal void Execute_ImportExportCarCommand(object parameter)
        {
            if ((string) parameter == "import")
            {

            }
            else if ((string) parameter == "export")
            {

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
                case "search_list":
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

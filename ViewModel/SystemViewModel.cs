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
using System.Windows.Documents;
using Zeus.WpfBindingUtilities;
using System.Windows.Input;

namespace Zeus
{
    /// <summary>
    /// View model to access data regarding POS detailed information
    /// </summary>
    public class SystemViewModel : ObservableObject
    {
        #region Fields

        private Pos _posInstance;

        private string _printerName;
        private string _fiscalNumber;
        private string _fiscalName;
        private string _address;
        private string _city;
        private string _phoneNumber;
        private string _email;
        private string _facebook;
        private string _instagram;
        private string _website;
        private string _footerMessage;
        private string _version;
        private string _emailSender;
        private string _emailSenderPassword;
        private string _emailReports;
        private string _emailOrders;
        private decimal _discountPercent;
        private decimal _pointsPercent;
        private string _comments;
        private string _businessName;
        private ObservableCollection<string> _printers;

        #endregion

        #region Constructors

        public SystemViewModel()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");

            //Initialize POS information
            //TODO: Update this file to XML in the future
            _posInstance = Pos.GetInstance(Constants.DataFolderPath + Constants.PosDataFileName);

            PrinterName = _posInstance.PrinterName;
            BusinessName = _posInstance.BusinessName;
            FiscalNumber = _posInstance.FiscalNumber;
            FiscalName = _posInstance.FiscalName;
            Address = _posInstance.FiscalStreetAddress;
            City = _posInstance.FiscalCityAndZipCode;
            PhoneNumber = _posInstance.FiscalPhoneNumber;
            Email = _posInstance.FiscalEmail;
            Facebook = _posInstance.Facebook;
            Instagram = _posInstance.Instagram;
            Website = _posInstance.Website;
            Comments = _posInstance.Comments;
            FooterMessage = _posInstance.FooterMessage;
            Version = _posInstance.System;
            EmailSender = _posInstance.EmailSender;
            EmailSenderPassword = _posInstance.EmailSenderPassword;
            EmailReports = _posInstance.EmailReports;
            EmailOrders = _posInstance.EmailOrders;
            DiscountPercent = _posInstance.DiscountPercent;
            PointsPercent = _posInstance.PointsPercent;

            //Get list of printers installed
            Printers = new ObservableCollection<string>(Utilities.GetAvailablePrinters());
        }

        #endregion

        #region Observable Properties

        public ObservableCollection<string> Printers
        {
            get { return _printers; }
            set { _printers = value; }
        }

        public string PrinterName
        {
            get
            {
                return _printerName;
            }
            set
            {
                _printerName = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        private FixedDocumentSequence _document;
        public FixedDocumentSequence Document
        {
            get
            {
                return _document;
            }
            set
            {
                _document = value;
                OnPropertyChanged();
            }
        }

        public string FiscalNumber
        {
            get
            {
                return _fiscalNumber;
            }
            set
            {
                _fiscalNumber = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string FiscalName
        {
            get
            {
                return _fiscalName;
            }
            set
            {
                _fiscalName = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }
        public string BusinessName
        {
            get
            {
                return _businessName;
            }
            set
            {
                _businessName = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string Facebook
        {
            get
            {
                return _facebook;
            }
            set
            {
                _facebook = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string Instagram
        {
            get
            {
                return _instagram;
            }
            set
            {
                _instagram = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string Website
        {
            get
            {
                return _website;
            }
            set
            {
                _website = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string FooterMessage
        {
            get
            {
                return _footerMessage;
            }
            set
            {
                _footerMessage = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string EmailSender
        {
            get
            {
                return _emailSender;
            }
            set
            {
                _emailSender = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string EmailSenderPassword
        {
            get
            {
                return _emailSenderPassword;
            }
            set
            {
                _emailSenderPassword = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string EmailReports
        {
            get
            {
                return _emailReports;
            }
            set
            {
                _emailReports = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public string EmailOrders
        {
            get
            {
                return _emailOrders;
            }
            set
            {
                _emailOrders = Formatter.SanitizeInput(value);
                OnPropertyChanged();
            }
        }

        public decimal DiscountPercent
        {
            get { return _discountPercent; }
            set { _discountPercent = value; OnPropertyChanged(); }
        }

        public decimal PointsPercent
        {
            get { return _pointsPercent; }
            set { _pointsPercent = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands
        
        #region  SystemSaveChangesCommand
        public ICommand SystemSaveChangesCommand { get { return _systemSaveChangesCommand ?? (_systemSaveChangesCommand = new DelegateCommand(Execute_SystemSaveChangesCommand, CanExecute_SystemSaveChangesCommand)); } }
        private ICommand _systemSaveChangesCommand;

        internal void Execute_SystemSaveChangesCommand(object parameter)
        {
            //Save all properties and check if a required one is missing
            _posInstance.PrinterName = PrinterName;
            _posInstance.FiscalNumber = FiscalNumber;
            _posInstance.FiscalName = FiscalName;
            _posInstance.FiscalStreetAddress = Address;
            _posInstance.FiscalPhoneNumber = PhoneNumber;
            _posInstance.FiscalEmail = Email;
            _posInstance.Facebook = Facebook;
            _posInstance.Instagram = Instagram;
            _posInstance.Website = Website;
            _posInstance.FooterMessage = FooterMessage;
            _posInstance.System = Version;
            _posInstance.EmailSender = EmailSender;
            _posInstance.EmailSenderPassword = EmailSenderPassword;
            _posInstance.EmailReports = EmailReports;
            _posInstance.EmailOrders = EmailOrders;
            _posInstance.DiscountPercent = DiscountPercent;
            _posInstance.PointsPercent = PointsPercent;
            _posInstance.BusinessName = BusinessName;
            _posInstance.Comments = Comments;
            //Save Data
            _posInstance.UpdateAllData();
            _posInstance.SaveDataTableToCsv();
            //Log
            MainWindowViewModel.GetInstance(null, null).Log.Write(MainWindowViewModel.GetInstance(null, null).CurrentUser.Name, this.ToString() + " " + System.Reflection.MethodBase.GetCurrentMethod().Name, "Menu de Sistema Actualizado");
            //Message
            MainWindowViewModel.GetInstance(null, null).Code = "¡Datos Actualizados!";
            MainWindowViewModel.GetInstance(null, null).CodeColor = Constants.ColorCodeSave;
            //Return
            MainWindowViewModel.GetInstance(null, null).CurrentPage = Constants.PosGeneralPage;
        }

        internal bool CanExecute_SystemSaveChangesCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region SystemSaveLogoCommand

        private ICommand _systemSaveLogoCommand;
        public ICommand SystemSaveLogoCommand { get { return _systemSaveLogoCommand ?? (_systemSaveLogoCommand = new DelegateCommand(Execute_SystemSaveLogoCommand, CanExecute_SystemSaveLogoCommand)); } }

        internal void Execute_SystemSaveLogoCommand(object parameter)
        {           
            SelectImage();
        }
        internal bool CanExecute_SystemSaveLogoCommand(object parameter)
        {
            return true;
        }
        #endregion

        #region DocumentPreviewCommand

        private ICommand _documentPreviewCommand;
        public ICommand DocumentPreviewCommand { get { return _documentPreviewCommand ?? (_documentPreviewCommand = new DelegateCommand(Execute_DocumentPreviewCommand, CanExecute_DocumentPreviewCommand)); } }

        internal void Execute_DocumentPreviewCommand(object parameter)
        {
            ////Create new document
            //string fileName = @"G:\wibsar-pos-solution\ticket.oxps";
            //var doc = new System.Windows.Xps.Packaging.XpsDocument(fileName, FileAccess.Read);
            //FixedDocumentSequence fds = doc.GetFixedDocumentSequence();
            //Document = fds;
            //Generate receipt
            var transaction = new Transaction(null);
            var salesData = new SalesDataStruct();
            

            var receipt = new Receipt(MainWindowViewModel.PosInstance, transaction, salesData);

            //save receipt

            //open receipt
        }
        internal bool CanExecute_DocumentPreviewCommand(object parameter)
        {
            return true;
        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Display a file diagalog to allow the user to select .jpg file to use as a logo
        /// </summary>
        /// <returns></returns>
        public string SelectImage()
        {
            ///TODO: Program does not load the file property without restarting the program
            //Open dialog and select jpg image
            var dialog = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".jpg" };
            //Display dialog
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var fileName = Path.GetFileName(dialog.FileName);

                //Move the file to the images file and append the time at the beginning of the name
                fileName = DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() +
                           DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_" + fileName;

                File.Copy(dialog.FileName, Constants.DataFolderPath + Constants.ImagesFolderPath + "tulogo.png", true);
                return fileName;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}

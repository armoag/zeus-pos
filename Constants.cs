using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus
{
    public static class Constants
    {
        //Main data path
        public static string DataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + 
            @"\Wibsar\Pos\Data\";
        public static string LogsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                              @"\Wibsar\Pos\Logs\";
        public static string ResourcesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                              @"\Wibsar\Pos\Resources\";
        public const string PosDataFileName = "PosData.csv";
        public const string ReceiptBackupFolderPath = @"ReceiptCustomerBackUp\";
        public const string MasterReceiptBackupFolderPath = @"MasterReceiptCustomerBackUp\";

        //Inventory
        public const string InventoryBackupFolderPath = @"InventoryBackUp\";
        public const string InventoryFileName = "Inventario.csv";

        //Transactions Files
        public const string TransactionsBackupFolderPath = @"TransactionBackUp\";
        public const string TransactionsFileName = "Transacciones.csv";
        public const string TransactionsMasterFileName = "TransaccionesMaster.csv";
        public const string TransactionsHistoryFileName = "TransaccionesHistorial.csv";
        public const string TransactionsTypesFileName = "TransactionTypes.txt";
        public const string TransactionBlankFileName = @"\Blanks\TransaccionesBlank.csv";
        public const string TransactionMasterBlankFileName = @"\Blanks\TransaccionesMasterBlank.csv";
        public const string TransactionsPaymentsFileName = "Pagos.csv";
        public const string TransactionsPaymentsBlankFileName = @"\Blanks\PagosBlank.csv";

        //New transaction Files
        public const string TransactionsFullFileName = "TransaccionesHistorial.csv";
        public const string TransactionsXFileName = "TransaccionesX.csv";
        public const string TransactionsZFileName = "TransaccionesZ.csv";
        public const string TransactionsPaymentsXFileName = "PagosX.csv";
        public const string TransactionsPaymentsZFileName = "PagosZ.csv";

        //Expenses Files
        public const string ExpenseFileName = "Gastos.csv";
        public const string ExpenseXFileName = "GastosX.csv";
        public const string ExpenseZFileName = "GastosZ.csv";
        public const string ExpenseHistoryFileName = "GastosHistorial.csv";
        public const string ExpenseBlankFileName = @"\Blanks\GastosBlank.csv";


        //End of day reports
        public const string EndOfDaySalesFileName = "CorteZ.csv";
        public const string CurrentDaySalesFileName = "CorteX.csv";
        public const string MasterEndOfDaySalesFileName = "CorteZMaster.csv";
        public const string EndOfDaySalesBackupFolderPath = @"CorteZBackUp\";

        //Product pages by categories
        public const string CategoryListFileName = "CategoryCatalog.txt";
        public const string ProductPageOne = @"ProductPages\ProductsPage1.txt";
        public const string ProductPageTwo = @"ProductPages\ProductsPage2.txt";
        public const string ProductPageThree = @"ProductPages\ProductsPage3.txt";
        public const string ProductPageFour = @"ProductPages\ProductsPage4.txt";
        public const string ProductPageFive = @"ProductPages\ProductsPage5.txt";

        //Orders Files
        public const string OrdersFolderPath = @"Orders\";
        public const string OrdersFileName = "Pedidos.csv";

        //Users and clients
        public const string UsersFileName = "Users.csv";
        public const string CustomersFileName = "Clientes.csv";
        public const string VendorsFileName = "Proveedores.csv";

        //Images
        public const string ImagesFolderPath = @"Images\";

        //Items list
        public const int MaxNumberListItems = 20;

        //Returns
        public const string ReturnsFileName = "Devoluciones.csv";

        //Colors
        public const string ColorCodeError = "Red";
        public const string ColorCodeSave = "#0285BD";
        public const string ColorCodeRegular = "##0285BD0";

        //log
        public const string LogFileName = "Log.txt";

        //Car Related files
        public const string CarBrandListFileName = "Marcas.txt";
        public const string LocationListFileName = "Locaciones.txt";
        public const string TransmissiondListFileName = "Transmisiones.txt";
        public const string DefaultPartsListFileName = "Lista.csv";

        //Page names
        public const string AnalysisMainPage = "\\View\\AnalysisMainPage.xaml";
        public const string CarRegistrationMainPage = "\\View\\CarRegistrationMainPage.xaml";
        public const string CarRegistrationListPage = "\\View\\CarRegistrationListPage.xaml";
        public const string CategoryListPage = "\\View\\CategoryListPage.xaml";
        public const string CustomerDetailPage = "\\View\\CustomerDetailPage.xaml";
        public const string CustomerMainPage = "\\View\\CustomerMainPage.xaml";
        public const string EndSalesPage = "\\View\\EndSalesPage.xaml";
        public const string ExchangeRatePage = "\\View\\ExchangeRatePage.xaml";
        public const string ExpenseMainPage = "\\View\\ExpenseMainPage.xaml";
        public const string ExpenseDetailPage = "\\View\\ExpenseDetailPage.xaml";
        public const string InventoryItemPage = "\\View\\InventoryItemPage.xaml";
        public const string InventoryMainPage = "\\View\\InventoryMainPage.xaml";
        public const string LoginPage = "\\View\\LoginPage.xaml";
        public const string OrderMainPage = "\\View\\OrderMainPage.xaml";
        public const string OrderPage = "\\View\\OrderPage.xaml";
        public const string PaymentEndPage = "\\View\\PaymentEndPage.xaml";
        public const string PaymentPage = "\\View\\PaymentPage.xaml";
        public const string PaymentPartialPage = "\\View\\PaymentPartialPage.xaml";
        public const string PosGeneralPage = "\\View\\PosGeneralPage.xaml";
        public const string PosMenuPage = "\\View\\PosMenuPage.xaml";
        public const string ProductListControl = "\\View\\ProductListControl.xaml";
        public const string ProductsListEditPage = "\\View\\ProductsListEditPage.xaml";
        public const string ProductsPage = "\\View\\ProductsPage.xaml";
        public const string RemoveInventoryPage = "\\View\\RemoveInventoryPage.xaml";
        public const string ReturnsPage = "\\View\\ReturnsPage.xaml";
        public const string SystemPage = "\\View\\SystemPage.xaml";
        public const string TechSupportPage = "\\View\\TechSupportPage.xaml";
        public const string TransactionDetailPage = "\\View\\TransactionDetailPage.xaml";
        public const string TransactionMainPage = "\\View\\TransactionMainPage.xaml";
        public const string UserDetailPage = "\\View\\UserDetailPage.xaml";
        public const string UserMainPage = "\\View\\UserMainPage.xaml";
        public const string VendorDetailPage = "\\View\\VendorDetailPage.xaml";
        public const string VendorMainPage = "\\View\\VendorMainPage.xaml";
        public const string QueueMainPage = "\\View\\QueueMainPage.xaml";

    }
}

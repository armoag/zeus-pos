using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus
{
    public class AccessManager
    {
        #region Fields
        
        #endregion

        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Methods      

        public static bool ValidateAccess(UserAccessLevelEnum accessLevel, string page)
        {
            var list = new AccessManager().GetAccessPerPageList();
            var found = list.Find(x => x.Item1 == page);
            return found?.Item2 >= accessLevel;
        }

        private List<Tuple<string, UserAccessLevelEnum>> GetAccessPerPageList()
        {
            var list = new List<Tuple<string, UserAccessLevelEnum>>
            {
                new Tuple<string, UserAccessLevelEnum>(Constants.AnalysisMainPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.CarRegistrationMainPage,
                    UserAccessLevelEnum.Intermedio),
                new Tuple<string, UserAccessLevelEnum>(Constants.CarRegistrationListPage,
                    UserAccessLevelEnum.Intermedio),
                new Tuple<string, UserAccessLevelEnum>(Constants.CategoryListPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.CustomerDetailPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.CustomerMainPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.EndSalesPage, UserAccessLevelEnum.Intermedio),
                new Tuple<string, UserAccessLevelEnum>(Constants.ExchangeRatePage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.ExpenseMainPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.ExpenseDetailPage, UserAccessLevelEnum.Intermedio),
                new Tuple<string, UserAccessLevelEnum>(Constants.InventoryItemPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.InventoryMainPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.LoginPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.OrderMainPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.OrderPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.PaymentEndPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.PaymentPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.PaymentPartialPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.PosMenuPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.PosGeneralPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.ProductListControl, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.ProductsListEditPage, UserAccessLevelEnum.Intermedio),
                new Tuple<string, UserAccessLevelEnum>(Constants.ProductsPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.RemoveInventoryPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.ReturnsPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.SystemPage, UserAccessLevelEnum.Administrador),
                new Tuple<string, UserAccessLevelEnum>(Constants.TechSupportPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.TransactionDetailPage,
                    UserAccessLevelEnum.Administrador),
                new Tuple<string, UserAccessLevelEnum>(Constants.TransactionMainPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.UserDetailPage, UserAccessLevelEnum.Administrador),
                new Tuple<string, UserAccessLevelEnum>(Constants.UserMainPage, UserAccessLevelEnum.Basico),
                new Tuple<string, UserAccessLevelEnum>(Constants.VendorMainPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.VendorDetailPage, UserAccessLevelEnum.Avanzado),
                new Tuple<string, UserAccessLevelEnum>(Constants.QueueMainPage, UserAccessLevelEnum.Basico)
            };

            return list;
        }
        #endregion
    }
}

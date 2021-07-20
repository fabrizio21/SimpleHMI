using SimpleHMI.ViewModels;
using System.Windows.Controls;

namespace SimpleHMI.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow
    /// </summary>
    public partial class LoginWindow : UserControl
    {
        public LoginWindow(){
            InitializeComponent();
        }

        private void txtPwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e) {
            LoginWindowViewModel vm = this.DataContext as LoginWindowViewModel;
            vm.Password = (sender as PasswordBox).Password;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project1.Classes;
using Project1.Models;

namespace Project1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tb_login.Text.Trim();
            string pass = pb_password.Password;

            var user = DB.Context.User.FirstOrDefault(u => u.Login == login && u.Password == pass);

            if (user != null)
                OpenUsersWindow(user);
            else
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var reg = new UserAddEdit();
            reg.Owner = this;
            reg.ShowDialog();
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            OpenUsersWindow(null);
        }

        private void OpenUsersWindow(User user)
        {
            var main = new Users(user);  // ← сюда попадает либо пользователь, либо null (гость)
            main.Show();
            this.Close();
        }
    }
}
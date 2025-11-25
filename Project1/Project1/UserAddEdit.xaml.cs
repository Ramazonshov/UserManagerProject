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
using System.Windows.Shapes;
using Project1.Classes;
using Project1.Models;
namespace Project1
{
    public partial class UserAddEdit : Window
    {
        private readonly User _user;
        private readonly bool _isAdd;
        private readonly User _currentUser; // кто сейчас вошёл в систему

        public UserAddEdit(User userToEdit = null, User currentUser = null)
        {
            InitializeComponent();

            _currentUser = currentUser;

            if (userToEdit == null)
            {
                _isAdd = true;
                _user = new User();
                txtTitle.Text = "Добавить пользователя";
            }
            else
            {
                _isAdd = false;
                _user = userToEdit;
                txtTitle.Text = "Редактировать пользователя";

                txtLogin.Text = _user.Login;
                txtPass.Password = _user.Password;
                txtPass2.Password = _user.Password;
            }

            LoadRoles();
        }

        private void LoadRoles()
        {
            var allRoles = DB.Context.Role.ToList();

            // Только администратор может назначать любые роли
            if (_currentUser == null || _currentUser.IdRole != 1)
            {
                // Менеджер и остальные могут создавать только «Пользователь» и «Гость»
                allRoles = allRoles.Where(r => r.Id == 4 || r.Id == 5).ToList();
            }

            cmRole.ItemsSource = allRoles;
            cmRole.DisplayMemberPath = "Title";
            cmRole.SelectedValuePath = "Id";

            if (!_isAdd)
                cmRole.SelectedValue = _user.IdRole;
            else
                cmRole.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPass.Password != txtPass2.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка уникальности логина
            if (DB.Context.User.Any(u => u.Login == txtLogin.Text.Trim() && u.Id != _user.Id))
            {
                MessageBox.Show("Такой логин уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _user.Login = txtLogin.Text.Trim();
            _user.Password = txtPass.Password;
            _user.IdRole = (int)cmRole.SelectedValue;

            if (_isAdd)
                DB.Context.User.Add(_user);

            try
            {
                DB.Context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
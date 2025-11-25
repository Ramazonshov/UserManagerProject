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
using System.ComponentModel;


namespace Project1
{
    public partial class Users : Window
    {
        private readonly User _currentUser;
        private bool _canAdd, _canEdit, _canDelete;

        private CollectionViewSource _cvs;

        public Users(User currentUser = null)
        {
            InitializeComponent();
            _currentUser = currentUser;

            SetupPermissions();
            LoadData();

            Title = _currentUser == null
                ? "Режим гостя"
                : $"Добро пожаловать, {_currentUser.Login} [{_currentUser.Role.Title}]";
        }

        private void SetupPermissions()
        {
            if (_currentUser == null || _currentUser.IdRole >= 3)
            {
                _canAdd = _canEdit = _canDelete = false;
            }
            else if (_currentUser.IdRole == 1) // админ
            {
                _canAdd = _canEdit = _canDelete = true;
            }
            else if (_currentUser.IdRole == 2) // менеджер
            {
                _canAdd = _canEdit = true;
                _canDelete = false;
            }

            btnAdd.Visibility = _canAdd ? Visibility.Visible : Visibility.Collapsed;

            if (dgUsers.Columns.Count > 2)
            {
                var col = dgUsers.Columns[2] as DataGridTemplateColumn;
                col.Visibility = (_canEdit || _canDelete) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            var list = DB.Context.User.Include("Role").ToList();

            _cvs = new CollectionViewSource { Source = list };
            _cvs.SortDescriptions.Add(new SortDescription("Login", ListSortDirection.Ascending));
            _cvs.Filter += Cvs_Filter;

            ;

            dgUsers.ItemsSource = _cvs.View;

            // Заполняем ComboBox'ы
            cmbSort.SelectedIndex = 0;
            cmbFilterRole.Items.Add("Все роли");
            foreach (var r in DB.Context.Role.OrderBy(r => r.Id))
                cmbFilterRole.Items.Add(r.Title);
            cmbFilterRole.SelectedIndex = 0;
        }

        private void Cvs_Filter(object sender, FilterEventArgs e)
        {
            if (chkFilter.IsChecked != true)
            {
                e.Accepted = true;
                return;
            }

            var user = e.Item as User;
            if (user?.Role == null) { e.Accepted = false; return; }

            string selected = cmbFilterRole.SelectedItem as string;
            e.Accepted = selected == "Все роли" || selected == user.Role.Title;
        }

        // Сортировка
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cvs == null) return;

            _cvs.SortDescriptions.Clear();
            if (cmbSort.SelectedIndex == 0)
                _cvs.SortDescriptions.Add(new SortDescription("Login", ListSortDirection.Ascending));
            else
                _cvs.SortDescriptions.Add(new SortDescription("Login", ListSortDirection.Descending));

            _cvs.View.Refresh();
        }

        // Фильтрация
        private void cmbFilterRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _cvs?.View?.Refresh();
        }

        private void chkFilter_Checked(object sender, RoutedEventArgs e) => _cvs?.View?.Refresh();
        private void chkFilter_Unchecked(object sender, RoutedEventArgs e) => _cvs?.View?.Refresh();

        // Обновление после добавления/удаления
        private void RefreshData()
        {
            LoadData(); // просто перезагружаем всё
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!_canAdd) return;
            var w = new UserAddEdit(null, _currentUser);
            w.Owner = this;
            if (w.ShowDialog() == true) RefreshData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!_canEdit) return;
            if ((sender as Button)?.Tag is User user)
            {
                var w = new UserAddEdit(user, _currentUser);
                w.Owner = this;
                if (w.ShowDialog() == true) RefreshData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!_canDelete) return;
            if ((sender as Button)?.Tag is User user)
            {
                if (MessageBox.Show($"Удалить {user.Login}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DB.Context.User.Remove(user);
                    DB.Context.SaveChanges();
                    RefreshData();
                }
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            RefreshData();
        }
    }
}
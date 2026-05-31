using Model.Core;
using Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RestaurantMenu
{
    public partial class MenuWindow : Window
    {
        private Establishment _establishment;
        private bool _isSeasonal;
        private string _saveFormat;
        private List<Dish> _currentDisplayedDishes;
        private Dish _editingDish;

        public MenuWindow(Establishment establishment, bool isSeasonal, string saveFormat)
        {
            InitializeComponent();
            _establishment = establishment;
            _isSeasonal = isSeasonal;
            _saveFormat = saveFormat;

            lblTitle.Text = isSeasonal ? $"🍂 {establishment.Name} - Сезонное меню" : $"📋 {establishment.Name} - Обычное меню";

            LoadMenu();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadMenu()
        {
            if (_isSeasonal && _establishment is ISeasonalMenu seasonal)
            {
                _currentDisplayedDishes = seasonal.SeasonalDishes?.ToList() ?? new List<Dish>();
            }
            else
            {
                _currentDisplayedDishes = _establishment.CurrentMenu?.Dishes?.ToList() ?? new List<Dish>();
            }

            RefreshDishList();
        }

        private void RefreshDishList()
        {
            if (lvDishes == null)
                return;

            if (_currentDisplayedDishes == null)
                _currentDisplayedDishes = new List<Dish>();

            string filter = (cmbFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();

            IEnumerable<Dish> filtered = _currentDisplayedDishes;

            if (!string.IsNullOrEmpty(filter) && filter != "Все")
            {
                switch (filter)
                {
                    case "Горячие блюда":
                        filtered = filtered.Where(d => d.Category == "Hot");
                        break;
                    case "Напитки":
                        filtered = filtered.Where(d => d.Category == "Drink");
                        break;
                    case "Десерты":
                        filtered = filtered.Where(d => d.Category == "Dessert");
                        break;
                }
            }

            if (filtered == null)
                filtered = new List<Dish>();

            lvDishes.ItemsSource = filtered.ToList();
        }

        private void СmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDishList();
        }

        private void LvDishes_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lvDishes == null)
                return;

            var element = e.OriginalSource as DependencyObject;
            while (element != null && !(element is ListViewItem))
                element = VisualTreeHelper.GetParent(element);

            if (element is ListViewItem item && item.IsSelected)
            {
                lvDishes.SelectedItem = null;
                e.Handled = true;
            }
        }

        private void LvDishes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvDishes == null)
                return;

            var selected = lvDishes.SelectedItem as Dish;
            if (selected == null)
            {
                ClearDishForm();
                return;
            }

            _editingDish = selected;
            txtNewDishName.Text = selected.Name;
            txtNewDishPrice.Text = selected.Price.ToString();
            SelectCategory(selected.Category);
            SetDishFormEditMode(true);
        }

        private void SelectCategory(string category)
        {
            if (cmbCategory == null || string.IsNullOrEmpty(category))
                return;

            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                var item = cmbCategory.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == category)
                {
                    cmbCategory.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SetDishFormEditMode(bool isEdit)
        {
            if (lblDishPanelTitle != null)
                lblDishPanelTitle.Text = isEdit ? "✏️ Изменить блюдо" : "➕ Добавить новое блюдо";

            if (btnAddDish != null)
                btnAddDish.Content = isEdit ? "✏️ Изменить" : "➕ Добавить";
        }

        private void ClearDishForm()
        {
            _editingDish = null;
            if (txtNewDishName != null)
                txtNewDishName.Clear();
            if (txtNewDishPrice != null)
                txtNewDishPrice.Clear();
            if (cmbCategory != null && cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
            SetDishFormEditMode(false);
        }

        private void BtnAddDish_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewDishName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название блюда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNewDishPrice.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (_editingDish != null)
            {
                _editingDish.Name = name;
                _editingDish.Price = price;
                _editingDish.Category = category;
                RefreshDishList();
                lvDishes.SelectedItem = null;
                ClearDishForm();
                return;
            }

            Dish newDish;
            switch (category)
            {
                case "Hot":
                    newDish = new HotDish { Name = name, Price = price, Category = "Hot" };
                    break;
                case "Drink":
                    newDish = new Drink { Name = name, Price = price, Category = "Drink" };
                    break;
                case "Dessert":
                    newDish = new Dessert { Name = name, Price = price, Category = "Dessert" };
                    break;
                default:
                    newDish = new Dish { Name = name, Price = price, Category = category };
                    break;
            }

            if (_isSeasonal && _establishment is ISeasonalMenu seasonal)
            {
                seasonal.AddSeasonalDish(newDish);
                _currentDisplayedDishes = seasonal.SeasonalDishes;
            }
            else
            {
                if (_establishment.CurrentMenu == null)
                    _establishment.CurrentMenu = new Model.Core.Menu();
                _establishment.CurrentMenu.AddDish(newDish);
                _currentDisplayedDishes = _establishment.CurrentMenu.Dishes;
            }

            ClearDishForm();
            RefreshDishList();
        }

        private void BtnDeleteDish_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvDishes.SelectedItem as Dish;
            if (selected == null)
            {
                MessageBox.Show("Выберите блюдо для удаления", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show($"Удалить блюдо \"{selected.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_isSeasonal && _establishment is ISeasonalMenu seasonal)
                {
                    seasonal.RemoveSeasonalDish(selected);
                    _currentDisplayedDishes = seasonal.SeasonalDishes;
                }
                else
                {
                    _establishment.CurrentMenu.RemoveDish(selected);
                    _currentDisplayedDishes = _establishment.CurrentMenu.Dishes;
                }

                ClearDishForm();
                RefreshDishList();
            }
        }

        private void BtnSaveMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_establishment.FilePath))
                {
                    string filesDir = FilePaths.GetFilesDirectory();
                    string fileName = _establishment.Name.Replace(" ", "_").ToLower();
                    string extension = _saveFormat.ToLower() == "json" ? ".json" : ".xml";
                    _establishment.FilePath = Path.Combine(filesDir, fileName + extension);
                }

                string currentFormat = Path.GetExtension(_establishment.FilePath)?.ToLower().TrimStart('.');
                string selectedFormat = _saveFormat.ToLower();

                bool needsConversion = (currentFormat == "json" && selectedFormat == "xml") ||
                                       (currentFormat == "xml" && selectedFormat == "json");

                DataService service;
                if (selectedFormat == "json")
                    service = new JsonDataService();
                else
                    service = new XmlDataService();

                string newPath = _establishment.FilePath;

                if (needsConversion)
                {
                    string directory = Path.GetDirectoryName(_establishment.FilePath);
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_establishment.FilePath);
                    string newExtension = selectedFormat == "json" ? ".json" : ".xml";
                    newPath = Path.Combine(directory, fileNameWithoutExt + newExtension);

                    if (File.Exists(_establishment.FilePath))
                        File.Delete(_establishment.FilePath);

                    _establishment.FilePath = newPath;
                }

                service.Save(_establishment.FilePath, _establishment);

                MessageBox.Show($"Меню сохранено в {_saveFormat} формате\n\nПуть: {_establishment.FilePath}",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\n{ex.StackTrace}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

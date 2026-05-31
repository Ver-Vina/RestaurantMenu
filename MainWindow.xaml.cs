using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model.Core;
using Model.Data;

namespace RestaurantMenu
{
    public partial class MainWindow : Window
    {
        private List<Establishment> _allEstablishments = new List<Establishment>();
        private List<Establishment> _filteredEstablishments = new List<Establishment>();

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Убеждаемся, что все контролы созданы
            if (cmbEstablishmentType != null && cmbEstablishmentType.Items.Count > 0)
                cmbEstablishmentType.SelectedIndex = 0;

            if (cmbEstablishment != null && cmbEstablishment.Items.Count > 0)
                cmbEstablishment.SelectedIndex = 0;
            LoadEstablishments();
        }

        private void LoadEstablishments()
        {
            try
            {
                string filesDir = FilePaths.GetFilesDirectory();
                var jsonService = new JsonDataService();
                var xmlService = new XmlDataService();

                _allEstablishments.Clear();

                // Загружаем JSON файлы
                foreach (string file in Directory.GetFiles(filesDir, "*.json"))
                {
                    var establishment = LoadFromFile(file, jsonService);
                    if (establishment != null)
                    {
                        establishment.FilePath = file;
                        _allEstablishments.Add(establishment);
                    }
                }

                // Загружаем XML файлы
                foreach (string file in Directory.GetFiles(filesDir, "*.xml"))
                {
                    var establishment = LoadFromFile(file, xmlService);
                    if (establishment != null)
                    {
                        establishment.FilePath = file;
                        _allEstablishments.Add(establishment);
                    }
                }

                UpdateEstablishmentList();
                lblStatus.Text = $"Загружено заведений: {_allEstablishments.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //    private void LoadEstablishments()
        //{
        //    try
        //    {
        //        string filesDir = FilePaths.GetFilesDirectory();

            //        // Показываем, какую папку ищем
            //        MessageBox.Show($"Ищем файлы в: {filesDir}");

            //        // Проверяем, существует ли папка
            //        if (!Directory.Exists(filesDir))
            //        {
            //            MessageBox.Show($"Папка не существует: {filesDir}");
            //            return;
            //        }

            //        // Получаем все JSON файлы
            //        var jsonFiles = Directory.GetFiles(filesDir, "*.json");
            //        MessageBox.Show($"Найдено JSON файлов: {jsonFiles.Length}\n\nСписок:\n{string.Join("\n", jsonFiles)}");

            //        var jsonService = new JsonDataService();

            //        _allEstablishments.Clear();

            //        // Загружаем JSON файлы
            //        foreach (string file in jsonFiles)
            //        {
            //            try
            //            {
            //                MessageBox.Show($"Пытаемся загрузить: {file}");
            //                var establishment = LoadFromFile(file, jsonService);
            //                if (establishment != null)
            //                {
            //                    establishment.FilePath = file;
            //                    _allEstablishments.Add(establishment);
            //                    MessageBox.Show($"✅ Успешно загружено: {establishment.Name}");
            //                }
            //                else
            //                {
            //                    MessageBox.Show($"❌ Не удалось загрузить: {file}");
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show($"❌ Ошибка при загрузке {file}: {ex.Message}");
            //            }
            //        }

            //        // Загружаем XML файлы (если есть)
            //        var xmlFiles = Directory.GetFiles(filesDir, "*.xml");
            //        var xmlService = new XmlDataService();

            //        foreach (string file in xmlFiles)
            //        {
            //            try
            //            {
            //                var establishment = LoadFromFile(file, xmlService);
            //                if (establishment != null)
            //                {
            //                    establishment.FilePath = file;
            //                    _allEstablishments.Add(establishment);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show($"Ошибка загрузки XML {file}: {ex.Message}");
            //            }
            //        }

            //        UpdateEstablishmentList();

            //        if (lblStatus != null)
            //            lblStatus.Text = $"Загружено заведений: {_allEstablishments.Count}";

            //        if (_allEstablishments.Count == 0)
            //            MessageBox.Show("Не загружено ни одного заведения!");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Общая ошибка: {ex.Message}\n\n{ex.StackTrace}");
            //    }
            //}

        private Establishment LoadFromFile(string path, DataService service)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(path);

                if (fileName.Contains("restaurant"))
                    return service.Load<Restaurant>(path);
                if (fileName.Contains("cafe"))
                    return service.Load<Cafe>(path);
                if (fileName.Contains("coffee"))
                    return service.Load<CoffeeShop>(path);

                // Пробуем определить по содержимому
                var content = File.ReadAllText(path);
                if (content.Contains("\"Type\": \"Restaurant\"") || content.Contains("<Type>Restaurant</Type>"))
                    return service.Load<Restaurant>(path);
                if (content.Contains("\"Type\": \"Cafe\"") || content.Contains("<Type>Cafe</Type>"))
                    return service.Load<Cafe>(path);
                if (content.Contains("\"Type\": \"CoffeeShop\"") || content.Contains("<Type>CoffeeShop</Type>"))
                    return service.Load<CoffeeShop>(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки {path}: {ex.Message}");
            }
            return null;
        }

        private void UpdateEstablishmentList()
        {
            // Защита от null (если контрол ещё не создан)
            if (cmbEstablishmentType == null || cmbEstablishment == null)
                return;

            // Защита от пустого выбора
            if (cmbEstablishmentType.SelectedItem == null)
            {
                _filteredEstablishments = _allEstablishments.ToList();
                cmbEstablishment.ItemsSource = _filteredEstablishments;
                if (_filteredEstablishments.Any())
                    cmbEstablishment.SelectedIndex = 0;
                return;
            }

            var selectedItem = cmbEstablishmentType.SelectedItem as ComboBoxItem;
            if (selectedItem == null || selectedItem.Content == null)
            {
                _filteredEstablishments = _allEstablishments.ToList();
                cmbEstablishment.ItemsSource = _filteredEstablishments;
                if (_filteredEstablishments.Any())
                    cmbEstablishment.SelectedIndex = 0;
                return;
            }

            string selectedType = selectedItem.Content.ToString();

            if (selectedType == "Все")
            {
                _filteredEstablishments = _allEstablishments.ToList();
            }
            else
            {
                _filteredEstablishments = _allEstablishments
                    .Where(e => e.Type == selectedType)
                    .ToList();
            }

            cmbEstablishment.ItemsSource = null;
            cmbEstablishment.ItemsSource = _filteredEstablishments;

            if (_filteredEstablishments.Any())
                cmbEstablishment.SelectedIndex = 0;
        }

        private void CmbEstablishmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEstablishmentList();
            UpdateEstablishmentInfo();
        }

        private void CmbEstablishment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEstablishmentInfo();
        }

        private void UpdateEstablishmentInfo()
        {
            // Защита от null
            if (cmbEstablishment == null || lblEstablishmentInfo == null || btnShowMenu == null)
                return;

            var selected = cmbEstablishment.SelectedItem as Establishment;

            if (selected != null)
            {
                string menuType = (rbSeasonal != null && rbSeasonal.IsChecked == true) ? "Сезонное" : "Обычное";
                lblEstablishmentInfo.Text = $"🏢 {selected.Name}\n📍 {selected.Address}\n📂 Тип меню: {menuType}";
                btnShowMenu.IsEnabled = true;
            }
            else
            {
                lblEstablishmentInfo.Text = "Выберите заведение";
                btnShowMenu.IsEnabled = false;
            }
        }

        private void BtnShowMenu_Click(object sender, RoutedEventArgs e)
        {
            var selected = cmbEstablishment.SelectedItem as Establishment;
            if (selected == null) return;

            bool isSeasonal = rbSeasonal.IsChecked == true;
            string saveFormat = (cmbSaveFormat.SelectedItem as ComboBoxItem)?.Content.ToString();

            var menuWindow = new MenuWindow(selected, isSeasonal, saveFormat);
            menuWindow.Owner = this;
            menuWindow.ShowDialog();

            // Обновляем информацию после возможных изменений
            UpdateEstablishmentInfo();
        }

    }
}
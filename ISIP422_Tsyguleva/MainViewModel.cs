using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ISIP422_Tsyguleva
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private string _searchText;
        private string _selectedSearchType = "Код";
        private string _newName;
        private decimal _newPrice;
        private int _newQuantity;
        private string _newCategory;
        private int _supplyQuantity = 1;
        private int _sellQuantity = 1;

        public MainViewModel()
        {
            Products = new ObservableCollection<Product>();
            InitializeCategories();
            InitializeTestData();

            AddProductCommand = new RelayCommand(AddProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct, CanDeleteProduct);
            OrderSupplyCommand = new RelayCommand(OrderSupply, CanModifyProduct);
            SellProductCommand = new RelayCommand(SellProduct, CanSellProduct);
            SearchCommand = new RelayCommand(SearchProducts);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                if (DeleteProductCommand is RelayCommand deleteCommand)
                    deleteCommand.RaiseCanExecuteChanged();
                if (OrderSupplyCommand is RelayCommand supplyCommand)
                    supplyCommand.RaiseCanExecuteChanged();
                if (SellProductCommand is RelayCommand sellCommand)
                    sellCommand.RaiseCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public string SelectedSearchType
        {
            get => _selectedSearchType;
            set
            {
                _selectedSearchType = value;
                OnPropertyChanged(nameof(SelectedSearchType));
            }
        }

        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> SearchTypes { get; set; } = new ObservableCollection<string> { "Код", "Название", "Категория" };

        public string NewName
        {
            get => _newName;
            set
            {
                _newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }

        public decimal NewPrice
        {
            get => _newPrice;
            set
            {
                _newPrice = value;
                OnPropertyChanged(nameof(NewPrice));
            }
        }

        public int NewQuantity
        {
            get => _newQuantity;
            set
            {
                _newQuantity = value;
                OnPropertyChanged(nameof(NewQuantity));
            }
        }

        public string NewCategory
        {
            get => _newCategory;
            set
            {
                _newCategory = value;
                OnPropertyChanged(nameof(NewCategory));
            }
        }

        public int SupplyQuantity
        {
            get => _supplyQuantity;
            set
            {
                _supplyQuantity = value;
                OnPropertyChanged(nameof(SupplyQuantity));
            }
        }

        public int SellQuantity
        {
            get => _sellQuantity;
            set
            {
                _sellQuantity = value;
                OnPropertyChanged(nameof(SellQuantity));
            }
        }

        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand OrderSupplyCommand { get; }
        public ICommand SellProductCommand { get; }
        public ICommand SearchCommand { get; }

        private void InitializeCategories()
        {
            Categories = new ObservableCollection<string>
            {
                "Электроника",
                "Одежда",
                "Продукты питания",
                "Книги",
                "Спорттовары"
            };
        }

        private void InitializeTestData()
        {
            Products.Clear();

            AddTestProduct("Ноутбук Lenovo", 50000, 5, "Электроника");
            AddTestProduct("Футболка", 1500, 10, "Одежда");
            AddTestProduct("Яблоки", 100, 50, "Продукты питания");
            AddTestProduct("Война и мир", 500, 3, "Книги");
            AddTestProduct("Футбольный мяч", 2000, 8, "Спорттовары");
        }

        private void AddTestProduct(string name, decimal price, int quantity, string category)
        {
            var product = new Product
            {
                Code = GenerateProductCode(),
                Name = name,
                Price = price,
                Quantity = quantity,
                Category = category
            };
            Products.Add(product);
        }

        private string GenerateProductCode()
        {
            if (!Products.Any()) return "10001";

            try
            {
                var maxCode = Products.Max(p => int.Parse(p.Code));
                return (maxCode + 1).ToString();
            }
            catch
            {
                return "10001";
            }
        }

        private void AddProduct(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NewName) || NewPrice <= 0 || NewQuantity < 0 || string.IsNullOrWhiteSpace(NewCategory))
            {
                MessageBox.Show("Заполните все поля корректно!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var product = new Product
            {
                Code = GenerateProductCode(),
                Name = NewName,
                Price = NewPrice,
                Quantity = NewQuantity,
                Category = NewCategory
            };

            Products.Add(product);

            NewName = string.Empty;
            NewPrice = 0;
            NewQuantity = 0;
            NewCategory = null;
            OnPropertyChanged(nameof(NewName));
            OnPropertyChanged(nameof(NewPrice));
            OnPropertyChanged(nameof(NewQuantity));
            OnPropertyChanged(nameof(NewCategory));

            MessageBox.Show($"Товар успешно добавлен!\nКод товара: {product.Code}", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteProduct(object parameter)
        {
            if (SelectedProduct != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар {SelectedProduct.Name}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Products.Remove(SelectedProduct);
                    MessageBox.Show("Товар успешно удален!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private bool CanDeleteProduct(object parameter) => SelectedProduct != null;

        private void OrderSupply(object parameter)
        {
            if (SelectedProduct != null && SupplyQuantity > 0)
            {
                SelectedProduct.Quantity += SupplyQuantity;
                MessageBox.Show($"Поставка успешно добавлена!\nНовое количество: {SelectedProduct.Quantity}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanModifyProduct(object parameter) => SelectedProduct != null;

        private void SellProduct(object parameter)
        {
            if (SelectedProduct != null)
            {
                if (SellQuantity <= 0)
                {
                    MessageBox.Show("Количество для продажи должно быть больше 0!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SelectedProduct.Quantity < SellQuantity)
                {
                    MessageBox.Show($"Недостаточно товара на складе!\nДоступно: {SelectedProduct.Quantity}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SelectedProduct.Quantity -= SellQuantity;
                MessageBox.Show($"Товар успешно продан!\nОстаток: {SelectedProduct.Quantity}\nВыручка: {SelectedProduct.Price * SellQuantity:C}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanSellProduct(object parameter) => SelectedProduct != null && SelectedProduct.Quantity > 0;

        private void SearchProducts(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
               
                return;
            }

            var searchTextLower = SearchText.ToLower();
            var searchResults = new ObservableCollection<Product>();

            switch (SelectedSearchType)
            {
                case "Код":
                    searchResults = new ObservableCollection<Product>(
                        Products.Where(p => p.Code.ToLower().Contains(searchTextLower)));
                    break;
                case "Название":
                    searchResults = new ObservableCollection<Product>(
                        Products.Where(p => p.Name.ToLower().Contains(searchTextLower)));
                    break;
                case "Категория":
                    searchResults = new ObservableCollection<Product>(
                        Products.Where(p => p.Category.ToLower().Contains(searchTextLower)));
                    break;
            }

            if (searchResults.Any())
            {
                MessageBox.Show($"Найдено товаров: {searchResults.Count}", "Результаты поиска",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Товары не найдены!", "Результаты поиска",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
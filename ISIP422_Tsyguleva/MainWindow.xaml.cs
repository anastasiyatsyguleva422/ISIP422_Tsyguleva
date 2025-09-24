using System.Windows;
using System.Collections.ObjectModel;

namespace ISIP422_Tsyguleva
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SearchText = string.Empty;
                var allProducts = new ObservableCollection<Product>(viewModel.Products);
                viewModel.Products = allProducts;
            }
        }
    }
}
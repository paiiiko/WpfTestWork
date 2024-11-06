using System.Windows;
using WpfApplication.Model;

namespace WpfApplication.ApplicationWindows
{
    public partial class CompanyWindow : Window
    {
        public Company Company { get; private set; }
        public CompanyWindow(Company company)
        {
            InitializeComponent();
            Company = company;
            DataContext = company;
        }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (Company.Name == null)
            {
                return;
            }
            DialogResult = true;
            MainWindow._refresh = true;
        }
    }
}

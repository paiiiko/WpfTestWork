using System.Windows;
using WpfApplication.Model;
using System.Windows.Controls;

namespace WpfApplication.ApplicationWindows
{
    public partial class EmployeeWindow : Window
    {
        public Employee Employee { get; private set; }
        public Company Company { get; private set; }
        public EmployeeWindow(Employee employee, List<Company> names)
        {
            InitializeComponent();
            Employee = employee;
            DataContext = employee;
            companyComboBox.ItemsSource = names;
        }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (Employee.Name == null || Employee.Surname == null || companyComboBox.SelectedItem == null)
            {
                return;
            }
            MainWindow._refresh = true;
            DialogResult = true;
        }
        private void companyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             Company = (Company)companyComboBox.SelectedItem;
        }
    }
}
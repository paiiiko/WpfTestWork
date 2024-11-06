using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApplication.Tools;
using WpfApplication.Model;
using WpfApplication.Interfaces;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Media;
using WpfApplication.Data;

namespace WpfApplication.ApplicationWindows
{
    public partial class MainWindow : Window
    {
        static public bool _refresh = false;
        private int _counter = 1;
        private ICompaniesDbContext _companiesDbContext;
        private IEmloyeesDbContext _emloyeeDbContext;
        private readonly ILogger _logger;
        public MainWindow(IServiceProvider serviceProvider, ILogger logger)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                try
                {
                    var context = serviceProvider.GetRequiredService<DataContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
                catch (Exception exeption)
                {
                    Console.WriteLine(exeption);
                }
            }
            _companiesDbContext = serviceProvider.GetService<ICompaniesDbContext>();
            _emloyeeDbContext   = serviceProvider.GetService<IEmloyeesDbContext>();
            _logger = logger;
            InitializeComponent();
            companies.ItemsSource = _companiesDbContext.GetAll();
            employees.ItemsSource = _emloyeeDbContext.GetAll();
            var thread = new Thread(Refresher);
            thread.IsBackground = true;
            thread.Start();
            ((INotifyCollectionChanged)console.Items).CollectionChanged += Items_CollectionChanged;
        }

        private async void AddApplicationData_Click(object sender, RoutedEventArgs e)
        {
            if (_companiesDbContext.HasData())
            {
                return;
            }
            for (int i = 1; i <= 10; i++)
            {
                Company company = new Company
                {
                    Name = $"ГрОбРекордз {i}"
                };
                _companiesDbContext.Companies.Add(company);
                for (int j = 0; j < 100; j++)
                {
                    await _emloyeeDbContext.CreateEmloyee(company,
                                                          ImageWorking.ConvertImageToByteArray("letov.jpg"),
                                                          $"Егор {_counter}",
                                                          $"Летов {_counter}");
                    _counter++;
                }
            }
            await _companiesDbContext.SaveChangesAsync(CancellationToken.None);
            _logger.LogInformation("Добавлены изначальные данные");
            console.Items.Add("Добавлены изначальные данные");
            companies.ItemsSource = _companiesDbContext.GetAll();
            employees.ItemsSource = _emloyeeDbContext.GetAll();
            companies.Items.Refresh();
        }
        private void Companies_GetEmloyees(object sender, RoutedEventArgs e)
        {
            var selectedColumn = companies.CurrentCell.Item as Company;
            if (selectedColumn != null)
            {
                List<Employee> companyEmloyee = _emloyeeDbContext.GetEmployeesByCompany(selectedColumn.Id);
                employees.ItemsSource = companyEmloyee;
            }
            else
            {
                employees.ItemsSource = _emloyeeDbContext.GetAll();
            }
            _logger.LogInformation($"Выбрана компания \"{selectedColumn.Name}\"");
            console.Items.Add($"Выбрана компания \"{selectedColumn.Name}\"");
        }
        private void Refresher()
        {
            while (true)
            {
                if (_refresh == true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        companies.ItemsSource = _companiesDbContext.GetAll();
                        employees.ItemsSource = _emloyeeDbContext.GetAll();
                    });
                    _refresh = false;
                }
            }
        }
        private void Grid_KeyEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string info =  search.Text;
                if (info != null)
                {
                    var x = _emloyeeDbContext.GetBySting(info);
                    employees.ItemsSource = x;
                    companies.UnselectAll();
                    _logger.LogInformation($"Выполнен поиск по строке \"{search.Text}\"");
                    console.Items.Add($"Выполнен поиск по строке \"{search.Text}\"");
                }
            }
        }
        private async void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            CompanyWindow addWindow = new CompanyWindow(new Company());
            if (addWindow.ShowDialog() == true)
            {
                Company company = new Company
                {
                    Name = addWindow.Name,
                };
                _companiesDbContext.AddCompany(addWindow.Company);
                await _companiesDbContext.SaveChangesAsync(CancellationToken.None);
                _logger.LogInformation($"Добавлена новая компания");
                console.Items.Add($"Добавлена новая компания");
            }
        }
        private async void EditCompany_Click(object sender, RoutedEventArgs e)
        {
            Company company = companies.SelectedItem as Company;
            string oldName = company.Name;
            if (company != null)
            {
                CompanyWindow editWindow = new CompanyWindow(company);
                if (editWindow.ShowDialog() == true)
                {
                    company = _companiesDbContext.FindCompanyById(editWindow.Company.Id);
                    company.Name = editWindow.Company.Name;
                    await _companiesDbContext.SaveChangesAsync(CancellationToken.None);
                    _logger.LogInformation($"Компания \"{oldName}\" была изменена на \"{company.Name}\"");
                    console.Items.Add($"Компания \"{oldName}\" была изменена на \"{company.Name}\"");
                }
            }
        }
        private async void DeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            Company company = companies.SelectedItem as Company;
            if (company != null)
            {
                string deletedCompany = company.Name;
                _companiesDbContext.RemoveById(company.Id);
                await _companiesDbContext.SaveChangesAsync(CancellationToken.None);
                _refresh = true;
                _logger.LogInformation($"Компания \"{deletedCompany}\" была удалена");
                console.Items.Add($"Компания \"{deletedCompany}\" была удалена");
            }
        }
        private async void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            List<Company> companiesName = _companiesDbContext.GetAll();
            EmployeeWindow addWindow = new EmployeeWindow(new Employee(), companiesName);
            if (addWindow.ShowDialog() == true)
            {
                Employee employee = new Employee
                {
                    Name = addWindow.Employee.Name,
                    Surname = addWindow.Employee.Surname,
                    Company = addWindow.Company,
                };
                _emloyeeDbContext.AddEmployee(employee);
                await _companiesDbContext.SaveChangesAsync(CancellationToken.None);
                _refresh = true;
                _logger.LogInformation($"В компанию \"{employee.Company.Name}\" добавлен новый сотрудник {employee.Name} {employee.Surname}");
                console.Items.Add($"В компанию \"{employee.Company.Name}\" добавлен новый сотрудник {employee.Name} {employee.Surname}");
            }
        }
        private async void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = employees.SelectedItem as Employee;
            if (employee != null)
            {
                string oldEmployee = employee.Name + " " + employee.Surname;
                List<Company> companiesName = _companiesDbContext.GetAll();
                EmployeeWindow editWindow = new EmployeeWindow(employee, companiesName);
                if (editWindow.ShowDialog() == true)
                {
                    employee = _emloyeeDbContext.FindEmployeeById(editWindow.Employee.Id);
                    employee.Name = editWindow.Employee.Name;
                    employee.Surname = editWindow.Employee.Surname;
                    employee.Company = editWindow.Company;
                    await _emloyeeDbContext.SaveChangesAsync(CancellationToken.None);
                    employees.SelectedItem = employee;
                    _logger.LogInformation($"Сотрудник {oldEmployee} был изменён");
                    console.Items.Add($"Сотрудник {oldEmployee} был изменён");
                }
            }
        }
        private async void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = employees.SelectedItem as Employee;
            if (employee != null)
            {
                string deletedEmployee = employee.Name + " " + employee.Surname;
                _emloyeeDbContext.RemoveById(employee.Id);
                await _emloyeeDbContext.SaveChangesAsync(CancellationToken.None);
                _refresh = true;
                _logger.LogInformation($"Сотрудник \"{deletedEmployee}\" был удален");
                console.Items.Add($"Сотрудник \"{deletedEmployee}\" был удален");
            }
        }
        private void Items_CollectionChanged(object? sender, EventArgs e)
        {
            Border border = (Border)VisualTreeHelper.GetChild(console, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            scrollViewer.ScrollToBottom();
        }
    }
}
using WpfApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace WpfApplication.Interfaces
{
    public interface IEmloyeesDbContext
    {
        DbSet<Employee> Employees { get; set; }

        async Task CreateEmloyee(Company company, byte[] image, string name, string surname)
        {
            Employee employee = new Employee
            {
                Company = company,
                Photo = image,
                Name = name,
                Surname = surname
            };
            Employees.AddAsync(employee);
        }
        List<Employee> GetEmployeesByCompany(int companyId)
        {
            List<Employee> employees = Employees.Where(company => company.CompanyId == companyId)
                                                .ToList();
            return employees;
        }
        List<Employee> GetAll()
        {
            List<Employee> employees = Employees.ToList();
            return employees;
        }
        List<Employee> GetBySting(string info)
        {
            List<Employee> searchListByName = Employees.Where(x => EF.Functions.Like(x.Name!, $"%{info}%"))
                                                       .ToList();         
            List<Employee> searchListBySurname = Employees.Where(x => EF.Functions.Like(x.Surname!, $"%{info}%"))
                                                          .ToList();
            searchListByName = (searchListByName.Union(searchListBySurname)).ToList();
            return searchListByName;
        }
        void AddEmployee(Employee employee)
        {
            Employees.Add(employee);
        }
        Employee FindEmployeeById(int id)
        {
            Employee employee = Employees.First(employeeId => employeeId.Id == id);
            return employee;
        }
        void RemoveById(int id)
        {
            Employee employee = Employees.First(employeeId => employeeId.Id == id);
            Employees.Remove(employee);
        }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

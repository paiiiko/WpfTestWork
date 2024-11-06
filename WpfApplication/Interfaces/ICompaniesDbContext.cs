using WpfApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace WpfApplication.Interfaces
{
    public interface ICompaniesDbContext
    {
        DbSet<Company> Companies { get; set; }

        bool HasData()
        {
            var x = Companies.FirstOrDefault();
            if (Companies.FirstOrDefault() == null)
            {
                return false;
            }
            return true;
        }
        void AddCompany(Company company)
        {
            Companies.Add(company);
        }
        Company FindCompanyById(int id)
        {
            Company company = Companies.First(companyId => companyId.Id == id);
            return company;
        }
        
        List<Company> GetAll()
        {
            List<Company> companies = Companies.ToList();
            return companies;
        }
        void RemoveById(int id)
        {
            Company company = Companies.First(companyId => companyId.Id == id);
            Companies.Remove(company);
        }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
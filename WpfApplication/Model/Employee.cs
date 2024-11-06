namespace WpfApplication.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public byte[]? Photo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
    }
}
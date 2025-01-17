﻿namespace WpfApplication.Model
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Employee>? Employees { get; set; }
        public override string ToString() => $"{Name}";
    }
}

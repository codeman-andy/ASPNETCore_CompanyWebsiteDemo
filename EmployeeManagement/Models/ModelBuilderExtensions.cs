using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder model_builder)
        {
            model_builder.Entity<Employee>().HasData(
                new Employee
                {
                    ID = 1,
                    Name = "Mary",
                    Email = "mary@pragimtech.com",
                    Department = Dept.IT
                },
                new Employee
                {
                    ID = 2,
                    Name = "John",
                    Email = "john@pragimtech.com",
                    Department = Dept.HR
                }
            );
        }
    }
}

namespace EmployeeManagement.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDBContext context;

        public SQLEmployeeRepository(AppDBContext context)
        {
            this.context = context;
        }

        public Employee Add(Employee employee)
        {
            context.Employees.Add(employee);

            context.SaveChanges();

            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = context.Employees.Find(id);

            if (employee != null)
            {
                context.Employees.Remove(employee);

                context.SaveChanges();
            }

            return employee;
        }

        public IEnumerable<Employee> GetAll()
        {
            return context.Employees;
        }

        public Employee GetByID(int id)
        {
            return context.Employees.Find(id);
        }

        public Employee Update(Employee updated_employee)
        {
            var employee = context.Employees.Attach(updated_employee);

            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            context.SaveChanges();

            return updated_employee;
        }
    }
}

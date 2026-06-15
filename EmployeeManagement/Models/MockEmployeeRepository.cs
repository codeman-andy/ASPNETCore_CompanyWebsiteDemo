namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employees;

        public MockEmployeeRepository()
        {
            _employees = new List<Employee>()
            {
                new Employee() { ID = 1, Name = "Mary", Email = "mary@pragimtech.com", Department = Dept.HR},
                new Employee() { ID = 2, Name = "John", Email = "john@pragimtech.com", Department = Dept.IT},
                new Employee() { ID = 3, Name = "Sam" , Email = "sam@pragimtech.com" , Department = Dept.IT},
            };
        }

        public Employee GetByID(int id)
        {
            return _employees.FirstOrDefault(employee => employee.ID == id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employees;
        }

        public Employee Add(Employee employee)
        {
            employee.ID = _employees.Max(x => x.ID) + 1;

            _employees.Add(employee);

            return employee;
        }

        public Employee Update(Employee updated_employee)
        {
            Employee employee = _employees.FirstOrDefault(e => e.ID == updated_employee.ID);

            if (employee != null)
            {
                employee.Name = updated_employee.Name;
                employee.Email = updated_employee.Email;
                employee.Department = updated_employee.Department;
            }

            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = _employees.FirstOrDefault(e => e.ID == id);

            if (employee != null)
            {
                _employees.Remove(employee);
            }

            return employee;
        }
    }
}

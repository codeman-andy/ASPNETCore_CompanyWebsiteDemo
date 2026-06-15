namespace EmployeeManagement.Models
{
    public interface IEmployeeRepository
    {
        Employee GetByID(int id);

        IEnumerable<Employee> GetAll();

        Employee Add(Employee employee);

        Employee Update(Employee updated_employee);

        Employee Delete(int id);
    }
}

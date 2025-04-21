using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedEmployees(DataContext context)
        {
            if(await context.Employee.AnyAsync())

            return ;

            var employeeData = await File.ReadAllTextAsync("Data/EmployeeSeedData.json");
            var departmentData = await File.ReadAllTextAsync("Data/DepartmentSeedData.json");
            _ = JsonSerializer.Deserialize<List<Employee>>(employeeData);

            var departments = JsonSerializer.Deserialize<List<AppDepartment>>(departmentData);
            foreach(var department in departments)
            {
                context.Department.Add(department);
            }
            await context.SaveChangesAsync();
        }
    }
}
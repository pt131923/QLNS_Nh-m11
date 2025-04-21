using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Employee
    {
        [Key]
        public int id { get; set; }
        public string employeeName { get; set; }
        public int departmentId { get; set; }
        public AppDepartment Department { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class AppDepartment
    {
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public int slNhanVien { get; set; }
    public string description { get; set; }
    public string adresses { get; set; }
    public string notes { get; set; }
    public List<Employee> Employee { get; set; } = new List<Employee>();

    }
}


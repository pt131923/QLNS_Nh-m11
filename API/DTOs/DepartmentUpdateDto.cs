namespace API.DTOs
{
    public class DepartmentUpdateDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public int slNhanVien { get; set; }
        public string description { get; set; }
        public string adresses { get; set; }
        public string notes { get; set; }
    }
}
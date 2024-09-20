using System.Net;

namespace XML_Generation_from__HTML_view.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }
        public Address Address { get; set; }
    }
}

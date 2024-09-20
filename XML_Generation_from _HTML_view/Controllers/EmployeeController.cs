using Microsoft.AspNetCore.Mvc;
using XML_Generation_from__HTML_view.Models.VModels;
using XML_Generation_from__HTML_view.Models;
using XML_Generation_from__HTML_view.Services;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using HtmlAgilityPack;
using System.IO.Compression;
using System.Xml.Serialization;
using XML_Generation_from__HTML_view.Models.XmlContent;

namespace XML_Generation_from__HTML_view.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ViewRenderService _viewRenderService;

        public async Task<IActionResult> Index()
        {
            var employees = GetEmployeeList();

            // Generate the XML file when the Employee List page loads
            var htmlContent = await _viewRenderService.RenderViewToStringAsync("Views/Employee/Index.cshtml", employees);
            var htmlToXmlConverter = new HtmlToXmlConverter();
            var xmlDocument = htmlToXmlConverter.ConvertHtmlToXml(htmlContent);

            // Save the XML string to a file
            var xmlString = xmlDocument.ToString();
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"EmployeeList_{timestamp}.xml";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "xml");

            // Check if the directory exists and create it if not
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);
            System.IO.File.WriteAllText(filePath, xmlString);

            return View(employees);
        }

        public EmployeeController(ViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        private EmployeeListViewModel GetEmployeeList()
        {
            return new EmployeeListViewModel
            {
                Employees = new List<Employee>
                {
                    new Employee
                    {
                        EmployeeId = 1, EmployeeName = "John Doe",
                        Department = new Department { DepartmentId = 1, DepartmentName = "HR" },
                        Position = new Position { PositionId = 1, PositionName = "Manager" },
                        Address = new Address { Street = "123 Main St", City = "CityA", ZipCode = "12345" }
                    },
                    new Employee
                    {
                        EmployeeId = 2, EmployeeName = "Jane Smith",
                        Department = new Department { DepartmentId = 2, DepartmentName = "IT" },
                        Position = new Position { PositionId = 2, PositionName = "Developer" },
                        Address = new Address { Street = "456 Elm St", City = "CityB", ZipCode = "67890" }
                    }
                }
            };
        }

        [HttpPost]
        public IActionResult CreateZipFile(int employeeId)
        {
            var employee = GetEmployeeList().Employees.FirstOrDefault(e => e.EmployeeId == employeeId); // Fetch the specific employee

            if (employee == null)
            {
                return NotFound();
            }

            // Create a MemoryStream to hold the ZIP file
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Create document file (example: text file)
                    var documentEntry = zipArchive.CreateEntry($"{employee.EmployeeName}_Document.txt");
                    using (var entryStream = documentEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        writer.Write($"Document for {employee.EmployeeName}");
                    }

                    // Create XML file
                    var xmlData = new XmlDataModel
                    {
                        XmlDataId = employee.EmployeeId,
                        XmlContent = new EmployeeContent
                        {
                            Name = employee.EmployeeName,
                            Department = employee.Department.DepartmentName,
                            Position = employee.Position.PositionName,
                            Address = new AddressContent
                            {
                                Street = employee.Address.Street,
                                City = employee.Address.City,
                                ZipCode = employee.Address.ZipCode
                            }
                        }
                    };

                    var xmlEntry = zipArchive.CreateEntry($"{employee.EmployeeName}_Data.xml");
                    using (var entryStream = xmlEntry.Open())
                    {
                        var serializer = new XmlSerializer(typeof(XmlDataModel));
                        serializer.Serialize(entryStream, xmlData);
                    }
                }

                // Return the ZIP file
                var zipFileName = $"{employee.EmployeeName}_Files.zip"; // Name the ZIP file according to the employee
                return File(memoryStream.ToArray(), "application/zip", zipFileName);
            }
        }
    }
}

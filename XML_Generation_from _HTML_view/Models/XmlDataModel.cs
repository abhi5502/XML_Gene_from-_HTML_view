using System.Xml.Serialization;
using XML_Generation_from__HTML_view.Models.XmlContent;

namespace XML_Generation_from__HTML_view.Models
{
    //[XmlRoot("XmlDataModel")]
    //public class XmlDataModel
    //{
    //    [XmlElement("XmlDataId")]
    //    public int XmlDataId { get; set; }

    //    [XmlElement("XmlContent")]
    //    public string XmlContent { get; set; }
    //}


    [XmlRoot("XmlDataModel")]
    public class XmlDataModel
    {
        [XmlElement("XmlDataId")]
        public int XmlDataId { get; set; }

        [XmlElement("XmlContent")]
        public EmployeeContent XmlContent { get; set; }
    }
}

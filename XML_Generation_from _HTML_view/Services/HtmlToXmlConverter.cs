using HtmlAgilityPack;
using System.Text;

namespace XML_Generation_from__HTML_view.Services
{
    public class HtmlToXmlConverter
    {
        public string ConvertHtmlToXml(string html)
        {
            var xml = new StringBuilder();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append("<root>");
            xml.Append(TraverseNodes(root));
            xml.Append("</root>");
            return xml.ToString();
        }

        private string TraverseNodes(HtmlNode node)
        {
            var xml = new StringBuilder();
            var nodeName = SanitizeNodeName(node.Name);
            xml.Append("<" + nodeName + ">");
            if (node.HasChildNodes)
            {
                foreach (var child in node.ChildNodes)
                {
                    xml.Append(TraverseNodes(child));
                }
            }
            else
            {
                xml.Append(System.Security.SecurityElement.Escape(node.InnerText));
            }
            xml.Append("</" + nodeName + ">");
            return xml.ToString();
        }

        private string SanitizeNodeName(string nodeName)
        {
            // Replace invalid characters with an underscore
            var sanitized = new StringBuilder();
            foreach (var ch in nodeName)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-')
                {
                    sanitized.Append(ch);
                }
                else
                {
                    sanitized.Append('_');
                }
            }
            return sanitized.ToString();
        }
    }
}

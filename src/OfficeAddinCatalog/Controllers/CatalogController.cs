using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace OfficeAddinCatalog.Controllers
{
    [Route("catalog/_api/web")]
    public class CatalogController : Controller
    {
        [HttpGet]
        [Route("lists")]
        [Produces("application/atom+xml")]
        public IActionResult Index()
        {
            var sharepointListCategory = new SyndicationCategory
            {
                Name = "SP.List",
                Scheme = "http://schemas.microsoft.com/ado/2007/08/dataservices/scheme"
            };

            XNamespace nsD = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            XNamespace nsM = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
            var properties = new XElement(nsM + "properties", new XElement(nsD + "Id", new XAttribute(nsM + "Type", "Edm.Guid"), "6ea75bf4-6deb-4369-ad42-0a4651a760b6"));

            var addinEntry = new SyndicationItem()
            {
                Id = "Lists(guid'6ea75bf4-6deb-4369-ad42-0a4651a760b6')",
                Categories = { sharepointListCategory },
                Content = SyndicationContent.CreateXmlContent(properties)
            };

            var feed = new SyndicationFeed()
            {
                Id = "321BF989-6C1D-4121-A138-B7369F22A6C8",
                Title = SyndicationContent.CreatePlaintextContent("Microsoft Office Add-in Catalog"),
                LastUpdatedTime = DateTimeOffset.UtcNow,
                Items = new List<SyndicationItem> { addinEntry }
                //BaseUri = new Uri(this.Url.Content("~"))
            };
            feed.AttributeExtensions.Add(new XmlQualifiedName("d", XNamespace.Xmlns.NamespaceName), nsD.NamespaceName);
            feed.AttributeExtensions.Add(new XmlQualifiedName("m", XNamespace.Xmlns.NamespaceName), nsM.NamespaceName);

            return new AtomResult(feed);
        }
    }
}

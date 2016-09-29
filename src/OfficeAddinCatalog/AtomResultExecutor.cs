using System;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Net.Http.Headers;

namespace OfficeAddinCatalog
{
    public class AtomResultExecutor
    {
        private static readonly string DefaultContentType = new MediaTypeHeaderValue("application/atom+xml")
        {
            Encoding = Encoding.UTF8
        }.ToString();

        public AtomResultExecutor(IHttpResponseStreamWriterFactory writerFactory)
        {
            if (writerFactory == null)
            {
                throw new ArgumentNullException(nameof(writerFactory));
            }

            WriterFactory = writerFactory;
        }

        protected IHttpResponseStreamWriterFactory WriterFactory { get; }

        /// <summary>
        /// Executes the <see cref="JsonResult"/> and writes the response.
        /// </summary>
        /// <param name="context">The <see cref="ActionContext"/>.</param>
        /// <param name="result">The <see cref="JsonResult"/>.</param>
        /// <returns>A <see cref="Task"/> which will complete when writing has completed.</returns>
        public Task ExecuteAsync(ActionContext context, AtomResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var response = context.HttpContext.Response;

            string resolvedContentType = null;
            Encoding resolvedContentTypeEncoding = null;
            ResponseContentTypeHelper.ResolveContentTypeAndEncoding(
                DefaultContentType,
                response.ContentType,
                DefaultContentType,
                out resolvedContentType,
                out resolvedContentTypeEncoding);

            response.ContentType = resolvedContentType;

            var feedFormatter = new Atom10FeedFormatter(result.SyndicationFeed);
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = true;

            using (var writer = WriterFactory.CreateWriter(response.Body, resolvedContentTypeEncoding))
            {
                using (var xmlWriter = XmlWriter.Create(writer, xmlWriterSettings))
                {
                    feedFormatter.WriteTo(xmlWriter);
                }
            }

            return TaskCache.CompletedTask;
        }
    }
}

using System;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace OfficeAddinCatalog
{
    public class AtomResult : ActionResult
    {
        public AtomResult(SyndicationFeed syndicationFeed)
        {
            this.SyndicationFeed = syndicationFeed;
        }

        public SyndicationFeed SyndicationFeed { get; set; }


        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<AtomResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }
    }
}

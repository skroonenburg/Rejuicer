using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Rejuicer
{
	public class RejuicerHandler : DefaultHttpHandler
	{
        public override bool IsReusable
        {
            get { return true; }
        }

        public override void ProcessRequest(HttpContext context)
        {
            var requested = context.Request.Path;

            if (RejuicerEngine.HasConfigurationFor(requested))
            {
                var result = RejuicerEngine.GetContentFor(requested);
                context.Response.Write(result);
                context.Response.Flush();

                context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                base.ProcessRequest(context);
            }
        }
    }
}

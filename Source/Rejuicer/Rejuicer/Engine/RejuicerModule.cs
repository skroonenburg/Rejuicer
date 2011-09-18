using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Rejuicer
{
    public class RejuicerModule : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        /// <summary>
        /// A request has been received.
        /// </summary>
        private static void context_BeginRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var requested = request.Url;

            if (RejuicerEngine.HasConfigurationFor(requested))
            {
                var result = RejuicerEngine.GetContentFor(requested);

                var response = HttpContext.Current.Response;
                
                response.ContentType = result.ContentType;

                if (HttpRuntime.UsingIntegratedPipeline)
                {
                    response.Headers.Add("Last-Modified", result.LastModifiedDate.ToString());
                }

                if (result.AllowClientCaching)
                {
                    response.Expires = 525600; // Browser cache expires after one year
                    response.CacheControl = "public"; // Any entity can cache this - be it the browser or a proxy server
                }
                else
                {
                    response.CacheControl = "no-cache";
                }

                if (HttpRuntime.UsingIntegratedPipeline && result.LastModifiedDate != null && result.LastModifiedDate.Equals(request.Headers["If-Modified-Since"]))
                {
                    response.StatusCode = 304;
                }
                else
                {
                    var buffer = new byte[1024];
                    result.Content.Seek(0, SeekOrigin.Begin);

                    while (result.Content.Read(buffer, 0, buffer.Length) > 0)
                    {
                        response.BinaryWrite(buffer);
                    }
                    
                    result.Content.Seek(0, SeekOrigin.Begin);
                }

                response.Flush();

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}

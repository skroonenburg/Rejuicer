using Rejuicer;

[assembly: WebActivator.PostApplicationStartMethod(typeof(MvcDemo.App_Start.RejuicerContent), "Configure")]
namespace MvcDemo.App_Start
{
    public static class RejuicerContent
    {
        public static void Configure()
        {
            OnRequest.ForJs("~/Combined-{0}.js")
                     .Compact
                     .FilesIn("~/Scripts/")
                       .Matching("*.js")
                     .FilesIn("~/Scripts/")
                       .Matching("*.coffee")
                     .Configure();

            OnRequest.ForCss("~/Combined.css")
                     .Compact
                     .File("~/Content/Site.css")
                     .Configure();
        }
    }
}

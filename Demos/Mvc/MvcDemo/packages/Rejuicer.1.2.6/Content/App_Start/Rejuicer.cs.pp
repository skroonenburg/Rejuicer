using Rejuicer;

[assembly: WebActivator.PostApplicationStartMethod(typeof($rootnamespace$.App_Start.RejuicerContent), "Configure")]
namespace $rootnamespace$.App_Start
{
    public static class RejuicerContent
    {
        public static void Configure()
        {
            /*
            OnRequest.ForJs("~/Combined-{0}.js")
                .Compact
                .FilesIn("~/Scripts/")
                .Matching("*.js")
                .FilesIn("~/Scripts/")  // Include coffee script, these will be automatically compiled to javascript
                .Matching("*.coffee")
                .Configure();

            OnRequest.ForCss("~/Combined.css")
                .Compact
                .File("~/Content/Site.css")
                .Configure();
            */
        }
    }
}

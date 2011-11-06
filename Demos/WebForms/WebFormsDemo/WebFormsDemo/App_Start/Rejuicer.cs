using Rejuicer;

[assembly: WebActivator.PostApplicationStartMethod(typeof(WebFormsDemo.App_Start.RejuicerContent), "Configure")]
namespace WebFormsDemo.App_Start
{
    public static class RejuicerContent
    {
        public static void Configure()
        {
            OnRequest.ForJs("~/Combined-{0}.js")
                     .Compact
                     .FilesIn("~/Scripts/")
                       .Matching("*.js")
                     .Configure();

            OnRequest.ForCss("~/Combined.css")
                     .Compact
                     .FilesIn("~/Styles/")
                         .Matching("*.css")
                     .Configure();
        }
    }
}

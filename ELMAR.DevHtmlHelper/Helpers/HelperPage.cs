using System.Web.Mvc;
using System.Web.WebPages;

namespace ELMAR.DevHtmlHelper.Helpers
{
    public class HelperPage : System.Web.WebPages.HelperPage
    {
        public static new HtmlHelper Html
        {
            get
            {
                return ((WebViewPage)WebPageContext.Current.Page).Html;
            }
        }
    }
}
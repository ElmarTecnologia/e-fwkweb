using System.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Helpers
{
    public static class FwkHelper
    {
        public static MvcHtmlString Submit(this HtmlHelper helper, string text)
        {
            string str = string.Format("<input id=\"Submit1\" type=\"submit\" value=\"{0}\"/>", text);
            return new MvcHtmlString(str);
        }

        public static MvcHtmlString Button(string Name, string Titulo, string Tema = "Default", string Controller = "", string Action = "", string Url = "", bool VerificaAutorizacao = true)
        {
            return new MvcHtmlString("");
        }

    }
}
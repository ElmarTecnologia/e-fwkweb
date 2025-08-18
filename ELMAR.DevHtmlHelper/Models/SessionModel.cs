using ELMAR.DevHtmlHelper.Controllers;
using System.Web;

namespace ELMAR.DevHtmlHelper.Models
{
    public class SessionModel
    {
        public string getCTXCode(System.Web.SessionState.HttpSessionState Session, System.Web.HttpRequest Request, string key)
        {

            string sessionCode = string.Empty;
            if (Session[key] != null && !string.IsNullOrEmpty(Session[key].ToString()))
                sessionCode = Session[key].ToString();
            else
            {
                if (SessionModel.getCookie(Request, key) != null && SessionModel.getCookie(Request, key).Value != null)
                    Session[key] = SessionModel.getCookie(Request, key).Value;
                new UsuarioController().Logout();
            }

            return sessionCode;
        }

        public string getCTXCode(HttpSessionStateBase Session, HttpRequestBase Request, string key)
        {
            string sessionCode = string.Empty;
            if (Session[key] != null && !string.IsNullOrEmpty(Session[key].ToString()))
                sessionCode = Session[key].ToString();
            else
            {
                if (SessionModel.getCookie(Request, key) != null && !string.IsNullOrEmpty(SessionModel.getCookie(Request, key).Value))
                    Session[key] = SessionModel.getCookie(Request, key).Value;

                if (Session[key] != null && !Session[key].ToString().Equals(string.Empty))
                    new UsuarioController().Logout();
            }

            return sessionCode;
        }

        public static HttpCookie getCookie(HttpRequest Request, string key)
        {
            try
            {
                return (Request != null) ? Request.Cookies[key] : null;
            }
            catch { return null; }
        }

        public static HttpCookie getCookie(HttpRequestBase Request, string key)
        {
            try
            {
                return (Request != null) ? Request.Cookies[key] : null;
            }
            catch { return null; }
        }
    }
}
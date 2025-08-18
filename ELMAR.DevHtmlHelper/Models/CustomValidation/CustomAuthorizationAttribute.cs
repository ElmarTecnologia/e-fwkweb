using Microsoft.Owin.Security;
using ELMAR.DevHtmlHelper.Models.TokenProvider;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace ELMAR.DevHtmlHelper.Models.CustomValidation
{
    /// <summary>
    /// Validação customizada para autorização
    /// </summary>
    public class FwkAuthorizeAttribute : AuthorizeAttribute
    {
        private string customLogin;
        public string CustomLogin {
            get {
                if (string.IsNullOrEmpty(customLogin))
                    return FwkConfig.GetSettingValue("pathApp") + "/Usuario/Logout/";
                return customLogin;
            }
            set { customLogin = value; }
        }
        public string CustomCall { get; set; }
        public string CustomError { get; set; }
        public string Contexto { get; set; }
        UsuarioAutorizacao userAut = null;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //write here custom authorization logic
            #region Valida a autorização/parametrização a partir do FWK
            this.Contexto = httpContext.Request["ctx"] ?? httpContext.Request["e"];
            //Define o contexto para verificação (Evita a troca de contexto sem efetuar novo login)
            Contexto = Core.GetSetCTX(httpContext);
            if (!Usuario.VerificaAutorizacao(out userAut, httpContext, this.CustomLogin, this.Contexto, this.CustomCall))
            {
                //return Redirect(userAut.Redirect);
                return false;
            }
            #endregion

            #region Define os parâmetros da rota a partir da chave de sessão 'SelectedIDs'
            if (httpContext.Session["selectedIDs"] != null)
            {
                Dictionary<int, RouteValueDictionary> selectedIDs = (Dictionary<int, RouteValueDictionary>)httpContext.Session["selectedIDs"];
                //Adiciona os parâmetros
                if (selectedIDs.Count > 0)
                {
                    foreach (var item in selectedIDs[0])
                    {
                        if (!httpContext.Request.RequestContext.RouteData.Values.ContainsKey(item.Key))
                            httpContext.Request.RequestContext.RouteData.Values.Add(item.Key, item.Value);
                    }
                    httpContext.Session["selectedIDs"] = null;
                }
            }
            #endregion

            #region Definindo os parâmetros da action, caso existam
            //Parametrização específica por Chamada. Possui prioridade à parametrização global do usuário
            if (this.userAut.getRouteParameters().Count > 0)
            {
                foreach (KeyValuePair<string, object> item in this.userAut.getRouteParameters())
                    httpContext.Request.RequestContext.RouteData.Values.Add(item.Key, item.Value);
            }
            //Adiciona parametrização global do usuário
            Usuario usu = new Usuario().GetUsuario(userAut.UsuAutUsuCodigo);
            if (usu != null && usu.getRouteParameters().Count > 0)
            {
                foreach (KeyValuePair<string, object> item in usu.getRouteParameters())
                {
                    if (!httpContext.Request.RequestContext.RouteData.Values.ContainsKey(item.Key))
                        httpContext.Request.RequestContext.RouteData.Values.Add(item.Key, item.Value);
                }
            }
            #endregion

            return true;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            Controller controller = filterContext.Controller as Controller;

            if (controller != null)
            {
                if (string.IsNullOrEmpty(this.userAut.Redirect)/*!this.userAut.Redirect.StartsWith("http")*/)
                {
                    //Retorna apenas a mensagem caso não autorizado, sem redirect
                    filterContext.Result = new JavaScriptResult() { Script = string.IsNullOrEmpty(this.CustomError) ? userAut.Info : this.CustomError };
                    //new RedirectResult(FwkConfig.GetSettingValue("pathApp", this.Contexto) + this.userAut.Redirect);
                }
                else
                {
                    filterContext.Result = new System.Web.Mvc.RedirectResult(this.userAut.Redirect);
                }
            }

        }
    }
    /// <summary>
    /// Utilizado para autenticar métodos de api
    /// </summary>
    public class IFWAuthorizeApi : System.Web.Mvc.FilterAttribute, System.Web.Http.Filters.IAuthorizationFilter
    {
        public string Bearer { get; set; }

        public Task<System.Net.Http.HttpResponseMessage> ExecuteAuthorizationFilterAsync(System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken, Func<System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage>> continuation)
        {
            bool isValid = false;
            if (actionContext.Request.Headers.Authorization != null)
            {
                AuthenticationTicket authenticationTicket = null;
                try
                {
                    string pathApp = ConfigurationManager.AppSettings["pathApp"] == null ? 
                        "https://" + actionContext.Request.RequestUri.Host : ConfigurationManager.AppSettings["pathApp"].ToString();
                    authenticationTicket =
                        new CustomJwtFormat(pathApp) //Issuer inativado
                        //new CustomJwtFormat(1pathApp) //Issuer ativado
                        .Unprotect(actionContext.Request.Headers.Authorization.Parameter);
                }
                catch(Exception ex){
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Not Authorized. Details: "+ex.Message) });
                    //throw new Exception("not authorized");
                }
                
                var claims = authenticationTicket.Identity.Claims;

                var routeData = ((System.Web.Http.ApiController)actionContext.ControllerContext.Controller).RequestContext.RouteData;

                var ecodeRouteParameter = routeData != null && routeData.Values.ContainsKey("ctx") ? routeData.Values["ctx"].ToString() : string.Empty;
                ecodeRouteParameter = ecodeRouteParameter ?? (routeData != null && routeData.Values.ContainsKey("ecode") ? routeData.Values["ecode"].ToString() : string.Empty);

                //Route ctx validation
                foreach (var item in claims)
                {
                    // Master Claim - MultiTenancy
                    if (item.Value.Equals("999025") || item.Value.Equals("elmar") || item.Value.Equals("SUPER"))
                    {
                        isValid = true;
                        break;
                    }
                    //Valid route execution
                    if (item.Value.Equals(ecodeRouteParameter))
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Not Allowed") });
                }

                foreach (var item in claims)
                {
                    #region Perfil SUPER - Autoriza de todas as chamadas
                    if (item.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        && item.Value.Equals("SUPER"))
                    {
                        isValid = true;
                        break;
                    }
                    #endregion

                    #region  Valida autorização chamada
                    if (!isValid)
                    {
                        string action = (actionContext.ActionDescriptor).ActionName;
                        string controller = ((System.Web.Http.ApiController)actionContext.ControllerContext.Controller).ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                        string call = "/" + controller.ToString() + "/" + (action == null ? "Get" : action.ToString());
                        if (call.ToLower().Equals(item.Value.ToString().ToLower()))
                        {
                            isValid = true;
                            break;
                        }
                    }
                    #endregion
                }
            }

            if (!isValid) 
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Not Authorized") });
            }            

            return continuation();
        }
    }    
}
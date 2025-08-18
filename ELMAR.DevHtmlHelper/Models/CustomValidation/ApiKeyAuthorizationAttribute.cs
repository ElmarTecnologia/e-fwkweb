using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace ELMAR.DevHtmlHelper.Models.CustomValidation
{    
    /// <summary>
    /// Utilizado para autenticar métodos de api
    /// </summary>
    public class IFWAuthorizeApiKey : System.Web.Mvc.FilterAttribute, System.Web.Http.Filters.IAuthorizationFilter
    {
        private readonly List<string> _apiKeys = new List<string>() { "EA4E800A-5DB3-48E5-88BA-4CB75124750B" };

        public Task<System.Net.Http.HttpResponseMessage> ExecuteAuthorizationFilterAsync(System.Web.Http.Controllers.HttpActionContext context, System.Threading.CancellationToken cancellationToken, Func<System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage>> continuation)
        {
            try
            {
                var apiKey = (string[])context.Request.Headers.GetValues("x-api-key");

                if (string.IsNullOrEmpty(apiKey[0]) && !_apiKeys.Contains(apiKey[0]))
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Not Allowed") });
                }

                return continuation();
            }
            catch
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Not Allowed") });
            }
        }                               
    }    
}
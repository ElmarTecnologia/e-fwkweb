using Microsoft.Owin;
using Owin;
using System;
using System.Web.Http;
using ELMAR.DevHtmlHelper.Models.TokenProvider;
using Microsoft.Owin.Security.OAuth;
using System.Configuration;

[assembly: Microsoft.Owin.OwinStartup(typeof(ELMAR.DevHtmlHelper.Startup))]
namespace ELMAR.DevHtmlHelper
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            //WebApiConfig.Register(config);
            //app.UseWebApi(config);

            var opcoesConfiguracaoToken = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
                Provider = new TokenProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["pathApp"].ToString()),                
                //AuthorizeEndpointPath = new PathString("/authorize")
            };

            app.UseOAuthAuthorizationServer(opcoesConfiguracaoToken);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions(){});

            //var issuer = ConfigurationManager.AppSettings["pathApp"].ToString();
            //var audience = WebApplicationAccess.WebApplicationAccessList.Select(x => x.Value.ClientId).AsEnumerable();
            //var secretsSymmetricKey = (from x in WebApplicationAccess.WebApplicationAccessList
            //                           select new SymmetricKeyIssuerSecurityKeyProvider(issuer, TextEncodings.Base64Url.Decode(x.Value.SecretKey))).ToArray();
            //app.UseJwtBearerAuthentication(
            //    new JwtBearerAuthenticationOptions
            //    {
            //        AuthenticationMode = AuthenticationMode.Active,
            //        AllowedAudiences = audience,
            //        IssuerSecurityKeyProviders = secretsSymmetricKey
            //    });

        }
    }
}

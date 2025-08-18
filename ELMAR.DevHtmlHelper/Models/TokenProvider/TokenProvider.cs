using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ELMAR.DevHtmlHelper.Models.TokenProvider
{
    public class TokenProvider : OAuthAuthorizationServerProvider
    {
        private readonly FwkContexto _contextoFwk = new FwkContexto();

        public override Task ValidateClientAuthentication
            (OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            string symmetricKeyAsBase64 = string.Empty;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            //if (context.ClientId == null)
            //{
            //    context.SetError("invalid_clientId", "client_Id não pode ser nulo");
            //    return Task.FromResult<object>(null);
            //}
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task GrantResourceOwnerCredentials
                (OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // encontrando o usuário
            string senha = Core.Crypto(context.Password);
            Usuario usuario = null;
            List<PerfilAutorizacao> lstPerAut = new List<PerfilAutorizacao>();
            ClaimsIdentity identity = null;

            using (var _contextoFwk = new FwkContexto())
            {
                usuario = _contextoFwk.Usuarios
                    .FirstOrDefault(x => x.Login == context.UserName
                                    && x.Senha == senha && x.Ativo == true);            

                // cancelando a emissão do token se o usuário não for encontrado
                if (usuario == null)
                {
                    context.SetError("invalid_grant",
                        "Credentials Validation Error. Invalid User.");
                    return Task.FromResult<object>(null);
                }

                //Verifica se o usuário possui o contexto definido
                bool validContext = false;
                foreach (var item in usuario.Contextos)
                {
                    if (item.Contexto.Equals(context.Scope[0]))
                    {
                        validContext = true;
                    }
                }

                if (!validContext)
                {
                    context.SetError("invalid_grant",
                        "Error on credentials validation. Invalid Context.");
                    return Task.FromResult<object>(null);
                }

                identity = new ClaimsIdentity("JWT");
                identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, context.UserName));
                identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.PrimarySid, context.Scope[0]));

                UsuarioPerfil usuPer = usuario.getCtxUsuPerfil(context.Scope[0]);
                identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, usuPer.Titulo));
                lstPerAut = (from p_alt in _contextoFwk.PerfilAutorizacoes 
                             where 
                             p_alt.AutPerfil.Equals(usuPer.Titulo) 
                             && p_alt.PerAutAtivo 
                             select p_alt).ToList<PerfilAutorizacao>();
             }

            foreach (var item in lstPerAut)
            {
                if (!item.PerAutAtivo)
                    continue;
                identity.AddClaim(new System.Security.Claims.Claim("call", item.AutChamada));
            }
            var props = new Microsoft.Owin.Security.AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         "audience", (context.ClientId == null) ? string.Empty : context.ClientId
                    }
                });
            var ticket = new AuthenticationTicket(identity, props);
            ticket.Properties.IssuedUtc = DateTime.Now;
            ticket.Properties.ExpiresUtc = ticket.Properties.IssuedUtc.Value.AddHours(10);
            
            context.OwinContext.Authentication.SignIn(identity);

            context.Validated(ticket);
            WebApplicationAccess.GrantApplication(ConfigurationManager.AppSettings["appName"].ToString());
            return Task.FromResult<object>(null);
        }

    }

}
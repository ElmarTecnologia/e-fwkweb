using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ELMAR.DevHtmlHelper.Models.TokenProvider
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer = string.Empty;
        private SigningConfigurations _signingConfigurations = new SigningConfigurations();

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }
        public string Protect(AuthenticationTicket data)
        {
            var signigKey = _signingConfigurations.SigningCredentials;

            if (data == null)
                throw new ArgumentNullException("data");

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                _issuer,
                null, 
                data.Identity.Claims, 
                issued.Value.UtcDateTime, 
                expires.Value.UtcDateTime, 
                _signingConfigurations.SigningCredentials                
            );
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);
            return jwt;
        }
        public AuthenticationTicket Unprotect(string protectedText)
        {
            string audienceId = "";
            Guid clientId;
            bool isValidAudience = Guid.TryParse(audienceId, out clientId);
            
            var signigKey = _signingConfigurations.SigningCredentials;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = audienceId,
                ValidIssuer = _issuer, //Valida o host da aquisição do token
                IssuerSigningKey = _signingConfigurations.Key,
                ValidateLifetime = true,
                ValidateAudience = isValidAudience,
                ValidateIssuer = true,
                RequireSignedTokens = false,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = false                                
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken token = null;
            var principal = handler.ValidateToken(protectedText, tokenValidationParameters, out token);
            var identity = principal.Identities;

            return new AuthenticationTicket(identity.First(), new Microsoft.Owin.Security.AuthenticationProperties());
        }
    }
}
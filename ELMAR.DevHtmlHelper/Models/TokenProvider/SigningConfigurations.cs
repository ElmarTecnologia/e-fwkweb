using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ELMAR.DevHtmlHelper.Models.TokenProvider
{
    public class SigningConfigurations
    {
        public Microsoft.IdentityModel.Tokens.SecurityKey Key { get; }
        public Microsoft.IdentityModel.Tokens.SigningCredentials SigningCredentials { get; }

        public SigningConfigurations()
        {
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                //Key = new RsaSecurityKey(provider.ExportParameters(true));
                Key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes("ASHJASJKhhshasgyasuauyywJTATRDSSGKJUwwunsx"));
            }

            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                Key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);
        }
    }
}
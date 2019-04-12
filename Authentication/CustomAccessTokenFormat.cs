using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Authentication
{
    public class CustomAccessTokenFormat: ISecureDataFormat<AuthenticationTicket>
    {
        public string Protect(AuthenticationTicket data)
        {
            var thumbprint = ConfigurationManager.AppSettings["certificate.thumbprint"];
            var certificate = GetCertificate(thumbprint);
            if (certificate == null)
            {
                var exception = new Exception("Fail to load certificate");
                exception.Data["thumbprint"] = thumbprint;
                throw exception;
            }

            var key = new X509SecurityKey(certificate);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var issuer = "issuer";
            var audience = "audience";
            var subject = data.Identity;
            var issuedAt = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddMinutes(20);
            var jwtSecurityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(issuer, audience, subject, issuedAt, expires, issuedAt, signingCredentials);
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }

        private X509Certificate2 GetCertificate(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(i => i.Thumbprint == thumbprint);
                return certificate;
            }
            finally
            {
                store.Close();
            }
        }
    }
}
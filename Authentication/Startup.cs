using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Authentication.Startup))]

namespace Authentication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var oAuthAuthorizationServerProvider = new OAuthAuthorizationServerProvider
            {
                OnValidateClientAuthentication = ValidateClientAuthentication
            };
            var oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions
            {
#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString("/token"),
                Provider = oAuthAuthorizationServerProvider
            };
            app.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);

            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Hello World!");
            });
        }

        private static Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.TryGetFormCredentials(out var clientId, out var clientSecret);
            if (clientId == "client_id" && clientSecret == "client_secret")
            {
                context.Validated(clientId);
            }

            return Task.FromResult<object>(null);
        }

    }
}

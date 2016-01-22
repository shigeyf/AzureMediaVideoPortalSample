//----------------------------------------------------------------------------------------------
//    Copyright 2014 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
//using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.MediaServices.Client;
using Owin;
using VideoPortalDemo.Models;

namespace VideoPortalDemo
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        //string graphResourceId = "https://graph.windows.net";

        // added by shigeyf@microsoft.com to the original template
        private string graphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];
        private string kdResourceId = ConfigurationManager.AppSettings["ida:KeyDeliveryResourceId"];
        private string mediaServicesAccount = ConfigurationManager.AppSettings["ams:MediaServicesAccount"];
        private string mediaServicesKey = ConfigurationManager.AppSettings["ams:MediaServicesKey"];
        // added by shigeyf@microsoft.com to the original template


        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = Authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                       AuthorizationCodeReceived = (context) => 
                       {
                           var code = context.Code;
                           ClientCredential credential = new ClientCredential(clientId, appKey);
                           string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                           AuthenticationContext authContext = new AuthenticationContext(Authority, new ADALTokenCache(signedInUserID));
                           AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                               code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId);

                           // added by shigeyf@microsoft.com to the original template
                           // Getting KeyDelivery Access Token
                           AuthenticationResult kdAPiresult = authContext.AcquireTokenByAuthorizationCode(
                               code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, kdResourceId);
                           string kdAccessToken = kdAPiresult.AccessToken;
                           System.IdentityModel.Tokens.JwtSecurityToken kdAccessJwtToken = new System.IdentityModel.Tokens.JwtSecurityToken(kdAccessToken);

                           // Initializing MediaServicesCredentials in order to obtain access token to be used to connect 
                           var amsCredentials = new MediaServicesCredentials(mediaServicesAccount, mediaServicesKey);
                           // Forces to get access token
                           amsCredentials.RefreshToken();
                           //Adding media services access token as claim so it can be accessible within controller
                           context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(VideoPortalDemo.Configurations.ClaimsAmsAcessToken, amsCredentials.AccessToken));

                           //context.AuthenticationTicket.Identity.AddClaim(
                           //    new System.Security.Claims.Claim("KdAccessJwtSecurityTokenClaim", kdAccessJwtToken.RawData));
                           // added by shigeyf@microsoft.com to the original template

                           return Task.FromResult(0);
                       }
                    }
                });
        }
    }
}

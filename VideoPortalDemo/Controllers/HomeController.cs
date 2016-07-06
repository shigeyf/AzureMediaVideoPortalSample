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
using System.Linq;
using System.Web;
using System.Web.Mvc;

// added by shigeyf@microsoft.com to the original template
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using VideoPortalDemo.Models;
using WebGrease.Css.Extensions;
using AuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;
// added by shigeyf@microsoft.com to the original template

namespace VideoPortalDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // added by shigeyf@microsoft.com to the original template
        private ApplicationDbContext db = new ApplicationDbContext();
        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private string graphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];
        private string kdResourceId = ConfigurationManager.AppSettings["ida:KeyDeliveryResourceId"];
        private string mediaServicesAccount = ConfigurationManager.AppSettings["ams:MediaServicesAccount"];
        private string mediaServicesKey = ConfigurationManager.AppSettings["ams:MediaServicesKey"];
        // added by shigeyf@microsoft.com to the original template

        public ActionResult Index()
        {
            // added by shigeyf@microsoft.com to the original template
            //Initializing a model
            VideoPortalDemoModel model = new VideoPortalDemoModel();
            model.VideoList = new List<Tuple<IAsset, ILocator, Uri, string>>();
            model.StreamingEndPoint = null;

            try
            {
                string accessToken = GetTokenForKeyDelivery();
                if (accessToken == null)
                {
                    ViewBag.ErrorMessage = "AuthorizationRequired";
                    return View(model);
                }

                model.KdAccessJwtToken = new JwtSecurityToken(accessToken);
                //model.KdAccessJwtToken = new System.IdentityModel.Tokens.JwtSecurityToken(ClaimsPrincipal.Current.FindFirst("ADJwtSecurityTokenClaim").Value);

                // Media Services
                CloudMediaContext cloudMediaContext = GetCloudMediaContext();
                IStreamingEndpoint streamingEndPoint = cloudMediaContext.StreamingEndpoints.FirstOrDefault();
                model.StreamingEndPoint = streamingEndPoint;
                // Locate all files with smooth streaming Manifest
                ListExtensions.ForEach(cloudMediaContext.Files.Where(c => c.Name.EndsWith(".ism")), file =>
                {
                    // skip all assets where DynamicEncryption can't be applied
                    //if (file.Asset.Options != AssetCreationOptions.None)
                    //{
                    //    return;
                    //}
                    if (file.Asset.AssetType != AssetType.MultiBitrateMP4 && file.Asset.AssetType != AssetType.SmoothStreaming)
                    {
                        return;
                    }
                    ILocator originLocator = null;
                    string drm = "none";
                    originLocator = file.Asset.Locators.FirstOrDefault();
                    AssetEncryptionState assetEncryptionState = file.Asset.GetEncryptionState(AssetDeliveryProtocol.SmoothStreaming | AssetDeliveryProtocol.HLS | AssetDeliveryProtocol.Dash);
                    if (assetEncryptionState == AssetEncryptionState.DynamicCommonEncryption)
                    {
                        drm = "drm";
                    }
                    else if (assetEncryptionState == AssetEncryptionState.DynamicEnvelopeEncryption)
                    {
                        drm = "aes";
                    }
                    // If no policy has been found we are storing nulls in a model
                    if (originLocator != null)
                    {
                        Tuple<IAsset, ILocator, Uri, string> item = new Tuple<IAsset, ILocator, Uri, string>(
                            file.Asset,
                            originLocator,
                            originLocator != null ? new Uri(originLocator.Path.Replace("http://", "https://") + file.Name) : null,
                            drm
                            );

                        model.VideoList.Add(item);
                    }
                });

                return View(model);
            }
            catch (AdalException ex)
            {
                // Return to error page.
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                // Return to error page.
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
            // added by shigeyf@microsoft.com to the original template
            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        // added by shigeyf@microsoft.com to the original template
        private string GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            AuthenticationContext authenticationContext = null;
            AuthenticationResult authenticationResult = null;
            try
            {
                // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
                ClientCredential clientcred = new ClientCredential(clientId, appKey);
                // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
                authenticationContext = new AuthenticationContext(aadInstance + tenantID, new ADALTokenCache(signedInUserID));
                authenticationResult = authenticationContext.AcquireTokenSilent(graphResourceId, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
                if (authenticationResult == null) throw new Exception();
            }
            catch
            {
                authenticationContext.TokenCache.Clear();
                ViewBag.ErrorMessage = "AuthorizationRequired";
                if (Request.QueryString["reauth"] == "True")
                {
                    //
                    // Send an OpenID Connect sign-in request to get a new set of tokens.
                    // If the user still has a valid session with Azure AD, they will not be prompted for their credentials.
                    // The OpenID Connect middleware will return to this controller after the sign-in response has been handled.
                    //
                    HttpContext.GetOwinContext().Authentication.Challenge(OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
                return null;
            }

            return authenticationResult.AccessToken;
        }

        // added by shigeyf@microsoft.com to the original template
        private string GetTokenForKeyDelivery()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            AuthenticationContext authenticationContext = null;
            AuthenticationResult authenticationResult = null;
            try
            {
                // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
                ClientCredential clientcred = new ClientCredential(clientId, appKey);
                // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
                authenticationContext = new AuthenticationContext(aadInstance + tenantID, new ADALTokenCache(signedInUserID));
                authenticationResult = authenticationContext.AcquireTokenSilent(kdResourceId, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
                if (authenticationResult == null) throw new Exception();
            }
            catch
            {
                if (authenticationContext != null) authenticationContext.TokenCache.Clear();
                ViewBag.ErrorMessage = "AuthorizationRequired";
                if (Request.QueryString["reauth"] == "True")
                {
                    //
                    // Send an OpenID Connect sign-in request to get a new set of tokens.
                    // If the user still has a valid session with Azure AD, they will not be prompted for their credentials.
                    // The OpenID Connect middleware will return to this controller after the sign-in response has been handled.
                    //
                    HttpContext.GetOwinContext().Authentication.Challenge(OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
                return null;
            }

            return authenticationResult.AccessToken;
        }
        // added by shigeyf@microsoft.com to the original template

        // added by shigeyf@microsoft.com to the original template
        private CloudMediaContext GetCloudMediaContext()
        {
            var amsAccessToken = ClaimsPrincipal.Current.FindFirst(Configurations.ClaimsAmsAcessToken).Value;
            var amsCredentials = new MediaServicesCredentials(mediaServicesAccount, mediaServicesKey);
            amsCredentials.AccessToken = amsAccessToken;
            CloudMediaContext cntx = new CloudMediaContext(amsCredentials);
            return cntx;
        }
        // added by shigeyf@microsoft.com to the original template
    }
}
# AzureMediaVideoPortalSample

This is a sample demo application of Azure Media Player web application integrated with Azure Active Directory.
This sample shows how to build an MVC web application that uses Azure Media Services .NET SDK to display video gallery to user. Based on AD user group membership they will be able to see content.
 
## How To Run This Sample
To run this sample you will need:
- Visual Studio 2013 or 2015
- An Internet connection
- An Azure subscription

Every Azure subscription has an associated Azure Active Directory tenant.  If you don't already have an Azure subscription, you can get a free subscription by signing up at [http://wwww.windowsazure.com](http://www.windowsazure.com).  All of the Azure AD features used by this sample are available free of charge.
Azure Media Services tenant can be provisioned through Azure Portal and regular pricing is applied.


### Step 1:  Deploy this web application through "Azure Deploy"

1. Click the button below.

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

2. Select Azure subscription to deploy this sample web application, and select the destination of deployment (Resource Group, Site Name, Site Location, Web App SKU). Then click "Next" button.

3. Then click "Deploy" button to actually deploy the sample web application to you Azure Subscription.


### Step 2:  Provision Azure Media Service account and encode few video files to be used in example

1. Use the Azure Management Portal to create an Azure Media Services account. For more information, see [How to Create a Media Services Account](http://azure.microsoft.com/en-us/documentation/articles/media-services-create-account/).
2. Click on Media Services in the left navigation bar.
3. Click the ceated media services account.
4. In the bottom bar, click "Manage keys".
5. Find the Media Service Account Name value and Media Service Access Key, and then copy it aside, you will need these later when configuring Web.config of your application at Step 5.
6. Use the Portal to upload an asset. See the steps described in the [How to: Upload content](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section. 
7.  Use the Portal to encode the asset. See the steps described in the [How to: Encode content](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section and choose the **Playback on PC/Mac (via Flash/Silverlight)** preset from the Azure Media Encoder dialog box.
8.  Use the Portal to publish the asset. See the steps described in the [How to: Publish content](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section.

Once your asset is published, you can use the steps described in [How to: Play content from the portal](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section to stream your asset. You can also use one of the following players to test your stream: [http://amsplayer.azurewebsites.net/](http://amsplayer.azurewebsites.net/) or [http://smf.cloudapp.net/healthmonitor](http://smf.cloudapp.net/healthmonitor)


### Step 3:  Create a few user accounts and groups in your Azure Active Directory tenant

1. If you already have a user account in your Azure Active Directory tenant, you can skip to the next step.  This sample will not work with a Microsoft account, so if you signed in to the Azure portal with a Microsoft account and have never created a user account in your directory before, you need to do that now.  If you create an account and want to use it to sign-in to the Azure portal, don't forget to add the user account as a co-administrator of your Azure subscription.
2. Create few more accounts to be able to see that different users have different access right to video gallery
3. Create Admin Group and save aside value ObjectID of this group
4. Create one or more additional groups
5. Assign one users to be in admin group. This user will be able to configure authorization policies within MediaLibraryWebApp 
6. Assign other users between other groups


### Step 4:  Register the sample with your Azure Active Directory tenant

#### Register the KeyDelivery resource app

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Active Directory in the left navigation bar.
3. Click the directory tenant where you wish to register the sample application.
4. Click the "APPLICATIONS" tab.
5. In the bottom bar, click Add.
6. Click "Add an application my organization is developing".
7. Enter a friendly name for the application, for example "KeyDelivery", select "Web Application and/or Web API", and click next.
8. For the sign-on URL, enter the base URL for the resource app. For example, `https://<your_tenant_name>/KeyDelivery`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.
9. For the App ID URI, enter `https://<your_tenant_name>/KeyDelivery`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.  Click OK to complete the registration.
10. While still in the Azure portal, click the "CONFIGURE" tab of your application.
11. Find the App ID URI value and copy it aside, you will need this later when configuring Web.config of your application at Step 5.
12. In section 'Permission to other applications', select Windows Azure Active Directory Delegated Permissions and check "Sign in and read user profile" checkbox.
13. Save the confituation.
14. Download 'MediaLibraryWebApp' application manifest from Azure portal
15. Find property `groupMembershipClaims` and change it value to `All`. `"groupMembershipClaims": "All",` 
16. Upload application manifest back to Azure portal
17. Save the confituation.



#### Register the AzureMediaVideoPortalSample web app

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Active Directory in the left navigation bar.
3. Click the directory tenant where you wish to register the sample application.
4. Click the "APPLICATIONS" tab.
5. In the bottom bar, click Add.
6. Click "Add an application my organization is developing".
7. Enter a friendly name for the application, for example "Webapp-azuremediavideoportalsample.azurewebsites.net", select "Web Application and/or Web API", and click next.
8. For the sign-on URL, enter the base URL for the sample, which is by default `https://<your_site_name>.azurewebsites.net/`.  NOTE:  It is important, due to the way Azure AD matches URLs, to ensure there is a trailing slash on the end of this URL.  If you don't include the trailing slash, you will receive an error when the application attempts to redeem an authorization code.
9. For the App ID URI, enter `https://<your_tenant_name>/azuremediavideoportalsample`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.  Click OK to complete the registration.
10. While still in the Azure portal, click the "CONFIGURE" tab of your application.
11. Find the Client ID value and copy it aside, you will need this later when configuring Web.config of your application at Step 5.
12. Create a new key for the application.  Save the configuration so you can view the key value.  Save this aside for when you configure Web.config of your application at Step 5.
13. In section 'Permission to other applications', select Windows Azure Active Directory Application Permissions and check "Read directory data" checkbox.
14. In section 'Permission to other applications', select Windows Azure Active Directory Delegated Permissions and check "Read directory data" and "Sign in and read user profile" checkboxes.
15. In section 'Permission to other applications', click "Add application" and select "All apps" in "SHOW". Add "KeyDelivery" to the selection.
16. Once "KeyDelivery" resource app is added to section 'Permission to other applications', select KeyDelivery Delegated Permissions and check "Access KeyDelivery" checkbox.
17. Save the confituation.



### Step 5:  Configure the sample to use your Azure AD tenant and Azure Media Service

#### Configure Web.config through Visual Studio Online

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Web Apps in the left navigation bar.
3. Click the deployed web app.
4. Click the "CONFIGURE" tab.
5. Enable "EDIT IN VISUAL STUIDO" to set `ON'.
6. In the bottom bar, click Save.
7. Access https://<your_site_name>.scm.azurewebsites.net/dev/wwwroot/ to edit Web.config.
8. Configure with the Azure Active Directoy app values and domain information.
    <add key="ida:ClientId" value="09601466-af6d-4866-b034-94f54d03cdd1" />
    <add key="ida:ClientSecret" value="rfr0YsoE/htH5xay4gBsG/bP0cQaHGamUV6mkT/Y2o4=" />
    <add key="ida:Domain" value="contosodemoaad.onmicrosoft.com" />
    <add key="ida:TenantId" value="70b7d40b-d476-4c82-b3b5-7f5786b3ed3b" />
    <add key="ida:PostLogoutRedirectUri" value="http://azuremediavideoportalsample.azurewebsites.net/" />
    <add key="ida:KeyDeliveryResourceId" value="https://contosodemoaad.onmicrosoft.com/KeyDelivery" />
9. Configure with the media services account and key to replace the default values
    <add key="ams:MediaServicesAccount" value="<your_media_services_account_name>" />
    <add key="ams:MediaServicesKey" value="<your_media_services_account_key>" />

3. Find the app key `ida:Domain` and replace the value with your AAD tenant name.
3. Find the app key `ida:TenantId` and replace the value with your AAD tenant identifier.
4. Find the app key `ida:ClientId` and replace the value with the Client ID for the AzureMediaVideoPortalSample from the Azure portal.
6. Find the app key `ida:ClientSecret` and replace the value with the key for the AzureMediaVideoPortalSample from the Azure portal.
7. If you changed the base URL of the AzureMediaVideoPortalSample sample, find the app key `ida:PostLogoutRedirectUri` and replace the value with the new base URL of the sample.
5. Find the app key `ida:KeyDeliveryResourceId' and replace the value with the App ID URI for the Keydelivery app from the Azure portal.
10.  Find the app key `ida:MediaServicesAccount` and replace the value with you Azure Media Services account name 
11. Find the app key `ida:MediaServicesKey` and replace the value with you Azure Media Services account key. You can find value in Azure portal.      


### Step 6:  Run the sample

1. Access https://<your_site_name>.azurewebsites.net/



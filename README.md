# AzureMediaVideoPortalSample
==================================

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

Click the button below, and 

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)


### Step 2:  Provision  Azure Media Service account and encode few video files to be used in example

1. Use the Azure Management Portal to create an Azure Media Services account. For more information, see [How to Create a Media Services Account](http://azure.microsoft.com/en-us/documentation/articles/media-services-create-account/).

2. Use the Portal to upload an asset. See the steps described in the [How to: Upload content](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section. 

3.  Use the Portal to encode the asset. See the steps described in the [How to: Encode content](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section and choose the **Playback on PC/Mac (via Flash/Silverlight)** preset from the Azure Media Encoder dialog box.

4.  Use the Portal to publish the asset. See the steps described in the [How to: Publish content](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section.

Once your asset is published, you can use the steps described in [How to: Play content from the portal](http://azure.microsoft.com/en-us/documentation/articles/media-services-manage-content/) section to stream your asset. You can also use one of the following players to test your stream: [http://amsplayer.azurewebsites.net/](http://amsplayer.azurewebsites.net/) or [http://smf.cloudapp.net/healthmonitor](http://smf.cloudapp.net/healthmonitor)


### Step 3:  Create a few user accounts and groups in your Azure Active Directory tenant

1. If you already have a user account in your Azure Active Directory tenant, you can skip to the next step.  This sample will not work with a Microsoft account, so if you signed in to the Azure portal with a Microsoft account and have never created a user account in your directory before, you need to do that now.  If you create an account and want to use it to sign-in to the Azure portal, don't forget to add the user account as a co-administrator of your Azure subscription.
2. Create few more accounts to be able to see that different users have different access right to video gallery
3. Create Admin Group and save aside value ObjectID of this group
4. Create one or more additional groups
5. Assign one users to be in admin group. This user will be able to configure authorization policies within MediaLibraryWebApp 
6. Assign other users between other groups


### Step 4:  Register the sample with your Azure Active Directory tenant



### Step 5:  Configure the sample to use your Azure AD tenant and Azure Media Service

#### Configure Web.config through Visual Studio Online



### Step 6:  Run the sample



// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelLocator.cs" company="Nokia Developer Wiki">
//   Copyright (c) 2013 Nokia Developer Wiki. All rights reserved.
// </copyright>
// <summary>
//   The view model locator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NokiaDev.AboutPageSample.ViewModels
{
    using Cimbalino.Phone.Toolkit.Services;

    using GalaSoft.MvvmLight.Ioc;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The view model locator.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
            }

            if (!SimpleIoc.Default.IsRegistered<IMarketplaceReviewService>())
            {
                SimpleIoc.Default.Register<IMarketplaceReviewService, MarketplaceReviewService>();
            }

            if (!SimpleIoc.Default.IsRegistered<IShareLinkService>())
            {
                SimpleIoc.Default.Register<IShareLinkService, ShareLinkService>();
            }
            
            if (!SimpleIoc.Default.IsRegistered<IApplicationManifestService>())
            {
                SimpleIoc.Default.Register<IApplicationManifestService, ApplicationManifestService>();
            }

            if (!SimpleIoc.Default.IsRegistered<IEmailComposeService>())
            {
                SimpleIoc.Default.Register<IEmailComposeService, EmailComposeService>();
            }


            SimpleIoc.Default.Register<AboutViewModel>();
        }

        /// <summary>
        /// Gets the home view model.
        /// </summary>
        

        /// <summary>
        /// Gets the about view model.
        /// </summary>
        public AboutViewModel AboutViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}

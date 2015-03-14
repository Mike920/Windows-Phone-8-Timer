// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutViewModel.cs" company="Nokia Developer Wiki">
//   Copyright (c) 2013 Nokia Developer Wiki. All rights reserved.
// </copyright>
// <summary>
//   Defines the AboutViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NokiaDev.AboutPageSample.ViewModels
{
    using System;
    using System.Windows.Input;

    using Cimbalino.Phone.Toolkit.Helpers;
    using Cimbalino.Phone.Toolkit.Services;

    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// The about view model.
    /// </summary>
    public class AboutViewModel
    {
        /// <summary>
        /// The email compose service.
        /// </summary>
        private readonly IEmailComposeService _emailComposeService;
        
        /// <summary>
        /// The marketplace review service.
        /// </summary>
        private readonly IMarketplaceReviewService _marketplaceReviewService;

        /// <summary>
        /// The share link service.
        /// </summary>
        private readonly IShareLinkService _shareLinkService;

        /// <summary>
        /// The public application url.
        /// </summary>
        private readonly string _appUrl;

        /// <summary>
        /// The application manifest.
        /// </summary>
        private readonly ApplicationManifest _applicationManifest;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        /// <param name="emailComposeService">
        /// The email Compose Service.
        /// </param>
        /// <param name="applicationManifestService">
        /// The application Manifest Service.
        /// </param>
        /// <param name="marketplaceReviewService">
        /// The marketplace review service
        /// </param>
        /// <param name="shareLinkService">
        /// The share Link Service.
        /// </param>
        public AboutViewModel(
            IEmailComposeService emailComposeService,
            IApplicationManifestService applicationManifestService,
            IMarketplaceReviewService marketplaceReviewService,
            IShareLinkService shareLinkService)
        {
            _emailComposeService = emailComposeService;
            _marketplaceReviewService = marketplaceReviewService;
            _shareLinkService = shareLinkService;
            RateCommand = new RelayCommand(this.Rate);
            SendFeedbackCommand = new RelayCommand(this.SendFeedback);
            ShareToMailCommand = new RelayCommand(this.ShareToMail);
            ShareSocialNetworkCommand = new RelayCommand(this.ShareSocialNetwork);
            _applicationManifest = applicationManifestService.GetApplicationManifest();
            _appUrl = string.Concat("http://windowsphone.com/s?appid=", _applicationManifest.App.ProductId);
        }

        /// <summary>
        /// Gets the author.
        /// </summary>
        public string Author
        {
            get
            {
                return _applicationManifest.App.Author;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version
        {
            get
            {
                return _applicationManifest.App.Version;
            }
        }

        /// <summary>
        /// Gets the rate command.
        /// </summary>
        public ICommand RateCommand { get; private set; }

        /// <summary>
        /// Gets the send feedback command.
        /// </summary>
        public ICommand SendFeedbackCommand { get; private set; }

        /// <summary>
        /// Gets the share social network command.
        /// </summary>
        public ICommand ShareSocialNetworkCommand { get; private set; }

        /// <summary>
        /// Gets the share to e-mail command.
        /// </summary>
        public ICommand ShareToMailCommand { get; private set; }

        /// <summary>
        /// The rate.
        /// </summary>
        private void Rate()
        {
            _marketplaceReviewService.Show();
        }

        /// <summary>
        /// The send feedback.
        /// </summary>
        private void SendFeedback()
        {
            const string To = "MRappDevelopment@gmail.com";
            const string Subject = "Stopwatch + Timer Free Feedback";
            var body = string.Format(
                "Application {0}\n Version: {1}",
                _applicationManifest.App.Title,
                _applicationManifest.App.Version);
            _emailComposeService.Show(To, Subject, body);
        }

        /// <summary>
        /// The share social network.
        /// </summary>
        private void ShareSocialNetwork()
        {
            const string Message = "This application is amazing, should try it! See in";
            _shareLinkService.Show(_applicationManifest.App.Title, Message, new Uri(_appUrl, UriKind.Absolute));
        }

        /// <summary>
        /// The share to mail.
        /// </summary>
        private void ShareToMail()
        {
            const string Subject = "Stopwatch + Timer Free";
            var body = string.Concat("This application is amazing, you should try it! See in", _appUrl);
            _emailComposeService.Show(Subject, body);
        }
    }
}

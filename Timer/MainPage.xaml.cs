using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Timer
{
    
    public partial class MainPage : PhoneApplicationPage
    {

        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private StopWatch stopWatch = new StopWatch();
        private Timer timer = new Timer();
        private bool recordRemoveRequest = false;
        private bool timerStartRequest = false;
        private bool timerResetRequest = false;

       
        public SolidColorBrush RecordColor { get; set; }


     
        // Constructor
        public MainPage()
        {
            if (LightThemeUsed())
                RecordColor = new SolidColorBrush(Color.FromArgb(255, 190, 190, 190));
            else
                RecordColor = new SolidColorBrush(Color.FromArgb(255, 41, 41, 41));
            DataContext = this;

            InitializeComponent();

            if (!settings.Contains("pivotPageNumber"))
                settings.Add("pivotPageNumber", 0);

            if (!settings.Contains("startTime"))
                settings.Add("startTime",null);
            if (!settings.Contains("stopTime"))
                settings.Add("stopTime", null);
            if (!settings.Contains("lapStartTime"))
                settings.Add("lapStartTime", null);
            if (!settings.Contains("SWrecords"))
                settings.Add("SWrecords", null);
            if (!settings.Contains("isSWrunning"))
                settings.Add("isSWrunning", false);
            if (!settings.Contains("screensText"))
                settings.Add("screensText", null);

            //Timer
            if (!settings.Contains("exitTime"))
                settings.Add("exitTime", null);
            if (!settings.Contains("timerRecords"))
                settings.Add("timerRecords", null);
            


            
            txtStopWatch.DataContext = stopWatch;
            txtStopWatchRight.DataContext = stopWatch;
            txtSecondStopWatch.DataContext = stopWatch;
            longListSelector.DataContext = stopWatch;
            longListSelector.ItemsSource = stopWatch.Records;
            timerLongListSelector.DataContext = timer;
            timerLongListSelector.ItemsSource = timer.Records;

            txtTimer.Value = new TimeSpan();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

           // sPanel.Children.Add(timer.Sound);
        }
        bool LightThemeUsed()
        {
            return Visibility.Visible == (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"];
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ContentPanel.SelectedIndex = (int)settings["pivotPageNumber"];

            if (settings["screensText"] != null)
            {
                stopWatch.OutputText = ((string[]) settings["screensText"])[0];
                stopWatch.SecondStopWatchText = ((string[])settings["screensText"])[1];
            }

            if(!stopWatch.IsEnabled)
                if(settings["startTime"]!=null)
                    if ((bool)settings["isSWrunning"])
                        stopWatch.StartStopWatch();
                    else
                        stopWatch.StartStopWatch(true);

            if (!stopWatch.IsEnabled)
            {
                btnStopWatchStart.Content = "START";
                btnStopWatchStart.Background = new SolidColorBrush(Color.FromArgb(255, 18, 174, 18));
                btnReset.Content = "RESET";
            }
            else
            {
                btnStopWatchStart.Content = "PAUSE";
                btnStopWatchStart.Background = new SolidColorBrush(Color.FromArgb(255, 180, 12, 12));
                btnReset.Content = "LAP";
            }

            //TIMER

            timer.RestoreData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            stopWatch.SaveData();
            timer.SaveData();
            settings["screensText"] = new string[] {txtStopWatch.Text+txtStopWatchRight.Text, txtSecondStopWatch.Text};
            settings.Save();

            settings["pivotPageNumber"] = ContentPanel.SelectedIndex;
        }

        private void btnStopWatchStart_Click(object sender, RoutedEventArgs e)
        {
            if (stopWatch.IsEnabled)
            {
                btnStopWatchStart.Content = "START";
                btnStopWatchStart.Background= new SolidColorBrush(Color.FromArgb(255,18,174,18));
                btnReset.Content = "RESET";
            }
            else
            {
                btnStopWatchStart.Content = "PAUSE";
                btnStopWatchStart.Background = new SolidColorBrush(Color.FromArgb(255, 180,12,12));
                btnReset.Content = "LAP";
            }
           stopWatch.StartStopWatch();

           /* await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
            ShellTile.ActiveTiles.First().Update(
            new FlipTileData()
            {
                WideBackContent = "Lock screen text test"
            });*/
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (stopWatch.IsEnabled)
            {
                stopWatch.AddLap();
              //  longListSelector.ScrollTo(this.longListSelector.ItemsSource[0]);
                if(stopWatch.Records.Count>7)
                    longListSelector.ScrollTo(this.longListSelector.ItemsSource[longListSelector.ItemsSource.Count-1]);
            }
            else
                stopWatch.Clear();
                
            
        }

        private void btnTimerAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((TimeSpan) txtTimer.Value != TimeSpan.Zero)
            {
                timer.AddRecord((TimeSpan) txtTimer.Value, txtTimerLabel.Text);
                timerLongListSelector.ScrollTo(timerLongListSelector.ItemsSource[0]);
            }
        }

        private void btnTimerRemove_Click(object sender, RoutedEventArgs e)
        {
            recordRemoveRequest = true;
        }

        private void timerLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recordRemoveRequest)
            {
                timer.Records.Remove((TimerRecord) timerLongListSelector.SelectedItem);
                recordRemoveRequest = false;
            }

            if (timerStartRequest)
            { 
               // ((TimerRecord)timerLongListSelector.SelectedItem).IsEnabled = !(((TimerRecord)timerLongListSelector.SelectedItem).IsEnabled);
                timer.StartPause((TimerRecord)timerLongListSelector.SelectedItem);
                timerStartRequest = false;
            }

            if (timerResetRequest)
            {
                ((TimerRecord) timerLongListSelector.SelectedItem).IsEnabled = false;
                ((TimerRecord) timerLongListSelector.SelectedItem).RemainingTime =
                    ((TimerRecord) timerLongListSelector.SelectedItem).Duration;
                timerResetRequest = false;
            }

            timerLongListSelector.SelectedItem = null;
        }

        private void btnTimerReset_Click(object sender, RoutedEventArgs e)
        {
            timerResetRequest = true;
        }

        private void btnTimerStartPause_Click(object sender, RoutedEventArgs e)
        {
            timerStartRequest = true;
        }

        private void clickAbout(object sender, EventArgs e)
        {
           // AboutPrompt about = new AboutPrompt();
          //  about.Show("Michał Rzepka",null,"MRappDevelopment@gmail.com");

            //FEEDBACK
          /*  EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = "Feedback to App";           //message subject
            emailComposeTask.Body = "Write your feedback here...";  //message body
            emailComposeTask.To = "michalrzk@gmail.com";          //email id
            emailComposeTask.Show();*/

            NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative));
            
        }

        private void clickSettings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));

        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
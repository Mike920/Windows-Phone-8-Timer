using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Devices;
using Microsoft.Xna.Framework;
using Timer.ViewModels;
using Microsoft.Xna.Framework.Audio;
using Color = System.Windows.Media.Color;

namespace Timer
{
    public class TimerRecord : INotifyPropertyChanged
    {
        private bool _isEnabled = false;
        private TimeSpan _duration;
        private TimeSpan _remainingTime;
        [System.Runtime.Serialization.IgnoreDataMember]
        private TimeSpan _endTime;
        private string _title;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged("IsEnabled");
                NotifyPropertyChanged("ButtonColor");
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

       
        public TimeSpan RemainingTime
        {
            get { return _remainingTime; }
            set
            {
                _remainingTime = value;
                NotifyPropertyChanged("RemainingTime");
            }
        }

         [System.Runtime.Serialization.IgnoreDataMemberAttribute]
        public TimeSpan EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                NotifyPropertyChanged("EndTime");
            }
        }
        
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public SolidColorBrush ButtonColor
        {
            get
            {
                if(IsEnabled)
                    return new SolidColorBrush(Color.FromArgb(255, 180, 12, 12));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 18, 174, 18));
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class Timer
    {
        DispatcherTimer timer = new DispatcherTimer();
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private SettingsViewModel appSettings = new SettingsViewModel();
        private int tickDuration = 200;
        private VibrateController vibration = VibrateController.Default;

        private SoundEffectInstance _soundInstance;
       // MediaElement sound = new MediaElement() { Source = new Uri("/Assets/sound.mp3", UriKind.Relative), AutoPlay = false};
        public ObservableCollection<TimerRecord> Records { get; set; }

        public Timer()
        {
            Records = new ObservableCollection<TimerRecord>();
            timer.Interval = new TimeSpan(0, 0, 0, 0, tickDuration);
            timer.Tick += stopWatch_Tick;
            timer.Start();

            //sound.MediaEnded += sound_MediaEnded;
             Stream stream = TitleContainer.OpenStream("Assets/sound.wav");
             SoundEffect sound = SoundEffect.FromStream(stream);
            _soundInstance = sound.CreateInstance();
            _soundInstance.IsLooped = true;
            FrameworkDispatcher.Update();
        }


        public void stopWatch_Tick(object sender, EventArgs e)
        {
            foreach (TimerRecord timerRecord in Records)
            {
                if (timerRecord.IsEnabled)
                {
                    
                    timerRecord.RemainingTime = timerRecord.EndTime - DateTime.Now.TimeOfDay;
                    if (timerRecord.RemainingTime <= TimeSpan.Zero)
                        CountingComplete(timerRecord);
                }
                //timerRecord.RemainingTime=timerRecord.RemainingTime.Subtract(TimeSpan.FromMilliseconds(tickDuration));
            }
        }

        public void StartPause(TimerRecord record)
        {
            record.EndTime = DateTime.Now.TimeOfDay + record.RemainingTime;
            record.IsEnabled = !record.IsEnabled;
        }

        public void CountingComplete(TimerRecord timerRecord)
        {
            timerRecord.IsEnabled = false;
            timerRecord.RemainingTime = timerRecord.Duration;

            if(appSettings.SoundSetting)
                _soundInstance.Play();
            if(appSettings.VibrationsSetting)
                vibration.Start(TimeSpan.FromSeconds(5));

            Popup my_popup_cs = new Popup();
            my_popup_cs.Margin = new Thickness(12,12,12,12);

            
            my_popup_cs.HorizontalAlignment= HorizontalAlignment.Center;
            my_popup_cs.VerticalAlignment = VerticalAlignment.Center;

            Border border = new Border() { Width = 400, Height = 400 };  
            border.CornerRadius = new CornerRadius(10);
            border.HorizontalAlignment= HorizontalAlignment.Center;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(0);
            border.Margin = new Thickness(10, 10, 10, 10);
            border.Background = new SolidColorBrush(Color.FromArgb(240, 41, 41, 41));

            Grid grid = new Grid();                             // stack panel 
            grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(1,GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.HorizontalAlignment= HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            
            

            TextBlock txt_blk1 = new TextBlock();                                         // Textblock 1
            txt_blk1.HorizontalAlignment = HorizontalAlignment.Center;
            txt_blk1.VerticalAlignment = VerticalAlignment.Center;
            txt_blk1.Text =timerRecord.Title+ " time is up!";
            
            txt_blk1.TextAlignment = TextAlignment.Center;
            txt_blk1.TextWrapping = TextWrapping.Wrap;
            txt_blk1.FontSize = 40;
            txt_blk1.Margin = new Thickness(10, 0, 10, 0);
            txt_blk1.Foreground = new SolidColorBrush(Color.FromArgb(255,42,129,234));
            Grid.SetRow(txt_blk1,0);
            


            //Adding control to stack panel
           // skt_pnl_outter.Children.Add(img_disclaimer);
            grid.Children.Add(txt_blk1);


            //Button btnSTOP = new Button();                                         // Button continue
            RoundButton btnSTOP = new RoundButton();
            btnSTOP.HorizontalAlignment = HorizontalAlignment.Center;
            btnSTOP.VerticalAlignment = VerticalAlignment.Center;
            btnSTOP.Content = "STOP";
            btnSTOP.ButtonHeight = 200;
            btnSTOP.ButtonWidth = 200;
            btnSTOP.FontSize = 40;
            btnSTOP.Width = 200;
            btnSTOP.Height = 200;
            btnSTOP.Background = new SolidColorBrush(Color.FromArgb(255, 180, 12, 12));
            btnSTOP.BorderThickness = new Thickness(0);
            btnSTOP.Click += (sender, args) =>
            {
                _soundInstance.Stop();
                vibration.Stop();
                my_popup_cs.IsOpen = false;
            };
            Grid.SetRow(btnSTOP,1);
           // btn_continue.Click += new RoutedEventHandler(btn_continue_Click);

            
           // btn_cancel.Click += new RoutedEventHandler(btn_cancel_Click);

            grid.Children.Add(btnSTOP);

            // Adding stackpanel  to border
            border.Child = grid;

            // Adding border to pup-up
            my_popup_cs.Child = border;

           // my_popup_cs.VerticalOffset = 400;
           // my_popup_cs.HorizontalOffset = 10;


        
            my_popup_cs.VerticalOffset = Application.Current.Host.Content.ActualHeight / 2 - border.Height / 2;
            my_popup_cs.HorizontalOffset = Application.Current.Host.Content.ActualWidth / 2 - border.Width / 2 -20;
            
            my_popup_cs.IsOpen = true;

           // AboutPrompt ap = new AboutPrompt();
           // ap.Show();
        }

        public void AddRecord(TimeSpan time,string title )
        {
            
                Records.Insert(0,
                 new TimerRecord{Duration = time,Title = title, RemainingTime = time});
           /* ScheduledAction reminder = new Reminder("kupa")
            {
                BeginTime = DateTime.Now + time,
                Title = title
            };
            ScheduledActionService.Add(reminder);*/

            
        }

        public void SaveData()
        {
            settings["exitTime"] = DateTime.Now;
            settings["timerRecords"] = Records;
            settings.Save();
        }

        public void RestoreData()
        {
            DateTime exitTime = new DateTime();
            if (settings["exitTime"] != null)
            {
                exitTime = (DateTime) settings["exitTime"];
                settings["exitTime"] = null;
            }

            if (settings["timerRecords"] != null)
            {
               // Records.Clear();
                if(Records.Count==0)
                    foreach (var record in (ObservableCollection<TimerRecord>)settings["timerRecords"])
                    {
                        
                        if (record.IsEnabled)
                        {
                            record.RemainingTime = record.RemainingTime - (DateTime.Now - exitTime);
                            record.EndTime = DateTime.Now.TimeOfDay + record.RemainingTime;
                            Records.Add(record);

                            if (record.RemainingTime <= TimeSpan.Zero)
                                CountingComplete(record);
                        }
                        else
                            Records.Add(record);
                    }
                settings["timerRecords"] = null;
            }
        }
    }
}

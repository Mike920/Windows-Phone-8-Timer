using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows.Threading;

namespace Timer
{
    /// <summary>
    /// Single StopWatch record
    /// </summary>
    public class SWrecord : INotifyPropertyChanged
    {
        private int _id;
        private string _elapsedTime;
        private string _recordedTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                _elapsedTime = value;
                NotifyPropertyChanged("ElapsedTime");
            }
        }

        public string RecordedTime
        {
            get { return _recordedTime; }
            set
            {
                _recordedTime = value;
                NotifyPropertyChanged("RecordedTime");
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
    
    public class StopWatch : INotifyPropertyChanged
    {
        private TimeSpan startTime;
        private TimeSpan lapStartTime;
        DispatcherTimer stopWatch = new DispatcherTimer();
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private string _outputText="00:00.00";
        private string _secondStopWatchText = "00:00.00";
        private int idCounter = 1; 
        public ObservableCollection<SWrecord> Records { get; private set; }
        
        public string OutputText
        {
            get { return _outputText; }
            set
            {
                _outputText = value;
                NotifyPropertyChanged("OutputText");
                NotifyPropertyChanged("OutputTextLeftPart");
                NotifyPropertyChanged("OutputTextRightPart");
            }
        }

        public string OutputTextLeftPart
        {
            get { return _outputText.Substring(0,6); }
        }
        public string OutputTextRightPart
        {
            get { return _outputText.Substring(6, 2); }
        }

        public string SecondStopWatchText
        {
            get { return _secondStopWatchText; }
            set
            {
                _secondStopWatchText = value;
                NotifyPropertyChanged("SecondStopWatchText");
            }
        }

        public bool IsEnabled
        {
            get { return stopWatch.IsEnabled; }
        }

        public StopWatch()
        {
            Records = new ObservableCollection<SWrecord>();
        }
        public void StartStopWatch(bool onlyRestore=false)
        {
            if (!stopWatch.IsEnabled)
            {
                try
                {

                    //Restore records
                    if (settings["SWrecords"] != null)
                    {

                        foreach (SWrecord record in (ObservableCollection<SWrecord>) settings["SWrecords"])
                        {
                            Records.Add(record);
                        }

                        idCounter = Records.Count + 1;
                        settings["SWrecords"] = null;
                    }
                }
                catch (Exception e){}

                if (!onlyRestore)
                {
                    if (settings["stopTime"] != null)
                    {
                        //Oblicz przesuniecie czasowe po zatrzymaniu stopera
                        settings["startTime"] = (TimeSpan) settings["startTime"] +
                                                (DateTime.Now - (DateTime) settings["stopTime"]);
                        if (settings["lapStartTime"] != null)
                            settings["lapStartTime"] = (TimeSpan) settings["lapStartTime"] +
                                                       (DateTime.Now - (DateTime) settings["stopTime"]);
                        settings["stopTime"] = null;
                    }
                    else if (settings["startTime"] != null)
                        startTime = (TimeSpan) settings["startTime"];
                    else
                        settings["startTime"] = DateTime.Now.TimeOfDay;
                }
                

                stopWatch.Interval = new TimeSpan(0, 0, 0, 0, 10);
                stopWatch.Tick += stopWatch_Tick;
                startTime = (TimeSpan)settings["startTime"];
                if (settings["lapStartTime"]!=null)
                    lapStartTime = (TimeSpan) settings["lapStartTime"];
                if(!onlyRestore)
                    stopWatch.Start();
            }
            else
            {
                stopWatch.Stop();
                settings["stopTime"] = DateTime.Now;
            }
        }
       public void stopWatch_Tick(object sender, EventArgs e)
        {

            TimeSpan t = DateTime.Now.TimeOfDay - startTime;

            OutputText = t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2") + "." +
                (t.Milliseconds < 100 ? t.Milliseconds.ToString("D2") : (t.Milliseconds / 10).ToString("D2"));

           if (Records.Count != 0)
           {
               t = DateTime.Now.TimeOfDay - lapStartTime;

               SecondStopWatchText = t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2") + "." +
                   (t.Milliseconds < 100 ? t.Milliseconds.ToString("D2") : (t.Milliseconds / 10).ToString("D2"));
           }

        }

     

        public void AddLap()
        {

            if (Records.Count == 0)
                lapStartTime = startTime;

            TimeSpan elapsed = DateTime.Now.TimeOfDay - lapStartTime;
            TimeSpan recorded = DateTime.Now.TimeOfDay - startTime;
            lapStartTime = DateTime.Now.TimeOfDay;
            settings["lapStartTime"] = lapStartTime;

            //Records.Insert(0,
            Records.Add(
            new SWrecord()
            {
                Id = idCounter,
                ElapsedTime = elapsed.Minutes.ToString("D2") + ":" + elapsed.Seconds.ToString("D2") + "." +
                   (elapsed.Milliseconds < 100 ? elapsed.Milliseconds.ToString("D2") : (elapsed.Milliseconds / 10).ToString("D2")),
                RecordedTime = OutputText = recorded.Minutes.ToString("D2") + ":" + recorded.Seconds.ToString("D2") + "." +
                   (recorded.Milliseconds < 100 ? recorded.Milliseconds.ToString("D2") : (recorded.Milliseconds / 10).ToString("D2"))

            });

            idCounter++;
        }

        public void SaveData()
        {
            settings["SWrecords"] = Records;
            settings["isSWrunning"] = IsEnabled;
        }

        public void Clear()
        {
            OutputText = "00:00.00";
            SecondStopWatchText = "00:00.00";
            settings["startTime"] = null;
            settings["stopTime"] = null;
            settings["lapStartTime"] = null;
            settings["SWrecords"] = null;
            Records.Clear();
            idCounter = 1;

        }

       public event PropertyChangedEventHandler PropertyChanged;

       // NotifyPropertyChanged will raise the PropertyChanged event, 
       // passing the source property that is being updated.
       public void NotifyPropertyChanged(string propertyName)
       {
           if (PropertyChanged != null)
           {
               PropertyChanged(this,
                   new PropertyChangedEventArgs(propertyName));
           }
       }
    }
    

 
}

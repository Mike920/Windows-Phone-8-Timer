using System;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Phone.Shell;

namespace Timer.ViewModels
{
    public class SettingsViewModel 
    {
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings; 

        // The key names of our settings
        const string SoundSettingKeyName = "SoundSetting";
        const string VibrationsSettingKeyName = "VibrationsSetting";
        private const string ScreenAwakeKeyName = "ScreenAwakeSetting";
        private const string UnderLockscreenKeyName = "UnderLockscreenSetting";


        // The default value of our settings
        const bool SoundSettingDefault = true;
        const bool VibrationsSettingDefault = true;
        private const bool ScreenAwakeDefault = false;
        private const bool UnderLockscreenDefault = false;


        

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
           if(valueChanged)
               Save();
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }


        /// <summary>
        /// Property to get and set a Sound Setting Key.
        /// </summary>
        public bool SoundSetting
        {
            get { return GetValueOrDefault<bool>(SoundSettingKeyName, SoundSettingDefault); }
            set { AddOrUpdateValue(SoundSettingKeyName, value); }
        }

        public bool VibrationsSetting
        {
            get { return GetValueOrDefault<bool>(VibrationsSettingKeyName, VibrationsSettingDefault); }
            set { AddOrUpdateValue(VibrationsSettingKeyName, value); }
        }

        public bool ScreenAwakeSetting
        {
            get { return GetValueOrDefault<bool>(ScreenAwakeKeyName, ScreenAwakeDefault); }
            set
            {
                AddOrUpdateValue(ScreenAwakeKeyName, value); 
                if(value)
                    PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
                else
                    PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            }
        }

        public bool UnderLockscreenSetting
        {
            get { return GetValueOrDefault<bool>(UnderLockscreenKeyName, UnderLockscreenDefault); }
            set
            {
                try
                {
                    
                    if (value)
                        PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
                    else
                        PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Enabled;
                    AddOrUpdateValue(UnderLockscreenKeyName, value);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "You must restart application to change this setting again due to software limitations.");
                }
                
            }
        }
  
    }
}

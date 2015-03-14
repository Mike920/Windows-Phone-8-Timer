using Microsoft.Phone.Controls;
using Timer.ViewModels;

namespace Timer
{
    public partial class Settings : PhoneApplicationPage
    {
        SettingsViewModel settingsViewModel = new SettingsViewModel();

        public Settings()
        {
            this.DataContext = settingsViewModel;
            InitializeComponent();
            
        }
    }
}
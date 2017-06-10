namespace IPdevices
{
    public class SplashScreenViewModel : ViewModelBase
    {
        public SplashScreenViewModel()
        {
            
        }

        private string splashScreenText = "Initializing...";
        public string SplashScreenText
        {
            get { return splashScreenText; }
            set
            {
                splashScreenText = value;
                OnPropertyChanged("SplashScreenText");
            }
        }
    }
}
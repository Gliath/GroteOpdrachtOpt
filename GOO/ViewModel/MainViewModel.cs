using GOO.Utilities;

namespace GOO.ViewModel
{
    public class MainViewModel : INPC
    {
        public MainViewModel()
        {
            ProgressMaximum = 1;
            ProgressValue = 0;
        }

        private double progressMaximum;
        public double ProgressMaximum
        {
            get { return progressMaximum; }
            set
            {
                if (value < 1)
                    value = 1;

                progressMaximum = value;
                OnPropertyChanged("ProgressMaximum");
            }
        }

        private double progressValue;
        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (value < 0)
                    value = 0;

                progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }
    }
}
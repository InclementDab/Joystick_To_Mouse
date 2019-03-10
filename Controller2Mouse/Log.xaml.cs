using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Controller2Mouse
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class Log : UserControl
    {
        public static ObservableCollection<LogEntry> LogEntries { get; set; }
        
        public Log()
        {
            InitializeComponent();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
        }


        public static void Add(object message) => LogEntries.Add(new LogEntry
            {
                DateTime = DateTime.Now,
                Message = message
            });
        

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight) && e.ExtentHeightChange != 0)
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
        }
    }



    public class LogEntry : PropertyChangedBase
    {
        public DateTime DateTime { get; set; }
        public object Message { get; set; }
    }

    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }));
        }
    }
}

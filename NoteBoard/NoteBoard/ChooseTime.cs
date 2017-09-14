using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBoard
{
    /// <summary>
    /// Класс для храниения возможных значений времени выполнения задач ежедневника
    /// </summary>
    public class ChooseTimes : ObservableCollection<ChooseTime> { }
    public class ChooseTime : INotifyPropertyChanged
    {
        private string time;
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged(nameof(Time));
            }
        }

        public ChooseTime(string time)
        {
            Time = time;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

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
    /// Класс для храниения возможных значений времени будущих покупок (только месяцы)
    /// </summary>
    public class ChooseMonths : ObservableCollection<ChooseMonth> { }
    public class ChooseMonth : INotifyPropertyChanged
    {
        private string month;
        public string Month
        {
            get { return month; }
            set
            {
                month = value;
                NotifyPropertyChanged(nameof(Month));
            }
        }

        public ChooseMonth() { }
        public ChooseMonth(string month)
        {
            Month = month;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

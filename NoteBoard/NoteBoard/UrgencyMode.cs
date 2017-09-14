using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBoard
{
    /// <summary>
    /// Класс для храниения возможных значений важности задач ежедневника
    /// </summary>
    public class UrgencyMode : INotifyPropertyChanged
    {
        private string urgency;
        public string Urgency
        {
            get { return urgency; }
            set
            {
                urgency = value;
                NotifyPropertyChanged(nameof(Urgency));
            }
        }

        public UrgencyMode() { }
        public UrgencyMode(string um)
        {
            Urgency = um;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

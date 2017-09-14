using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NoteBoard
{
    /// <summary>
    /// Класс для хранения данных ежедневника
    /// </summary>
    [Serializable]
    public class DoNowCollection : ObservableCollection<DoNow> { }
    [Serializable]
    public class DoNow : INotifyPropertyChanged, IDataErrorInfo
    {
        private string task;
        public string Task
        {
            get { return task; }
            set
            {
                task = value;
                NotifyPropertyChanged(nameof(Task));
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                NotifyPropertyChanged(nameof(Date));
            }
        }

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

        private bool isCompleted;
        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                isCompleted = value;
                NotifyPropertyChanged(nameof(IsCompleted));
            }
        }

        private bool isValid = true;
        [XmlIgnore]
        public bool IsValid
        {
            get { return isValid; }
            private set
            {
                isValid = value;
                NotifyPropertyChanged(nameof(IsValid));
            }
        }

        public DoNow()
        { }

        public DoNow(string task, DateTime date, string time, string urgency, bool isCompleted)
        {
            Task = task;
            Date = date;
            Time = time;
            Urgency = urgency;
            IsCompleted = isCompleted;
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Task):
                        if (Task.Length < 1)
                        {
                            error = "Введите наименование";
                            IsValid = false;
                        }
                        else
                            IsValid = true;
                        break;
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

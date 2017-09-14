using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NoteBoard
{
    /// <summary>
    /// Класс для хранения будущих покупок
    /// </summary>
    [Serializable]
    public class BuyLaterCollection : ObservableCollection<BuyLater> { }
    [Serializable]
    public class BuyLater : Purchase, INotifyPropertyChanged, IDataErrorInfo
    {
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

        [XmlIgnore]
        public int MonthId
        {
            get;
            private set;
        }

        private string month;
        public string Month
        {
            get { return month; }
            set
            {
                month = value;
                NotifyPropertyChanged(nameof(Month));
                for (int i = 1; i < 13; i++)
                {
                    if (month == DateTimeFormatInfo.CurrentInfo.GetMonthName(i))
                        MonthId = i;
                    else
                        MonthId = 0;
                }
            }
        }

        public BuyLater()
        { }

        public BuyLater(string title, double quantity, double price, string month)
        {
            Title = title;
            Quantity = quantity;
            Price = price;
            Month = month;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    case nameof(Title):
                        if (Title.Length < 1)
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

    }
}

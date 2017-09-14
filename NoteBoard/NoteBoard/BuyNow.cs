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
    /// Класс для хранения текущих покупок
    /// </summary>
    [Serializable]
    public class BuyNowCollection : ObservableCollection<BuyNow> { }
    [Serializable]
    public class BuyNow : Purchase, INotifyPropertyChanged, IDataErrorInfo
    {

        public BuyNow()
        { }

        public BuyNow(string title, double quantity, double price)
        {
            Title = title;
            Quantity = quantity;
            Price = price;
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }
        private bool isValid = true;
        [XmlIgnore]
        public bool IsValid
        {
            get { return isValid; }
            private set { isValid = value; NotifyPropertyChanged(nameof(IsValid)); }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

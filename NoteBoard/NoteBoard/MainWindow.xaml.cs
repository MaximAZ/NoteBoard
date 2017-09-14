using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace NoteBoard
{
    public partial class MainWindow : Window
    {
        public BuyNowCollection BuyNowData { get; set; }
        public BuyLaterCollection BuyLaterData { get; set; }
        public DoNowCollection DoNowData { get; set; }
        public NBData NBDataCollection { get; set; }
        private ChooseTimes chTimes;
        private ChooseMonths months;
        public ObservableCollection<DateTime> MyBlackoutDates;
        public CalendarDateRange BlDates { get; set; }
        public static ObservableCollection<UrgencyMode> UrgencyModes = new ObservableCollection<UrgencyMode>
        {
            new UrgencyMode {Urgency = "Высокая" },
            new UrgencyMode {Urgency = "Средняя" },
            new UrgencyMode {Urgency = "Низкая" }
        };

        public MainWindow()
        {
            InitializeComponent();

            BuyNowData = (BuyNowCollection)Resources[nameof(BuyNowCollection)];
            BuyLaterData = (BuyLaterCollection)Resources[nameof(BuyLaterCollection)];
            DoNowData = (DoNowCollection)Resources[nameof(DoNowCollection)];
            NBDataCollection = new NBData()
            {
                BuyNowCol = BuyNowData,
                BuyLaterCol = BuyLaterData,
                DoNowCol = DoNowData,
            };

            LoadData();

            SetThemes();

            // Установка возможных значений времени покупок
            months = new ChooseMonths();
            for(int i = 1; i < 13; i++)
            {
                months.Add(new ChooseMonth(DateTimeFormatInfo.CurrentInfo.GetMonthName(i)));
            }
            BuyLaterMonth.ItemsSource = months;

            // Установка возможных значений важности задачи
            DoUrgency.ItemsSource = UrgencyModes;
            DoUrgency.SelectedIndex = 1;
            DoNowUrgency.ItemsSource = UrgencyModes;

            // Установка возможных значений времени, выбираемых для новой задачи
            chTimes = new ChooseTimes();
            for (int i = 0; i < 24; i++)
                {
                    for (int j = 0; j < 31; j += 30)
                    {
                        string tm = string.Format("{0:d2}:{1:d2}", i, j);
                        ChooseTime chtm = new ChooseTime(tm);
                        chTimes.Add(chtm);
                    }
                }
            DoTime.ItemsSource = chTimes;
            DoTime.SelectedIndex = 24;
            DoNowTime.ItemsSource = chTimes;

            // Установка доступных дат календаря для показа задач
            MyBlackoutDates = new ObservableCollection<DateTime>();
            CalendarShowingTasks.DisplayDateStart = DateTime.Today;
            CalendarShowingTasks.DisplayDateEnd = DateTime.Today;
            CalendarShowingTasks.SelectedDate = DateTime.Today;
            DoNowData.CollectionChanged += OnCollectionChanged;
            RefreshBlackoutDates();

            // Ограничение выбора даты для новой задачи
            DoDate.DisplayDateStart = DateTime.Today;

            // Обработчики для кнопки "сохранить и закрыть"
            SaveAndExitButton.Click += SaveItems_Click;
            SaveAndExitButton.Click += ExitButton_Click;

            // Загрузка уведомлений
            Notifications.Text = string.Format("{0} - Приложение запущено", DateTime.Now.ToString());
        }
        
        /// <summary>
        /// Установка цветовых схем
        /// </summary>
        private void SetThemes()
        {
            List<string> styles = new List<string>();
            foreach (string theme in Settings1.Default.Color_Themes)
            {
                styles.Add(theme);
            }
            StyleBox.SelectionChanged += ThemeChange;
            StyleBox.ItemsSource = styles;
            StyleBox.SelectedItem = Settings1.Default.Default_Color_Theme;
        }
        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        private void LoadData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBData));
            using (FileStream stream = new FileStream(Settings1.Default.Data_File, FileMode.OpenOrCreate))
            {
                try
                {
                    NBData nbdata = (NBData)serializer.Deserialize(stream);
                    foreach (BuyNow bn in nbdata.BuyNowCol)
                    {
                        BuyNowData.Add(bn);
                    }
                    foreach (BuyLater bl in nbdata.BuyLaterCol)
                    {
                        BuyLaterData.Add(bl);
                    }
                    foreach (DoNow dn in nbdata.DoNowCol)
                    {
                        DoNowData.Add(dn);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(string.Format("Добавьте и сохраните данные.\n{0}", ex.Message.ToString()));
                }
            }
        }
        /// <summary>
        /// Обновление доступных дат в календаре для показа задач
        /// </summary>
        private void RefreshBlackoutDates()
        {
            CalendarShowingTasks.BlackoutDates.Clear();
            MyBlackoutDates.Clear();

            // При отсутствии задач оставим в календаре только сегодняшний день
            if (DoNowData.Count < 1)
            {
                CalendarShowingTasks.DisplayDateStart = DateTime.Today;
                CalendarShowingTasks.DisplayDateEnd = DateTime.Today;
                CalendarShowingTasks.SelectedDate = DateTime.Today;
            }
            else
            {
                DateTime minTaskDate = DoNowData.Min(dn => dn.Date);
                DateTime maxTaskDate = DoNowData.Max(dn => dn.Date);
                CalendarShowingTasks.DisplayDateStart = (minTaskDate < DateTime.Today) ? minTaskDate : DateTime.Today;
                CalendarShowingTasks.DisplayDateEnd = (maxTaskDate > DateTime.Today) ? maxTaskDate : DateTime.Today;

                // Добавим все дни календаря в промежуточную коллекцию недоступных для выбора дней
                for (DateTime i = (DateTime)CalendarShowingTasks.DisplayDateStart; i <= CalendarShowingTasks.DisplayDateEnd; i = i.AddDays(1))
                {
                    MyBlackoutDates.Add(i);
                }

                // Уберем из недоступных те дни, на которые есть задачи, и сегодняшний день
                MyBlackoutDates.Remove(DateTime.Today);
                foreach (DoNow doDate in DoNowData)
                {
                    MyBlackoutDates.Remove(doDate.Date);
                }

                // Оставим выбранную дату, если она еще доступна в календаре. Иначе установим сегодняшний день
                if (MyBlackoutDates.Contains((DateTime)CalendarShowingTasks.SelectedDate))
                {
                    CalendarShowingTasks.SelectedDate = DateTime.Today;
                }
                else if (CalendarShowingTasks.SelectedDate < CalendarShowingTasks.DisplayDateStart && CalendarShowingTasks.SelectedDate > CalendarShowingTasks.DisplayDateEnd)
                {
                    CalendarShowingTasks.SelectedDate = DateTime.Today;
                }

                // Добавим недоступные дни в календарь
                if (MyBlackoutDates.Count > 0)
                {
                    foreach (DateTime dt in MyBlackoutDates)
                    {
                        CalendarShowingTasks.BlackoutDates.Add(new CalendarDateRange(dt));
                    }
                }
            }
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshBlackoutDates();
        }

        /// <summary>
        /// Изменение темы оформления
        /// </summary>
        private void ThemeChange(object sender, SelectionChangedEventArgs e)
        {
            string style = StyleBox.SelectedItem as string;
            var uri = new Uri(style + ".xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            Settings1.Default.Default_Color_Theme = style;
        }

        /// <summary>
        /// Добавление текущей покупки
        /// </summary>
        private void AddBuyNowButton_Click(object sender, RoutedEventArgs e)
        {
            BuyNow newbuynow = new BuyNow("Ввести наименование", 1.0, 1.0);
            BuyNowData.Add(newbuynow);
            ToBuyNow.SelectedItem = newbuynow;
        }

        /// <summary>
        /// Удаление все текущей покупки
        /// </summary>
        private void DeleteAllBuyNow_Click(object sender, RoutedEventArgs e)
        {
            BuyNowData.Clear();
            RefreshNotifications("Все текущие покупки удалены");
        }

        /// <summary>
        /// Добавление будущей покупки
        /// </summary>
        private void AddBuyLaterButton_Click(object sender, RoutedEventArgs e)
        {
            BuyLater newbuylater = new BuyLater("Ввести наименование", 1.0, 1.0, "");
            BuyLaterData.Add(newbuylater);
            ToBuyLater.SelectedItem = newbuylater;
        }

        /// <summary>
        /// Удаление всех будущих покупок
        /// </summary>
        private void DeleteAllBuyLater_Click(object sender, RoutedEventArgs e)
        {
            BuyLaterData.Clear();
            RefreshNotifications("Все будущие покупки удалены");
        }

        /// <summary>
        /// Добавление задачи в ежедневник
        /// </summary>
        private void AddDoNowButton_Click(object sender, RoutedEventArgs e)
        {
            string task = DoTask.Text;
            DateTime date = DoDate.SelectedDate.Value.Date;
            string time = chTimes[DoTime.SelectedIndex].Time;
            string urgency = UrgencyModes[DoUrgency.SelectedIndex].Urgency;
            if (String.IsNullOrWhiteSpace(task))
            {
                MessageBox.Show("Укажите текст задачи");
            }
            else
            {
                DoNow newdonow = new DoNow(task, date, time, urgency, false);
                DoNowData.Add(newdonow);
                DoTask.Clear();
                DoTime.SelectedIndex = 24;
                RefreshNotifications("Добавлена задача: " + newdonow.Task);
            }
        }

        /// <summary>
        /// Фильтрация данных ежедневника по выбранной в календаре дате
        /// </summary>
        private void DoNowCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            DoNow dn = e.Item as DoNow;
            if (dn != null)
            {
                e.Accepted =
                    dn.Date == CalendarShowingTasks.SelectedDate // Сортировка по дате
                    && (!(bool)ShowNotCompleted.IsChecked || !dn.IsCompleted); // Показать только невыполненные задачи (при включенной опции)
            }
        }

        /// <summary>
        /// Изменение выбранной в календаре даты
        /// </summary>
        private void CalendarShowingTasks_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ToDoNow.ItemsSource).Refresh();
        }

        /// <summary>
        /// Удаление всех задач из ежедневника
        /// </summary>
        private void DeleteAllDoNow_Click(object sender, RoutedEventArgs e)
        {
            if (DoNowData.Count > 0)
            {
                DoNowData.Clear();
                RefreshNotifications("Все задачи удалены");
            }
            else
                RefreshNotifications("Задач нет");
        }

        /// <summary>
        /// Удаление старых задач (до вчерашнего дня включительно)
        /// </summary>
        private void DeleteOldDoNow_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            for (int i = DoNowData.Count - 1; i >= 0; i--)
            {
                DoNow dn = DoNowData.ElementAt(i);
                if (dn.Date < DateTime.Today)
                {
                    DoNowData.RemoveAt(i);
                    count += 1;
                }
            }
            if(count > 0)
                RefreshNotifications("Старые задачи удалены");
            else
                RefreshNotifications("Старых задач нет");
        }

        /// <summary>
        /// Изменение опции показа задач (все/невыполненные)
        /// </summary>
        private void ShowNotCompleted_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ToDoNow.ItemsSource).Refresh();
        }

        /// <summary>
        /// Удаление выполненных задач
        /// </summary>
        private void DeleteComplitedDoNow_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            for (int i = DoNowData.Count - 1; i >= 0; i--)
            {
                DoNow dn = DoNowData.ElementAt(i);
                if (dn.IsCompleted)
                {
                    DoNowData.RemoveAt(i);
                    count += 1;
                }
            }
            if (count > 0)
                RefreshNotifications("Все выполненные задачи удалены");
            else
                RefreshNotifications("Выполненных задач нет");
        }

        /// <summary>
        /// Сохранение данных в файл
        /// </summary>
        private void SaveItems_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBData));
            using (FileStream stream = new FileStream(Settings1.Default.Data_File, FileMode.Create))
            {
                try
                {
                    serializer.Serialize(stream, NBDataCollection);
                    RefreshNotifications("Все данные сохранены");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// Закрытие приложения
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Удаление выбранной задачи
        /// </summary>
        private void DeleteDoNowItem_Click(object sender, RoutedEventArgs e)
        {
            if (ToDoNow.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать задачу");
            }
            else
            {
                DoNow dn = (DoNow)ToDoNow.SelectedItem;
                string task = dn.Task;
                DoNowData.Remove((DoNow)ToDoNow.SelectedItem);
                RefreshNotifications("Задача \"" + task + "\" удалена");
            }
        }

        /// <summary>
        /// Обновление области уведомлений
        /// </summary>
        private void RefreshNotifications(string newInfo)
        {
            string existingInfo = Notifications.Text;
            string curTime = DateTime.Now.ToString();
            Run newRun = new Run(curTime + " - " + newInfo);
            newRun.FontWeight = FontWeights.Bold;
            Run oldRun = new Run("\n" + existingInfo);
            Notifications.Text = null;
            Notifications.Inlines.Add(newRun);
            Notifications.Inlines.Add(oldRun);
            NotificationArea.BeginStoryboard((Storyboard)FindResource("Animation"));
        }

        /// <summary>
        /// Сохранение используемой цветовой темы
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NoteBoard.Settings1.Default.Save();
        }
    }
}

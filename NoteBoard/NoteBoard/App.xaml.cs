using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NoteBoard
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Закрыть запущенную копию приложения, если оно уже открыто
        System.Threading.Mutex mutex;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutexName = "Приложение";
            mutex = new System.Threading.Mutex(true, mutexName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Приложение уже открыто");
                Shutdown();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;


namespace ServicesKiller
{
    public partial class MainWindow : Window
    {
        class ProcessData : IEquatable<ProcessData>, IComparable<ProcessData>
        {
            public string name { get; }
            public int id;
            public long size { get; }
            public ProcessData(string name, long size, int id)
            {
                this.name = name;
                this.size = size;
                this.id = id;
            }

            public bool Equals(ProcessData other)
            {
                if (other.name == this.name && other.size == this.size) return true;
                else return false;
            }

            public int CompareTo(ProcessData other)
            {
                if (other.size < this.size) return -1;
                else if (other.size > this.size) return 1;
                else return 0;
            }
        }

        class ServiceData
        {
            public string DisplayName { get; set; }
            public Brush Color { get; set; }

            public ServiceData()
            {
            }
        }

        NotifyIcon ni;
        BackgroundWorker observer = new BackgroundWorker();
        ObservableCollection<ProcessData> processesList = new ObservableCollection<ProcessData>();

        System.ServiceProcess.ServiceController[] services;
        ObservableCollection<ServiceData> servicesL;
        Process[] processes;

        public MainWindow()
        {
            InitializeComponent();

            // Для сворачивания в трей
            ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon("icon.ico");
            ni.Visible = true;
            ni.MouseClick += (o, e) => {
                this.Show();
                this.WindowState = WindowState.Normal;
            }; 
            this.StateChanged += MainWindow_StateChanged;
            this.Closing += (o, e) => ni.Visible = false;

            // подгрузка списка сервисов и раскраска их в таблице
            services = System.ServiceProcess.ServiceController.GetServices();
            servicesL = new ObservableCollection<ServiceData>();
            this.servicesListBox.ItemsSource = servicesL;
            for (int i = 0; i < services.Length; i++)
            {
                ServiceData a = new ServiceData();
                a.DisplayName = services[i].DisplayName;
                switch (services[i].Status)
                {
                    case System.ServiceProcess.ServiceControllerStatus.Paused:
                        a.Color = Brushes.Gray;
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Running:
                        a.Color = Brushes.LightGreen;
                        break;
                    case System.ServiceProcess.ServiceControllerStatus.Stopped:
                        a.Color = Brushes.LightSalmon;
                        break;
                    default:
                        a.Color = Brushes.White;
                        break;
                }
                servicesL.Add(a);
            };



            
            this.processesTable.ItemsSource = processesList;
            this.processesTable.LoadingRow += processesTable_LoadingRow;

            // запуск фонового обозревателя процессов
            observer.DoWork += Observer_StartObserving;
            observer.ProgressChanged += Observer_ProgressChanged;
            observer.WorkerReportsProgress = true;
            observer.RunWorkerAsync();
        }

        private void ServicesObserver_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void processesTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.GetIndex() < 3) e.Row.Background = new SolidColorBrush(Colors.Red);
            else e.Row.Background = new SolidColorBrush(Colors.White);

        }
        
        private void Observer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Получение процессов, сортировка и внесение изменений в таблицу
            List<ProcessData> temp = new List<ProcessData>();
            for (int i = 0; i < processes.Length; i++)
            {
                temp.Add(new ProcessData(processes[i].ProcessName, processes[i].WorkingSet64 / 1024, processes[i].Id));
            }
            temp.Sort();
            for (int i = 0; i < temp.Count; i++)
            {
                if (i >= processesList.Count)
                {
                    processesList.Insert(i, temp[i]);
                }
                else
                {
                    if (temp[i] != processesList[i])
                        processesList[i] = temp[i];
                }
                while (processesList.Count > temp.Count)
                {
                    processesList.RemoveAt(processesList.Count - 1);
                }
            }
            this.processesTable.ItemsSource = processesList;

        }

        private void Observer_StartObserving(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                this.processes = Process.GetProcesses();
                (sender as BackgroundWorker).ReportProgress(0);
                Thread.Sleep(1000);
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void killButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                processes.First(x => x.Id == processesList[i].id).Kill();
            }
        }

        private void serviceButton_Click(object sender, RoutedEventArgs e)
        {
            int index = this.servicesListBox.SelectedIndex;
            if (index >= 0)
            {
                try
                {
                    if (services[index].Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    {
                        if (services[index].CanStop == true)
                            services[index].Stop();
                    }
                    else
                    {
                        services[index].Start();
                    }
                }
                catch (Exception er)
                {
                    System.Windows.MessageBox.Show("Не удалось запустить/остановить службу " + services[index].DisplayName);
                }

                servicesL.Clear();

                services = System.ServiceProcess.ServiceController.GetServices();
                for (int i = 0; i < services.Length; i++)
                {
                    ServiceData a = new ServiceData();
                    a.DisplayName = services[i].DisplayName;
                    switch (services[i].Status)
                    {
                        case System.ServiceProcess.ServiceControllerStatus.Paused:
                            a.Color = Brushes.Gray;
                            break;
                        case System.ServiceProcess.ServiceControllerStatus.Running:
                            a.Color = Brushes.LightGreen;
                            break;
                        case System.ServiceProcess.ServiceControllerStatus.Stopped:
                            a.Color = Brushes.LightSalmon;
                            break;
                        default:
                            a.Color = Brushes.White;
                            break;
                    }
                    servicesL.Add(a);
                };
            }
        }
    }
}

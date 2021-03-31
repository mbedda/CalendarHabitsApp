using CalendarHabitsApp.Helpers;
using CalendarHabitsApp.Models;
using log4net;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using IOPath = System.IO.Path;

namespace CalendarHabitsApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private Settings _settings;
        public Settings Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }

        public CalendarCell CurrentDateCell { get; set; }

        DispatcherTimer RefreshTimer = new DispatcherTimer();
        DispatcherTimer AutoSaveTimer = new DispatcherTimer();

        public DelegateCommand UpdateCommand { get; set; }

        string DataPath = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Calendar Habits", "hbts.calhab");
        public bool DataChanged = false;

        public MainViewModel()
        {
            log4net.Config.XmlConfigurator.Configure();
            UpdateCommand = new DelegateCommand(Update);

            //InitAsync();
        }

        private void Update()
        {
            UpdateWallpaper();
        }

        public async Task InitAsync()
        {
            RefreshTimer.Interval = TimeSpan.FromSeconds(10);
            RefreshTimer.Tick += RefreshTimer_Tick;

            AutoSaveTimer.Interval = TimeSpan.FromSeconds(5);
            AutoSaveTimer.Tick += AutoSaveTimer_Tick;

            CurrentDateCell = new CalendarCell();

            Settings = await Task.Run(() => Common.LoadJson<Settings>(DataPath));

            if (Settings == null)
                Settings = new Settings();

            Settings.PropertyChanged += Settings_PropertyChanged;
            Settings.HabitDays.CollectionChanged += HabitDays_CollectionChanged;

            FillSelectedMonthInfo();

            Update();
            RefreshTimer.Start();
        }

        private void HabitDays_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Settings_PropertyChanged(sender, null);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DataChanged = true;
            AutoSaveTimer.Stop();
            AutoSaveTimer.Start();
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            Save();
            AutoSaveTimer.Stop();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (Settings.CurrentDate.ToString("MM/dd/yyyy") != DateTime.Now.ToString("MM/dd/yyyy"))
            {
                if (Settings.CurrentDate.Month != DateTime.Now.Month)
                {
                    Settings.CurrentDate = DateTime.Now;
                    FillSelectedMonthInfo();
                }

                Update();
            }
        }

        public void FillSelectedMonthInfo()
        {
            Settings.SelectedMonthDays = new ObservableCollection<MonthDay>();
            DateTime indexedDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            while (!indexedDay.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                indexedDay = indexedDay.AddDays(-1);
            }


            for (int i = 0; i < 6; i++)
            {
                List<int> colList = new List<int>();

                for (int j = 0; j < 7; j++)
                {
                    bool habitCheck = false;

                    var habitDay = Settings.HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == indexedDay.ToString("MM/dd/yyyy"));
                    if (habitDay != -1)
                    {
                        habitCheck = true;
                    }

                    if (indexedDay.Month != Settings.CurrentDate.Month)
                    {
                        Settings.SelectedMonthDays.Add(new MonthDay() { Date = indexedDay, FromCurrentMonth = false, Checked = habitCheck });
                    }
                    else
                    {
                        Settings.SelectedMonthDays.Add(new MonthDay() { Date = indexedDay, FromCurrentMonth = true, Checked = habitCheck });
                    }

                    indexedDay = indexedDay.AddDays(1);
                }
            }
        }

        public async Task UpdateWallpaper()
        {
            if (Settings.PauseWallpaperUpdate)
            {
                return;
            }

            MainWindow.log.Info("Updating wallpaper - starting...");
            Settings.CurrentDate = DateTime.Now;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (Settings.SelectedMonthDays[(i * 7) + j].Date.ToString("MM/dd/yyyy") == Settings.CurrentDate.ToString("MM/dd/yyyy"))
                    {
                        CurrentDateCell.X = j + 1;
                        CurrentDateCell.Y = i + 1;
                    }
                }
            }

            await Task.Run(() => Wallpaper.CreateCalendar(Settings.DarkMode, Settings.CurrentDate,
                CurrentDateCell, Settings.SelectedMonthDays.ToList(), Settings.HabitDays.ToList()));

            Wallpaper.Set(IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.png"));
        }

        public void Save()
        {
            if (DataChanged)
            {
                Common.SaveJson(Settings, DataPath);
                DataChanged = false;
                UpdateWallpaper();
                Console.WriteLine("Data saved");
            }
        }
    }
}

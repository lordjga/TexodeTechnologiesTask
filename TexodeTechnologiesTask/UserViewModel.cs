using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Newtonsoft.Json;
using TexodeTechnologiesTask.Annotations;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using ServiceStack.Text;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace TexodeTechnologiesTask
{
    class UserViewModel : INotifyPropertyChanged
    {
        private UserModel _selectedUser;
        private SeriesCollection _lineChartSeriesCollection;
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<ObservableCollection<UserModel>> Days { get; set; }
        public SeriesCollection LineChartSeriesCollection {
            get => _lineChartSeriesCollection;
            set
            {
                _lineChartSeriesCollection = value;
                OnPropertyChanged("LineChartSeriesCollection");
            }
        }
        string[] Files { get; set; }
        List<string> JsonsList { get; set; }
        public UserModel SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; 
                OnPropertyChanged("SelectedUser");
                Chart();
            }
        }
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(obj =>
                {
                    string objN = obj.ToString();
                    switch (objN)
                    {
                        case "XML":
                            XmlSerializer formatter = new XmlSerializer(typeof(UserModel));
                            using (FileStream fs = new FileStream(@"Users/"+SelectedUser.User+".xml", FileMode.OpenOrCreate))
                            {
                                formatter.Serialize(fs, SelectedUser);
                            }
                            break;
                        case "JSON":
                            string fileName = SelectedUser.User + ".json";
                            string jsonString = JsonConvert.SerializeObject(SelectedUser);
                            File.WriteAllText(fileName, jsonString);
                            break;
                        case "CSV":
                            using (FileStream fs = new FileStream(SelectedUser.User + ".csv", FileMode.OpenOrCreate))
                            {
                                CsvSerializer.SerializeToStream(SelectedUser, fs);
                            }
                            break;
                    }
                    MessageBox.Show("Сохранено в " + objN);
                });
            }
        }

        public UserViewModel()
        {
            FilesArray(@"TestData/");
            FilesToStrings();
            FindTheLongestJson();
            WriteSteps();
            ColorFind();
        }
        public void FilesArray(string dirName)
        {
            if (Directory.Exists(dirName))
            {
                Files = Directory.GetFiles(dirName);
            }
        }
        private void FindTheLongestJson()
        {
            Users = new ObservableCollection<UserModel>();
            Days = new ObservableCollection<ObservableCollection<UserModel>>();
            for (int i = 0; i < JsonsList.Count; i++)
            {
                Users = JsonConvert.DeserializeObject<ObservableCollection<UserModel>>(JsonsList[i]);
                Days.Add(Users);
            }
            Users = Days.FirstOrDefault(ex => ex.Count() >= 100);

        }

        private void FilesToStrings()
        {
            JsonsList = new List<string>();
            for (int i = 1; i <= Files.Length; i++)
            {
                using (FileStream fs = new FileStream(@"TestData/day"+i+".json", FileMode.Open))
                {
                    byte[] array = new byte[fs.Length];
                    fs.Read(array, 0, array.Length);
                    JsonsList.Add(System.Text.Encoding.Default.GetString(array));
                }
            }
        }
        
        public void WriteSteps()
        {
            for (int i=0;i<Users.Count;i++)
            {
                Users[i].StepsL = new List<int>();
                Users[i].RanksL = new List<int>();
                for (int j = 0; j < Days.Count; j++)
                {
                    Users[i].StepsL.Add(Convert.ToInt32(Days[j].FirstOrDefault(ex => ex.User == Users[i].User)?.Steps));
                    Users[i].RanksL.Add(Convert.ToInt32(Days[j].FirstOrDefault(ex => ex.User == Users[i].User)?.Rank));
                }
            }

            foreach (var i in Users)
            {
                i.AverageRank = (int)i.RanksL.Average();
                i.AverageSteps = (int)i.StepsL.Average();
                i.MaxSteps = i.StepsL.Max();
                i.MinSteps = i.StepsL.Min();
            }
            SelectedUser = Users.First();
            
            
        }
        private void ColorFind()
        {
            foreach (var i in Users)
            {
                i.IsAnother = false;
                if ((i.AverageSteps + i.MaxSteps * 20 / 100) < i.MaxSteps || (i.AverageSteps - i.MinSteps * 20 / 100) > i.MinSteps)
                {
                    i.IsAnother = true;
                }
            }
        }
        public void Chart()
        {
            var t = SelectedUser.StepsL;
            LineChartSeriesCollection = new SeriesCollection();
            LineChartSeriesCollection.Add(new LineSeries
            {
                Title = SelectedUser.User,
                Values = t.AsChartValues()
            }) ;
        }
        

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

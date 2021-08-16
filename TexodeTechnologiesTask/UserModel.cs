using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TexodeTechnologiesTask.Annotations;

namespace TexodeTechnologiesTask
{
    public class UserModel : INotifyPropertyChanged
    {
        private bool _isAnother;
        public int Rank { get; set; }
        public int AverageRank { get; set; }
        public List<int> RanksL { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public int Steps { get; set; }
        public int AverageSteps { get; set; }
        public int MinSteps { get; set; }
        public int MaxSteps { get; set; }
        public List<int> StepsL { get; set; }
        public bool IsAnother 
        {
            get => _isAnother;
            set
            {
                _isAnother = value;
                OnPropertyChanged("IsAnother");
            }
        }
        public bool ShouldSerializeRank()
        {
            return false;
        }
        public bool ShouldSerializeSteps()
        {
            return false;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

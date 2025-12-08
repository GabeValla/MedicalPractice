using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.MedicalPractice.Models
{
    public class Patient : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _address = string.Empty;
        private DateTime _birthDate = DateTime.Today.AddYears(-30);
        private string _race = string.Empty;
        private string _gender = string.Empty;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (SetProperty(ref _birthDate, value))
                {
                    OnPropertyChanged(nameof(Age));
                    OnPropertyChanged(nameof(IsMinor));
                    OnPropertyChanged(nameof(HighlightColor));
                    OnPropertyChanged(nameof(AgeDisplay));
                }
            }
        }

        public string Race
        {
            get => _race;
            set => SetProperty(ref _race, value);
        }

        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        public List<MedicalNote> MedicalNotes { get; set; } = new();

        public int Age => DateTime.Today.Year - BirthDate.Year - 
            (DateTime.Today.DayOfYear < BirthDate.DayOfYear ? 1 : 0);

        /// <summary>
        /// Returns true if patient is under 18 years old
        /// </summary>
        public bool IsMinor => Age < 18;

        public string DisplayInfo => $"{Name} | {Gender} | Age: {Age}";

        public string AgeDisplay => IsMinor ? $"Age {Age} (Minor)" : $"Age {Age}";

        /// <summary>
        /// Color coding: Orange for minors, Primary color for adults
        /// </summary>
        public Color HighlightColor => IsMinor ? Colors.Orange : Color.FromArgb("#0D7377");

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override string ToString() => DisplayInfo;
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.MedicalPractice.Models
{
    public class Physician : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _licenseNumber = string.Empty;
        private DateTime _graduationDate = DateTime.Today.AddYears(-10);
        private string _specialization = string.Empty;

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

        public string LicenseNumber
        {
            get => _licenseNumber;
            set => SetProperty(ref _licenseNumber, value);
        }

        public DateTime GraduationDate
        {
            get => _graduationDate;
            set => SetProperty(ref _graduationDate, value);
        }

        public string Specialization
        {
            get => _specialization;
            set => SetProperty(ref _specialization, value);
        }

        public int YearsOfExperience => DateTime.Today.Year - GraduationDate.Year;

        public string DisplayInfo => $"Dr. {Name} | {Specialization}";

        public string FullDisplayInfo => $"Dr. {Name} | {Specialization} | License: {LicenseNumber}";

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


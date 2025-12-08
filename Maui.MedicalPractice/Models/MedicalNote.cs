using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.MedicalPractice.Models
{
    public class MedicalNote : INotifyPropertyChanged
    {
        private int _id;
        private int _patientId;
        private int? _physicianId;
        private DateTime _dateCreated = DateTime.Now;
        private string _diagnosis = string.Empty;
        private string _prescription = string.Empty;
        private string _notes = string.Empty;
        private Physician? _physician;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int PatientId
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        public int? PhysicianId
        {
            get => _physicianId;
            set => SetProperty(ref _physicianId, value);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        public string Diagnosis
        {
            get => _diagnosis;
            set => SetProperty(ref _diagnosis, value);
        }

        public string Prescription
        {
            get => _prescription;
            set => SetProperty(ref _prescription, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public Physician? Physician
        {
            get => _physician;
            set => SetProperty(ref _physician, value);
        }

        public string DisplayInfo => $"{DateCreated:MMM dd, yyyy} - {Diagnosis}";

        public string PhysicianDisplay => Physician != null ? $"Dr. {Physician.Name}" : "N/A";

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


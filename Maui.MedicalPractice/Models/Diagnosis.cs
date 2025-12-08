using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.MedicalPractice.Models
{
    public class Diagnosis : INotifyPropertyChanged
    {
        private int _id;
        private int _appointmentId;
        private string _code = string.Empty;
        private string _description = string.Empty;
        private string _notes = string.Empty;
        private DateTime _dateCreated = DateTime.Now;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int AppointmentId
        {
            get => _appointmentId;
            set => SetProperty(ref _appointmentId, value);
        }

        /// <summary>
        /// ICD-10 or similar diagnostic code
        /// </summary>
        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        public string DisplayInfo => string.IsNullOrEmpty(Code) ? Description : $"{Code}: {Description}";

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


using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.MedicalPractice.Models
{
    public class Treatment : INotifyPropertyChanged
    {
        private int _id;
        private int _appointmentId;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _cost;
        private DateTime _datePerformed = DateTime.Now;
        private string _notes = string.Empty;

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

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        public DateTime DatePerformed
        {
            get => _datePerformed;
            set => SetProperty(ref _datePerformed, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public string DisplayInfo => $"{Name} - {Cost:C}";
        public string CostDisplay => Cost.ToString("C");

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


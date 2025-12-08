using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.MedicalPractice.Models
{
    public class Appointment : INotifyPropertyChanged
    {
        private int _id;
        private int _patientId;
        private int _physicianId;
        private DateTime _appointmentDate;
        private TimeSpan _appointmentTime;
        private string _notes = string.Empty;
        private AppointmentStatus _status = AppointmentStatus.Scheduled;
        private string _room = string.Empty;

        // Navigation properties (populated by service)
        private Patient? _patient;
        private Physician? _physician;
        private List<Diagnosis> _diagnoses = new();
        private List<Treatment> _treatments = new();

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

        public int PhysicianId
        {
            get => _physicianId;
            set => SetProperty(ref _physicianId, value);
        }

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set => SetProperty(ref _appointmentDate, value);
        }

        public TimeSpan AppointmentTime
        {
            get => _appointmentTime;
            set => SetProperty(ref _appointmentTime, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public AppointmentStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                {
                    OnPropertyChanged(nameof(StatusDisplay));
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }

        /// <summary>
        /// Room number/name where the appointment takes place
        /// </summary>
        public string Room
        {
            get => _room;
            set => SetProperty(ref _room, value);
        }

        public Patient? Patient
        {
            get => _patient;
            set => SetProperty(ref _patient, value);
        }

        public Physician? Physician
        {
            get => _physician;
            set => SetProperty(ref _physician, value);
        }

        public List<Diagnosis> Diagnoses
        {
            get => _diagnoses;
            set => SetProperty(ref _diagnoses, value);
        }

        public List<Treatment> Treatments
        {
            get => _treatments;
            set => SetProperty(ref _treatments, value);
        }

        public DateTime FullDateTime => AppointmentDate.Date.Add(AppointmentTime);

        public string DisplayInfo => $"{AppointmentDate:MMM dd, yyyy} at {DateTime.Today.Add(AppointmentTime):hh:mm tt}";

        public string FullDisplayInfo => Patient != null && Physician != null
            ? $"{DisplayInfo} | {Patient.Name} with Dr. {Physician.Name}"
            : DisplayInfo;

        public string StatusDisplay => Status.ToString();

        public string RoomDisplay => string.IsNullOrEmpty(Room) ? "No Room" : $"Room {Room}";

        public bool IsToday => AppointmentDate.Date == DateTime.Today;

        public decimal TotalCost => Treatments?.Sum(t => t.Cost) ?? 0;

        public string TotalCostDisplay => TotalCost.ToString("C");

        public Color StatusColor => Status switch
        {
            AppointmentStatus.Scheduled => Colors.DodgerBlue,
            AppointmentStatus.Completed => Colors.Green,
            AppointmentStatus.Cancelled => Colors.Gray,
            AppointmentStatus.NoShow => Colors.OrangeRed,
            _ => Colors.Gray
        };

        /// <summary>
        /// Color coding: Orange for today's appointments, standard status color otherwise
        /// </summary>
        public Color HighlightColor => IsToday && Status == AppointmentStatus.Scheduled
            ? Colors.Orange
            : StatusColor;

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

        public override string ToString() => FullDisplayInfo;
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;

namespace Maui.MedicalPractice.ViewModels
{
    [QueryProperty(nameof(AppointmentId), "AppointmentId")]
    public class AppointmentDetailViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private int _appointmentId;
        private Patient? _selectedPatient;
        private Physician? _selectedPhysician;
        private DateTime _appointmentDate = DateTime.Today;
        private TimeSpan _appointmentTime = new TimeSpan(9, 0, 0);
        private string _notes = string.Empty;
        private AppointmentStatus _selectedStatus = AppointmentStatus.Scheduled;
        private string _selectedRoom = string.Empty;
        private bool _isNewAppointment = true;

        // Form fields
        public Patient? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        public Physician? SelectedPhysician
        {
            get => _selectedPhysician;
            set => SetProperty(ref _selectedPhysician, value);
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

        public AppointmentStatus SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public string SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }

        public bool IsNewAppointment
        {
            get => _isNewAppointment;
            set
            {
                if (SetProperty(ref _isNewAppointment, value))
                    OnPropertyChanged(nameof(IsExistingAppointment));
            }
        }

        public bool IsExistingAppointment => !IsNewAppointment;

        public int AppointmentId
        {
            get => _appointmentId;
            set
            {
                _appointmentId = value;
                if (value > 0)
                {
                    IsNewAppointment = false;
                    LoadAppointmentAsync(value);
                }
            }
        }

        // Collections for pickers
        public ObservableCollection<Patient> Patients { get; } = new();
        public ObservableCollection<Physician> Physicians { get; } = new();
        public ObservableCollection<Diagnosis> Diagnoses { get; } = new();
        public ObservableCollection<Treatment> Treatments { get; } = new();

        public List<AppointmentStatus> StatusOptions { get; } = new()
        {
            AppointmentStatus.Scheduled,
            AppointmentStatus.Completed,
            AppointmentStatus.Cancelled,
            AppointmentStatus.NoShow
        };

        public List<TimeSpan> AvailableTimeSlots { get; } = new()
        {
            new TimeSpan(8, 0, 0),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0),
            new TimeSpan(16, 0, 0)
        };

        public List<string> AvailableRooms { get; private set; } = new();

        public decimal TotalTreatmentCost => Treatments.Sum(t => t.Cost);
        public string TotalCostDisplay => TotalTreatmentCost.ToString("C");

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand AddDiagnosisCommand { get; }
        public ICommand DeleteDiagnosisCommand { get; }
        public ICommand AddTreatmentCommand { get; }
        public ICommand DeleteTreatmentCommand { get; }

        public AppointmentDetailViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "New Appointment";
            AvailableRooms = dataService.GetAvailableRooms();

            SaveCommand = new Command(async () => await SaveAppointmentAsync());
            DeleteCommand = new Command(async () => await DeleteAppointmentAsync());
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            AddDiagnosisCommand = new Command(async () => await AddDiagnosisAsync());
            DeleteDiagnosisCommand = new Command<Diagnosis>(async (d) => await DeleteDiagnosisAsync(d));
            AddTreatmentCommand = new Command(async () => await AddTreatmentAsync());
            DeleteTreatmentCommand = new Command<Treatment>(async (t) => await DeleteTreatmentAsync(t));
        }

        public async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var patients = await _dataService.GetAllPatientsAsync();
                var physicians = await _dataService.GetAllPhysiciansAsync();

                Patients.Clear();
                foreach (var patient in patients)
                {
                    Patients.Add(patient);
                }

                Physicians.Clear();
                foreach (var physician in physicians)
                {
                    Physicians.Add(physician);
                }
            });
        }

        private async void LoadAppointmentAsync(int id)
        {
            await LoadDataAsync();

            await ExecuteAsync(async () =>
            {
                var appointment = await _dataService.GetAppointmentByIdAsync(id);
                if (appointment != null)
                {
                    SelectedPatient = Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                    SelectedPhysician = Physicians.FirstOrDefault(p => p.Id == appointment.PhysicianId);
                    AppointmentDate = appointment.AppointmentDate;
                    AppointmentTime = appointment.AppointmentTime;
                    Notes = appointment.Notes;
                    SelectedStatus = appointment.Status;
                    SelectedRoom = appointment.Room;
                    Title = $"Edit Appointment";

                    // Load diagnoses and treatments
                    await LoadDiagnosesAsync(id);
                    await LoadTreatmentsAsync(id);
                }
            });
        }

        private async Task LoadDiagnosesAsync(int appointmentId)
        {
            var diagnoses = await _dataService.GetDiagnosesByAppointmentAsync(appointmentId);
            Diagnoses.Clear();
            foreach (var diagnosis in diagnoses)
            {
                Diagnoses.Add(diagnosis);
            }
        }

        private async Task LoadTreatmentsAsync(int appointmentId)
        {
            var treatments = await _dataService.GetTreatmentsByAppointmentAsync(appointmentId);
            Treatments.Clear();
            foreach (var treatment in treatments)
            {
                Treatments.Add(treatment);
            }
            OnPropertyChanged(nameof(TotalTreatmentCost));
            OnPropertyChanged(nameof(TotalCostDisplay));
        }

        private async Task SaveAppointmentAsync()
        {
            if (SelectedPatient == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a patient.", "OK");
                return;
            }

            if (SelectedPhysician == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a physician.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(SelectedRoom))
            {
                await Shell.Current.DisplayAlert("Error", "Please select a room.", "OK");
                return;
            }

            if (!_dataService.IsValidAppointmentTime(AppointmentDate, AppointmentTime))
            {
                await Shell.Current.DisplayAlert(
                    "Invalid Time",
                    "Appointments are only available Monday-Friday, 8 AM - 5 PM.",
                    "OK");
                return;
            }

            // Check physician availability
            var physicianAvailable = await _dataService.IsPhysicianAvailableAsync(
                SelectedPhysician.Id,
                AppointmentDate,
                AppointmentTime,
                IsNewAppointment ? null : _appointmentId);

            if (!physicianAvailable)
            {
                await Shell.Current.DisplayAlert(
                    "Conflict",
                    $"Dr. {SelectedPhysician.Name} is not available at this time.",
                    "OK");
                return;
            }

            // Check room availability
            var roomAvailable = await _dataService.IsRoomAvailableAsync(
                SelectedRoom,
                AppointmentDate,
                AppointmentTime,
                IsNewAppointment ? null : _appointmentId);

            if (!roomAvailable)
            {
                await Shell.Current.DisplayAlert(
                    "Room Conflict",
                    $"Room {SelectedRoom} is already booked at this time.",
                    "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                var appointment = new Appointment
                {
                    Id = _appointmentId,
                    PatientId = SelectedPatient.Id,
                    PhysicianId = SelectedPhysician.Id,
                    AppointmentDate = AppointmentDate,
                    AppointmentTime = AppointmentTime,
                    Notes = Notes,
                    Status = SelectedStatus,
                    Room = SelectedRoom
                };

                if (IsNewAppointment)
                {
                    await _dataService.AddAppointmentAsync(appointment);
                    await Shell.Current.DisplayAlert("Success", "Appointment scheduled.", "OK");
                }
                else
                {
                    await _dataService.UpdateAppointmentAsync(appointment);
                    await Shell.Current.DisplayAlert("Success", "Appointment updated.", "OK");
                }

                await Shell.Current.GoToAsync("..");
            });
        }

        private async Task DeleteAppointmentAsync()
        {
            if (IsNewAppointment) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Delete Appointment",
                "Are you sure you want to delete this appointment?",
                "Delete", "Cancel");

            if (!confirm) return;

            await ExecuteAsync(async () =>
            {
                await _dataService.DeleteAppointmentAsync(_appointmentId);
                await Shell.Current.DisplayAlert("Success", "Appointment deleted.", "OK");
                await Shell.Current.GoToAsync("..");
            });
        }

        #region Diagnosis Management

        private async Task AddDiagnosisAsync()
        {
            if (IsNewAppointment)
            {
                await Shell.Current.DisplayAlert("Info", "Save the appointment first before adding diagnoses.", "OK");
                return;
            }

            string code = await Shell.Current.DisplayPromptAsync("Add Diagnosis", "ICD-10 Code (optional):") ?? "";
            string description = await Shell.Current.DisplayPromptAsync("Add Diagnosis", "Description:");
            if (string.IsNullOrWhiteSpace(description)) return;

            string notes = await Shell.Current.DisplayPromptAsync("Add Diagnosis", "Notes (optional):") ?? "";

            var diagnosis = new Diagnosis
            {
                AppointmentId = _appointmentId,
                Code = code,
                Description = description,
                Notes = notes,
                DateCreated = DateTime.Now
            };

            await _dataService.AddDiagnosisAsync(diagnosis);
            Diagnoses.Add(diagnosis);
        }

        private async Task DeleteDiagnosisAsync(Diagnosis diagnosis)
        {
            if (diagnosis == null) return;

            var confirm = await Shell.Current.DisplayAlert("Delete", $"Delete diagnosis: {diagnosis.Description}?", "Delete", "Cancel");
            if (confirm)
            {
                await _dataService.DeleteDiagnosisAsync(diagnosis.Id);
                Diagnoses.Remove(diagnosis);
            }
        }

        #endregion

        #region Treatment Management

        private async Task AddTreatmentAsync()
        {
            if (IsNewAppointment)
            {
                await Shell.Current.DisplayAlert("Info", "Save the appointment first before adding treatments.", "OK");
                return;
            }

            string name = await Shell.Current.DisplayPromptAsync("Add Treatment", "Treatment Name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string description = await Shell.Current.DisplayPromptAsync("Add Treatment", "Description (optional):") ?? "";
            
            string costStr = await Shell.Current.DisplayPromptAsync("Add Treatment", "Cost ($):", keyboard: Keyboard.Numeric);
            if (!decimal.TryParse(costStr, out decimal cost)) cost = 0;

            string notes = await Shell.Current.DisplayPromptAsync("Add Treatment", "Notes (optional):") ?? "";

            var treatment = new Treatment
            {
                AppointmentId = _appointmentId,
                Name = name,
                Description = description,
                Cost = cost,
                Notes = notes,
                DatePerformed = DateTime.Now
            };

            await _dataService.AddTreatmentAsync(treatment);
            Treatments.Add(treatment);
            OnPropertyChanged(nameof(TotalTreatmentCost));
            OnPropertyChanged(nameof(TotalCostDisplay));
        }

        private async Task DeleteTreatmentAsync(Treatment treatment)
        {
            if (treatment == null) return;

            var confirm = await Shell.Current.DisplayAlert("Delete", $"Delete treatment: {treatment.Name}?", "Delete", "Cancel");
            if (confirm)
            {
                await _dataService.DeleteTreatmentAsync(treatment.Id);
                Treatments.Remove(treatment);
                OnPropertyChanged(nameof(TotalTreatmentCost));
                OnPropertyChanged(nameof(TotalCostDisplay));
            }
        }

        #endregion
    }
}

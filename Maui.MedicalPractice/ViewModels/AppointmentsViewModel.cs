using System.Collections.ObjectModel;
using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;
using Maui.MedicalPractice.Views;

namespace Maui.MedicalPractice.ViewModels
{
    public class AppointmentsViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private Appointment? _selectedAppointment;
        private DateTime _selectedDate = DateTime.Today;
        private string _selectedSortOption = "Time (Earliest)";
        private bool _showAllAppointments = false;
        private List<Appointment> _allAppointments = new();

        public ObservableCollection<Appointment> Appointments { get; } = new();

        public Appointment? SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                if (SetProperty(ref _selectedAppointment, value) && value != null)
                {
                    NavigateToAppointmentDetail(value);
                    SelectedAppointment = null;
                }
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    _showAllAppointments = false;
                    _ = LoadAppointmentsAsync();
                }
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    ApplySorting();
                }
            }
        }

        public List<string> SortOptions { get; } = new()
        {
            "Time (Earliest)",
            "Time (Latest)",
            "Patient (A-Z)",
            "Patient (Z-A)",
            "Status"
        };

        public ICommand LoadAppointmentsCommand { get; }
        public ICommand AddAppointmentCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewAllCommand { get; }
        public ICommand EditAppointmentCommand { get; }
        public ICommand DeleteAppointmentCommand { get; }
        public ICommand CancelAppointmentCommand { get; }

        public AppointmentsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Appointments";

            LoadAppointmentsCommand = new Command(async () => await LoadAppointmentsAsync());
            AddAppointmentCommand = new Command(async () => await NavigateToAddAppointment());
            RefreshCommand = new Command(async () => await LoadAppointmentsAsync());
            ViewAllCommand = new Command(async () => await LoadAllAppointmentsAsync());
            EditAppointmentCommand = new Command<Appointment>(async (a) => await EditAppointment(a));
            DeleteAppointmentCommand = new Command<Appointment>(async (a) => await DeleteAppointment(a));
            CancelAppointmentCommand = new Command<Appointment>(async (a) => await CancelAppointment(a));
        }

        public async Task LoadAppointmentsAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (_showAllAppointments)
                {
                    _allAppointments = await _dataService.GetAllAppointmentsAsync();
                    Title = "All Appointments";
                }
                else
                {
                    _allAppointments = await _dataService.GetAppointmentsByDateAsync(SelectedDate);
                    Title = $"Appointments - {SelectedDate:MMM dd}";
                }
                ApplySorting();
            });
        }

        private async Task LoadAllAppointmentsAsync()
        {
            _showAllAppointments = true;
            await LoadAppointmentsAsync();
        }

        private void ApplySorting()
        {
            var sorted = SelectedSortOption switch
            {
                "Time (Earliest)" => _allAppointments.OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime).ToList(),
                "Time (Latest)" => _allAppointments.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.AppointmentTime).ToList(),
                "Patient (A-Z)" => _allAppointments.OrderBy(a => a.Patient?.Name).ToList(),
                "Patient (Z-A)" => _allAppointments.OrderByDescending(a => a.Patient?.Name).ToList(),
                "Status" => _allAppointments.OrderBy(a => a.Status).ThenBy(a => a.AppointmentTime).ToList(),
                _ => _allAppointments.OrderBy(a => a.AppointmentTime).ToList()
            };

            Appointments.Clear();
            foreach (var appointment in sorted)
            {
                Appointments.Add(appointment);
            }
        }

        private async Task NavigateToAddAppointment()
        {
            await Shell.Current.GoToAsync(nameof(AppointmentDetailPage));
        }

        private async void NavigateToAppointmentDetail(Appointment appointment)
        {
            await Shell.Current.GoToAsync($"{nameof(AppointmentDetailPage)}?AppointmentId={appointment.Id}");
        }

        private async Task EditAppointment(Appointment appointment)
        {
            if (appointment != null)
            {
                await Shell.Current.GoToAsync($"{nameof(AppointmentDetailPage)}?AppointmentId={appointment.Id}");
            }
        }

        private async Task DeleteAppointment(Appointment appointment)
        {
            if (appointment == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Delete Appointment",
                $"Delete appointment for {appointment.Patient?.Name}?",
                "Delete", "Cancel");

            if (confirm)
            {
                await _dataService.DeleteAppointmentAsync(appointment.Id);
                _allAppointments.Remove(appointment);
                Appointments.Remove(appointment);
            }
        }

        private async Task CancelAppointment(Appointment appointment)
        {
            if (appointment == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Cancel Appointment",
                $"Cancel appointment for {appointment.Patient?.Name}?",
                "Yes", "No");

            if (confirm)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _dataService.UpdateAppointmentAsync(appointment);
                ApplySorting(); // Refresh to show updated status
            }
        }
    }
}

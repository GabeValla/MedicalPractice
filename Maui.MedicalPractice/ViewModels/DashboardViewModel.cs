using System.Collections.ObjectModel;
using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;
using Maui.MedicalPractice.Views;

namespace Maui.MedicalPractice.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private int _patientCount;
        private int _physicianCount;
        private int _todayAppointmentCount;
        private Appointment? _selectedAppointment;

        public int PatientCount
        {
            get => _patientCount;
            set => SetProperty(ref _patientCount, value);
        }

        public int PhysicianCount
        {
            get => _physicianCount;
            set => SetProperty(ref _physicianCount, value);
        }

        public int TodayAppointmentCount
        {
            get => _todayAppointmentCount;
            set => SetProperty(ref _todayAppointmentCount, value);
        }

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

        public ObservableCollection<Appointment> TodayAppointments { get; } = new();

        public ICommand LoadDashboardCommand { get; }
        public ICommand NavigateToPatientsCommand { get; }
        public ICommand NavigateToPhysiciansCommand { get; }
        public ICommand NavigateToAppointmentsCommand { get; }
        public ICommand NewAppointmentCommand { get; }

        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Medical Practice";

            LoadDashboardCommand = new Command(async () => await LoadDashboardAsync());
            NavigateToPatientsCommand = new Command(async () => await Shell.Current.GoToAsync("//Patients"));
            NavigateToPhysiciansCommand = new Command(async () => await Shell.Current.GoToAsync("//Physicians"));
            NavigateToAppointmentsCommand = new Command(async () => await Shell.Current.GoToAsync("//Appointments"));
            NewAppointmentCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AppointmentDetailPage)));
        }

        public async Task LoadDashboardAsync()
        {
            await ExecuteAsync(async () =>
            {
                var patients = await _dataService.GetAllPatientsAsync();
                var physicians = await _dataService.GetAllPhysiciansAsync();
                var todayAppointments = await _dataService.GetAppointmentsByDateAsync(DateTime.Today);

                PatientCount = patients.Count;
                PhysicianCount = physicians.Count;
                TodayAppointmentCount = todayAppointments.Count;

                TodayAppointments.Clear();
                foreach (var appointment in todayAppointments.Take(5))
                {
                    TodayAppointments.Add(appointment);
                }
            });
        }

        private async void NavigateToAppointmentDetail(Appointment appointment)
        {
            await Shell.Current.GoToAsync($"{nameof(AppointmentDetailPage)}?AppointmentId={appointment.Id}");
        }
    }
}


using System.Collections.ObjectModel;
using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;
using Maui.MedicalPractice.Views;

namespace Maui.MedicalPractice.ViewModels
{
    public class PatientsViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private Patient? _selectedPatient;
        private string _selectedSortOption = "Name (A-Z)";
        private List<Patient> _allPatients = new();

        public ObservableCollection<Patient> Patients { get; } = new();

        public Patient? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                if (SetProperty(ref _selectedPatient, value) && value != null)
                {
                    NavigateToPatientDetail(value);
                    SelectedPatient = null;
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
            "Name (A-Z)",
            "Name (Z-A)",
            "Age (Youngest)",
            "Age (Oldest)"
        };

        public ICommand LoadPatientsCommand { get; }
        public ICommand AddPatientCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand EditPatientCommand { get; }
        public ICommand DeletePatientCommand { get; }

        public PatientsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Patients";

            LoadPatientsCommand = new Command(async () => await LoadPatientsAsync());
            AddPatientCommand = new Command(async () => await NavigateToAddPatient());
            RefreshCommand = new Command(async () => await LoadPatientsAsync());
            EditPatientCommand = new Command<Patient>(async (patient) => await EditPatient(patient));
            DeletePatientCommand = new Command<Patient>(async (patient) => await DeletePatient(patient));
        }

        public async Task LoadPatientsAsync()
        {
            await ExecuteAsync(async () =>
            {
                _allPatients = await _dataService.GetAllPatientsAsync();
                ApplySorting();
            });
        }

        private void ApplySorting()
        {
            var sorted = SelectedSortOption switch
            {
                "Name (A-Z)" => _allPatients.OrderBy(p => p.Name).ToList(),
                "Name (Z-A)" => _allPatients.OrderByDescending(p => p.Name).ToList(),
                "Age (Youngest)" => _allPatients.OrderBy(p => p.Age).ToList(),
                "Age (Oldest)" => _allPatients.OrderByDescending(p => p.Age).ToList(),
                _ => _allPatients.OrderBy(p => p.Name).ToList()
            };

            Patients.Clear();
            foreach (var patient in sorted)
            {
                Patients.Add(patient);
            }
        }

        private async Task NavigateToAddPatient()
        {
            await Shell.Current.GoToAsync(nameof(PatientDetailPage));
        }

        private async void NavigateToPatientDetail(Patient patient)
        {
            await Shell.Current.GoToAsync($"{nameof(PatientDetailPage)}?PatientId={patient.Id}");
        }

        private async Task EditPatient(Patient patient)
        {
            if (patient != null)
            {
                await Shell.Current.GoToAsync($"{nameof(PatientDetailPage)}?PatientId={patient.Id}");
            }
        }

        private async Task DeletePatient(Patient patient)
        {
            if (patient == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Delete Patient",
                $"Delete {patient.Name}? This will also remove their appointments and medical records.",
                "Delete", "Cancel");

            if (confirm)
            {
                await _dataService.DeletePatientAsync(patient.Id);
                _allPatients.Remove(patient);
                Patients.Remove(patient);
            }
        }
    }
}

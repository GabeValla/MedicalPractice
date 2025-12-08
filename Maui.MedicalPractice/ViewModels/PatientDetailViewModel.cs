using System.Collections.ObjectModel;
using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;

namespace Maui.MedicalPractice.ViewModels
{
    [QueryProperty(nameof(PatientId), "PatientId")]
    public class PatientDetailViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private int _patientId;
        private string _name = string.Empty;
        private string _address = string.Empty;
        private DateTime _birthDate = DateTime.Today.AddYears(-30);
        private string _selectedRace = string.Empty;
        private string _selectedGender = string.Empty;
        private bool _isNewPatient = true;
        private MedicalNote? _selectedNote;

        // Form fields
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
            set => SetProperty(ref _birthDate, value);
        }

        public string SelectedRace
        {
            get => _selectedRace;
            set => SetProperty(ref _selectedRace, value);
        }

        public string SelectedGender
        {
            get => _selectedGender;
            set => SetProperty(ref _selectedGender, value);
        }

        public bool IsNewPatient
        {
            get => _isNewPatient;
            set
            {
                if (SetProperty(ref _isNewPatient, value))
                    OnPropertyChanged(nameof(IsExistingPatient));
            }
        }

        public bool IsExistingPatient => !IsNewPatient;

        public int PatientId
        {
            get => _patientId;
            set
            {
                _patientId = value;
                if (value > 0)
                {
                    IsNewPatient = false;
                    LoadPatientAsync(value);
                }
            }
        }

        public MedicalNote? SelectedNote
        {
            get => _selectedNote;
            set
            {
                if (SetProperty(ref _selectedNote, value) && value != null)
                {
                    ViewNoteDetails(value);
                    SelectedNote = null;
                }
            }
        }

        // Picker options
        public List<string> RaceOptions { get; } = new()
        {
            "American Indian or Alaska Native",
            "Asian",
            "Black or African American",
            "Hispanic or Latino",
            "Native Hawaiian or Other Pacific Islander",
            "White",
            "Two or More Races",
            "Other",
            "Prefer not to say"
        };

        public List<string> GenderOptions { get; } = new()
        {
            "Male",
            "Female",
            "Non-binary",
            "Other",
            "Prefer not to say"
        };

        public ObservableCollection<MedicalNote> MedicalNotes { get; } = new();
        public ObservableCollection<Physician> Physicians { get; } = new();

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddNoteCommand { get; }
        public ICommand LoadDataCommand { get; }

        public PatientDetailViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "New Patient";

            SaveCommand = new Command(async () => await SavePatientAsync());
            DeleteCommand = new Command(async () => await DeletePatientAsync());
            AddNoteCommand = new Command(async () => await AddMedicalNoteAsync());
            LoadDataCommand = new Command(async () => await LoadPhysiciansAsync());
        }

        private async void LoadPatientAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                var patient = await _dataService.GetPatientByIdAsync(id);
                if (patient != null)
                {
                    Name = patient.Name;
                    Address = patient.Address;
                    BirthDate = patient.BirthDate;
                    SelectedRace = patient.Race;
                    SelectedGender = patient.Gender;
                    Title = patient.Name;

                    // Load medical notes
                    await LoadMedicalNotesAsync(id);
                }
            });
        }

        private async Task LoadMedicalNotesAsync(int patientId)
        {
            var notes = await _dataService.GetMedicalNotesByPatientAsync(patientId);
            MedicalNotes.Clear();
            foreach (var note in notes)
            {
                MedicalNotes.Add(note);
            }
        }

        private async Task LoadPhysiciansAsync()
        {
            var physicians = await _dataService.GetAllPhysiciansAsync();
            Physicians.Clear();
            foreach (var physician in physicians)
            {
                Physicians.Add(physician);
            }
        }

        private async Task SavePatientAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Patient name is required.", "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                var patient = new Patient
                {
                    Id = _patientId,
                    Name = Name,
                    Address = Address,
                    BirthDate = BirthDate,
                    Race = SelectedRace,
                    Gender = SelectedGender
                };

                if (IsNewPatient)
                {
                    await _dataService.AddPatientAsync(patient);
                    await Shell.Current.DisplayAlert("Success", "Patient added successfully.", "OK");
                }
                else
                {
                    await _dataService.UpdatePatientAsync(patient);
                    await Shell.Current.DisplayAlert("Success", "Patient updated successfully.", "OK");
                }

                await Shell.Current.GoToAsync("..");
            });
        }

        private async Task DeletePatientAsync()
        {
            if (IsNewPatient) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete {Name}? This will also delete all associated medical notes and appointments.",
                "Delete",
                "Cancel");

            if (!confirm) return;

            await ExecuteAsync(async () =>
            {
                await _dataService.DeletePatientAsync(_patientId);
                await Shell.Current.DisplayAlert("Success", "Patient deleted successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            });
        }

        private async Task AddMedicalNoteAsync()
        {
            if (IsNewPatient)
            {
                await Shell.Current.DisplayAlert("Info", "Please save the patient first before adding medical notes.", "OK");
                return;
            }

            // Show a popup to add a new medical note
            await LoadPhysiciansAsync();

            string diagnosis = await Shell.Current.DisplayPromptAsync("New Medical Note", "Enter diagnosis:");
            if (string.IsNullOrWhiteSpace(diagnosis)) return;

            string prescription = await Shell.Current.DisplayPromptAsync("New Medical Note", "Enter prescription (optional):") ?? "";
            string notes = await Shell.Current.DisplayPromptAsync("New Medical Note", "Enter additional notes (optional):") ?? "";

            // Select physician
            var physicianNames = Physicians.Select(p => p.DisplayInfo).ToArray();
            string selectedPhysician = await Shell.Current.DisplayActionSheet("Select Physician", "Cancel", null, physicianNames);
            
            if (selectedPhysician == "Cancel" || string.IsNullOrEmpty(selectedPhysician)) return;

            var physician = Physicians.FirstOrDefault(p => p.DisplayInfo == selectedPhysician);

            var note = new MedicalNote
            {
                PatientId = _patientId,
                PhysicianId = physician?.Id,
                DateCreated = DateTime.Now,
                Diagnosis = diagnosis,
                Prescription = prescription,
                Notes = notes
            };

            await _dataService.AddMedicalNoteAsync(note);
            await LoadMedicalNotesAsync(_patientId);
        }

        private async void ViewNoteDetails(MedicalNote note)
        {
            var message = $"Date: {note.DateCreated:MMM dd, yyyy}\n" +
                         $"Physician: {note.PhysicianDisplay}\n\n" +
                         $"Diagnosis:\n{note.Diagnosis}\n\n" +
                         $"Prescription:\n{(string.IsNullOrEmpty(note.Prescription) ? "None" : note.Prescription)}\n\n" +
                         $"Notes:\n{(string.IsNullOrEmpty(note.Notes) ? "None" : note.Notes)}";

            await Shell.Current.DisplayAlert("Medical Note Details", message, "OK");
        }
    }
}


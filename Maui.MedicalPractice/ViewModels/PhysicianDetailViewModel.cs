using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;

namespace Maui.MedicalPractice.ViewModels
{
    [QueryProperty(nameof(PhysicianId), "PhysicianId")]
    public class PhysicianDetailViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private int _physicianId;
        private string _name = string.Empty;
        private string _licenseNumber = string.Empty;
        private DateTime _graduationDate = DateTime.Today.AddYears(-10);
        private string _selectedSpecialization = string.Empty;
        private bool _isNewPhysician = true;

        // Form fields
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

        public string SelectedSpecialization
        {
            get => _selectedSpecialization;
            set => SetProperty(ref _selectedSpecialization, value);
        }

        public bool IsNewPhysician
        {
            get => _isNewPhysician;
            set
            {
                if (SetProperty(ref _isNewPhysician, value))
                    OnPropertyChanged(nameof(IsExistingPhysician));
            }
        }

        public bool IsExistingPhysician => !IsNewPhysician;

        public int PhysicianId
        {
            get => _physicianId;
            set
            {
                _physicianId = value;
                if (value > 0)
                {
                    IsNewPhysician = false;
                    LoadPhysicianAsync(value);
                }
            }
        }

        // Specialization options
        public List<string> SpecializationOptions { get; } = new()
        {
            "Allergy and Immunology",
            "Anesthesiology",
            "Cardiology",
            "Dermatology",
            "Emergency Medicine",
            "Endocrinology",
            "Family Medicine",
            "Gastroenterology",
            "General Surgery",
            "Geriatric Medicine",
            "Hematology",
            "Infectious Disease",
            "Internal Medicine",
            "Nephrology",
            "Neurology",
            "Obstetrics and Gynecology",
            "Oncology",
            "Ophthalmology",
            "Orthopedics",
            "Otolaryngology (ENT)",
            "Pathology",
            "Pediatrics",
            "Physical Medicine and Rehabilitation",
            "Plastic Surgery",
            "Psychiatry",
            "Pulmonology",
            "Radiology",
            "Rheumatology",
            "Urology"
        };

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public PhysicianDetailViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "New Physician";

            SaveCommand = new Command(async () => await SavePhysicianAsync());
            DeleteCommand = new Command(async () => await DeletePhysicianAsync());
        }

        private async void LoadPhysicianAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                var physician = await _dataService.GetPhysicianByIdAsync(id);
                if (physician != null)
                {
                    Name = physician.Name;
                    LicenseNumber = physician.LicenseNumber;
                    GraduationDate = physician.GraduationDate;
                    SelectedSpecialization = physician.Specialization;
                    Title = $"Dr. {physician.Name}";
                }
            });
        }

        private async Task SavePhysicianAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Physician name is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(LicenseNumber))
            {
                await Shell.Current.DisplayAlert("Validation Error", "License number is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedSpecialization))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Specialization is required.", "OK");
                return;
            }

            await ExecuteAsync(async () =>
            {
                var physician = new Physician
                {
                    Id = _physicianId,
                    Name = Name,
                    LicenseNumber = LicenseNumber,
                    GraduationDate = GraduationDate,
                    Specialization = SelectedSpecialization
                };

                if (IsNewPhysician)
                {
                    await _dataService.AddPhysicianAsync(physician);
                    await Shell.Current.DisplayAlert("Success", "Physician added successfully.", "OK");
                }
                else
                {
                    await _dataService.UpdatePhysicianAsync(physician);
                    await Shell.Current.DisplayAlert("Success", "Physician updated successfully.", "OK");
                }

                await Shell.Current.GoToAsync("..");
            });
        }

        private async Task DeletePhysicianAsync()
        {
            if (IsNewPhysician) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete Dr. {Name}? This will also cancel all associated appointments.",
                "Delete",
                "Cancel");

            if (!confirm) return;

            await ExecuteAsync(async () =>
            {
                await _dataService.DeletePhysicianAsync(_physicianId);
                await Shell.Current.DisplayAlert("Success", "Physician deleted successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            });
        }
    }
}


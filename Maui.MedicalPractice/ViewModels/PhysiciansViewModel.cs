using System.Collections.ObjectModel;
using System.Windows.Input;
using Maui.MedicalPractice.Models;
using Maui.MedicalPractice.Services;
using Maui.MedicalPractice.Views;

namespace Maui.MedicalPractice.ViewModels
{
    public class PhysiciansViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private Physician? _selectedPhysician;
        private string _selectedSortOption = "Name (A-Z)";
        private List<Physician> _allPhysicians = new();

        public ObservableCollection<Physician> Physicians { get; } = new();

        public Physician? SelectedPhysician
        {
            get => _selectedPhysician;
            set
            {
                if (SetProperty(ref _selectedPhysician, value) && value != null)
                {
                    NavigateToPhysicianDetail(value);
                    SelectedPhysician = null;
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
            "Experience (Most)",
            "Experience (Least)",
            "Specialization"
        };

        public ICommand LoadPhysiciansCommand { get; }
        public ICommand AddPhysicianCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand EditPhysicianCommand { get; }
        public ICommand DeletePhysicianCommand { get; }

        public PhysiciansViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Title = "Physicians";

            LoadPhysiciansCommand = new Command(async () => await LoadPhysiciansAsync());
            AddPhysicianCommand = new Command(async () => await NavigateToAddPhysician());
            RefreshCommand = new Command(async () => await LoadPhysiciansAsync());
            EditPhysicianCommand = new Command<Physician>(async (p) => await EditPhysician(p));
            DeletePhysicianCommand = new Command<Physician>(async (p) => await DeletePhysician(p));
        }

        public async Task LoadPhysiciansAsync()
        {
            await ExecuteAsync(async () =>
            {
                _allPhysicians = await _dataService.GetAllPhysiciansAsync();
                ApplySorting();
            });
        }

        private void ApplySorting()
        {
            var sorted = SelectedSortOption switch
            {
                "Name (A-Z)" => _allPhysicians.OrderBy(p => p.Name).ToList(),
                "Name (Z-A)" => _allPhysicians.OrderByDescending(p => p.Name).ToList(),
                "Experience (Most)" => _allPhysicians.OrderByDescending(p => p.YearsOfExperience).ToList(),
                "Experience (Least)" => _allPhysicians.OrderBy(p => p.YearsOfExperience).ToList(),
                "Specialization" => _allPhysicians.OrderBy(p => p.Specialization).ToList(),
                _ => _allPhysicians.OrderBy(p => p.Name).ToList()
            };

            Physicians.Clear();
            foreach (var physician in sorted)
            {
                Physicians.Add(physician);
            }
        }

        private async Task NavigateToAddPhysician()
        {
            await Shell.Current.GoToAsync(nameof(PhysicianDetailPage));
        }

        private async void NavigateToPhysicianDetail(Physician physician)
        {
            await Shell.Current.GoToAsync($"{nameof(PhysicianDetailPage)}?PhysicianId={physician.Id}");
        }

        private async Task EditPhysician(Physician physician)
        {
            if (physician != null)
            {
                await Shell.Current.GoToAsync($"{nameof(PhysicianDetailPage)}?PhysicianId={physician.Id}");
            }
        }

        private async Task DeletePhysician(Physician physician)
        {
            if (physician == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Delete Physician",
                $"Delete Dr. {physician.Name}? This will also cancel their appointments.",
                "Delete", "Cancel");

            if (confirm)
            {
                await _dataService.DeletePhysicianAsync(physician.Id);
                _allPhysicians.Remove(physician);
                Physicians.Remove(physician);
            }
        }
    }
}

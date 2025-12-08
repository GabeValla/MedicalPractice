using Maui.MedicalPractice.ViewModels;

namespace Maui.MedicalPractice.Views;

public partial class PatientDetailPage : ContentPage
{
    public PatientDetailPage(PatientDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        // Set maximum date for birth date picker to today
        BirthDatePicker.MaximumDate = DateTime.Today;
    }
}

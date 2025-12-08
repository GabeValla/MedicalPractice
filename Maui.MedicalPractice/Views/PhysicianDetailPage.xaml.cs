using Maui.MedicalPractice.ViewModels;

namespace Maui.MedicalPractice.Views;

public partial class PhysicianDetailPage : ContentPage
{
    public PhysicianDetailPage(PhysicianDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        // Set maximum date for graduation date picker to today
        GraduationDatePicker.MaximumDate = DateTime.Today;
    }
}

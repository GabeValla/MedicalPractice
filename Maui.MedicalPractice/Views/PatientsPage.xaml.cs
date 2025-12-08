using Maui.MedicalPractice.ViewModels;

namespace Maui.MedicalPractice.Views;

public partial class PatientsPage : ContentPage
{
    private readonly PatientsViewModel _viewModel;

    public PatientsPage(PatientsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPatientsAsync();
    }
}


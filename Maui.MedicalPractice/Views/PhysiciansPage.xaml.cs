using Maui.MedicalPractice.ViewModels;

namespace Maui.MedicalPractice.Views;

public partial class PhysiciansPage : ContentPage
{
    private readonly PhysiciansViewModel _viewModel;

    public PhysiciansPage(PhysiciansViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPhysiciansAsync();
    }
}


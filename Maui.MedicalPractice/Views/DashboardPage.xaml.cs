using Maui.MedicalPractice.ViewModels;

namespace Maui.MedicalPractice.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        
        // Set today's date in the header
        TodayDateLabel.Text = DateTime.Today.ToString("dddd, MMMM dd, yyyy");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDashboardAsync();
    }
}

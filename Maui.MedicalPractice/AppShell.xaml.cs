using Maui.MedicalPractice.Views;

namespace Maui.MedicalPractice
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            // These routes enable Shell navigation using GoToAsync
            Routing.RegisterRoute(nameof(PatientDetailPage), typeof(PatientDetailPage));
            Routing.RegisterRoute(nameof(PhysicianDetailPage), typeof(PhysicianDetailPage));
            Routing.RegisterRoute(nameof(AppointmentDetailPage), typeof(AppointmentDetailPage));
        }
    }
}

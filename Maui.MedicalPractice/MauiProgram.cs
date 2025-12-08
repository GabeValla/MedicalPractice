using Maui.MedicalPractice.Converters;
using Maui.MedicalPractice.Services;
using Maui.MedicalPractice.ViewModels;
using Maui.MedicalPractice.Views;
using Microsoft.Extensions.Logging;

namespace Maui.MedicalPractice
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services (Singleton for data persistence across app lifetime)
            builder.Services.AddSingleton<IDataService, InMemoryDataService>();

            // Register ViewModels (Transient - new instance each time)
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<PatientsViewModel>();
            builder.Services.AddTransient<PatientDetailViewModel>();
            builder.Services.AddTransient<PhysiciansViewModel>();
            builder.Services.AddTransient<PhysicianDetailViewModel>();
            builder.Services.AddTransient<AppointmentsViewModel>();
            builder.Services.AddTransient<AppointmentDetailViewModel>();

            // Register Pages (Transient - new instance each time for fresh state)
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<PatientsPage>();
            builder.Services.AddTransient<PatientDetailPage>();
            builder.Services.AddTransient<PhysiciansPage>();
            builder.Services.AddTransient<PhysicianDetailPage>();
            builder.Services.AddTransient<AppointmentsPage>();
            builder.Services.AddTransient<AppointmentDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

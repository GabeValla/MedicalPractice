using Maui.MedicalPractice.Models;

namespace Maui.MedicalPractice.Services
{
    /// <summary>
    /// Interface for data service operations - enables easy testing and future database integration
    /// </summary>
    public interface IDataService
    {
        // Patient operations
        Task<List<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> AddPatientAsync(Patient patient);
        Task<Patient> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int id);

        // Physician operations
        Task<List<Physician>> GetAllPhysiciansAsync();
        Task<Physician?> GetPhysicianByIdAsync(int id);
        Task<Physician> AddPhysicianAsync(Physician physician);
        Task<Physician> UpdatePhysicianAsync(Physician physician);
        Task<bool> DeletePhysicianAsync(int id);

        // Appointment operations
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<List<Appointment>> GetAppointmentsByPhysicianAsync(int physicianId);
        Task<List<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<Appointment> AddAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);

        // Medical Note operations
        Task<List<MedicalNote>> GetMedicalNotesByPatientAsync(int patientId);
        Task<MedicalNote> AddMedicalNoteAsync(MedicalNote note);
        Task<MedicalNote> UpdateMedicalNoteAsync(MedicalNote note);
        Task<bool> DeleteMedicalNoteAsync(int id);

        // Diagnosis operations (A grade requirement)
        Task<List<Diagnosis>> GetDiagnosesByAppointmentAsync(int appointmentId);
        Task<Diagnosis> AddDiagnosisAsync(Diagnosis diagnosis);
        Task<Diagnosis> UpdateDiagnosisAsync(Diagnosis diagnosis);
        Task<bool> DeleteDiagnosisAsync(int id);

        // Treatment operations (A grade requirement)
        Task<List<Treatment>> GetTreatmentsByAppointmentAsync(int appointmentId);
        Task<Treatment> AddTreatmentAsync(Treatment treatment);
        Task<Treatment> UpdateTreatmentAsync(Treatment treatment);
        Task<bool> DeleteTreatmentAsync(int id);

        // Business logic validations
        Task<bool> IsPhysicianAvailableAsync(int physicianId, DateTime date, TimeSpan time, int? excludeAppointmentId = null);
        bool IsValidAppointmentTime(DateTime date, TimeSpan time);

        // Room availability (A grade requirement)
        Task<bool> IsRoomAvailableAsync(string room, DateTime date, TimeSpan time, int? excludeAppointmentId = null);
        List<string> GetAvailableRooms();
    }
}

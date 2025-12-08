using Maui.MedicalPractice.Models;

namespace Maui.MedicalPractice.Services
{
    /// <summary>
    /// In-memory implementation of IDataService for development and testing.
    /// </summary>
    public class InMemoryDataService : IDataService
    {
        private readonly List<Patient> _patients = new();
        private readonly List<Physician> _physicians = new();
        private readonly List<Appointment> _appointments = new();
        private readonly List<MedicalNote> _medicalNotes = new();
        private readonly List<Diagnosis> _diagnoses = new();
        private readonly List<Treatment> _treatments = new();

        private int _nextPatientId = 1;
        private int _nextPhysicianId = 1;
        private int _nextAppointmentId = 1;
        private int _nextMedicalNoteId = 1;
        private int _nextDiagnosisId = 1;
        private int _nextTreatmentId = 1;

        // Available rooms in the practice
        private readonly List<string> _rooms = new() { "101", "102", "103", "104", "105", "106" };

        public InMemoryDataService()
        {
            SeedData();
        }

        private void SeedData()
        {
            // Sample Physicians
            var physicians = new[]
            {
                new Physician { Name = "Sarah Johnson", LicenseNumber = "MD-12345", GraduationDate = new DateTime(2010, 5, 15), Specialization = "Family Medicine" },
                new Physician { Name = "Michael Chen", LicenseNumber = "MD-23456", GraduationDate = new DateTime(2008, 6, 20), Specialization = "Cardiology" },
                new Physician { Name = "Emily Rodriguez", LicenseNumber = "MD-34567", GraduationDate = new DateTime(2015, 5, 10), Specialization = "Pediatrics" },
                new Physician { Name = "James Wilson", LicenseNumber = "MD-45678", GraduationDate = new DateTime(2005, 6, 1), Specialization = "Orthopedics" }
            };

            foreach (var physician in physicians)
            {
                physician.Id = _nextPhysicianId++;
                _physicians.Add(physician);
            }

            // Sample Patients (including a minor for color coding demonstration)
            var patients = new[]
            {
                new Patient { Name = "John Smith", Address = "123 Main St, Springfield", BirthDate = new DateTime(1985, 3, 15), Race = "White", Gender = "Male" },
                new Patient { Name = "Maria Garcia", Address = "456 Oak Ave, Riverside", BirthDate = new DateTime(1990, 7, 22), Race = "Hispanic or Latino", Gender = "Female" },
                new Patient { Name = "Robert Brown", Address = "789 Pine Rd, Lakewood", BirthDate = new DateTime(1978, 11, 8), Race = "Black or African American", Gender = "Male" },
                new Patient { Name = "Emma Thompson", Address = "321 Elm St, Hillside", BirthDate = new DateTime(2015, 4, 30), Race = "Asian", Gender = "Female" }, // Minor
                new Patient { Name = "Tyler Johnson", Address = "555 Cedar Ln, Oakville", BirthDate = new DateTime(2010, 8, 12), Race = "White", Gender = "Male" } // Minor
            };

            foreach (var patient in patients)
            {
                patient.Id = _nextPatientId++;
                _patients.Add(patient);
            }

            // Sample Medical Notes
            var notes = new[]
            {
                new MedicalNote { PatientId = 1, PhysicianId = 1, DateCreated = DateTime.Today.AddDays(-30), Diagnosis = "Hypertension", Prescription = "Lisinopril 10mg daily", Notes = "Blood pressure elevated at 150/95." },
                new MedicalNote { PatientId = 1, PhysicianId = 2, DateCreated = DateTime.Today.AddDays(-15), Diagnosis = "Follow-up: Hypertension", Prescription = "Continue Lisinopril 10mg", Notes = "BP improved to 135/85." },
                new MedicalNote { PatientId = 2, PhysicianId = 1, DateCreated = DateTime.Today.AddDays(-7), Diagnosis = "Upper Respiratory Infection", Prescription = "Amoxicillin 500mg TID x 10 days", Notes = "Symptoms for 5 days." }
            };

            foreach (var note in notes)
            {
                note.Id = _nextMedicalNoteId++;
                note.Physician = _physicians.FirstOrDefault(p => p.Id == note.PhysicianId);
                _medicalNotes.Add(note);
            }

            // Sample Appointments (including today's for color coding demonstration)
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);
            while (tomorrow.DayOfWeek == DayOfWeek.Saturday || tomorrow.DayOfWeek == DayOfWeek.Sunday)
                tomorrow = tomorrow.AddDays(1);

            var appointments = new[]
            {
                new Appointment { PatientId = 1, PhysicianId = 1, AppointmentDate = today, AppointmentTime = new TimeSpan(9, 0, 0), Notes = "Follow-up for blood pressure", Status = AppointmentStatus.Scheduled, Room = "101" },
                new Appointment { PatientId = 2, PhysicianId = 1, AppointmentDate = today, AppointmentTime = new TimeSpan(10, 0, 0), Notes = "Annual checkup", Status = AppointmentStatus.Scheduled, Room = "102" },
                new Appointment { PatientId = 3, PhysicianId = 2, AppointmentDate = tomorrow, AppointmentTime = new TimeSpan(14, 0, 0), Notes = "Cardiac consultation", Status = AppointmentStatus.Scheduled, Room = "103" },
                new Appointment { PatientId = 4, PhysicianId = 3, AppointmentDate = tomorrow, AppointmentTime = new TimeSpan(11, 0, 0), Notes = "Well-child visit", Status = AppointmentStatus.Scheduled, Room = "104" },
                new Appointment { PatientId = 5, PhysicianId = 3, AppointmentDate = today, AppointmentTime = new TimeSpan(15, 0, 0), Notes = "Pediatric checkup", Status = AppointmentStatus.Scheduled, Room = "105" }
            };

            foreach (var appointment in appointments)
            {
                appointment.Id = _nextAppointmentId++;
                appointment.Patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                appointment.Physician = _physicians.FirstOrDefault(p => p.Id == appointment.PhysicianId);
                _appointments.Add(appointment);
            }

            // Sample Diagnoses
            var diagnoses = new[]
            {
                new Diagnosis { AppointmentId = 1, Code = "I10", Description = "Essential Hypertension", Notes = "Primary diagnosis" }
            };

            foreach (var diagnosis in diagnoses)
            {
                diagnosis.Id = _nextDiagnosisId++;
                _diagnoses.Add(diagnosis);
            }

            // Sample Treatments
            var treatments = new[]
            {
                new Treatment { AppointmentId = 1, Name = "Blood Pressure Check", Description = "Routine BP monitoring", Cost = 25.00m },
                new Treatment { AppointmentId = 1, Name = "ECG", Description = "Electrocardiogram", Cost = 150.00m }
            };

            foreach (var treatment in treatments)
            {
                treatment.Id = _nextTreatmentId++;
                _treatments.Add(treatment);
            }
        }

        #region Patient Operations

        public Task<List<Patient>> GetAllPatientsAsync()
        {
            return Task.FromResult(_patients.OrderBy(p => p.Name).ToList());
        }

        public Task<Patient?> GetPatientByIdAsync(int id)
        {
            return Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
        }

        public Task<Patient> AddPatientAsync(Patient patient)
        {
            patient.Id = _nextPatientId++;
            _patients.Add(patient);
            return Task.FromResult(patient);
        }

        public Task<Patient> UpdatePatientAsync(Patient patient)
        {
            var existing = _patients.FirstOrDefault(p => p.Id == patient.Id);
            if (existing != null)
            {
                existing.Name = patient.Name;
                existing.Address = patient.Address;
                existing.BirthDate = patient.BirthDate;
                existing.Race = patient.Race;
                existing.Gender = patient.Gender;
            }
            return Task.FromResult(existing ?? patient);
        }

        public Task<bool> DeletePatientAsync(int id)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient != null)
            {
                _patients.Remove(patient);
                _appointments.RemoveAll(a => a.PatientId == id);
                _medicalNotes.RemoveAll(n => n.PatientId == id);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion

        #region Physician Operations

        public Task<List<Physician>> GetAllPhysiciansAsync()
        {
            return Task.FromResult(_physicians.OrderBy(p => p.Name).ToList());
        }

        public Task<Physician?> GetPhysicianByIdAsync(int id)
        {
            return Task.FromResult(_physicians.FirstOrDefault(p => p.Id == id));
        }

        public Task<Physician> AddPhysicianAsync(Physician physician)
        {
            physician.Id = _nextPhysicianId++;
            _physicians.Add(physician);
            return Task.FromResult(physician);
        }

        public Task<Physician> UpdatePhysicianAsync(Physician physician)
        {
            var existing = _physicians.FirstOrDefault(p => p.Id == physician.Id);
            if (existing != null)
            {
                existing.Name = physician.Name;
                existing.LicenseNumber = physician.LicenseNumber;
                existing.GraduationDate = physician.GraduationDate;
                existing.Specialization = physician.Specialization;
            }
            return Task.FromResult(existing ?? physician);
        }

        public Task<bool> DeletePhysicianAsync(int id)
        {
            var physician = _physicians.FirstOrDefault(p => p.Id == id);
            if (physician != null)
            {
                _physicians.Remove(physician);
                _appointments.RemoveAll(a => a.PhysicianId == id);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion

        #region Appointment Operations

        public Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            var appointments = _appointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToList();

            PopulateAppointmentNavigation(appointments);
            return Task.FromResult(appointments);
        }

        public Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.Patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                appointment.Physician = _physicians.FirstOrDefault(p => p.Id == appointment.PhysicianId);
                appointment.Diagnoses = _diagnoses.Where(d => d.AppointmentId == id).ToList();
                appointment.Treatments = _treatments.Where(t => t.AppointmentId == id).ToList();
            }
            return Task.FromResult(appointment);
        }

        public Task<List<Appointment>> GetAppointmentsByPhysicianAsync(int physicianId)
        {
            var appointments = _appointments
                .Where(a => a.PhysicianId == physicianId)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToList();

            PopulateAppointmentNavigation(appointments);
            return Task.FromResult(appointments);
        }

        public Task<List<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = _appointments
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToList();

            PopulateAppointmentNavigation(appointments);
            return Task.FromResult(appointments);
        }

        public Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            var appointments = _appointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToList();

            PopulateAppointmentNavigation(appointments);
            return Task.FromResult(appointments);
        }

        public Task<Appointment> AddAppointmentAsync(Appointment appointment)
        {
            appointment.Id = _nextAppointmentId++;
            appointment.Patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
            appointment.Physician = _physicians.FirstOrDefault(p => p.Id == appointment.PhysicianId);
            _appointments.Add(appointment);
            return Task.FromResult(appointment);
        }

        public Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            var existing = _appointments.FirstOrDefault(a => a.Id == appointment.Id);
            if (existing != null)
            {
                existing.PatientId = appointment.PatientId;
                existing.PhysicianId = appointment.PhysicianId;
                existing.AppointmentDate = appointment.AppointmentDate;
                existing.AppointmentTime = appointment.AppointmentTime;
                existing.Notes = appointment.Notes;
                existing.Status = appointment.Status;
                existing.Room = appointment.Room;
                existing.Patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                existing.Physician = _physicians.FirstOrDefault(p => p.Id == appointment.PhysicianId);
            }
            return Task.FromResult(existing ?? appointment);
        }

        public Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
                _diagnoses.RemoveAll(d => d.AppointmentId == id);
                _treatments.RemoveAll(t => t.AppointmentId == id);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private void PopulateAppointmentNavigation(List<Appointment> appointments)
        {
            foreach (var appointment in appointments)
            {
                appointment.Patient = _patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                appointment.Physician = _physicians.FirstOrDefault(p => p.Id == appointment.PhysicianId);
                appointment.Diagnoses = _diagnoses.Where(d => d.AppointmentId == appointment.Id).ToList();
                appointment.Treatments = _treatments.Where(t => t.AppointmentId == appointment.Id).ToList();
            }
        }

        #endregion

        #region Medical Note Operations

        public Task<List<MedicalNote>> GetMedicalNotesByPatientAsync(int patientId)
        {
            var notes = _medicalNotes
                .Where(n => n.PatientId == patientId)
                .OrderByDescending(n => n.DateCreated)
                .ToList();

            foreach (var note in notes)
            {
                note.Physician = note.PhysicianId.HasValue 
                    ? _physicians.FirstOrDefault(p => p.Id == note.PhysicianId) 
                    : null;
            }

            return Task.FromResult(notes);
        }

        public Task<MedicalNote> AddMedicalNoteAsync(MedicalNote note)
        {
            note.Id = _nextMedicalNoteId++;
            note.Physician = note.PhysicianId.HasValue 
                ? _physicians.FirstOrDefault(p => p.Id == note.PhysicianId) 
                : null;
            _medicalNotes.Add(note);
            return Task.FromResult(note);
        }

        public Task<MedicalNote> UpdateMedicalNoteAsync(MedicalNote note)
        {
            var existing = _medicalNotes.FirstOrDefault(n => n.Id == note.Id);
            if (existing != null)
            {
                existing.Diagnosis = note.Diagnosis;
                existing.Prescription = note.Prescription;
                existing.Notes = note.Notes;
                existing.PhysicianId = note.PhysicianId;
                existing.Physician = note.PhysicianId.HasValue 
                    ? _physicians.FirstOrDefault(p => p.Id == note.PhysicianId) 
                    : null;
            }
            return Task.FromResult(existing ?? note);
        }

        public Task<bool> DeleteMedicalNoteAsync(int id)
        {
            var note = _medicalNotes.FirstOrDefault(n => n.Id == id);
            if (note != null)
            {
                _medicalNotes.Remove(note);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion

        #region Diagnosis Operations

        public Task<List<Diagnosis>> GetDiagnosesByAppointmentAsync(int appointmentId)
        {
            return Task.FromResult(_diagnoses
                .Where(d => d.AppointmentId == appointmentId)
                .OrderByDescending(d => d.DateCreated)
                .ToList());
        }

        public Task<Diagnosis> AddDiagnosisAsync(Diagnosis diagnosis)
        {
            diagnosis.Id = _nextDiagnosisId++;
            _diagnoses.Add(diagnosis);
            return Task.FromResult(diagnosis);
        }

        public Task<Diagnosis> UpdateDiagnosisAsync(Diagnosis diagnosis)
        {
            var existing = _diagnoses.FirstOrDefault(d => d.Id == diagnosis.Id);
            if (existing != null)
            {
                existing.Code = diagnosis.Code;
                existing.Description = diagnosis.Description;
                existing.Notes = diagnosis.Notes;
            }
            return Task.FromResult(existing ?? diagnosis);
        }

        public Task<bool> DeleteDiagnosisAsync(int id)
        {
            var diagnosis = _diagnoses.FirstOrDefault(d => d.Id == id);
            if (diagnosis != null)
            {
                _diagnoses.Remove(diagnosis);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion

        #region Treatment Operations

        public Task<List<Treatment>> GetTreatmentsByAppointmentAsync(int appointmentId)
        {
            return Task.FromResult(_treatments
                .Where(t => t.AppointmentId == appointmentId)
                .OrderByDescending(t => t.DatePerformed)
                .ToList());
        }

        public Task<Treatment> AddTreatmentAsync(Treatment treatment)
        {
            treatment.Id = _nextTreatmentId++;
            _treatments.Add(treatment);
            return Task.FromResult(treatment);
        }

        public Task<Treatment> UpdateTreatmentAsync(Treatment treatment)
        {
            var existing = _treatments.FirstOrDefault(t => t.Id == treatment.Id);
            if (existing != null)
            {
                existing.Name = treatment.Name;
                existing.Description = treatment.Description;
                existing.Cost = treatment.Cost;
                existing.Notes = treatment.Notes;
            }
            return Task.FromResult(existing ?? treatment);
        }

        public Task<bool> DeleteTreatmentAsync(int id)
        {
            var treatment = _treatments.FirstOrDefault(t => t.Id == id);
            if (treatment != null)
            {
                _treatments.Remove(treatment);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion

        #region Business Logic Validations

        public Task<bool> IsPhysicianAvailableAsync(int physicianId, DateTime date, TimeSpan time, int? excludeAppointmentId = null)
        {
            var conflictingAppointment = _appointments.FirstOrDefault(a =>
                a.PhysicianId == physicianId &&
                a.AppointmentDate.Date == date.Date &&
                a.AppointmentTime == time &&
                a.Status != AppointmentStatus.Cancelled &&
                (excludeAppointmentId == null || a.Id != excludeAppointmentId));

            return Task.FromResult(conflictingAppointment == null);
        }

        public bool IsValidAppointmentTime(DateTime date, TimeSpan time)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;

            var minTime = new TimeSpan(8, 0, 0);
            var maxTime = new TimeSpan(17, 0, 0);

            return time >= minTime && time < maxTime;
        }

        /// <summary>
        /// Checks if a room is available at the specified date and time.
        /// Prevents double-booking rooms.
        /// </summary>
        public Task<bool> IsRoomAvailableAsync(string room, DateTime date, TimeSpan time, int? excludeAppointmentId = null)
        {
            if (string.IsNullOrEmpty(room))
                return Task.FromResult(true); // No room specified is always "available"

            var conflictingAppointment = _appointments.FirstOrDefault(a =>
                a.Room == room &&
                a.AppointmentDate.Date == date.Date &&
                a.AppointmentTime == time &&
                a.Status != AppointmentStatus.Cancelled &&
                (excludeAppointmentId == null || a.Id != excludeAppointmentId));

            return Task.FromResult(conflictingAppointment == null);
        }

        public List<string> GetAvailableRooms()
        {
            return _rooms.ToList();
        }

        #endregion
    }
}

using System;

namespace CLI.MedicalPractice
{
    public class Appointment
    {
        public Patient Patient { get; set; }
        public Physician Physician { get; set; }
        public DateTime Date { get; set; }

        public Appointment(Patient p, Physician ph, DateTime date)
        {
            Patient = p;
            Physician = ph;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Date:g} | {Patient.Name} with Dr. {Physician.Name} ({Physician.Specialization})";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
//Gabriel Valladares-Ruiz Medcial Practice
namespace CLI.MedicalPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Patient> patients = new();
            List<Physician> physicians = new();
            List<Appointment> appointments = new();
            //While loop to keep the program running until the user decides to quit
            bool cont = true;
            while (cont)
            {
                Console.WriteLine("\n--- Medical Practice Management ---");
                Console.WriteLine("1. Create Patient");
                Console.WriteLine("2. List Patients");
                Console.WriteLine("3. Create Physician");
                Console.WriteLine("4. List Physicians");
                Console.WriteLine("5. Create Appointment");
                Console.WriteLine("6. List Appointments");
                Console.WriteLine("Q. Quit");
                Console.Write("Choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        patients.Add(CreatePatient());
                        break;
                    case "2":
                        foreach (var p in patients) Console.WriteLine(p);
                        break;
                    case "3":
                        physicians.Add(CreatePhysician());
                        break;
                    case "4":
                        foreach (var ph in physicians) Console.WriteLine(ph);
                        break;
                    case "5":
                        CreateAppointment(patients, physicians, appointments);
                        break;
                    case "6":
                        foreach (var a in appointments) Console.WriteLine(a);
                        break;
                    case "Q":
                    case "q":
                        cont = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        // Helpers Functions for my program will help it delegate 
        //Create patients from the class
        static Patient CreatePatient()
        {
            Patient p = new();
            Console.Write("Name: ");
            p.Name = Console.ReadLine() ?? "";
            Console.Write("Address: ");
            p.Address = Console.ReadLine() ?? "";
            Console.Write("Birthdate (yyyy-mm-dd): ");
            p.BirthDate = DateTime.Parse(Console.ReadLine() ?? "2000-01-01");
            Console.Write("Race: ");
            p.Race = Console.ReadLine() ?? "";
            Console.Write("Gender: ");
            p.Gender = Console.ReadLine() ?? "";

            Console.Write("Enter initial diagnosis (or blank to skip): ");
            string? diag = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(diag)) p.Diagnoses.Add(diag);

            Console.Write("Enter initial prescription (or blank to skip): ");
            string? rx = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(rx)) p.Prescriptions.Add(rx);

            Console.WriteLine("Patient created!");
            return p;
        }

        static Physician CreatePhysician()
        {
            Physician ph = new();
            Console.Write("Name: ");
            ph.Name = Console.ReadLine() ?? "";
            Console.Write("License Number: ");
            ph.LicenseNumber = Console.ReadLine() ?? "";
            Console.Write("Graduation Date (yyyy-mm-dd): ");
            ph.GraduationDate = DateTime.Parse(Console.ReadLine() ?? "2000-01-01");
            Console.Write("Specialization: ");
            ph.Specialization = Console.ReadLine() ?? "";
            Console.WriteLine("Physician created!");
            return ph;
        }

        //Create appointments from the class
        static void CreateAppointment(List<Patient> patients, List<Physician> physicians, List<Appointment> appointments)
        {
            if (patients.Count == 0 || physicians.Count == 0)
            {
                Console.WriteLine("Must have at least one patient and physician.");
                return;
            }

            Console.WriteLine("Select Patient:");
            for (int i = 0; i < patients.Count; i++)
                Console.WriteLine($"{i + 1}. {patients[i].Name}");
            int pIndex = int.Parse(Console.ReadLine() ?? "1") - 1;

            Console.WriteLine("Select Physician:");
            for (int i = 0; i < physicians.Count; i++)
                Console.WriteLine($"{i + 1}. {physicians[i].Name} ({physicians[i].Specialization})");
            int phIndex = int.Parse(Console.ReadLine() ?? "1") - 1;

            Console.Write("Enter appointment date and time (yyyy-mm-dd HH:mm): ");
            DateTime date = DateTime.Parse(Console.ReadLine() ?? "");

            // This is the logic to allow and mkae sure the appointment are within the time frame
            //To establish this I was able to use a if
            if (date.Hour < 8 || date.Hour >= 17 || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                Console.WriteLine("Appointments must be between 8am-5pm, Mon-Fri.");
                return;
            }

            // This will allow no double booking for th eporgram
            if (appointments.Any(a => a.Physician == physicians[phIndex] && a.Date == date))
            {
                Console.WriteLine("Physician is already booked at that time.");
                return;
            }

            appointments.Add(new Appointment(patients[pIndex], physicians[phIndex], date));
            Console.WriteLine("Appointment scheduled!");
        }
    }
}

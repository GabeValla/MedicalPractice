using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.MedicalPractice
{
    public class Patient
    {
        //getting and setting the properties of the patient class
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Race { get; set; } = "";
        public string Gender { get; set; } = "";
        //Lists to hold the diagnoses and prescriptions
        public List<string> Diagnoses { get; set; } = new();
        public List<string> Prescriptions { get; set; } = new();

        public override string ToString()
        {
            return $"{Name}, {Gender}, DOB: {BirthDate:d}, Race: {Race}, Address: {Address}";
        }
    }
}

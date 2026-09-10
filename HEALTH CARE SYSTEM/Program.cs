using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // ==========================================================
    // (a) Generic Repository
    // ==========================================================
    public class Repository<T>
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var match = items.FirstOrDefault(predicate);

            if (match == null)
            {
                return false;
            }

            return items.Remove(match);
        }
    }

    // ==========================================================
    // (b) Patient Class
    // ==========================================================
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"ID: {Id} | Name: {Name} | Age: {Age} | Gender: {Gender}";
        }
    }

    // ==========================================================
    // (c) Prescription Class
    // ==========================================================
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(
            int id,
            int patientId,
            string medicationName,
            DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription ID: {Id} | Medication: {MedicationName} | Date Issued: {DateIssued:dd/MM/yyyy}";
        }
    }

    // ==========================================================
    // (g) Health System Application
    // ==========================================================
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo =
            new Repository<Patient>();

        private readonly Repository<Prescription> _prescriptionRepo =
            new Repository<Prescription>();

        private readonly Dictionary<int, List<Prescription>> _prescriptionMap =
            new Dictionary<int, List<Prescription>>();

        // ======================================================
        // Seed sample data
        // ======================================================
        public void SeedData()
        {
            _patientRepo.Add(
                new Patient(1, "Ama Owusu", 29, "Female"));

            _patientRepo.Add(
                new Patient(2, "Kwame Mensah", 45, "Male"));

            _patientRepo.Add(
                new Patient(3, "Efua Boateng", 34, "Female"));

            _prescriptionRepo.Add(
                new Prescription(
                    101, 1, "Amoxicillin",
                    DateTime.Now.AddDays(-10)));

            _prescriptionRepo.Add(
                new Prescription(
                    102, 1, "Paracetamol",
                    DateTime.Now.AddDays(-3)));

            _prescriptionRepo.Add(
                new Prescription(
                    103, 2, "Metformin",
                    DateTime.Now.AddDays(-20)));

            _prescriptionRepo.Add(
                new Prescription(
                    104, 3, "Lisinopril",
                    DateTime.Now.AddDays(-7)));

            _prescriptionRepo.Add(
                new Prescription(
                    105, 2, "Atorvastatin",
                    DateTime.Now.AddDays(-1)));
        }

        // ======================================================
        // (e) Build prescription dictionary
        // ======================================================
        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] =
                        new List<Prescription>();
                }

                _prescriptionMap[prescription.PatientId]
                    .Add(prescription);
            }
        }

        // ======================================================
        // (f) Get prescriptions by Patient ID
        // ======================================================
        public List<Prescription> GetPrescriptionsByPatientId(
            int patientId)
        {
            if (_prescriptionMap.TryGetValue(
                patientId,
                out var prescriptions))
            {
                return prescriptions;
            }

            return new List<Prescription>();
        }

        // ======================================================
        // Print all patients
        // ======================================================
        public void PrintAllPatients()
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("                 ALL PATIENTS");
            Console.WriteLine("==============================================");

            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        // ======================================================
        // Print prescriptions for a specific patient
        // ======================================================
        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(
                p => p.Id == patientId);

            if (patient == null)
            {
                Console.WriteLine(
                    $"\nNo patient found with ID {patientId}.");

                return;
            }

            var prescriptions =
                GetPrescriptionsByPatientId(patientId);

            Console.WriteLine(
                $"\n==============================================");

            Console.WriteLine(
                $" PRESCRIPTIONS FOR {patient.Name.ToUpper()}");

            Console.WriteLine(
                $"==============================================");

            if (prescriptions.Count == 0)
            {
                Console.WriteLine(
                    "No prescriptions on record.");

                return;
            }

            foreach (var prescription in prescriptions)
            {
                Console.WriteLine(prescription);
            }
        }

        // ======================================================
        // Interactive application
        // ======================================================
        public void Run()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("==============================================");
                Console.WriteLine("           HEALTHCARE SYSTEM");
                Console.WriteLine("==============================================");

                Console.WriteLine("\n1. View all patients");
                Console.WriteLine("2. View prescriptions for a patient");
                Console.WriteLine("3. View all prescriptions");
                Console.WriteLine("4. Quit application");

                Console.Write("\nEnter your choice: ");

                string choice =
                    Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":

                        Console.Clear();

                        PrintAllPatients();

                        Pause();

                        break;

                    case "2":

                        Console.Clear();

                        PrintAllPatients();

                        Console.Write(
                            "\nEnter Patient ID: ");

                        string input =
                            Console.ReadLine() ?? "";

                        if (int.TryParse(
                            input,
                            out int patientId))
                        {
                            PrintPrescriptionsForPatient(
                                patientId);
                        }
                        else
                        {
                            Console.WriteLine(
                                "\nInvalid Patient ID. " +
                                "Please enter a number.");
                        }

                        Pause();

                        break;

                    case "3":

                        Console.Clear();

                        Console.WriteLine(
                            "==============================================");

                        Console.WriteLine(
                            "             ALL PRESCRIPTIONS");

                        Console.WriteLine(
                            "==============================================");

                        foreach (
                            var prescription
                            in _prescriptionRepo.GetAll())
                        {
                            Console.WriteLine(prescription);
                        }

                        Pause();

                        break;

                    case "4":

                        Console.WriteLine(
                            "\nThank you for using the Healthcare System.");

                        Console.WriteLine(
                            "Goodbye!");

                        return;

                    default:

                        Console.WriteLine(
                            "\nInvalid choice.");

                        Console.WriteLine(
                            "Please select 1, 2, 3, or 4.");

                        Pause();

                        break;
                }
            }
        }

        // ======================================================
        // Pause
        // ======================================================
        private void Pause()
        {
            Console.WriteLine(
                "\nPress Enter to continue...");

            Console.ReadLine();
        }
    }

    // ==========================================================
    // Main
    // ==========================================================
    public class Program
    {
        public static void Main(string[] args)
        {
            HealthSystemApp app =
                new HealthSystemApp();

            app.SeedData();

            app.BuildPrescriptionMap();

            app.Run();
        }
    }
}
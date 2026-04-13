using Hospital_System;
using System;
using System.Collections.Generic;
using System.Text;
internal class Nurse : Employee // 1. Changed to singular 'Nurse'
{

    private string licenseNumber = string.Empty;                                                
    private string speciality = string.Empty;// can be used to specify the nurse's area of expertise (e.g., pediatric nurse, surgical nurse, etc.)
    private string degree = string.Empty;// can be used to specify the nurse's educational background (e.g., Bachelor of Science in Nursing, Master of Science in Nursing, etc.)
    private string assignedWard = string.Empty;// can be used to specify the ward or department the nurse is assigned to (e.g., emergency, intensive care unit, etc.)
    private bool isOnCall;// can be used to indicate whether the nurse is currently on call or not
    private int currentPatientLoad; // to track number of patients to manage workload
    private bool canAdministerMedication; // can be used to indicate whether the nurse is authorized to administer medication to patients
    private bool isHeadNurse;// can be used to indicate whether the nurse holds a leadership position

    public string LicenseNumber
    {
        get => licenseNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("License number cannot be empty.");
            licenseNumber = value;
        }
    }

    public string Speciality
    {
        get => speciality;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Speciality cannot be empty.");
            speciality = value;
        }
    }

    public string Degree
    {
        get => degree;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Degree cannot be empty.");
            degree = value;
        }
    }

    public string AssignedWard
    {
        get => assignedWard;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Assigned ward cannot be empty.");
            assignedWard = value;
        }
    }

    public bool IsOnCall { get; set; }
    public bool CanAdministerMedication { get; set; }
    public bool IsHeadNurse { get; set; }

    public int CurrentPatientLoad
    {
        get => currentPatientLoad;
        set
        {
            if (value < 0) throw new ArgumentException("Current patient load cannot be negative.");
            currentPatientLoad = value;
        }
    }
    public Nurse(string name, int age, GenderType gender, string nationalId, string phoneNumber, string email,
                 string address, decimal salary, double arrivalTime, double departureTime, double experienceYears,
                 string licenseNumber, string speciality, string degree, string assignedWard,
                 bool isOnCall, int currentPatientLoad, bool canAdministerMedication, bool isHeadNurse)
        : base(name, age, gender, nationalId, phoneNumber, email, address, salary, arrivalTime, departureTime, experienceYears)
    {
        LicenseNumber = licenseNumber;
        Speciality = speciality;
        Degree = degree;
        AssignedWard = assignedWard;
        IsOnCall = isOnCall;
        CurrentPatientLoad = currentPatientLoad; 
        CanAdministerMedication = canAdministerMedication;
        IsHeadNurse = isHeadNurse;
    }
}
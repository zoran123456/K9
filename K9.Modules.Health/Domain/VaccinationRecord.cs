using K9.SharedKernel.Domain;

namespace K9.Modules.Health.Domain;

public class VaccinationRecord : Entity
{
    public string VaccineName { get; private set; } = string.Empty;
    public DateTime DateAdministered { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string? VetClinicName { get; private set; }

    private VaccinationRecord() { }

    internal VaccinationRecord(Guid id, string vaccineName, DateTime dateAdministered, int validityInMonths, string? vetClinicName)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(vaccineName))
            throw new ArgumentException("Vaccine name cannot be empty.");

        VaccineName = vaccineName;
        DateAdministered = dateAdministered;
        ExpiresAt = dateAdministered.AddMonths(validityInMonths);
        VetClinicName = vetClinicName;
    }
}
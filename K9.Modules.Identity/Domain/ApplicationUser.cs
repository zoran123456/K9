using K9.SharedKernel.Domain;

namespace K9.Modules.Identity.Domain;

public class ApplicationUser : Entity
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string GoogleSubjectId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ApplicationUser() { }

    public ApplicationUser(Guid id, string email, string firstName, string lastName, string googleSubjectId)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(googleSubjectId)) throw new ArgumentNullException(nameof(googleSubjectId));

        Email = email;
        FirstName = firstName;
        LastName = lastName;
        GoogleSubjectId = googleSubjectId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
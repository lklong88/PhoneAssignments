namespace PhoneAssignments.Models;

public record TeamMember(int Id, string FirstName, string LastName)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record PhoneNumber(int Id, string Number);

public record Assignment(int TeamMemberId, int PhoneNumberId);

public class SeedData
{
    public List<TeamMember> TeamMembers { get; set; } = new();
    public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    public List<Assignment> Assignments { get; set; } = new();
}

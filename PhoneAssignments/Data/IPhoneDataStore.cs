using PhoneAssignments.Models;

namespace PhoneAssignments.Data;

/// <summary>
/// Persistence boundary. Swap the implementation (e.g. EF Core / SQL Server)
/// without touching the service, controller or views.
/// </summary>
public interface IPhoneDataStore
{
    IReadOnlyList<TeamMember> GetTeamMembers();
    IReadOnlyList<PhoneNumber> GetPhoneNumbers();
    IReadOnlyList<Assignment> GetAssignments();
    void AddAssignment(Assignment assignment);
    bool RemoveAssignment(int teamMemberId);
}

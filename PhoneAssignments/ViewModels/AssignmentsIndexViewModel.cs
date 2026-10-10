using PhoneAssignments.Models;
using PhoneAssignments.Services;

namespace PhoneAssignments.ViewModels;

public class AssignmentsIndexViewModel
{
    public IReadOnlyList<TeamMemberRow> Rows { get; init; } = Array.Empty<TeamMemberRow>();
    public IReadOnlyList<PhoneNumber> AvailableNumbers { get; init; } = Array.Empty<PhoneNumber>();
}

using PhoneAssignments.Data;
using PhoneAssignments.Models;

namespace PhoneAssignments.Services;

public record OperationResult(bool Success, string Message);

public record TeamMemberRow(TeamMember Member, PhoneNumber? Phone);

public interface IPhoneAssignmentService
{
    IReadOnlyList<TeamMemberRow> GetTeamMemberRows();
    IReadOnlyList<PhoneNumber> GetAvailableNumbers();
    OperationResult Assign(int teamMemberId, int phoneNumberId);
    OperationResult Unassign(int teamMemberId);
}

/// <summary>All business rules live here, independent of the UI.</summary>
public class PhoneAssignmentService : IPhoneAssignmentService
{
    private readonly IPhoneDataStore _store;
    private readonly ILogger<PhoneAssignmentService> _logger;
    private readonly object _writeLock = new(); // makes check-then-write atomic

    public PhoneAssignmentService(IPhoneDataStore store, ILogger<PhoneAssignmentService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public IReadOnlyList<TeamMemberRow> GetTeamMemberRows()
    {
        var numbers = _store.GetPhoneNumbers().ToDictionary(n => n.Id);
        var assignments = _store.GetAssignments().ToDictionary(a => a.TeamMemberId);

        return _store.GetTeamMembers()
            .OrderBy(m => m.LastName).ThenBy(m => m.FirstName)
            .Select(m => new TeamMemberRow(m,
                assignments.TryGetValue(m.Id, out var a) && numbers.TryGetValue(a.PhoneNumberId, out var n) ? n : null))
            .ToList();
    }

    public IReadOnlyList<PhoneNumber> GetAvailableNumbers()
    {
        var taken = _store.GetAssignments().Select(a => a.PhoneNumberId).ToHashSet();
        return _store.GetPhoneNumbers().Where(n => !taken.Contains(n.Id)).OrderBy(n => n.Number).ToList();
    }

    public OperationResult Assign(int teamMemberId, int phoneNumberId)
    {
        lock (_writeLock)
        {
            var member = _store.GetTeamMembers().FirstOrDefault(m => m.Id == teamMemberId);
            if (member is null) return new(false, "That team member does not exist.");

            var phone = _store.GetPhoneNumbers().FirstOrDefault(n => n.Id == phoneNumberId);
            if (phone is null) return new(false, "That phone number does not exist.");

            var assignments = _store.GetAssignments();
            if (assignments.Any(a => a.TeamMemberId == teamMemberId))
                return new(false, $"{member.FullName} already has a phone number. Remove it first to assign a different one.");

            if (assignments.Any(a => a.PhoneNumberId == phoneNumberId))
                return new(false, $"{phone.Number} is already assigned to another team member.");

            _store.AddAssignment(new Assignment(teamMemberId, phoneNumberId));
            _logger.LogInformation("Assigned {Number} to member {MemberId}", phone.Number, teamMemberId);
            return new(true, $"Assigned {phone.Number} to {member.FullName}.");
        }
    }

    public OperationResult Unassign(int teamMemberId)
    {
        lock (_writeLock)
        {
            var member = _store.GetTeamMembers().FirstOrDefault(m => m.Id == teamMemberId);
            if (member is null) return new(false, "That team member does not exist.");

            if (!_store.RemoveAssignment(teamMemberId))
                return new(false, $"{member.FullName} does not have a phone number assigned.");

            _logger.LogInformation("Removed phone assignment from member {MemberId}", teamMemberId);
            return new(true, $"Removed the phone number from {member.FullName}.");
        }
    }
}

using System.Text.Json;
using PhoneAssignments.Models;

namespace PhoneAssignments.Data;

/// <summary>Singleton store seeded from seed.json. Thread-safe via a lock.</summary>
public class InMemoryPhoneDataStore : IPhoneDataStore
{
    private readonly object _lock = new();
    private readonly List<TeamMember> _members;
    private readonly List<PhoneNumber> _numbers;
    private readonly List<Assignment> _assignments;

    public InMemoryPhoneDataStore(SeedData seed)
    {
        _members = seed.TeamMembers.ToList();
        _numbers = seed.PhoneNumbers.ToList();
        _assignments = seed.Assignments.ToList();
    }

    public static SeedData LoadSeed(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SeedData>(json,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? throw new InvalidOperationException("Seed data could not be read.");
    }

    public IReadOnlyList<TeamMember> GetTeamMembers() { lock (_lock) return _members.ToList(); }
    public IReadOnlyList<PhoneNumber> GetPhoneNumbers() { lock (_lock) return _numbers.ToList(); }
    public IReadOnlyList<Assignment> GetAssignments() { lock (_lock) return _assignments.ToList(); }

    public void AddAssignment(Assignment assignment)
    {
        lock (_lock) _assignments.Add(assignment);
    }

    public bool RemoveAssignment(int teamMemberId)
    {
        lock (_lock) return _assignments.RemoveAll(a => a.TeamMemberId == teamMemberId) > 0;
    }
}

using Microsoft.Extensions.Logging.Abstractions;
using PhoneAssignments.Data;
using PhoneAssignments.Models;
using PhoneAssignments.Services;
using Xunit;

namespace PhoneAssignments.Tests;

public class PhoneAssignmentServiceTests
{
    private static PhoneAssignmentService Create() => new(
        new InMemoryPhoneDataStore(new SeedData
        {
            TeamMembers = { new(1, "John", "Smith"), new(2, "Jane", "Doe"), new(3, "Michael", "Johnson") },
            PhoneNumbers = { new(1, "714-555-1001"), new(2, "714-555-1002"), new(3, "714-555-1003"), new(4, "714-555-1004") },
            Assignments = { new(1, 1), new(2, 2) }
        }),
        NullLogger<PhoneAssignmentService>.Instance);

    [Fact]
    public void Assign_AvailableNumber_Succeeds()
    {
        var s = Create();
        Assert.True(s.Assign(3, 3).Success);
        Assert.Equal("714-555-1003", s.GetTeamMemberRows().Single(r => r.Member.Id == 3).Phone!.Number);
        Assert.DoesNotContain(s.GetAvailableNumbers(), n => n.Id == 3);
    }

    [Fact] public void Assign_NumberAlreadyTaken_Fails() => Assert.False(Create().Assign(3, 1).Success);
    [Fact] public void Assign_MemberAlreadyHasNumber_Fails() => Assert.False(Create().Assign(1, 3).Success);
    [Fact] public void Assign_UnknownMember_Fails() => Assert.False(Create().Assign(99, 3).Success);
    [Fact] public void Assign_UnknownNumber_Fails() => Assert.False(Create().Assign(3, 99).Success);

    [Fact]
    public void Unassign_FreesNumber()
    {
        var s = Create();
        Assert.True(s.Unassign(1).Success);
        Assert.Contains(s.GetAvailableNumbers(), n => n.Id == 1);
        Assert.True(s.Assign(3, 1).Success); // reassign to someone else
    }

    [Fact] public void Unassign_MemberWithoutNumber_Fails() => Assert.False(Create().Unassign(3).Success);
    [Fact] public void Unassign_UnknownMember_Fails() => Assert.False(Create().Unassign(99).Success);
}

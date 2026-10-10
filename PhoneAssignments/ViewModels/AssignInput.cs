using System.ComponentModel.DataAnnotations;

namespace PhoneAssignments.ViewModels;

public class AssignInput
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a team member.")]
    public int TeamMemberId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select a phone number.")]
    public int PhoneNumberId { get; set; }
}

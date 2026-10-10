using Microsoft.AspNetCore.Mvc;
using PhoneAssignments.Services;
using PhoneAssignments.ViewModels;

namespace PhoneAssignments.Controllers;

public class AssignmentsController : Controller
{
    private readonly IPhoneAssignmentService _service;

    public AssignmentsController(IPhoneAssignmentService service) => _service = service;

    [HttpGet]
    public IActionResult Index() => View(BuildViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Assign(AssignInput input)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }

        Flash(_service.Assign(input.TeamMemberId, input.PhoneNumberId));
        return RedirectToAction(nameof(Index)); // Post-Redirect-Get
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Unassign(int teamMemberId)
    {
        Flash(_service.Unassign(teamMemberId));
        return RedirectToAction(nameof(Index));
    }

    private AssignmentsIndexViewModel BuildViewModel() => new()
    {
        Rows = _service.GetTeamMemberRows(),
        AvailableNumbers = _service.GetAvailableNumbers()
    };

    private void Flash(OperationResult result) =>
        TempData[result.Success ? "Success" : "Error"] = result.Message;
}

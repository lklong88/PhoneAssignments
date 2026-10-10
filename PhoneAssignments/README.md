# Team Phone Assignments (ASP.NET Core 8 MVC)

Run: `dotnet run --project PhoneAssignments`  |  Test: `dotnet test`

Layers: Controller -> IPhoneAssignmentService (business rules) -> IPhoneDataStore (persistence; in-memory, seeded from Data/seed.json).
Rules: one number per team member; a number can belong to only one member; both IDs must exist.
Data resets on app restart (in-memory by design for the MVP).

using PhoneAssignments.Data;
using PhoneAssignments.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "seed.json");
builder.Services.AddSingleton<IPhoneDataStore>(_ => new InMemoryPhoneDataStore(InMemoryPhoneDataStore.LoadSeed(seedPath)));
builder.Services.AddSingleton<IPhoneAssignmentService, PhoneAssignmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("default", "{controller=Assignments}/{action=Index}/{id?}");

app.Run();

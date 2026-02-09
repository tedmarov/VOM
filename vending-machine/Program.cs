using VendingMachineApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<VendingMachine>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Redirect GET / to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.MapControllers();

app.Run();

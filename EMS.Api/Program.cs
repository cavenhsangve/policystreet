var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// TODO: Register your repository and service here, e.g.:
// builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
// builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

app.UseExceptionHandler(err => err.Run(async ctx =>
{
    ctx.Response.StatusCode = 500;
    ctx.Response.ContentType = "application/problem+json";
    await ctx.Response.WriteAsJsonAsync(new { title = "An unexpected error occurred.", status = 500 });
}));

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();

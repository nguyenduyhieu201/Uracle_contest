


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiServices(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseApiServices();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();
 
app.Run();

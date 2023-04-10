using Microsoft.EntityFrameworkCore;
using Atletika_Denik_API.Data.Services;
using Atletika_Denik_API.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
// Add services to the container.

builder.Services.AddControllers();

var connectionString_Atletika = builder.Configuration.GetConnectionString("ConnectionString3");

//Adding contexts
// builder.Services.AddDbContext<TrainingContext>(opt => opt.UseSqlServer(connectionString_Atletika));
builder.Services.AddDbContext<TrainingContext>();
builder.Services.AddDbContext<UserContext>();

//Adding services
builder.Services.AddTransient<TrainingService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<PDFService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Add Cors 
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

// Configure the HTTP request pipeline.w
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
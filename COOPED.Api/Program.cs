using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using COOPED.Application.Interfaces;
using COOPED.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CoopedDb");   // connexion à la base de cooped

// enregistrement du DbContext 
builder.Services.AddDbContext<CoopedDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));   // indique que la base zst mySql

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<ISuiviService, SuiviService>();

var app = builder.Build();     // constructeur de l'application 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // force à utiliser https
app.UseAuthorization();   //Active le système d'autorisation.
app.MapControllers();  //Utilise les routes définies dans mes contrôleurs.
app.Run(); // lance l'api
using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using COOPED.Application.Interfaces;
using COOPED.Infrastructure.Services;
using COOPED.Infrastructure.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;





var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CoopedDb");   // connexion à la base de cooped

// enregistrement du DbContext 
builder.Services.AddDbContext<CoopedDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));   // indique que la base zst mySql

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "COOPED API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT ici : Bearer {votre token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ITontineRepository, TontineRepository>();

// Enregistrement générique pour toutes les entités (couvre Pret, Frais, etc.)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPretService, PretService>();
builder.Services.AddScoped<IPenaliteService, PenaliteService>();

builder.Services.AddScoped<ISuiviService, SuiviService>();
builder.Services.AddScoped<ITontineService, TontineService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<IPromoteurService, PromoteurService>();
builder.Services.AddScoped<IRetraitService, RetraitService>();
builder.Services.AddScoped<IAchatService, AchatService>();

builder.Services.AddScoped<IAuthService, AuthService>();


var jwtCle = builder.Configuration["Jwt:Cle"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCle))
    };
});

builder.Services.AddAuthorization();


var app = builder.Build();     // constructeur de l'application 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // force à utiliser https
app.UseAuthentication();
app.UseAuthorization();   //Active le système d'autorisation.
app.MapControllers();  //Utilise les routes définies dans mes contrôleurs.
app.Run(); // lance l'api





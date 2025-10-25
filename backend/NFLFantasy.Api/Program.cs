using NFLFantasy.Api.Services;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// =============================
// Configuración principal de NFL Fantasy API (.NET 9)
// =============================

// Inicializa el builder de la aplicación web (configuración principal de ASP.NET Core)
var builder = WebApplication.CreateBuilder(args);

// Habilita el registro de los endpoints de la API para la exploración y documentación automática (Swagger)
builder.Services.AddEndpointsApiExplorer();

//<summary>
// Configura Swagger para generar la documentación interactiva de la API y habilitar autenticación JWT
//</summary>
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "NFL Fantasy API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme{
                Reference = new Microsoft.OpenApi.Models.OpenApiReference{
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Configura la conexión a la base de datos SQL Server y registra los servicios de dominio (UserService, NflTeamService, etc.)
builder.Services.AddDbContext<FantasyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de servicios de la aplicación
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<NflTeamService>();
builder.Services.AddScoped<SeasonService>();
builder.Services.AddScoped<LeagueService>();

// Añade controladores MVC
builder.Services.AddControllers();

// Configura CORS para permitir solicitudes desde el frontend Angular en localhost:4200
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200"));
});

// Configura la autenticación JWT para proteger los endpoints de la API
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new Exception("La clave JWT no está configurada correctamente o es demasiado corta. Verifica appsettings.json (Jwt:Key) y asegúrate de que tenga al menos 32 caracteres.");
var key = Encoding.ASCII.GetBytes(jwtKey);

// Configura la autenticación y validación de tokens JWT para proteger los endpoints de la API
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})


// Configura las opciones de validación del token JWT (firma, emisor, audiencia y expiración)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Construye la aplicación y configura el pipeline de middlewares (Swagger, CORS, archivos estáticos, autenticación, autorización y controladores)
var app = builder.Build();

// Configura el middleware de Swagger para la documentación de la API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplica los middlewares de CORS, archivos estáticos, autenticación, autorización y mapea los controladores de la API
app.UseCors();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Inicia la aplicación web
app.Run();


using System.Text;

using Horizon.Identity.Common;
using Horizon.Identity.Common.Auth;
using Horizon.Identity.Common.Behaviors;
using Horizon.Identity.Common.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Mapster
builder.Services.AddMapster();

// JWT: opciones validadas al arrancar (falla rápido si falta el secreto)
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer es requerido.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience es requerido.")
    .Validate(o => Encoding.UTF8.GetByteCount(o.SecretKey) >= 32, "Jwt:SecretKey debe tener al menos 32 caracteres.")
    .Validate(o => o.ExpiryMinutes > 0, "Jwt:ExpiryMinutes debe ser mayor que 0.")
    .ValidateOnStart();

builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();

// Recuperación de contraseña
builder.Services
    .AddOptions<PasswordResetOptions>()
    .Bind(builder.Configuration.GetSection(PasswordResetOptions.SectionName))
    .Validate(o => o.ExpiryMinutes > 0, "PasswordReset:ExpiryMinutes debe ser mayor que 0.")
    .Validate(o => Uri.TryCreate(o.FrontendBaseUrl, UriKind.Absolute, out _), "PasswordReset:FrontendBaseUrl debe ser una URL absoluta.")
    .ValidateOnStart();

builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName));

// Con Email:Enabled=false el enlace se escribe en el log (desarrollo).
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();

//manejo global de exepcions
app.UseExceptionHandler();

app.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
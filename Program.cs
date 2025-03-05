using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Login;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Habilita el comportamiento antiguo de Npgsql para DateTime
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Require authorization for everything...
    options.Conventions.AuthorizeFolder("/");
    // ...but let /Login be anonymous
    options.Conventions.AllowAnonymousToFolder("/Login");
})
.AddRazorPagesOptions(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});

// Habilitar WebSocket
builder.Services.AddWebSockets(options =>
{
    // Intervalo de keep-alive
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);
});

// Configuración de CORS para permitir llamadas AJAX desde cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder => policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

    // New policy for DataTables
    options.AddPolicy("DataTablesCDN",
        policyBuilder => policyBuilder
            .WithOrigins("https://cdn.datatables.net") // Use HTTPS
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configuración de sesión en memoria
builder.Services.AddDistributedMemoryCache();

// Configuración de la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Ajusta el tiempo de inactividad aquí
    options.Cookie.HttpOnly = true;                 // Asegura la cookie
    options.Cookie.IsEssential = true;              // Necesario para que funcione sin consentimiento del usuario
    // Importante para Chrome/Edge cuando se use HTTPS:
    // options.Cookie.SameSite = SameSiteMode.None;
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Configuración de autenticación JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Extract from cookie named "AuthorizationCookie"
            var cookie = context.Request.Cookies["AuthorizationCookie"];
            if (!string.IsNullOrEmpty(cookie))
            {
                context.Token = cookie;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
                context.Response.StatusCode = 401;
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/AccessDenied";
});

builder.Services.AddAuthorization();

// Se agregan controladores y MVC
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
builder.Services.AddControllers(options => options.EnableEndpointRouting = false);
builder.Services.AddMvc();

// Agregar el contexto de base de datos
builder.Services.AddDbContext<mecanico_plus.Data.local>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("connectionString")));



// Opcional: Limites del servidor Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1073741824; // 1 GB
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(120);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(120);
});

var app = builder.Build();

// Manejar errores en producción
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Usar WebSockets
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    ReceiveBufferSize = 16 * 1024 // Buffer de recepción 16KB
});

// Redirigir a HTTPS
app.UseHttpsRedirection();

// Servir archivos estáticos (wwwroot)
app.UseStaticFiles();

// Aplicar enrutamiento
app.UseRouting();

// Habilitar CORS, si lo necesitas en tus endpoints
app.UseCors("DataTablesCDN");

// Activar sesión
app.UseSession();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Crear middleware personalizado para validación de suscripción
app.Use(async (context, next) =>
{
    var path = context.Request.Path.ToString().ToLower();
    if (path.StartsWith("/Principal")&& 
        !path.Contains("/Principal/Usuario") && 
        !path.Contains("/Principal/suscripcion"))
    {
        var email = context.Session.GetString("SessionUser");
        if (!string.IsNullOrEmpty(email))
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<mecanico_plus.Data.local>();
                var user = await dbContext.t001_usuario
                    .FirstOrDefaultAsync(u => u.f001_correo_electronico == email);

               
            }
        }
    }
    await next();
});

// Mapear controladores y Razor Pages
app.MapControllers();
app.MapRazorPages();




// Redirect "/" to "/Login/Index"
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login/Index");
    return Task.CompletedTask;
});

// Iniciar la aplicación
app.Run();

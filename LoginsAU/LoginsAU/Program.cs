using LoginsAU.Datos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar conexion SQL
builder.Services.AddDbContext<AplicacionDbConext>(opciones => 
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("LoginsDbConexion"))
);

// Agregar servicio Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AplicacionDbConext>();

// URL de retorno para paginas no autorizadas
builder.Services.ConfigureApplicationCookie(options => 
{
    options.LoginPath = new PathString("/Cuentas/Acceso");
    // Ruta defecto para acceso denegado
    options.AccessDeniedPath = new PathString("/Cuentas/Bloqueado");
});

// Opciones de config Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar autenticacion
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

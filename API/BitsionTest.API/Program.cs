using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Exceptions;
using BitsionTest.API.Extensions;
using BitsionTest.API.Infrastructure.Context;
using BitsionTest.API.Infrastructure.Mapping;
using BitsionTest.API.Repositories.Implementation;
using BitsionTest.API.Repositories.Interface;
using BitsionTest.API.Services.Implementation;
using BitsionTest.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();  // inyectamos el servicio para acceder a los datos Http como headers, user, etc
builder.Services.AddProblemDetails();  // inyectamos el servicio que estructura errores HTTP en las responses (especificaci�n RFC 7807)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // inyectamos nuestro global exception handler
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);   // convert responses a camelCase

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// inyectamos el dbcontext y lo seteamos para conectarse a sqlserver mediante la connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BitsionTestConnectionString"));
});

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; // El email debe ser �nico
    options.User.AllowedUserNameCharacters = null; // Elimina restricciones en UserName
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// Services  
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<ITokenService, TokenServiceImpl>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserServiceImpl>();
builder.Services.AddScoped<IClientService, ClientServiceImpl>();

// Repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


// Del ApplicationService agregamos Jwt
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureCors();

var app = builder.Build();

// Seeding roles y usuario administrador generado autom�ticamente
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAndAdminUser(serviceProvider);
}

app.UseCors("CorsPolicy");  // especificamos que utilice la pol�tica de cors que definimos

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    var request = context.Request;
    var headers = string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"));
    Console.WriteLine($"Request: {request.Method} {request.Path}, Headers: {headers}");
    await next.Invoke();
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

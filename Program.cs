using ApiFuncional.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Ignora os filtros de validação que adicionamos a nossa model caso ela seja invalida
        options.SuppressModelStateInvalidFilter = true; 
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Definindo o EF Core
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Definindo o Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>() 
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();

//Definindo o comportamento da nossa App daqui pra baixo
var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//este sempre vem antes do Authorization, se for invertido a API não funciona
app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.Run();

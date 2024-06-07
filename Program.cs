using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

//Antes apenas adicionamos o gerador do Swagger, agora estamos adicionando mais algumas configurações
//código muito usado e geralmente é sempre a mesma coisa, faz alguns anos que não muda - adiciona o Suporte a JWT no swagger
builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme //tipo de definição do esquema de segurança
    {
        Description = "Insira o token JWT desta maneira: Bearer {seu token}", // ensinando como utilizar o token
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT", //formato do token é JWT
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
        //essa configuração é pra quando a OpenApi receber isso ela já sabe como repassar os dados
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement //esse requiremente é para dizer o tipo de schema que estamos usando
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
            new string[]{}
        }
    });
});

//Definindo o EF Core
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Definindo o Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>() 
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();

//Pegando o Token e gerando a chave encodada
var JwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(JwtSettingsSection); //essa configuração permite injetar o JwtSettings como injeção de dependência

var jwtSettings = JwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Segredo);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audiencia,
        ValidIssuer = jwtSettings.Emissor
    };
});

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

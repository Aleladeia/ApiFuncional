using ApiFuncional.Configuration;

var builder = WebApplication.CreateBuilder(args);

//Os códgigos existentes aqui para configuração do builder foi externalizado para o diretório Configuration em classes separadas para cada configuração.

// Add services to the container.
builder
    .AddApiConfig()
    .AddCorsConfig()
    .AddSwaggerConfig()
    .AddDbContextConfig()
    .AddIdentityConfig();
//Aqui é possivel chamar todos estes metódos de extensão em sequência pois eles retornam um WebApplicationBuilder e extende ele ao mesmo tempo
//então sempre que retornamos ele podemos chama o próximo método dele mesmo. Construimos um builder, isso deixou nosso código mais facil de entender
//e bonito.

//Definindo o comportamento da nossa App daqui pra baixo
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

app.UseHttpsRedirection();

//este sempre vem antes do Authorization, se for invertido a API não funciona
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

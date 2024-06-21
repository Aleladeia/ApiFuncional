using ApiFuncional.Configuration;

var builder = WebApplication.CreateBuilder(args);

//Os c�dgigos existentes aqui para configura��o do builder foi externalizado para o diret�rio Configuration em classes separadas para cada configura��o.

// Add services to the container.
builder
    .AddApiConfig()
    .AddCorsConfig()
    .AddSwaggerConfig()
    .AddDbContextConfig()
    .AddIdentityConfig();
//Aqui � possivel chamar todos estes met�dos de extens�o em sequ�ncia pois eles retornam um WebApplicationBuilder e extende ele ao mesmo tempo
//ent�o sempre que retornamos ele podemos chama o pr�ximo m�todo dele mesmo. Construimos um builder, isso deixou nosso c�digo mais facil de entender
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

//este sempre vem antes do Authorization, se for invertido a API n�o funciona
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

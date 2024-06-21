using Microsoft.OpenApi.Models;

namespace ApiFuncional.Configuration
{
    public static class SwaggerConfig
    {
        public static WebApplicationBuilder AddSwaggerConfig(this WebApplicationBuilder builder)
        {
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

            return builder;
        }

    }

}

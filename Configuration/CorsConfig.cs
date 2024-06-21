namespace ApiFuncional.Configuration
{
    public static class CorsConfig
    {
        public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
        {
            //Configurando CORS
            builder.Services.AddCors(options =>
            {
                //Abaixo fizemos a adição de 2 politicas, uma para o ambiente de dev que é mais aberta, aceita requisições de qualquer origem
                //que podem consumir a API, utilizar qualquer método(GET,POST,PUT...)
                options.AddPolicy("Development", builder =>
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());

                //Já a politica de Production, é mais restrita, só aceita requisições de localhost:9000 e o uso do método phost
                options.AddPolicy("Production", builder =>
                            builder
                                .WithOrigins("https://localhost:9000")
                                .WithMethods("POST")
                                .AllowAnyHeader());
            });

            return builder;
        }

    }

}

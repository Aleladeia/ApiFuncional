namespace ApiFuncional.Configuration
{
    public static class ApiConfig //Estatica pois vamos criar um método de extensão que vai extender o próprio builder da nossa Program.cs
    {
        //a palavra reservada this, quer dizer que estamos criando o método AddApiConfig dentro da classe WebApplicationBuilder, mesmo que nós não
        //tenhamos o código fonte. Ele se trata de um método de extensão.
        public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
        {
            //Aqui dentro vamos trazer o código da nossa Program.cs
            builder.Services.AddControllers()
                            .ConfigureApiBehaviorOptions(options =>
                            {
                                // Ignora os filtros de validação que adicionamos a nossa model caso ela seja invalida
                                options.SuppressModelStateInvalidFilter = true;
                            });

            return builder;
        }

    }

}

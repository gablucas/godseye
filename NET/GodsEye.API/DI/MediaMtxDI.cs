using GodsEye.API.MediaMtx;
using GodsEye.API.Interfaces;
using Microsoft.Extensions.Options;

namespace GodsEye.API.DI
{
    public static class MediaMtxDI
    {
        public static void AddMediaMtxDI(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddOptions<MediaMtxOptions>()
            .Bind(configuration.GetSection("MediaMtx"))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.ApiBaseUrl) &&
                !string.IsNullOrWhiteSpace(o.WebRtcBaseUrl),
                "Configuração do MediaMTX inválida"
            )
            .ValidateOnStart();

            services.AddHttpClient<IMediaMtxService, MediaMtxService>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<MediaMtxOptions>>()
                    .Value;


                if (string.IsNullOrWhiteSpace(options.ApiBaseUrl))
                    throw new InvalidOperationException("A URL do MediaMtx não foi configurada.");

                client.BaseAddress = new Uri(options.ApiBaseUrl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            });
        }
    }
}

using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Exceptions;
using GodsEye.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GodsEye.Infrastructure.MediaMtx
{
    public class MediaMtxService : IMediaMtxService
    {
        private readonly HttpClient _httpClient;
        private readonly MediaMtxOptions _options;

        public MediaMtxService(HttpClient httpClient, IOptions<MediaMtxOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<(bool, string)> GetStream(string rtspUrl)
        {
            var streamName = $"{GenerateHash(rtspUrl)}";

            var mediaMtxApiUrl = $"v3/config/paths/get/{streamName}";

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(mediaMtxApiUrl);
            }
            catch (Exception ex)
            {
                throw new MediaMtxServiceException($"Falha ao conectar ao MediaMTX: {ex.Message}", 503);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string message = errorContent;

                try
                {
                    var parsed = JsonSerializer.Deserialize<MediaMtxResponse>(errorContent);

                    if (parsed != null && !string.IsNullOrEmpty(parsed.Error))
                    {
                        message = parsed.Error;

                        return (false, "");
                    }
                }
                catch 
                {
                    throw new MediaMtxServiceException(
                        $"MediaMTX retornou erro ({(int)response.StatusCode}): {message}",
                        (int)response.StatusCode
                    );
                }
                
            }

            return (true, $"{_options.WebRtcBaseUrl}/{streamName}/whep");
        }

        public async Task<string> StartStream(string rtspUrl)
        {
            var streamName = $"{GenerateHash(rtspUrl)}";

            var (isValid, url) = await GetStream(rtspUrl);
            
            if (isValid)
            {
                return url;
            }

            var mediaMtxApiUrl = $"/v3/config/paths/add/{streamName}";

            var payload = new
            {
                source = rtspUrl,
                sourceOnDemand = true,
                runOnDemandCloseAfter = "10s"
            };

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync(mediaMtxApiUrl, payload);
            }
            catch (Exception ex)
            {
                throw new MediaMtxServiceException($"Falha ao conectar ao MediaMTX: {ex.Message}", 503);
            }


            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string mensagem = errorContent;

                try
                {
                    var parsed = JsonSerializer.Deserialize<MediaMtxResponse>(errorContent);

                    if (parsed != null && !string.IsNullOrEmpty(parsed.Error))
                    {
                        mensagem = parsed.Error;
                    }

                }
                catch { /* ignora parse */ }

                throw new MediaMtxServiceException(
                    $"MediaMTX retornou erro ({(int)response.StatusCode}): {mensagem}",
                    (int)response.StatusCode
                );
            }

            var result = await response.Content.ReadFromJsonAsync<MediaMtxResponse>();

            if (result == null || result.Status != "ok")
            {
                throw new MediaMtxServiceException(
                    $"MediaMTX retornou resposta inesperada: {result?.Error ?? "Nenhum erro especificado"}",
                    500
                );
            }

            var webRtcUrl = $"{_options.WebRtcBaseUrl}/{streamName}/whep";

            return webRtcUrl;
        }


        // Gera um hash MD5 limpo (apenas letras e números)
        private string GenerateHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Converte para string Hexadecimal (ex: "a3f5c2...")
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

    }
}


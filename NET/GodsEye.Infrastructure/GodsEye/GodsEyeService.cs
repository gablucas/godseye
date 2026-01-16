using GodsEye.Application.DTOs.External.GodsEye;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Exceptions;
using GodsEye.Application.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GodsEye.Infrastructure.GodsEye
{
    public class GodsEyeService : IGodsEyeService
    {
        private readonly HttpClient _httpClient;

        public GodsEyeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float[]> GenerateEmbedding(byte[] image)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(image);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "photo", "image.jpg");

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync("/api/face/embedding", content);
            }
            catch (Exception ex) { 
                throw new GodsEyeServiceException(
                    $"Falha ao conectar ao serviço de reconhecimento facial (Python): {ex.Message}",
                    503
                );
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                Console.WriteLine("ERRO DO PYTHON:");
                Console.WriteLine(error);

                throw new GodsEyeServiceException(
                    $"Python retornou erro ({(int)response.StatusCode}): {error}",
                    (int)response.StatusCode
                );
            }

            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

            if (result == null)
                throw new GodsEyeServiceException("O Python retornou um JSON vazio.");

            return result.Embedding;
        }

        public async Task<CameraPreviewResponse> StartStream(string name, string url)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(
                    $"/api/camera/start?name={name}&rtsp_url={url}",
                    null
                );
            }
            catch (Exception ex)
            {
                throw new GodsEyeServiceException(
                    $"Falha ao conectar ao serviço de streaming (Python): {ex.Message}",
                    503
                );
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new GodsEyeServiceException(
                    $"Python retornou erro ({(int)response.StatusCode}): {error}",
                    (int)response.StatusCode
                );
            }

            var result = await response.Content.ReadFromJsonAsync<CameraPreviewResponse>();

            if (result == null || string.IsNullOrWhiteSpace(result.Url))
                throw new GodsEyeServiceException("Python não retornou a URL de pré-visualização da câmera.");

            return result;
        }
    }
}

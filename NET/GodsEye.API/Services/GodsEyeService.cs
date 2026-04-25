using GodsEye.API.DTO;
using GodsEye.API.Exceptions;
using GodsEye.API.Interfaces;
using GodsEye.Shared;
using MediatR;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GodsEye.API.Services
{
    public class GodsEyeService : IGodsEyeService
    {
        private readonly HttpClient _httpClient;
        private readonly IFaceMatcherService _faceMatcherService;
        private readonly IGodsEyeState _godsEyeState;
        private readonly IMediator _mediator;

        public GodsEyeService(HttpClient httpClient, IFaceMatcherService faceMatcherService, IGodsEyeState godsEyeState, IMediator mediator)
        {
            _httpClient = httpClient;
            _faceMatcherService = faceMatcherService;
            _godsEyeState = godsEyeState;
            _mediator = mediator;
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
                response = await _httpClient.PostAsync("api/face/embedding", content);
            }
            catch (Exception ex)
            {
                throw new GodsEyeServiceException(
                    $"Falha ao conectar ao serviço de reconhecimento facial (Python): {ex.Message}",
                    503
                );
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string mensagem = errorContent;

                Console.WriteLine("ERRO DO PYTHON:");
                Console.WriteLine(mensagem);
                try
                {
                    var parsed = JsonSerializer.Deserialize<FastApiError>(errorContent);
                    if (!string.IsNullOrWhiteSpace(parsed?.Detail))
                        mensagem = parsed.Detail;
                }
                catch { /* ignora parse */ }

                throw new GodsEyeServiceException(
                    $"Python retornou erro ({(int)response.StatusCode}): {mensagem}",
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

        public async Task ProcessingEmbedding(int cameraId, float[] embedding, DateTime identifiedAt)
        {
            var (personId, score) = _faceMatcherService.FindMatch(embedding, _godsEyeState.GetPersons());
            var cameraFromRequest = _godsEyeState.GetCameraById(cameraId);

            if (personId == 0 || cameraFromRequest is null)
                return;

            await HandleFeatures(cameraFromRequest, personId, score, identifiedAt);

        }

        public async Task HandleFeatures(CameraCache cameraFromRequest, int personId, float score, DateTime identifiedAt)
        {

            if (cameraFromRequest.Features.Any(x => x.Id == 1))
            {
                await _mediator.Publish(new EnvironmentMonitoringNotification(cameraFromRequest.Id, personId, score, identifiedAt));

                //if (!_godsEyeState.TryUpdateDetection(personId, cameraFromRequest.Id, identifiedAt))
                //{
                //    await _mediator.Send(new CreateEnvironmentMonitoringLogRequest(cameraFromRequest.Id, personId, score, identifiedAt));
                //    await _mediator.Send(new CheckAccessViolationRequest(cameraFromRequest.Id, personId, identifiedAt));
                //} 
            }
        }
    }
}

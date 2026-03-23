using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Application.Interfaces;
using MediatR;
using System.Text.Json;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Application.UseCases.Person.Commands.CreateRecognize
{
    public class CreateRecognizeHandler : IRequestHandler<CreateRecognizeRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IGodsEyeService _godsEye;
        private readonly IFolderService _folderService;
        private readonly IPersonQuerie _personQuerie;
        private readonly IDapperContext _context;
        private readonly INotificationSignalR _notification;

        public CreateRecognizeHandler(IGodsEyeService godsEye, IFolderService folderService, IPersonQuerie personQuerie, IDapperContext context, INotificationSignalR notification)
        {
            _godsEye = godsEye;
            _folderService = folderService;
            _personQuerie = personQuerie;
            _context = context;
            _notification = notification;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateRecognizeRequest request, CancellationToken cancellationToken)
        {
            var embedding = await _godsEye.GenerateEmbedding(request.Photo);
            var jsonEmbedding = JsonSerializer.Serialize(embedding);

            var fileName = $"{Guid.NewGuid()}.jpg";

            var photoPath = _folderService.GeneratePersonPhotoPath(fileName);

            var sql = "CALL SP_PERSON_CREATE_RECOGNIZE(@P_PERSON_ID, @P_IMAGE_PATH, @P_EMBEDDING)";

            var parameteres = new
            {
                P_PERSON_ID = request.PersonId,
                P_IMAGE_PATH = photoPath,
                P_EMBEDDING = jsonEmbedding,
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameteres, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");

            var updatedPerson = await _personQuerie.GetById(result.Id, cancellationToken);

            if (updatedPerson is not null)
                await _notification.SendCreatedPerson(updatedPerson);

            await _folderService.SavePersonPhoto(request.Photo, fileName, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}

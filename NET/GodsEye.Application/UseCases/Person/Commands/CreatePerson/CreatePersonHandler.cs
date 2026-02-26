using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Application.Interfaces;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.Person.Commands.CreatePerson
{
    public class CreatePersonHandler : IRequestHandler<CreatePersonRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IGodsEyeService _godsEye;
        private readonly IFolderService _folderService;
        private readonly IApplicationDbContext _context;

        public CreatePersonHandler(IGodsEyeService godsEye, IFolderService folderService, IApplicationDbContext context)
        {
            _godsEye = godsEye;
            _folderService = folderService;
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            var embedding = await _godsEye.GenerateEmbedding(request.Photo);
            var jsonEmbedding = JsonSerializer.Serialize(embedding);


            var fileName = $"{Guid.NewGuid()}.jpg";

            var photoPath = _folderService.GeneratePersonPhotoPath(fileName);

            var sql = "CALL SP_PERSON_CREATE(@P_NAME, @P_EMBEDDING, @P_IMAGE_PATH, @P_MAIN_SECTOR_ID)";

            var parameteres = new
            {
                P_NAME = request.Name,
                P_EMBEDDING = jsonEmbedding,
                P_IMAGE_PATH = photoPath,
                P_MAIN_SECTOR_ID = request.SectorId
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameteres, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");
            
            await _folderService.SavePersonPhoto(request.Photo, fileName, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}

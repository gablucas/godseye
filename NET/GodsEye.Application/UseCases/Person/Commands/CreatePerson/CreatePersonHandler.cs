using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.Person.Commands.CreatePerson
{
    public class CreatePersonHandler : IRequestHandler<CreatePersonRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IGodsEyeService _godsEye;
        private readonly IPersonRepository _personRespository;
        private readonly IFolderService _folderService;

        public CreatePersonHandler(IGodsEyeService godsEye, IPersonRepository personRepository, IFolderService folderService)
        {
            _godsEye = godsEye;
            _personRespository = personRepository;
            _folderService = folderService;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            var embedding = await _godsEye.GenerateEmbedding(request.Photo);
            var jsonEmbedding = JsonSerializer.Serialize(embedding);


            var fileName = $"{Guid.NewGuid()}.jpg";

            var photoPath = _folderService.GeneratePersonPhotoPath(fileName);

            PersonEntity newPerson = new PersonEntity
            {
                Name = request.Name,
                Embedding = jsonEmbedding,
                ImagePath = photoPath,
                Sectors = request.Sectors,
            };

            var result = await _personRespository.Create(newPerson, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");
            
            await _folderService.SavePersonPhoto(request.Photo, fileName, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}

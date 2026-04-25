using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Response.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreatePersonRecognizeRequest(int personId, string photo);

    public class CreatePersonRecognizeValidator : AbstractValidator<CreatePersonRecognizeRequest>
    {
        public CreatePersonRecognizeValidator()
        {
            RuleFor(person => person.personId).NotEmpty();
            RuleFor(person => person.photo).NotEmpty();
        }
    }

    internal sealed record CreatePersonRecognizeCommand(int personId, byte[] photo) : IRequest<int>;

    internal sealed class CreatePersonRecognizeMapper : Profile
    {
        public CreatePersonRecognizeMapper()
        {
            CreateMap<CreatePersonRecognizeRequest, CreatePersonRecognizeCommand>();
        }
    }

    internal sealed class CreatePersonRecognizeHandler(Interfaces.IDapperContext context, IGodsEyeService godsEye, IFolderService folderService, ILogger<CreatePersonRecognizeHandler> logger, Interfaces.INotificationSignalR notification) : IRequestHandler<CreatePersonRecognizeCommand, int>
    {
        public async Task<int> Handle(CreatePersonRecognizeCommand request, CancellationToken cancellationToken)
        {
            var embedding = await godsEye.GenerateEmbedding(request.photo);
            var jsonEmbedding = JsonSerializer.Serialize(embedding);

            var fileName = $"{Guid.NewGuid()}.jpg";

            var photoPath = folderService.GeneratePersonPhotoPath(fileName);

            var result = await CreatePersonRecognizeWrite(request.personId, photoPath, jsonEmbedding, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");

            var updatedPerson = await GetById(result.Id, cancellationToken);

            //if (updatedPerson is not null)
            //    await notification.SendCreatedPerson(updatedPerson);

            await folderService.SavePersonPhoto(request.photo, fileName, cancellationToken);

            return 1;
        }

        public async Task<ProcedureResult?> CreatePersonRecognizeWrite(int personId, string photoPath, string jsonEmbedding, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_CREATE_RECOGNIZE(@P_PERSON_ID, @P_IMAGE_PATH, @P_EMBEDDING)";

            var parameters = new
            {
                P_PERSON_ID = personId,
                P_IMAGE_PATH = photoPath,
                P_EMBEDDING = jsonEmbedding,
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }

        public async Task<PersonResponse?> GetById(int personId, CancellationToken cancellationToken)
        {
            var query = "CALL SP_PERSON_GET_BY_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
            };

            return await context.QuerySingleSqlAsync<PersonResponse>(query, parameters, cancellationToken);
        }
    }

    public class CreatePersonRecognizeEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/person/recognize", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreatePersonRecognizeRequest request, 
            [FromServices] IValidator<CreatePersonRecognizeRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // Remove prefixo "data:image/png;base64,"
            var base64 = request.photo.Contains(",")
                ? request.photo.Split(",")[1]
                : request.photo;

            var photoBytes = Convert.FromBase64String(base64);

            var command = new CreatePersonRecognizeCommand(request.personId, photoBytes);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}

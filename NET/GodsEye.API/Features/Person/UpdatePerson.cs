using AutoMapper;

using GodsEye.API.Interfaces;
using GodsEye.Shared.Response;
using GodsEye.Shared.Response.Person;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record UpdatePersonRequest(int id, string name, int sectorId, int accessLevelId);

    internal sealed record UpdatePersonCommand(int id, string name, int sectorId, int accessLevelId) : IRequest<int>;
    
    internal sealed class UpdatePersonMapper : Profile
    {
        public UpdatePersonMapper()
        {
            CreateMap<UpdatePersonRequest, UpdatePersonCommand>();
        }
    }

    internal sealed class UpdatePersonHandler(IDapperContext context, ILogger<UpdatePersonHandler> logger) : IRequestHandler<UpdatePersonCommand, int>
    {
        public async Task<int> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var result = await UpdatePersonWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> UpdatePersonWrite(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_UPDATE(@P_PERSON_ID, @P_NAME, @P_MAIN_SECTOR_ID, @P_ACCESS_LEVEL_ID)";

            var parameters = new
            {
                P_PERSON_ID = request.id,
                P_NAME = request.name,
                P_MAIN_SECTOR_ID = request.sectorId,
                P_ACCESS_LEVEL_ID = request.accessLevelId
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class UpdatePersonEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPut("/api/person", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] UpdatePersonRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdatePersonCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}

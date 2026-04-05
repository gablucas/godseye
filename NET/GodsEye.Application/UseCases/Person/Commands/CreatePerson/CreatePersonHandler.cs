using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Application.Interfaces;
using MediatR;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Application.UseCases.Person.Commands.CreatePerson
{
    public class CreatePersonHandler : IRequestHandler<CreatePersonRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IDapperContext _context;
        private readonly IPersonQueries _personQuerie;
        private readonly INotificationSignalR _notification;

        public CreatePersonHandler(IDapperContext context, IPersonQueries personQuerie, INotificationSignalR notification)
        {
            _context = context;
            _personQuerie = personQuerie;
            _notification = notification;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_CREATE(@P_NAME, @P_MAIN_SECTOR_ID, @P_ACCESS_LEVEL_ID)";

            var parameteres = new
            {
                P_NAME = request.Name,
                P_MAIN_SECTOR_ID = request.SectorId,
                P_ACCESS_LEVEL_ID = request.AccessLevelId
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameteres, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");

            var createdPerson = await _personQuerie.GetById(result.Id, cancellationToken);

            if (createdPerson is not null)
                await _notification.SendCreatedPerson(createdPerson);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}

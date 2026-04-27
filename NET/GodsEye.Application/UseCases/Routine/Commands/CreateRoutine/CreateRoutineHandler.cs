using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Extensions;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.Routine.Commands.CreateRoutine
{
    public class CreateRoutineHandler : IRequestHandler<CreateRoutineRequest, int>
    {
        private readonly IDapperContext _context;
        private readonly IRoutineQuerie _routineQuerie;
        private readonly INotificationSignalR _notification;

        public CreateRoutineHandler(IDapperContext context, IRoutineQuerie routineQuerie, INotificationSignalR notification)
        {
            _context = context;
            _routineQuerie = routineQuerie;
            _notification = notification;
        }

        public async Task<int> Handle(CreateRoutineRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ROUTINE_CREATE_OR_UPDATE(@P_ID, @P_NAME, @P_TYPE, @P_ROUTINE_RULES_JSON)";

            var parameters = new
            {
                P_ID = request.Id,
                P_NAME = request.Name,
                P_TYPE = request.RuleType.GetDescription(),
                P_ROUTINE_RULES_JSON = JsonSerializer.Serialize(request.Rules),
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");

            var createdRoutine = await _routineQuerie.GetById(result.Id, cancellationToken);

            if (createdRoutine is not null)
                await _notification.SendCreatedRoutine(createdRoutine);

            return result.Id;
        }
    }
}

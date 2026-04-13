using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.Queries;
using MediatR;

namespace GodsEye.Application.UseCases.Routine.Queries
{
    public sealed record GetRoutineByIdRequest(int routineId) : IRequest<RoutineModel>;

    public class GetRoutineByRequestHandler : IRequestHandler<GetRoutineByIdRequest, RoutineModel>
    {
        private readonly IRoutineQuerie _routineQuerie;

        public GetRoutineByRequestHandler(IRoutineQuerie routineQuerie)
        {
            _routineQuerie = routineQuerie;
        }

        public async Task<RoutineModel> Handle(GetRoutineByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _routineQuerie.GetById(request.routineId, cancellationToken);

            if (result is null)
                throw new InvalidOperationException($"Routine with ID {request.routineId} not found.");

            return result;
        }
    }
}

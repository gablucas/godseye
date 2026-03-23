using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Commands.UpdatePerson
{
    public class UpdatePersonHandler : IRequestHandler<UpdatePersonRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IDapperContext _context;

        public UpdatePersonHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdatePersonRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_PERSON_UPDATE(@P_PERSON_ID, @P_NAME, @P_MAIN_SECTOR_ID, @P_ACCESS_LEVEL_ID)";

            var parameteres = new
            {
                P_PERSON_ID = request.Id,
                P_NAME = request.Name,
                P_MAIN_SECTOR_ID = request.SectorId,
                P_ACCESS_LEVEL_ID = request.AccessLevelId
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameteres, cancellationToken);

            if (result.Erro == 1)
                throw new InvalidOperationException("Falha ao criar a pessoa no banco de dados.");
            
            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}

using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Feature.Queries.GetAllFeatures
{
    public class GetAllFeaturesHandler : IRequestHandler<GetAllFeaturesRequest, ApiResponse<IReadOnlyCollection<FeatureModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllFeaturesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IReadOnlyCollection<FeatureModel>>> Handle(GetAllFeaturesRequest request, CancellationToken cancellationToken)
        {

            var sql = "CALL SP_FEATURE_GET_ALL()";

            var parameters = new { };

            var result = await _context.QuerySqlAsync<FeatureModel>(sql, parameters, cancellationToken);

            return ApiResponse<IReadOnlyCollection<FeatureModel>>.Ok(result);
        }
    }
}

using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Feature.Queries.GetAllFeatures
{
    public sealed record GetAllFeaturesRequest() : IRequest<ApiResponse<IReadOnlyCollection<FeatureModel>>>;
}

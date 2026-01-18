using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.Feature.Queries.GetAllFeatures
{
    public class GetAllFeaturesHandler : IRequestHandler<GetAllFeaturesRequest, ApiResponse<IReadOnlyCollection<FeatureModel>>>
    {
        private readonly IMapper _mapper;
        private readonly IFeatureRepository _featureRepository;

        public GetAllFeaturesHandler(IMapper mapper, IFeatureRepository featureRepository)
        {
            _mapper = mapper;
            _featureRepository = featureRepository;
        }

        public async Task<ApiResponse<IReadOnlyCollection<FeatureModel>>> Handle(GetAllFeaturesRequest request, CancellationToken cancellationToken)
        {
            var features = await _featureRepository.GetAll();
            var featuresModel = _mapper.Map<IReadOnlyCollection<FeatureModel>>(features);
            return ApiResponse<IReadOnlyCollection<FeatureModel>>.Ok(featuresModel);
        }
    }
}

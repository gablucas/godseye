using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.Entities;

namespace GodsEye.Application.UseCases.Feature.Queries.GetAllFeatures
{
    public class GetAllFeaturesMapper : Profile
    {
        public GetAllFeaturesMapper()
        {
            CreateMap<FeatureEntity, FeatureModel>();
        }
    }
}

using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Model.Forms;

namespace GodsEye.WEB.Mappings
{
    public class RoutineMapper : Profile
    {
        public RoutineMapper() 
        {
            CreateMap<RoutineModel, RoutineForm>();
            CreateMap<RoutineRuleSectorTransitionModel, RoutineRuleSectorTransitionForm>();
        }
    }
}

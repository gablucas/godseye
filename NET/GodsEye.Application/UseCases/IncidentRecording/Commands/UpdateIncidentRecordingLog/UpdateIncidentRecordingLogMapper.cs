using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.ValueObjects;

namespace GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog
{
    public class UpdateIncidentRecordingLogMapper : Profile
    {
        public UpdateIncidentRecordingLogMapper()
        {
            CreateMap<IncidentRecordingPersonModel, IncidentRecordingPersonVO>();
        }
    }
}

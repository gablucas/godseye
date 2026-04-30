using GodsEye.Shared;

namespace GodsEye.API.DTO
{
    public class AccessViolationDetailResponse
    {
        public string Person { get; set; }
        public string Sector { get; set; }
        public List<AccessViolationEmailsDTO> Emails { get; set; }
    }

    public class AccessViolationEmailsDTO : IJSonTypeList
    {
        public string Email { get; set; }
    }
}

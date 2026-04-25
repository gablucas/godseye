namespace GodsEye.API.DTO
{
    public class AccessViolationDetailDTO
    {
        public string Person { get; set; }
        public string Sector { get; set; }
        public List<AccessViolationEmailsDTO> Emails { get; set; }
    }

    public class AccessViolationEmailsDTO
    {
        public string Email { get; set; }
    }
}

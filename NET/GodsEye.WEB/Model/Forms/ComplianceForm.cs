using GodsEye.Domain.Enums;

namespace GodsEye.WEB.Model.Forms
{
    public class ComplianceForm
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ComplianceRuleEnum Rule { get; set; }
    }
}

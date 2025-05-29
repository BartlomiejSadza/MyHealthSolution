using System.Collections.Generic;

namespace MyHealth.Api.Models
{
    public class HealthResponse
    {
        public List<AssessmentItem> Assessments { get; set; }
    }
}

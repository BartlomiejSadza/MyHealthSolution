using System.Collections.Generic;

namespace MyHealth.Api.Models
{
    public class AssessmentItem
    {
        public string Prediction     { get; set; }
        public string Description    { get; set; }
        public List<string> Observations { get; set; }
    }
}

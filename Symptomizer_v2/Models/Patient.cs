using System.ComponentModel.DataAnnotations;

namespace Symptomizer_v2.Models
{
    public class Patient
    {
        public int Id { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,30}")]
        public string Firstname { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,30}")]
        public string Lastname { get; set; }
        public string Symptoms { get; set; } //We will hold our symptoms as string
        public string Disease { get; set; }
    }
}
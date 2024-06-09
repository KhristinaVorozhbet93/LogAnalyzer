using System.ComponentModel.DataAnnotations;

namespace LogAnalyzer.WebAPI
{
    public class LogMinioProcessorSettings
    {
        [Required]
        public string BucketName { get; set; }

        [Required]
        public string Directory { get; set; }

        [Required]
        public int TimeInterval { get; set; }

        [Required]
        public int CountFiles { get; set; }
    }
}

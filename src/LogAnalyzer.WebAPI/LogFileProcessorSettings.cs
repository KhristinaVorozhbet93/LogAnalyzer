using System.ComponentModel.DataAnnotations;

namespace LogAnalyzer.WebAPI
{
    public class LogFileProcessorSettings
    {
        [Required]
        public string DirectoryPath { get; set; }

        [Required]
        public int TimeInterval { get; set; }

        [Required]
        public int CountFiles { get; set; }
    }
}

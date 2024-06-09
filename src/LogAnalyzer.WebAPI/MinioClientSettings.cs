using System.ComponentModel.DataAnnotations;

namespace LogAnalyzer.WebAPI
{
    public class MinioClientSettings
    {
        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string AccesKey { get; set; }

        [Required]
        public string SecretKey { get; set; }

    }
}

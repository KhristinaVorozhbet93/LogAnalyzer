#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
using System.ComponentModel.DataAnnotations;

namespace LogAnalyzer.WebAPI
{
    public class PostgresConfig
    {
        [Required]
        public string ServerName { get; set; }

        [Required]
        public string Port { get; set; }

        [Required]
        public string DatabaseName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }


    }
}

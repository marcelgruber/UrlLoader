using System.ComponentModel.DataAnnotations;

namespace UrlLoader.Models
{
    public class IUrlScraperRepository
    {
        [Required]
        [StringLength(500)]
        [DataType(DataType.Url)] // TODO: could do more validation of URL as built in functionality is lacking
        public string Url { get; set; }
    }
}

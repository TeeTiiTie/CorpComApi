using System.ComponentModel.DataAnnotations;

namespace CorpCom_svc.DTOs.CorpCom.Request
{
    public class ImportBannerRequestDto
    {
        [Required]
        public IFormFile? File { get; set; }

        public int? BannerId { get; set; }
        public string? LinkUrl { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public int PositionId { get; set; }

        [Required]
        public bool IsPublish { get; set; }

        public string? Remark { get; set; }
    }
}
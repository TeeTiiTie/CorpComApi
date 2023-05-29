using System.ComponentModel.DataAnnotations;

namespace CorpCom_svc.DTOs.CorpCom.Request
{
    public class BannersRequestDto : PaginationDto
    {
        [Required]
        public int? PositionId { get; set; }

        public string? OrderingField { get; set; }
        public bool AscendingOrder { get; set; } = true;
    }
}
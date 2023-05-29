using System.ComponentModel.DataAnnotations;

namespace CorpCom_svc.DTOs.CorpCom.Request
{
    public class UpsertPositionRequestDto
    {
        public int? PositionId { get; set; }

        [Required]
        public string? PositionName { get; set; }

        public string? Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
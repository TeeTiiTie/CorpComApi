namespace CorpCom_svc.DTOs.CorpCom.Request
{
    public class PositionsRequestDto : PaginationDto
    {
        public int? PositionId { get; set; }
        public string? PositionName { get; set; }
        public string? OrderingField { get; set; }
        public bool AscendingOrder { get; set; } = true;
    }
}
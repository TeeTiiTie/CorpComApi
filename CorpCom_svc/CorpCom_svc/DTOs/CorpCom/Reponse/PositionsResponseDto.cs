namespace CorpCom_svc.DTOs.CorpCom.Reponse
{
    public class PositionsResponseDto
    {
        public int PositionId { get; set; }
        public string? PositionName { get; set; }
        public string? Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
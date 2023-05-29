namespace CorpCom_svc.DTOs.CorpCom.Reponse
{
    public class BannersResponseDto
    {
        public int BannerId { get; set; }
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int? Priority { get; set; }
        public int? PositionId { get; set; }
        public bool? IsPublish { get; set; }
        public string? Remark { get; set; }
    }
}
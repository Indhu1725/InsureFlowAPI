namespace InsureFlowAPI.DTOs.Claim
{
    public class ClaimDocumentResponseDto
    {
        public int DocumentId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public string ClaimNumber { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; }
    }
}
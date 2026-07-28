using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class ClaimDocumentRequestDto
{
    [Required]
    public int ClaimId { get; set; }

    [Required]
    public string DocumentName { get; set; } = string.Empty;

    [Required]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    public IFormFile DocumentReference { get; set; } = default!;
}
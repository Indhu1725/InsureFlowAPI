using System.ComponentModel.DataAnnotations;

public class UserStatusUpdateDTO
{
    [Required]
    public bool IsActive { get; set; }

    [StringLength(250)]
    public string? Remarks { get; set; }
}
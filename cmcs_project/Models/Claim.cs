
using System.ComponentModel.DataAnnotations;
namespace CMCS.Models;
public class Claim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string Lecturer { get; set; } = string.Empty;
    [Required] public int HoursWorked { get; set; }
    [Required] public decimal HourlyRate { get; set; }
    public string? Notes { get; set; }
    public decimal Amount => HoursWorked * HourlyRate;
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public string? UploadedFileName { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping;

[Table("semester")]

public class SemesterEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("seed")]
    public int Seed { get; set; }

    [Column("days_completed")]
    public short DaysCompleted { get; set; } = 0;

    [Column("total_enjoyment")]
    public int TotalEnjoyment { get; set; } = 0;

    [Column("is_expelled")]
    public bool IsExpelled { get; set; } = false;
}
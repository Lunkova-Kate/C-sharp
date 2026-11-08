// attendance_decision (
//         semester_id uuid NOT NULL,
//         day_number smallint NOT NULL,
//         subject_code varchar(32) NOT NULL,
//         attended boolean NOT NULL,
//         CONSTRAINT pk_attendance_decision PRIMARY KEY (semester_id, day_number, subject_code),
//         CONSTRAINT fk_attendance_decision_lesson FOREIGN KEY (semester_id, day_number, subject_code) REFERENCES lesson (semester_id, day_number, subject_code) ON DELETE CASCADE
//     );

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping;

[PrimaryKey(nameof(SemesterId), nameof(DayNumber), nameof(SubjectCode))]
[Table("attendance_decision")]
public class AttendDecisionEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;

    [Column("attended")]
    public bool Attended { get; set; }

    public LessonEntity Lesson { get; set; } = null!;
}

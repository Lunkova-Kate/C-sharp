// lesson (
//         semester_id uuid NOT NULL,
//         day_number smallint NOT NULL,
//         subject_code varchar(32) NOT NULL,
//         CONSTRAINT pk_lesson PRIMARY KEY (semester_id, day_number, subject_code),
//         CONSTRAINT fk_lesson_day FOREIGN KEY (semester_id, day_number) REFERENCES semester_day (semester_id, day_number) ON DELETE CASCADE,
//         CONSTRAINT fk_lesson_subject_setting FOREIGN KEY (semester_id, subject_code) REFERENCES subject_setting (semester_id, subject_code)
//     );

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping;

[PrimaryKey(nameof(SemesterId), nameof(DayNumber), nameof(SubjectCode))]
[Table("lesson")]
public class LessonEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;
}
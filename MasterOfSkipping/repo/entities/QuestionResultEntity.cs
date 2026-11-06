// question_result (
//         semester_id uuid NOT NULL,
//         day_number smallint NOT NULL,
//         subject_code varchar(32) NOT NULL,
//         was_asked boolean NOT NULL,
//         CONSTRAINT pk_question_result PRIMARY KEY (semester_id, day_number, subject_code),
//         CONSTRAINT fk_question_result_lesson FOREIGN KEY (semester_id, day_number, subject_code) REFERENCES lesson (semester_id, day_number, subject_code) ON DELETE CASCADE
//     );

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping;
[PrimaryKey(nameof(SemesterId), nameof(DayNumber), nameof(SubjectCode))]
[Table("question_result")]
public class QuestionResultEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;

    [Column("was_asked")]
    public bool WasAsked { get; set; }
}
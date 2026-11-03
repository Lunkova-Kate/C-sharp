BEGIN;

CREATE TABLE subject
(
    code         varchar(32)  PRIMARY KEY,
    name         varchar(100) NOT NULL UNIQUE,
    lesson_order smallint     NOT NULL UNIQUE,

    CONSTRAINT ck_subject_lesson_order
        CHECK (lesson_order BETWEEN 1 AND 6)
);

CREATE TABLE rule_type
(
    code varchar(32)  PRIMARY KEY,
    name varchar(100) NOT NULL UNIQUE
);

CREATE TABLE semester
(
    id                uuid     PRIMARY KEY,
    seed              integer  NOT NULL,
    days_completed    smallint NOT NULL DEFAULT 0,
    total_enjoyment   integer  NOT NULL DEFAULT 0,
    is_expelled       boolean  NOT NULL DEFAULT false,

    CONSTRAINT ck_semester_days_completed
        CHECK (days_completed BETWEEN 0 AND 100),
    CONSTRAINT ck_semester_total_enjoyment
        CHECK (total_enjoyment >= 0),
    CONSTRAINT ck_expelled_semester_enjoyment
        CHECK (NOT is_expelled OR total_enjoyment = 0)
);

CREATE TABLE subject_setting
(
    semester_id       uuid        NOT NULL,
    subject_code      varchar(32) NOT NULL,
    rule_code         varchar(32) NOT NULL,
    dependency_a_code varchar(32),
    dependency_b_code varchar(32),

    CONSTRAINT pk_subject_setting
        PRIMARY KEY (semester_id, subject_code),
    CONSTRAINT fk_subject_setting_semester
        FOREIGN KEY (semester_id)
        REFERENCES semester (id)
        ON DELETE CASCADE,
    CONSTRAINT fk_subject_setting_subject
        FOREIGN KEY (subject_code)
        REFERENCES subject (code),
    CONSTRAINT fk_subject_setting_rule
        FOREIGN KEY (rule_code)
        REFERENCES rule_type (code),
    CONSTRAINT fk_subject_setting_dependency_a
        FOREIGN KEY (dependency_a_code)
        REFERENCES subject (code),
    CONSTRAINT fk_subject_setting_dependency_b
        FOREIGN KEY (dependency_b_code)
        REFERENCES subject (code),
    CONSTRAINT ck_subject_setting_parameters
        CHECK
        (
            (rule_code = 'RANDOM_50'
                AND dependency_a_code IS NULL
                AND dependency_b_code IS NULL)
            OR
            (rule_code = 'YESTERDAY_A'
                AND dependency_a_code IS NOT NULL
                AND dependency_b_code IS NULL)
            OR
            (rule_code = 'XOR'
                AND dependency_a_code IS NOT NULL
                AND dependency_b_code IS NOT NULL
                AND dependency_a_code <> dependency_b_code)
        )
);

CREATE TABLE semester_day
(
    semester_id     uuid     NOT NULL,
    day_number      smallint NOT NULL,
    total_enjoyment integer  NOT NULL,
    is_expelled     boolean  NOT NULL DEFAULT false,

    CONSTRAINT pk_semester_day
        PRIMARY KEY (semester_id, day_number),
    CONSTRAINT fk_semester_day_semester
        FOREIGN KEY (semester_id)
        REFERENCES semester (id)
        ON DELETE CASCADE,
    CONSTRAINT ck_semester_day_number
        CHECK (day_number BETWEEN 1 AND 100),
    CONSTRAINT ck_semester_day_total_enjoyment
        CHECK (total_enjoyment >= 0),
    CONSTRAINT ck_expelled_day_enjoyment
        CHECK (NOT is_expelled OR total_enjoyment = 0)
);

CREATE TABLE lesson
(
    semester_id uuid        NOT NULL,
    day_number  smallint    NOT NULL,
    subject_code varchar(32) NOT NULL,

    CONSTRAINT pk_lesson
        PRIMARY KEY (semester_id, day_number, subject_code),
    CONSTRAINT fk_lesson_day
        FOREIGN KEY (semester_id, day_number)
        REFERENCES semester_day (semester_id, day_number)
        ON DELETE CASCADE,
    CONSTRAINT fk_lesson_subject_setting
        FOREIGN KEY (semester_id, subject_code)
        REFERENCES subject_setting (semester_id, subject_code)
);

CREATE TABLE attendance_decision
(
    semester_id uuid        NOT NULL,
    day_number  smallint    NOT NULL,
    subject_code varchar(32) NOT NULL,
    attended     boolean     NOT NULL,

    CONSTRAINT pk_attendance_decision
        PRIMARY KEY (semester_id, day_number, subject_code),
    CONSTRAINT fk_attendance_decision_lesson
        FOREIGN KEY (semester_id, day_number, subject_code)
        REFERENCES lesson (semester_id, day_number, subject_code)
        ON DELETE CASCADE
);

CREATE TABLE question_result
(
    semester_id uuid        NOT NULL,
    day_number  smallint    NOT NULL,
    subject_code varchar(32) NOT NULL,
    was_asked    boolean     NOT NULL,

    CONSTRAINT pk_question_result
        PRIMARY KEY (semester_id, day_number, subject_code),
    CONSTRAINT fk_question_result_lesson
        FOREIGN KEY (semester_id, day_number, subject_code)
        REFERENCES lesson (semester_id, day_number, subject_code)
        ON DELETE CASCADE
);

COMMIT;

create table person
(
    id                  integer not null
        constraint person_pk
            primary key,
    name                varchar,
    firstname           varchar,
    birthdate           timestamp,
    teacher_hiredate    timestamp,
    teacher_salary      integer,
    student_class_id_fk integer,
    student_grade       integer,
    discriminator       varchar,
    gender              integer
);

alter table person
    owner to postgres;

create table class
(
    id            integer not null
        constraint class_pk
            primary key,
    name          varchar,
    teacher_id_fk integer
        constraint teacher_id_fk
            references person
);

alter table class
    owner to postgres;

alter table person
    add constraint class_id_fk
        foreign key (student_class_id_fk) references class;

create table course
(
    id            integer not null
        constraint course_pk
            primary key,
    name          varchar,
    teacher_id_fk integer
        constraint teacher_id_fk
            references person,
    isactive      boolean
);

alter table course
    owner to postgres;

create table student_has_courses
(
    student_id_fk integer
        constraint student_id_fk
            references person,
    course_id_fk  integer
        constraint course_id_fk
            references course
);

alter table student_has_courses
    owner to postgres;


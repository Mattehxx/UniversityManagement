using Microsoft.Identity.Client;
using Microsoft.Identity.Client;
using System.Diagnostics;
using UniversityManagement.Data;

namespace UniversityManagement.Models
{
    public class Mapper
    {
        public CourseGeneralModel MapEntityToGeneralModel(Course entity)
        {
            return new CourseGeneralModel() 
            {
                CourseId = entity.CourseId,
                Title = entity.Title,
                StartYear = entity.StartYear,
                Type = entity.Type,
                StudentNumber = entity.Students == null ? 0 : entity.Students.Count,
                ExamNumber = entity.Exams == null ? 0 : entity.Exams.Count,
                IsActive = (entity.Exams?.Sum(e => e.Credits) >= 180 && !entity.Type) || 
                    (entity.Exams?.Sum(e => e.Credits) >= 120 && entity.Type)
            };
        }

        public CourseDetailModel MapEntityToDetailModel(Course entity)
        {
            return new CourseDetailModel()
            {
                CourseId = entity.CourseId,
                Title = entity.Title,
                StartYear = entity.StartYear,
                Type = entity.Type,
                IsActive = (entity.Exams?.Sum(e => e.Credits) >= 180 && !entity.Type) ||
                    (entity.Exams?.Sum(e => e.Credits) >= 120 && entity.Type),
                Students = entity.Students?.ConvertAll(MapEntityToStudentFromCourseModel),
                Exams = entity.Exams?.ConvertAll(MapEntityToExamFromCourseModel)
            };
        }

        public Course MapDetailModelToEntity(CourseDetailModel model)
        {
            return new Course()
            {
                CourseId = model.CourseId,
                Title = model.Title,
                StartYear = model.StartYear,
                Type = model.Type
            };
        }

        public StudentFromCourseModel MapEntityToStudentFromCourseModel(Student entity)
        {
            return new StudentFromCourseModel()
            {
                StudentId = entity.StudentId,
                Name = entity.Name,
                Surname = entity.Surname,
                BirthDate = entity.BirthDate,
                FC = entity.FC
            };
        }

        public ExamFromCourseModel MapEntityToExamFromCourseModel(Exam entity)
        {
            return new ExamFromCourseModel()
            {
                ExamId = entity.ExamId,
                Title = entity.Title,
                Credits = entity.Credits,
                ProfessorId = entity.ProfessorId,
                ProfessorName = entity.Professor.Name,
                ProfessorSurname = entity.Professor.Surname
            };
        }

        public ExamGeneralModel MapEntityToGeneralModel(Exam entity)
        {
            return new ExamGeneralModel()
            {
                ExamId = entity.ExamId,
                Title = entity.Title,
                Credits = entity.Credits,
                CourseId = entity.CourseId,
                CourseTitle = entity.Course.Title,
                ProfessorId = entity.ProfessorId,
                ProfessorName = entity.Professor.Name,
                ProfessorSurname = entity.Professor.Surname
            };
        }

        public Exam MapGeneralModelToEntity(ExamGeneralModel model)
        {
            return new Exam()
            {
                ExamId = model.ExamId,
                Title = model.Title,
                Credits = model.Credits,
                CourseId = model.CourseId,
                ProfessorId = model.ProfessorId
            };
        }

        public ExamDetailModel MapEntityToDetailModel(Exam entity)
        {
            return new ExamDetailModel()
            {
                ExamId = entity.ExamId,
                Title = entity.Title,
                Credits = entity.Credits,
                CourseId = entity.CourseId,
                CourseTitle = entity.Course.Title,
                CourseStartYear = entity.Course.StartYear,
                CourseType = entity.Course.Type,
                ProfessorId= entity.ProfessorId,
                ProfessorName = entity.Professor.Name,
                ProfessorSurname = entity.Professor.Surname,
                Sessions = entity.Sessions?.ConvertAll(MapEntityToExamSessionFromExamModel)
            };
        }

        public Exam MapDetailModelToEntity(ExamDetailModel model)
        {
            return new Exam()
            {
                ExamId = model.ExamId,
                Title = model.Title,
                Credits = model.Credits,
                CourseId = model.CourseId,
                ProfessorId = model.ProfessorId
            };
        }
        
        public ExamSessionFromExamModel MapEntityToExamSessionFromExamModel(ExamSession entity)
        {
            return new ExamSessionFromExamModel()
            {
                ExamSessionId = entity.ExamSessionId,
                DtSession = entity.DtSession,
                Location = entity.Location
            };
        }

        public ProfessorGeneralModel MapEntityToGeneralModel(Professor entity)
        {
            return new ProfessorGeneralModel()
            {
                ProfessorId = entity.ProfessorId,
                Name = entity.Name,
                Surname = entity.Surname,
                BirthDate = entity.BirthDate,
                FC = entity.FC
            };
        }
            
        public ProfessorDetailModel MapEntityToDetailModel(Professor entity)
        {
            return new ProfessorDetailModel()
            {
                ProfessorId = entity.ProfessorId,
                Name = entity.Name,
                Surname = entity.Surname,
                BirthDate = entity.BirthDate,
                FC = entity.FC,
                Exams = entity.Exams?.ConvertAll(MapEntityToExamFromProfessorModel)
            };
        }

        public Professor MapDetailModelToEntity(ProfessorDetailModel model)
        {
            return new Professor()
            {
                ProfessorId = model.ProfessorId,
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                FC = model.FC,
            };
        }
        
        public ExamFromProfessorModel MapEntityToExamFromProfessorModel(Exam Entity)
        {
            return new ExamFromProfessorModel()
            {
                ExamId = Entity.ExamId,
                Title = Entity.Title,
                Credits = Entity.Credits
            };
        }
        
        public StudentGeneralModel MapEntityToGeneralModel(Student entity)
        {
            return new StudentGeneralModel()
            {
                StudentId = entity.StudentId,
                Name = entity.Name,
                Surname = entity.Surname,
                BirthDate = entity.BirthDate,
                FC= entity.FC,
                CourseId = entity.CourseId,
                CourseTitle = entity.Course.Title
            };
        }

        public Student MapGeneralModelToEntity(StudentGeneralModel model)
        {
            return new Student()
            {
                StudentId = model.StudentId,
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                FC = model.FC,
                CourseId = model.CourseId
            };
        }

        public StudentDetailModel MapEntityToDetailModel(Student entity)
        {
            return new StudentDetailModel()
            {
                StudentId = entity.StudentId,
                Name = entity.Name,
                Surname = entity.Surname,
                BirthDate = entity.BirthDate,
                FC = entity.FC,
                CourseId = entity.CourseId,
                CourseTitle = entity.Course.Title,
                CourseStartYear = entity.Course.StartYear,
                Type = entity.Course.Type,
                Grades = entity.Grades?.ConvertAll(MapEntityToGradesFromStudentModel)
            };
        }

        public Student MapDetailModelToEntity(StudentDetailModel model)
        {
            return new Student()
            {
                StudentId = model.StudentId,
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                FC = model.FC,
                CourseId = model.CourseId
            };
        }

        public GradesFromStudentModel MapEntityToGradesFromStudentModel(Grade entity)
        {
            return new GradesFromStudentModel()
            {
                SessionId = entity.SessionId,
                DtSession = entity.Session.DtSession,
                Location = entity.Session.Location,
                ExamId = entity.Session.ExamId,
                ExamTitle = entity.Session.Exam.Title,
                ExamCredits = entity.Session.Exam.Credits,
                Mark = entity.Mark
            };
        }

        public ExamSessionGeneralModel MapEntityToGeneralModel(ExamSession entity)
        {
            return new ExamSessionGeneralModel()
            {
                ExamSessionId = entity.ExamSessionId,
                DtSession = entity.DtSession,
                Location = entity.Location,
                ExamId = entity.ExamId,
                ExamTitle = entity.Exam.Title,
                ExamCredits = entity.Exam.Credits
            };
        }

        public ExamSessionDetailModel MapEntityToDetailModel(ExamSession entity)
        {
            return new ExamSessionDetailModel()
            {
                ExamSessionId = entity.ExamSessionId,
                DtSession = entity.DtSession,
                Location = entity.Location,
                ExamId = entity.ExamId,
                ExamTitle = entity.Exam.Title,
                ExamCredits = entity.Exam.Credits,
                Grades = entity.Grades?.ConvertAll(MapEntityToGradeFromExamSessionModel)
            };
        }

        public GradeFromExamSessionModel MapEntityToGradeFromExamSessionModel(Grade entity)
        {
            return new GradeFromExamSessionModel()
            {
                StudentId = entity.StudentId,
                StudentName = entity.Student.Name,
                StudentSurname = entity.Student.Surname,
                Mark = entity.Mark
            };
        }

        public ExamSession MapDetailModelToEntity(ExamSessionDetailModel model)
        {
            return new ExamSession()
            {
                ExamSessionId = model.ExamSessionId,
                DtSession = model.DtSession,
                Location = model.Location,
                ExamId = model.ExamId
            };
        }

        public GradeGeneralModel MapEntityToGeneralModel(Grade entity)
        {
            return new GradeGeneralModel()
            {
                StudentId = entity.StudentId,
                SessionId = entity.SessionId,
                Mark = entity.Mark
            };
        }

        public Grade MapGeneralModelToEntity(GradeGeneralModel model)
        {
            return new Grade()
            {
                StudentId = model.StudentId,
                SessionId = model.SessionId,
                Mark = model.Mark
            };
        }

        public StudentBookletModel MapEntityToStudentBookletModel (Student entity)
        {
            return new StudentBookletModel()
            {
                StudentId = entity.StudentId,
                Name = entity.Name,
                Surname = entity.Surname,
                Grades = entity.Grades?.ConvertAll(MapEntityToGradesFromStudentModel)
            };
        }

        public ExamSessionForStudentListModel MapEntityToExamSessionForStudentModel(ExamSession entity)
        {
            return new ExamSessionForStudentListModel()
            {
                ExamSessionId = entity.ExamSessionId,
                GradeList = entity.Grades?.ConvertAll(MapEntityToGradeForStudentModel)
            };
        }

        public GradeForStudentListModel MapEntityToGradeForStudentModel(Grade entity)
        {
            return new GradeForStudentListModel()
            {
                Mark = entity.Mark,
                SessionId = entity.SessionId,
                StudentId = entity.StudentId,
                StudentName = entity.Student.Name,
                StudentSurname = entity.Student.Surname
            };
        }

        public ExamForStudentListModel MapEntityToExamForStudentListModel(Exam entity)
        {
            return new ExamForStudentListModel()
            {
                ExamId = entity.ExamId,
                Title = entity.Title,
                Credits = entity.Credits,
                CourseId = entity.CourseId,
                Sessions = entity.Sessions?.ConvertAll(MapEntityToExamSessionForStudentModel)
            };
        }

        public ProfessorForFutureSessionModel MapEntityToProfessorForFutureSessionModel(Professor entity)
        {
            return new ProfessorForFutureSessionModel()
            {
                ProfessorId = entity.ProfessorId,
                Exams = entity.Exams?.ConvertAll(MapEntityToExamForFutureSessionModel)
            };
        }

        public ExamForFutureSessionModel MapEntityToExamForFutureSessionModel(Exam entity)
        {
            return new ExamForFutureSessionModel()
            {
                ExamId = entity.ExamId,
                Sessions = entity.Sessions?.ConvertAll(MapEntityToGeneralModel)
            };
        }

        public List<Grade> MapGradeSubscriptionMassiveModelToEntities(GradeSubscriptionMassiveModel model)
        {
            List<Grade> grades = model.StudentIds.Select(s => new Grade()
            {
                SessionId = model.SessionId,
                StudentId = s,
                Mark = null
            }).ToList();
            
            return grades;
        }

        public List<Grade> MapGradeMassiveModelToEntities(GradeMassiveModel model)
        {
            List<Grade> grades = model.StudentMarks.Select(s => new Grade()
            {
                SessionId = model.SessionId,
                StudentId = s.StudentId,
                Mark = s.Mark
            }).ToList();

            return grades;
        }

        public List<Exam> MapCourseMassiveExamModelToEntities(CourseMassiveExamModel model)
        {
            List<Exam> exams = model.Exams.Select(s => new Exam()
            {
                ExamId = 0,
                Title = s.Title,
                Credits = s.Credits,
                CourseId = model.CourseId,
                ProfessorId = s.ProfessorId
            }).ToList();

            return exams;
        }

        public StudentExportModel MapEntityToStudentExportModel(Student entity)
        {
            return new StudentExportModel()
            {
                StudentId = entity.StudentId,
                Name = entity.Name,
                Surname = entity.Surname,
                BirthDate = entity.BirthDate,
                FC = entity.FC,
                AverageMark = entity.Grades?.Sum(g => g.Mark * g.Session.Exam.Credits) / entity.Grades?.Sum(g => g.Session.Exam.Credits)
            };
        }

        public ExamExportModel MapEntityToExamExportModel(Exam entity)
        {
            return new ExamExportModel()
            {
                ExamId = entity.CourseId,
                Title = entity.Title,
                Credits = entity.Credits,
                AverageMark = entity.Sessions?.SelectMany(es => es.Grades).Select(g => g.Mark).ToList().Average()
            };
        }

        public ProfessorExportModel MapEntityToProfessorExportModel(Professor entity)
        {
            return new ProfessorExportModel()
            {
                ProfessorId = entity.ProfessorId,
                Name = entity.Name,
                Surname = entity.Surname,
                FC = entity.FC,
                BirthDate = entity.BirthDate,
                AveragePromotedStudent = entity.Exams?
                            .SelectMany(e => e.Sessions
                            .Where(es => es != null && es.DtSession < DateTime.Now)
                            .Select(es => es.Grades?
                            .Where(g => g.Mark != null).Count()))
                            .Average()
            };
        }
    }
}

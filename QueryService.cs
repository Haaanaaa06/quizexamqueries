using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZEXAMSYSTEM
{
 
        public class QueryService
        {
            private readonly CourseManager _courseManager;
            private readonly QuizManager _quizManager;

            public QueryService(CourseManager courseManager, QuizManager quizManager)
            {
                _courseManager = courseManager;
                _quizManager = quizManager;
            }

            // 1) Course names with teacher name and category (courses that have a teacher)
            public List<string> GetCoursesWithTeacherAndCategory()
            {
                return _courseManager.GetAllCourses()
                    .Where(c => c.HasTeacher)
                    .Select(c => string.Format(
                        DisplayFormats.CourseTeacherCategoryFormat,
                        c.Name, c.Teacher.Name, c.Category))
                    .ToList();
            }

            // 2) Quiz names with number of questions
            public List<string> GetQuizzesWithQuestionCount()
            {
                return _quizManager.GetAllQuizzes()
                    .Select(q => string.Format(
                        DisplayFormats.QuizQuestionCountFormat,
                        q.Name, q.QuestionCount))
                    .ToList();
            }

            // 3) All questions of a specific quiz
            public List<string> GetQuestionsOfQuiz(Quiz quiz)
            {
                return quiz.GetQuestions()
                    .Select(q => q.GetDetails())
                    .ToList();
            }

            // 4) Student names with the course they are enrolled in (specific course)
            public List<string> GetStudentsOfCourse(Course course)
            {
                return course.GetStudents()
                    .Select(s => string.Format(
                        DisplayFormats.EnrollmentFormat, s.Name, course.Name))
                    .ToList();
            }

            // 5) Number of quizzes in a specific course
            public int GetQuizCountOfCourse(Course course)
            {
                return course.GetQuizzes().Count();
            }

            // 6) All courses with their teacher, including courses without a teacher
            public List<string> GetAllCoursesWithTeacher()
            {
                return _courseManager.GetAllCourses()
                    .Select(c => string.Format(
                        DisplayFormats.CourseTeacherFormat,
                        c.Name,
                        c.HasTeacher ? c.Teacher.Name : DisplayFormats.NoTeacher))
                    .ToList();
            }

            // 7) Average quiz duration
            public double GetAverageQuizDuration()
            {
                List<Quiz> quizzes = _quizManager.GetAllQuizzes();

                return quizzes.Any()
                    ? quizzes.Average(q => q.Duration)
                    : ValidationRules.NoCount;
            }

            // 8) Average student score per quiz
            public List<string> GetAverageScorePerQuiz()
            {
                return _quizManager.GetAllAttempts()
                    .GroupBy(a => a.Quiz)
                    .Select(g => string.Format(
                        DisplayFormats.AverageScoreFormat,
                        g.Key.Name,
                        g.Average(a => a.Score)))
                    .ToList();
            }

            // 9) Student name, student answer, correct answer, and score per quiz
            public List<string> GetAnswersAndScores()
            {
                return _quizManager.GetAllAttempts()
                    .SelectMany(a => a.StudentAnswers.Select(answer => string.Format(
                        DisplayFormats.AnswerRowFormat,
                        a.Student.Name,
                        a.Quiz.Name,
                        answer.Question.DescribeAnswer(answer.Answer),
                        answer.Question.GetCorrectAnswer(),
                        a.Score)))
                    .ToList();
            }

            // 10) Courses with more than 5 students
            public List<string> GetCoursesWithManyStudents()
            {
                return _courseManager.GetAllCourses()
                    .Where(c => c.StudentCount > ValidationRules.PopularCourseStudents)
                    .Select(c => string.Format(
                        DisplayFormats.CourseLabelFormat, c.Name, c.Category))
                    .ToList();
            }

            // 11) Courses that have quizzes
            public List<string> GetCoursesWithQuizzes()
            {
                return _courseManager.GetAllCourses()
                    .Where(c => c.QuizCount > ValidationRules.NoCount)
                    .Select(c => string.Format(
                        DisplayFormats.CourseLabelFormat, c.Name, c.Category))
                    .ToList();
            }

            // 12) How a specific student performed across quizzes and courses
            public List<string> GetStudentPerformance(Student student)
            {
                return _quizManager.GetAttemptsOfStudent(student)
                    .Select(a => string.Format(
                        DisplayFormats.PerformanceFormat,
                        a.Student.Name,
                        a.Quiz.Course.Name,
                        a.Quiz.Name,
                        a.Score,
                        a.Quiz.GetTotalMarks(),
                        a.GetResult()))
                    .ToList();
            }
        }
    }


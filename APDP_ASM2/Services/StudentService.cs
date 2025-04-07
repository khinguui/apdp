using APDP_ASM2.Interface;
using APDP_ASM2.Models;
using System.Text.Json;

namespace APDP_ASM2.Services
{
    public class StudentService : IStudentService
    {
        private const string FILE_NAME = "student.json";
        private List<Student> _students;

        public StudentService()
        {
            LoadStudents();
        }

        public List<Student> GetAllStudents()
        {
            return _students;
        }

        public Student GetStudentById(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public void AddStudent(Student student)
        {
            _students.Add(student);
            SaveStudents();
        }

        public void UpdateStudent(Student student)
        {
            var existingStudent = _students.FirstOrDefault(s => s.Id == student.Id);
            if (existingStudent != null)
            {
                existingStudent.Name = student.Name;
                existingStudent.DoB = student.DoB;
                existingStudent.Email = student.Email;
                existingStudent.Address = student.Address;
                existingStudent.Major = student.Major;
                SaveStudents();
            }
        }

        public void DeleteStudent(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _students.Remove(student);
                SaveStudents();
            }
        }

        private void LoadStudents()
        {
            if (File.Exists(FILE_NAME))
            {
                string jsonString = File.ReadAllText(FILE_NAME);
                _students = JsonSerializer.Deserialize<List<Student>>(jsonString) ?? new List<Student>();
            }
            else
            {
                _students = new List<Student>();
            }
        }

        private void SaveStudents()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_students, options);
            File.WriteAllText(FILE_NAME, jsonString);
        }
    }
}

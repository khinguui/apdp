using APDP_ASM2.Helpers;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        public IActionResult Delete(int id)
        {
            var teachers = FileHelper.LoadFromFile<List<Teacher>>("teacher.json");
            var searchTeacher = teachers?.FirstOrDefault(t => t.Id == id);

            if (searchTeacher != null)
            {
                teachers.Remove(searchTeacher);
                FileHelper.SaveToFile("teacher.json", teachers);
            }

            return RedirectToAction("ManageTeacher");
        }
        [HttpGet]
        public IActionResult EditTeacher(int id)
        {
            var teachers = FileHelper.LoadFromFile<List<Teacher>>("teacher.json");
            var teacher = teachers.FirstOrDefault(s => s.Id == id);

            if (teacher == null)
            {
                return View("NotFound");
            }

            var courses = FileHelper.LoadFromFile<List<Course>>("course.json");
            var courseExists = courses.Any(c => c.Name == teacher.Course);

            if (!courseExists)
            {
                teacher.Course = "Does not exist";
                ViewBag.CourseDisabled = true;
            }
            else
            {
                ViewBag.CourseDisabled = false;
            }

            PopulateViewBags();
            return View(teacher);
        }

        [HttpPost]
        public IActionResult Save(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return View("EditTeacher", teacher);
            }

            var teachers = FileHelper.LoadFromFile<List<Teacher>>("teacher.json");
            var existingTeacher = teachers.FirstOrDefault(t => t.Id == teacher.Id);
            if (existingTeacher == null) return NotFound();

            existingTeacher.Name = teacher.Name;
            existingTeacher.DoB = teacher.DoB;
            existingTeacher.Course = teacher.Course;

            FileHelper.SaveToFile("teacher.json", teachers);
            return RedirectToAction("ManageTeacher");
        }

        [HttpPost]
        public IActionResult NewTeacher(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var teachers = FileHelper.LoadFromFile<List<Teacher>>("teacher.json") ?? new List<Teacher>();
                teacher.Id = FileHelper.GetNextId(teachers);
                teachers.Add(teacher);
                FileHelper.SaveToFile("teacher.json", teachers);
                return RedirectToAction("ManageTeacher");
            }

            PopulateViewBags();
            return View();
        }

        [HttpGet]
        public IActionResult NewTeacher()
        {
            PopulateViewBags();
            return View();
        }

      
        [HttpGet]
        public IActionResult ManageTeacher(string searchQuery, int page = 1, int pageSize = 5)
        {
            var teachers = FileHelper.LoadFromFile<List<Teacher>>("teacher.json");
            var courses = FileHelper.LoadFromFile<List<Course>>("course.json");
            var courseNames = courses.Select(c => c.Name).ToHashSet();

            foreach (var teacher in teachers)
            {
                if (!courseNames.Contains(teacher.Course))
                {
                    teacher.Course = "Does not exist";
                }
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                teachers = teachers.Where(c =>
                    c.Name.ToLower().Contains(searchQuery) ||
                    c.Course.ToLower().Contains(searchQuery)
                ).ToList();
            }

            var paged = teachers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(teachers.Count / (double)pageSize);
            ViewBag.SearchQuery = searchQuery;

            PopulateViewBags();
            return View(paged);
        }

        private void PopulateViewBags()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.SelectCourse = FileHelper.LoadFromFile<List<Course>>("course.json");
        }
    }
}

using APDP_ASM2.Helpers;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.Json;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        static List<Course> courses = new List<Course>();
        [HttpGet]
        public IActionResult ManageCourse(string searchQuery, int page = 1, int pageSize = 5)
        {
            // Load the courses from the JSON file
            var courses = FileHelper.LoadFromFile<List<Course>>("course.json") ?? new List<Course>();

            // Filter the courses based on the search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                courses = courses.Where(c =>
                    c.Name.ToLower().Contains(searchQuery) ||
                    c.Major.ToLower().Contains(searchQuery)
                ).ToList();
            }

            // Implement pagination
            var pagedCourses = courses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(courses.Count / (double)pageSize);
            ViewBag.SearchQuery = searchQuery; // Pass the search query to the view

            PopulateViewBags();
            return View(pagedCourses);
        }


        public IActionResult Delete(int Id)
        {
            var courses = FileHelper.LoadFromFile<List<Course>>("course.json");
            var searchCourse = courses?.FirstOrDefault(t => t.Id == Id);

            if (searchCourse != null)
            {
                courses.Remove(searchCourse);
                FileHelper.SaveToFile("course.json", courses);
            }

            return RedirectToAction("ManageCourse");
        }

        [HttpPost]
        public IActionResult CreateCourse(Course course, List<Class> classes)
        {
            if (ModelState.IsValid)
            {
                courses = FileHelper.LoadFromFile<List<Course>>("course.json") ?? new List<Course>();
                course.Id = FileHelper.GetNextId(courses);
                courses.Add(course);
                FileHelper.SaveToFile("course.json", courses);
                return RedirectToAction("ManageCourse", new { courses });
            }

            PopulateViewBags();
            return View(course);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            PopulateViewBags();
            return View();
        }

        [HttpPost]
        public IActionResult Save(Course course)
        {
            if (ModelState.IsValid)
            {
                // Load the courses from the JSON file
                var courses = FileHelper.LoadFromFile<List<Course>>("course.json");

                var existingCourse = courses.FirstOrDefault(t => t.Id == course.Id);
                if (existingCourse == null) return NotFound();

                // Update the course details
                existingCourse.Name = course.Name;
                existingCourse.Class = course.Class;
                existingCourse.Major = course.Major;
                existingCourse.Status = course.Status;

                // Save the updated list back to the JSON file
                FileHelper.SaveToFile("course.json", courses);

                return RedirectToAction("ManageCourse");
            }

            PopulateViewBags();
            return View("EditCourse", course);
        }


        public IActionResult EditCourse(int Id)
        {
            // Load the courses from the JSON file
            var courses = FileHelper.LoadFromFile<List<Course>>("course.json");

            // Find the course by Id
            var course = courses?.FirstOrDefault(s => s.Id == Id);

            if (course == null)
            {
                return View("NotFound");  // or return RedirectToAction("ManageCourse") if you prefer
            }

            PopulateViewBags();
            return View("EditCourse", course);
        }


        [HttpPost]
        public IActionResult EditCourse(Course course, List<Class> classes)
        {
            if (ModelState.IsValid)
            {
                var existingCourse = courses.FirstOrDefault(t => t.Id == course.Id);
                if (existingCourse == null) return NotFound();

                existingCourse.Name = course.Name;
                existingCourse.Class = course.Class;
                existingCourse.Major = course.Major;
                existingCourse.Status = course.Status;

                FileHelper.SaveToFile("course.json", courses);
                return RedirectToAction("ManageCourse");
            }

            PopulateViewBags();
            return View("EditCourse", course);
        }

        private void PopulateViewBags()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.SelectTeacher = FileHelper.LoadFromFile<List<Teacher>>("teacher.json");
            ViewBag.SelectClass = FileHelper.LoadFromFile<List<Class>>("class.json");
        }

    }

}
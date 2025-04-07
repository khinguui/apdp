using APDP_ASM2.Helpers;
using APDP_ASM2.Interface;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEmailValidator _emailValidator;

        public StudentController(IStudentService studentService, IEmailValidator emailValidator)
        {
            _studentService = studentService;
            _emailValidator = emailValidator;
        }

        public IActionResult Delete(int id)
        {
            _studentService.DeleteStudent(id);
            return RedirectToAction("ManageStudent");
        }


        [HttpGet]
        public IActionResult ManageStudent(string searchQuery, int page = 1, int pageSize = 5)
        {
            
            var students = _studentService.GetAllStudents();

          
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                students = students.Where(c =>
                    c.Name.ToLower().Contains(searchQuery) ||
                    c.Major.ToLower().Contains(searchQuery) ||
                    c.Email.ToLower().Contains(searchQuery) 
                   
                ).ToList();
            }

            // Implement pagination
            var paged = students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(students.Count / (double)pageSize);
            ViewBag.SearchQuery = searchQuery; // Pass the search query to the view

            PopulateViewBags();
            return View(paged);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (student == null) return BadRequest("Student object cannot be null.");

            if (ModelState.IsValid && _emailValidator.IsValid(student.Email))
            {
                _studentService.UpdateStudent(student);
                return RedirectToAction("ManageStudent");
            }

            return View(student);
        }

        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null)
            {
                return View("NotFound");
            }
            else
            {
                PopulateViewBags();
                return View(student);
            }
        }

        [HttpPost]
        public IActionResult Save(Student student)
        {
            if (ModelState.IsValid && _emailValidator.IsValid(student.Email))
            {
                _studentService.UpdateStudent(student);
                return RedirectToAction("ManageStudent");
            }

            return View("EditStudent", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewStudent(Student student)
        {
            if (ModelState.IsValid && _emailValidator.IsValid(student.Email))
            {
                var students = _studentService.GetAllStudents() ?? new List<Student>();
                student.Id = FileHelper.GetNextId(students);
                _studentService.AddStudent(student);
                return RedirectToAction("ManageStudent");
            }

            return View(student);
        }

        [HttpGet]
        public IActionResult NewStudent() => View();

        public IActionResult Index()
        {
            PopulateViewBags();
            var students = _studentService.GetAllStudents();
            return View(students);
        }

        private void PopulateViewBags()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
        }
    }

}

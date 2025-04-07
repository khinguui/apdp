using APDP_ASM2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using APDP_ASM2.Models;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using APDP_ASM2.Helpers;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class ClassController : Controller
    {
        static List<Class> classes = new List<Class>();

        public IActionResult ViewClass()
        {
            PopulateViewBags();
            classes = FileHelper.LoadFromFile<List<Class>>("class.json");
            return View(classes);
        }

        [HttpGet]
        public IActionResult ManageClass(string searchQuery, int page = 1, int pageSize = 5)
        {
          
            var Class = FileHelper.LoadFromFile<List<Class>>("class.json") ?? new List<Class>();

            // Filter the courses based on the search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                Class = Class.Where(c =>
                    c.ClassName.ToLower().Contains(searchQuery) ||
                    c.Major.ToLower().Contains(searchQuery) ||
                    c.Lecturer.ToLower().Contains(searchQuery)
                ).ToList();
            }

            // Implement pagination
            var paged = Class
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(Class.Count / (double)pageSize);
            ViewBag.SearchQuery = searchQuery; // Pass the search query to the view

            PopulateViewBags();
            return View(paged);
        }


        public IActionResult Delete(int Id)
        {
            var classes = FileHelper.LoadFromFile<List<Class>>("class.json");
            var searchClass = classes?.FirstOrDefault(t => t.Id == Id);

            if (searchClass != null)
            {
                classes.Remove(searchClass);
                FileHelper.SaveToFile("class.json", classes);
            }

            return RedirectToAction("ManageClass");
        }

        [HttpPost]
        public IActionResult NewClass(Class @class)
        {
            if (ModelState.IsValid)
            {
                classes = FileHelper.LoadFromFile<List<Class>>("class.json") ?? new List<Class>();
                @class.Id = FileHelper.GetNextId(classes);
                classes.Add(@class);
                FileHelper.SaveToFile("class.json", classes);
                return RedirectToAction("ManageClass", new { classes });
            }

            PopulateViewBags();
            return View();
        }

        [HttpGet]
        public IActionResult NewClass()
        {
            PopulateViewBags();
            return View();
        }

        [HttpGet]
        public IActionResult EditClass(int id)
        {
            var classes = FileHelper.LoadFromFile<List<Class>>("class.json") ?? new List<Class>();
            var classToEdit = classes.FirstOrDefault(s => s.Id == id);
            if (classToEdit == null)
            {
                return View("NotFound");
            }
            else
            {
                PopulateViewBags();
                return View(classToEdit);
            }
         
        }

        [HttpPost]
        public IActionResult Save(Class @class)
        {
            var classes = FileHelper.LoadFromFile<List<Class>>("class.json") ?? new List<Class>();
            var existingClass = classes.FirstOrDefault(t => t.Id == @class.Id);
            if (existingClass == null) return NotFound();

            if (ModelState.IsValid)
            {
                existingClass.ClassName = @class.ClassName;
                existingClass.Major = @class.Major;
                existingClass.Lecturer = @class.Lecturer;
                FileHelper.SaveToFile("class.json", classes);
                return RedirectToAction("ManageClass");
            }

            PopulateViewBags();
            return View("EditClass", @class);
        }

        private void PopulateViewBags()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.SelectTeacher = FileHelper.LoadFromFile<List<Teacher>>("teacher.json");
        }
    }
}

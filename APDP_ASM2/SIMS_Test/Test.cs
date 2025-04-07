using APDP_ASM2.Controllers;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace SIMS_Test
{
    public class CourseControllerTests
    {
       
        [Fact]
        public void TestCreateCourse()
        {
            // Arrange
            var controller = new CourseController();
            var courseToAdd = new Course
            {
                Id = 1,
                Name = "New Course",
                Class = "SE1234",
                Major = "IT",
                Lecturer = "John Doe",
                Status = "Active"
            };
            var classes = new List<Class> { new Class { ClassName = "SE1234" } };

            // Act
            var result = controller.CreateCourse(courseToAdd, classes) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.Equals("ManageCourse", result.ActionName);
        }
       
        [Fact]
        public void TestDeleteCourse()
        {
            // Arrange
            var controller = new CourseController();
            var idToDelete = 1;
            var fileName = "course.json";

            // Create initial course data
            var initialCourses = new List<Course>
            {
                new Course { Id = 1, Name = "Design", Class = "SE06301", Major = "IT", Lecturer = "test", Status = "Active" },
                new Course { Id = 2, Name = "Computing", Class = "SE06103", Major = "IT", Lecturer = "Nguyen Thanh Trieu", Status = "Active" }
            };
            var initialJson = JsonSerializer.Serialize(initialCourses);
            System.IO.File.WriteAllText(fileName, initialJson);

            // Act
            var result = controller.Delete(idToDelete) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.Equals("ManageCourse", result.ActionName);

            // Verify the course was actually deleted
            var remainingCoursesJson = System.IO.File.ReadAllText(fileName);
            var remainingCourses = JsonSerializer.Deserialize<List<Course>>(remainingCoursesJson);

            Assert.ContainsSingle(remainingCourses);
          
        }
    }
}
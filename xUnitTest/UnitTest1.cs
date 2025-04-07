using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using APDP_ASM2.Controllers;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using System.IO;
using System.Text.Json;
using APDP_ASM2.Helpers;

namespace APDP_ASM2.Tests
{
    public class ClassControllerTests
    {
        private readonly ClassController _controller;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ISession> _mockSession;

        public ClassControllerTests()
        {
            _mockHttpContext = new Mock<HttpContext>();
            _mockSession = new Mock<ISession>();

            // Setup session to return test strings
            _mockSession.Setup(s => s.TryGetValue("UserName", out It.Ref<byte[]>.IsAny))
              .Returns((string key, out byte[] val) =>
                 {
                     val = System.Text.Encoding.UTF8.GetBytes("testuser");
                     return true;
                 });

            _mockSession.Setup(s => s.TryGetValue("Role", out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] val) =>
                {
                    val = System.Text.Encoding.UTF8.GetBytes("Admin");
                    return true;
                });

            _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

            _controller = new ClassController();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object
            };

            // Create mock class.json file for tests
            var mockClasses = new List<Class>
            {
                new Class { Id = 1, ClassName = "Math", Major = "Science", Lecturer = "Mr. A" },
                new Class { Id = 2, ClassName = "Literature", Major = "Arts", Lecturer = "Ms. B" }
            };
            FileHelper.SaveToFile("class.json", mockClasses);
        }

        [Fact]
        public void ViewClass_ShouldReturnViewWithListOfClasses()
        {
            var result = _controller.ViewClass();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Class>>(viewResult.Model);
            Assert.NotEmpty(model);
        }

        [Fact]
        public void ManageClass_WithSearchQuery_ShouldFilterResults()
        {
            var result = _controller.ManageClass("math");
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Class>>(viewResult.Model);
            Assert.Single(model);
            Assert.Contains(model, c => c.ClassName.ToLower().Contains("math"));
        }

        [Fact]
        public void ManageClass_Pagination_ShouldReturnCorrectPage()
        {
            var result = _controller.ManageClass(null, 1, 1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Class>>(viewResult.Model);
            Assert.Single(model); // pageSize = 1
        }

        [Fact]
        public void Delete_ShouldRemoveClassAndRedirect()
        {
            var result = _controller.Delete(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageClass", redirect.ActionName);

            var classes = FileHelper.LoadFromFile<List<Class>>("class.json");
            Assert.DoesNotContain(classes, c => c.Id == 1);
        }

        [Fact]
        public void NewClass_Get_ShouldReturnView()
        {
            var result = _controller.NewClass();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void NewClass_Post_ValidModel_ShouldRedirect()
        {
            var newClass = new Class { ClassName = "Physics", Major = "Science", Lecturer = "Mr. C" };
            _controller.ModelState.Clear(); // ensure model is valid

            var result = _controller.NewClass(newClass);


            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageClass", redirect.ActionName);
        }

        [Fact]
        public void NewClass_Post_InvalidModel_ShouldReturnView()
        {
            var invalidClass = new Class { ClassName = "", Major = "", Lecturer = "" };

            _controller.ModelState.AddModelError("ClassName", "Required");

            var result = _controller.NewClass(invalidClass);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EditClass_ValidId_ShouldReturnViewWithModel()
        {
            var result = _controller.EditClass(2);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Class>(viewResult.Model);
            Assert.Equal(2, model.Id);
        }

        [Fact]
        public void EditClass_InvalidId_ShouldReturnNotFound()
        {
            var result = _controller.EditClass(999);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("NotFound", viewResult.ViewName);
        }

        [Fact]
        public void Save_ValidClass_ShouldUpdateAndRedirect()
        {
            var updatedClass = new Class { Id = 2, ClassName = "Updated", Major = "UpdatedMajor", Lecturer = "UpdatedLec" };

            _controller.ModelState.Clear();

            var result = _controller.Save(updatedClass);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("ManageClass", redirect.ActionName);

            var classes = FileHelper.LoadFromFile<List<Class>>("class.json");
            var updated = classes.FirstOrDefault(c => c.Id == 2);
            Assert.Equal("Updated", updated.ClassName);
        }

        [Fact]
        public void Save_InvalidClass_ShouldReturnEditView()
        {
            var invalidClass = new Class { Id = 2 };
            _controller.ModelState.AddModelError("ClassName", "Required");

            var result = _controller.Save(invalidClass);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditClass", viewResult.ViewName);
        }
    }
}

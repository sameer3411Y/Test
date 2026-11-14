using Microsoft.AspNetCore.Mvc;
using Test.Controller;
using Test.Models;
using Test.Tests.Helpers;
using Xunit;

namespace Test.Tests.Controllers
{
    public class StudentControllerCreateTests
    {
        // ------------------------------------------------------------------ tests

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithNewStudent()
        {
            var context    = DbContextFactory.Create(nameof(Create_ReturnsCreatedAtAction_WithNewStudent));
            var controller = new StudentController(context);
            var student    = StudentFactory.Valid();

            var result = await controller.Create(student);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(controller.GetById), created.ActionName);

            var returned = Assert.IsType<Student>(created.Value);
            Assert.True(returned.Id > 0);
            Assert.Equal("John", returned.FirstName);
        }

        [Fact]
        public async Task Create_PersistsStudentToDatabase()
        {
            var context    = DbContextFactory.Create(nameof(Create_PersistsStudentToDatabase));
            var controller = new StudentController(context);

            await controller.Create(StudentFactory.Valid());

            Assert.Equal(1, context.Students.Count());
        }

        [Fact]
        public async Task Create_SetsCreatedAtToUtcNow()
        {
            var before     = DateTime.UtcNow.AddSeconds(-1);
            var context    = DbContextFactory.Create(nameof(Create_SetsCreatedAtToUtcNow));
            var controller = new StudentController(context);

            var result  = await controller.Create(StudentFactory.Valid());
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var student = Assert.IsType<Student>(created.Value);

            Assert.True(student.CreatedAt >= before);
            Assert.True(student.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var context    = DbContextFactory.Create(nameof(Create_ReturnsBadRequest_WhenModelStateIsInvalid));
            var controller = new StudentController(context);
            controller.ModelState.AddModelError("FirstName", "Required");

            var result = await controller.Create(StudentFactory.Valid());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WithCorrectRouteValues()
        {
            var context    = DbContextFactory.Create(nameof(Create_ReturnsCreated_WithCorrectRouteValues));
            var controller = new StudentController(context);
            var student    = StudentFactory.Valid();

            var result = await controller.Create(student);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(created.RouteValues);
            Assert.True(created.RouteValues!.ContainsKey("id"));
        }
    }
}

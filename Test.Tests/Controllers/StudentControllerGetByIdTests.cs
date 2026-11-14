using Microsoft.AspNetCore.Mvc;
using Test.Controller;
using Test.Models;
using Test.Tests.Helpers;
using Xunit;

namespace Test.Tests.Controllers
{
    public class StudentControllerGetByIdTests
    {
        // ------------------------------------------------------------------ tests

        [Fact]
        public async Task GetById_ReturnsOk_WithStudent_WhenIdExists()
        {
            var context = DbContextFactory.Create(nameof(GetById_ReturnsOk_WithStudent_WhenIdExists));
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            var result = await controller.GetById(student.Id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<Student>(ok.Value);
            Assert.Equal(student.Id, returned.Id);
            Assert.Equal("John", returned.FirstName);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var controller = new StudentController(
                DbContextFactory.Create(nameof(GetById_ReturnsNotFound_WhenIdDoesNotExist)));

            var result = await controller.GetById(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsInactiveStudent_WhenItExists()
        {
            // GetById does NOT filter by IsActive – only GetAll does
            var context = DbContextFactory.Create(nameof(GetById_ReturnsInactiveStudent_WhenItExists));
            var student = StudentFactory.Valid();
            student.IsActive = false;
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            var result = await controller.GetById(student.Id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<Student>(ok.Value);
            Assert.False(returned.IsActive);
        }

        [Fact]
        public async Task GetById_NotFound_ContainsMessageWithId()
        {
            var controller = new StudentController(
                DbContextFactory.Create(nameof(GetById_NotFound_ContainsMessageWithId)));

            var result = await controller.GetById(42);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFound.Value);
            var message = notFound.Value!.ToString();
            Assert.Contains("42", message);
        }
    }
}

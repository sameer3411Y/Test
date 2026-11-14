using Microsoft.AspNetCore.Mvc;
using Test.Controller;
using Test.Models;
using Test.Tests.Helpers;
using Xunit;

namespace Test.Tests.Controllers
{
    public class StudentControllerUpdateTests
    {
        // ------------------------------------------------------------------ helpers

        private static async Task<(StudentController controller, Student seeded)>
            SeedAndCreateAsync(string dbName)
        {
            var context = DbContextFactory.Create(dbName);
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();
            return (new StudentController(context), student);
        }

        // ------------------------------------------------------------------ tests

        [Fact]
        public async Task Update_ReturnsNoContent_WhenStudentExists()
        {
            var (controller, seeded) = await SeedAndCreateAsync(
                nameof(Update_ReturnsNoContent_WhenStudentExists));

            var updated = StudentFactory.Valid(
                id: seeded.Id,
                firstName: "Jane",
                email: "jane.doe@example.com");

            var result = await controller.Update(seeded.Id, updated);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_PersistsChangesToDatabase()
        {
            var context = DbContextFactory.Create(nameof(Update_PersistsChangesToDatabase));
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            var updated = StudentFactory.Valid(
                id: student.Id,
                firstName: "UpdatedName",
                email: "updated@example.com");

            await controller.Update(student.Id, updated);

            var fromDb = await context.Students.FindAsync(student.Id);
            Assert.Equal("UpdatedName", fromDb!.FirstName);
            Assert.Equal("updated@example.com", fromDb.Email);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenStudentDoesNotExist()
        {
            var context    = DbContextFactory.Create(nameof(Update_ReturnsNotFound_WhenStudentDoesNotExist));
            var controller = new StudentController(context);
            var student    = StudentFactory.Valid(id: 99);

            var result = await controller.Update(99, student);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var (controller, seeded) = await SeedAndCreateAsync(
                nameof(Update_ReturnsBadRequest_WhenIdMismatch));

            var student = StudentFactory.Valid(id: seeded.Id);

            var result = await controller.Update(seeded.Id + 1, student);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var (controller, seeded) = await SeedAndCreateAsync(
                nameof(Update_ReturnsBadRequest_WhenModelStateIsInvalid));

            controller.ModelState.AddModelError("Email", "Invalid");
            var student = StudentFactory.Valid(id: seeded.Id);

            var result = await controller.Update(seeded.Id, student);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_CanChangeDepartment()
        {
            var context = DbContextFactory.Create(nameof(Update_CanChangeDepartment));
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            var updated = StudentFactory.Valid(id: student.Id);
            updated.Department = "Mathematics";

            await controller.Update(student.Id, updated);

            var fromDb = await context.Students.FindAsync(student.Id);
            Assert.Equal("Mathematics", fromDb!.Department);
        }
    }
}

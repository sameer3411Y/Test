using Microsoft.AspNetCore.Mvc;
using Test.Controller;
using Test.Tests.Helpers;
using Xunit;

namespace Test.Tests.Controllers
{
    public class StudentControllerDeleteTests
    {
        // ------------------------------------------------------------------ helpers

        private static async Task<(StudentController controller, int id)>
            SeedAndCreateAsync(string dbName)
        {
            var context = DbContextFactory.Create(dbName);
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();
            return (new StudentController(context), student.Id);
        }

        // ------------------------------------------------------------------ tests

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenStudentExists()
        {
            var (controller, id) = await SeedAndCreateAsync(
                nameof(Delete_ReturnsNoContent_WhenStudentExists));

            var result = await controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_SoftDeletes_SetsIsActiveFalse()
        {
            var context = DbContextFactory.Create(nameof(Delete_SoftDeletes_SetsIsActiveFalse));
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            await controller.Delete(student.Id);

            var fromDb = await context.Students.FindAsync(student.Id);
            Assert.NotNull(fromDb);           // record still exists in DB
            Assert.False(fromDb!.IsActive);   // but marked inactive
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenStudentDoesNotExist()
        {
            var controller = new StudentController(
                DbContextFactory.Create(nameof(Delete_ReturnsNotFound_WhenStudentDoesNotExist)));

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_SoftDeleted_StudentDoesNotAppearInGetAll()
        {
            var context = DbContextFactory.Create(nameof(Delete_SoftDeleted_StudentDoesNotAppearInGetAll));
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            await controller.Delete(student.Id);

            var getAll = await controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(getAll.Result);
            var students = Assert.IsAssignableFrom<IEnumerable<Test.Models.Student>>(ok.Value).ToList();
            Assert.Empty(students);
        }

        [Fact]
        public async Task Delete_DoesNotRemoveRecord_FromDatabase()
        {
            var context = DbContextFactory.Create(nameof(Delete_DoesNotRemoveRecord_FromDatabase));
            var student = StudentFactory.Valid();
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            await controller.Delete(student.Id);

            Assert.Equal(1, context.Students.Count());
        }
    }
}

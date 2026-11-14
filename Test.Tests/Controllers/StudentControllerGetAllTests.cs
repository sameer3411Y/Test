using Microsoft.AspNetCore.Mvc;
using Test.Controller;
using Test.Models;
using Test.Tests.Helpers;
using Xunit;

namespace Test.Tests.Controllers
{
    public class StudentControllerGetAllTests
    {
        // ------------------------------------------------------------------ helpers

        private static StudentController CreateController(string dbName)
            => new StudentController(DbContextFactory.Create(dbName));

        // ------------------------------------------------------------------ tests

        [Fact]
        public async Task GetAll_ReturnsOk_WithEmptyList_WhenNoStudentsExist()
        {
            var controller = CreateController(nameof(GetAll_ReturnsOk_WithEmptyList_WhenNoStudentsExist));

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var students = Assert.IsAssignableFrom<IEnumerable<Student>>(ok.Value);
            Assert.Empty(students);
        }

        [Fact]
        public async Task GetAll_ReturnsOnlyActiveStudents()
        {
            var context = DbContextFactory.Create(nameof(GetAll_ReturnsOnlyActiveStudents));
            context.Students.Add(StudentFactory.Valid(email: "active@test.com", studentCode: "A001"));
            var inactive = StudentFactory.Valid(email: "inactive@test.com", studentCode: "A002");
            inactive.IsActive = false;
            context.Students.Add(inactive);
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var students = Assert.IsAssignableFrom<IEnumerable<Student>>(ok.Value).ToList();
            Assert.Single(students);
            Assert.Equal("active@test.com", students[0].Email);
        }

        [Fact]
        public async Task GetAll_ReturnsAllActiveStudents_WhenMultipleExist()
        {
            var context = DbContextFactory.Create(nameof(GetAll_ReturnsAllActiveStudents_WhenMultipleExist));
            context.Students.AddRange(
                StudentFactory.Valid(email: "s1@test.com", studentCode: "B001"),
                StudentFactory.Valid(email: "s2@test.com", studentCode: "B002"),
                StudentFactory.Valid(email: "s3@test.com", studentCode: "B003")
            );
            await context.SaveChangesAsync();

            var controller = new StudentController(context);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var students = Assert.IsAssignableFrom<IEnumerable<Student>>(ok.Value).ToList();
            Assert.Equal(3, students.Count);
        }
    }
}

using Test.Models;

namespace Test.Tests.Helpers
{
    /// <summary>
    /// Builds valid Student objects for use across tests.
    /// </summary>
    public static class StudentFactory
    {
        public static Student Valid(
            int id = 0,
            string firstName   = "John",
            string lastName    = "Doe",
            string email       = "john.doe@example.com",
            string studentCode = "STU001",
            int    enrollYear  = 2023,
            string department  = "Computer Science") => new Student
        {
            Id             = id,
            FirstName      = firstName,
            LastName       = lastName,
            Email          = email,
            StudentCode    = studentCode,
            EnrollmentYear = enrollYear,
            Department     = department,
            IsActive       = true,
            CreatedAt      = DateTime.UtcNow
        };
    }
}

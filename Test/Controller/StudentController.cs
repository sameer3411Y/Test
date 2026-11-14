using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAll()
        {
            var students = await _context.Students
                .Where(s => s.IsActive)
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/student/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Student>> GetById(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound(new { message = $"Student with id {id} not found." });

            return Ok(student);
        }

        // POST: api/student
        [HttpPost]
        public async Task<ActionResult<Student>> Create([FromBody] Student student)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            student.CreatedAt = DateTime.UtcNow;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        // PUT: api/student/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Student student)
        {
            if (id != student.Id)
                return BadRequest(new { message = "ID in URL does not match ID in body." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Students.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Student with id {id} not found." });

            existing.FirstName      = student.FirstName;
            existing.LastName       = student.LastName;
            existing.Email          = student.Email;
            existing.StudentCode    = student.StudentCode;
            existing.EnrollmentYear = student.EnrollmentYear;
            existing.Department     = student.Department;
            existing.IsActive       = student.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/student/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound(new { message = $"Student with id {id} not found." });

            // Soft delete
            student.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

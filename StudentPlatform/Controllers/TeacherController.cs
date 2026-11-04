using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPlatform.Data;
using StudentPlatform.Models;

namespace StudentPlatform.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var lectures = await _context.Lectures
                .Include(l => l.Assignments)
                .ToListAsync();
            return View(lectures);
        }

        [HttpGet]
        public IActionResult CreateLecture()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLecture(Lecture lecture, IFormFile? lectureFile)
        {
            if (lectureFile != null && lectureFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assignments");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + lectureFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await lectureFile.CopyToAsync(fileStream);
                }

                lecture.FilePath = "/assignments/" + uniqueFileName;
            }

            _context.Lectures.Add(lecture);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Teacher");
        }

        [HttpGet]
        public async Task<IActionResult> ViewSubmissions(int assignmentId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Lecture)
                .Include(a => a.Submissions)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> GradeSubmission(int submissionId, string grade)
        {
            var submission = await _context.StudentSubmissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.Grade = grade;
                _context.Update(submission);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ViewSubmissions), new { assignmentId = submission.AssignmentId });
        }
    }
}
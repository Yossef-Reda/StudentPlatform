using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPlatform.Data;
using StudentPlatform.Models;

namespace StudentPlatform.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int lectureId)
        {
            ViewBag.LectureId = lectureId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Assignment assignment, IFormFile? assignmentFile)
        {
            if (assignmentFile != null && assignmentFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assignments");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + assignmentFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await assignmentFile.CopyToAsync(fileStream);
                }

                assignment.FilePath = "/assignments/" + uniqueFileName;
            }

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Teacher");
        }
    }
}
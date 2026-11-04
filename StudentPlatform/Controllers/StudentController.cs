using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPlatform.Data;
using StudentPlatform.Models;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public StudentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var lectures = await _context.Lectures
            .Include(l => l.Assignments)
            .ThenInclude(a => a.Submissions) 
            .ToListAsync();

        return View(lectures);
    }
    [HttpPost]
    public async Task<IActionResult> SubmitAssignment(int assignmentId, string studentName, IFormFile submissionFile)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (submissionFile != null && submissionFile.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "submissions");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + submissionFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await submissionFile.CopyToAsync(fileStream);
            }
            var submission = new StudentSubmission
            {
                AssignmentId = assignmentId,
                StudentId = studentId ?? "Unknown",
                StudentName = studentName,
                FilePath = "/submissions/" + uniqueFileName,
                SubmittedAt = DateTime.Now
            };

            _context.StudentSubmissions.Add(submission);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Student");
    }
}
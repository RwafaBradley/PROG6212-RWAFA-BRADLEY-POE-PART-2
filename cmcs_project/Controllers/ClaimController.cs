
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CMCS.Controllers;
[Authorize]
public class ClaimController : Controller
{
    private readonly IClaimRepository _repo;
    private readonly IWebHostEnvironment _env;
    public ClaimController(IClaimRepository repo, IWebHostEnvironment env) { _repo = repo; _env = env; }

    [Authorize(Roles = "Lecturer")]
    public IActionResult Create() => View(new Claim { Lecturer = User.Identity?.Name ?? "" });

    [HttpPost]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Create(Claim model, IFormFile? file)
    {
        if (!ModelState.IsValid) return View(model);
        if (file != null && file.Length > 0)
        {
           
            var maxBytes = 5 * 1024 * 1024; // 5MB
            var allowedExt = new[] { ".pdf", ".docx", ".xlsx" };
            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
            if (file.Length > maxBytes || !allowedExt.Contains(ext))
            {
                ModelState.AddModelError("", "Invalid file. Ensure it's .pdf/.docx/.xlsx and under 5MB.");
                return View(model);
            }

            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            var unique = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var dest = Path.Combine(uploads, unique);
            using var fs = new FileStream(dest, FileMode.Create);
            await file.CopyToAsync(fs);
            model.UploadedFileName = unique;
        }
        _repo.Add(model);
        TempData["Message"] = "Claim submitted successfully.";
        return RedirectToAction("MyClaims");
    }

    [Authorize(Roles = "Lecturer")]
    public IActionResult MyClaims() { var claims = _repo.GetByLecturer(User.Identity?.Name ?? ""); return View(claims); }

    public IActionResult Download(Guid id)
    {
        var c = _repo.Get(id);
        if (c == null || string.IsNullOrEmpty(c.UploadedFileName)) return NotFound();
        var path = Path.Combine(_env.WebRootPath, "uploads", c.UploadedFileName);
        if (!System.IO.File.Exists(path)) return NotFound();
        var mime = "application/octet-stream";
        return PhysicalFile(path, mime, Path.GetFileName(path));
    }
}

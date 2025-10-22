
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMCS.Services;
using CMCS.Models;

namespace CMCS.Controllers;
[Authorize(Roles = "Coordinator,Manager")]
public class AdminController : Controller
{
    private readonly IClaimRepository _repo;
    public AdminController(IClaimRepository repo) => _repo = repo;

    public IActionResult Index() => View(_repo.GetAll());

    public IActionResult Details(Guid id) { var c = _repo.Get(id); if (c == null) return NotFound(); return View(c); }

    [HttpPost]
    public IActionResult Approve(Guid id) { _repo.UpdateStatus(id, ClaimStatus.Approved); return RedirectToAction("Index"); }

    [HttpPost]
    public IActionResult Reject(Guid id) { _repo.UpdateStatus(id, ClaimStatus.Rejected); return RedirectToAction("Index"); }
}

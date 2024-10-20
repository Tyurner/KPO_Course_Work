using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SuccessfulAdmission.DataLogic.Services;
using SuccessfulAdmission.Web.Models;

namespace SuccessfulAdmission.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FacultyService _facultyService = new FacultyService();
    private readonly SpecialityService _specialityService = new SpecialityService();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Faculties()
    {
        return View(_facultyService.GetAllFaculties());
    }
    
    [HttpGet]
    public IActionResult FacultyCreate()
    {
        return View();
    }
    [HttpPost]
    public IActionResult FacultyCreate(string name, string? desc)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите название";
            return View();
        }

        _facultyService.AddFaculty(name, desc ?? string.Empty);
        return RedirectToAction("Faculties");
    }
    [HttpGet]
    public IActionResult FacultySetting(int id)
    {
        return View(_facultyService.GetFacultyById(id));
    }
    [HttpPost]
    public IActionResult UpdateFaculty(int id, string name, string? desc)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите название";
            return RedirectToAction("FacultySetting",  new { id });
        }
        _facultyService.UpdateFaculty(id, name, desc  ?? string.Empty);
        return RedirectToAction("Faculties");
    }

    public void DeleteFaculty(int id)
    {
        _facultyService.DeleteFaculty(id);
        Response.Redirect("/Home/Faculties");
    }
    
    [HttpGet]
    public IActionResult Specialities()
    {
        return View(_specialityService.GetAllSpecialities());
    }
    
    [HttpGet]
    public IActionResult SpecialityCreate()
    {
        ViewBag.Faculties = _facultyService.GetAllFaculties();
        return View();
    }
    [HttpPost]
    public IActionResult SpecialityCreate(string name, string? desc, string countStr, int? facultyId)
    {
        ViewBag.Faculties = _facultyService.GetAllFaculties();
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите название";
            return View();
        }
        
        if (!int.TryParse(countStr, out int count) || count < 0)
        {
            TempData["ErrorMessage"] = "Количество мест должно быть числом и не может быть отрицательным";
            return View();
        }

        _specialityService.AddSpeciality(name, desc ?? string.Empty, count, facultyId);
        return RedirectToAction("Specialities");
    }
    
    [HttpGet]
    public IActionResult SpecialitySetting(int id)
    {
        ViewBag.Faculties = _facultyService.GetAllFaculties();
        return View(_specialityService.GetSpecialityById(id));
    }
    [HttpPost]
    public IActionResult UpdateSpeciality(int id, string name, string? desc, string countStr, int? facultyId)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите название";
            return RedirectToAction("SpecialitySetting",  new { id });
        }
        if (!int.TryParse(countStr, out int count) || count < 0)
        {
            TempData["ErrorMessage"] = "Количество мест должно быть числом и не может быть отрицательным";
            return RedirectToAction("SpecialitySetting",  new { id });
        }
        _specialityService.UpdateSpeciality(id, name, desc ?? string.Empty, count, facultyId);
        return RedirectToAction("Specialities");
    }

    public void DeleteSpeciality(int id)
    {
        _specialityService.DeleteSpeciality(id);
        Response.Redirect("/Home/Specialities");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
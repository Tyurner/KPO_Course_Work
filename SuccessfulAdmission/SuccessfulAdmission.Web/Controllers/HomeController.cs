using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using SuccessfulAdmission.DataLogic.Services;
using SuccessfulAdmission.Web.Models;

namespace SuccessfulAdmission.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FacultyService _facultyService = new FacultyService();
    private readonly SpecialityService _specialityService = new SpecialityService();
    private readonly ApplicantService _applicantService = new ApplicantService();
    private readonly SubjectService _subjectService = new SubjectService();
    private readonly UserService _userService = new UserService();
    private readonly ListApplicantsService _listService = new ListApplicantsService();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View();
    }
    
    [HttpGet]
    public IActionResult Enter()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Enter(string login, string password)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            TempData["ErrorMessage"] = "Введите логин и пароль";
            return View();
        }
        ApiClient.Client = _userService.GetUserByLoginPassword(login, password);
        if (ApiClient.Client == null)
        {
            TempData["ErrorMessage"] = "Неверный логин/пароль";
            return View();
        }
        return ApiClient.Client.IsTwoFactor ? RedirectToAction("Enter2") : RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult Enter2()
    {
        if (ApiClient.Client == null || !ApiClient.Client.IsTwoFactor)
        {
            return Redirect("~/Home/Enter");
        }
        return View();
    }
    
    [HttpPost]
    public IActionResult Enter2(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            TempData["ErrorMessage"] = "Введите код";
            return View();
        }

        ApiClient.ValidCode = _userService.CheckCodeValid(ApiClient.Client.Key, code);
        if (!ApiClient.ValidCode)
        {
            TempData["ErrorMessage"] = "Неверный код";
            return View();
        }
        return RedirectToAction("Index");
    }
    
    public void GenerateSecretKey(int id, string login)
    {
        var codes = _userService.RegisterUser("SuccessfulAdmission", login);
        _userService.UpdateTwoFactorKeys(id, codes.Item1, codes.Item2);
        ApiClient.Client = _userService.GetUserById(id);
        Response.Redirect("/Home/Profile");
    }
    
    public void Exit()
    {
        ApiClient.Client = null;
        Response.Redirect("/Home/Enter");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string email, string login, string password)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = "Введите почту, логин, пароль";
            return View();
        }
        if (_userService.GetUserByLogin(login) != null)
        {
            TempData["ErrorMessage"] = "Пользователь с таким логином уже существует";
            return View();
        }
        if (_userService.GetUserByEmail(email) != null)
        {
            TempData["ErrorMessage"] = "Пользователь с такой почтой уже существует";
            return View();
        }
        string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
        Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
        if (!isMatch.Success)
        {
            TempData["ErrorMessage"] = "Введите корректную почту";
            return View();
        }
        if (login.Length < 5)
        {
            TempData["ErrorMessage"] = "Логин не должен быть короче 5 символов";
            return View();
        }
        if (password.Length < 6)
        {
            TempData["ErrorMessage"] = "Пароль не должен быть короче 6 символов";
            return View();
        }
        _userService.AddUser(login, password, email);
        return RedirectToAction("Enter");
    }
    
    [HttpGet]
    public IActionResult Profile()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        var model = new ApiClient
        {
            Id = ApiClient.Client.Id,
            Email = ApiClient.Client.Email,
            Login = ApiClient.Client.Login,
            Password = ApiClient.Client.Password,
            IsAdmin = ApiClient.Client.IsAdmin,
            IsTwoFactor = ApiClient.Client.IsTwoFactor,
            Key = ApiClient.Client.Key ?? string.Empty,
            Qr = ApiClient.Client.Qr ?? string.Empty
        };

        return View(model);
    }
    
    [HttpPost]
    public IActionResult UpdateUser(int id, string login, string password, string email, bool isTwoFactor=false)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = "Введите почту, логин, пароль";
            return RedirectToAction("Profile");
        }
        if (_userService.GetUserByLogin(login) != null && _userService.GetUserByLogin(login)!.Id != id)
        {
            TempData["ErrorMessage"] = "Пользователь с таким логином уже существует";
            return RedirectToAction("Profile");
        }
        if (_userService.GetUserByEmail(email) != null && _userService.GetUserByEmail(email)!.Id != id)
        {
            TempData["ErrorMessage"] = "Пользователь с такой почтой уже существует";
            return RedirectToAction("Profile");
        }
        string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
        Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
        if (!isMatch.Success)
        {
            TempData["ErrorMessage"] = "Введите корректную почту";
            return RedirectToAction("Profile");
        }
        if (login.Length < 5)
        {
            TempData["ErrorMessage"] = "Логин не должен быть короче 5 символов";
            return RedirectToAction("Profile");
        }
        if (password.Length < 6)
        {
            TempData["ErrorMessage"] = "Пароль не должен быть короче 6 символов";
            return RedirectToAction("Profile");
        }
        _userService.UpdateUser(id, login, password, email);
        _userService.UpdateTwoFactorSetting(id, isTwoFactor);
        ApiClient.Client = _userService.GetUserById(id);
        var model = new ApiClient
        {
            Id = ApiClient.Client.Id,
            Email = ApiClient.Client.Email,
            Login = ApiClient.Client.Login,
            Password = ApiClient.Client.Password,
            IsAdmin = ApiClient.Client.IsAdmin,
            IsTwoFactor = ApiClient.Client.IsTwoFactor,
            Key = ApiClient.Client.Key ?? string.Empty,
            Qr = ApiClient.Client.Qr ?? string.Empty
        };
        return RedirectToAction("Profile", model);
    }
    
    public void DeleteUser(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode) || id <= 0)
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        _userService.DeleteUser(id);
        Exit();
    }

    [HttpGet]
    public IActionResult Faculties()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_facultyService.GetAllFaculties());
    }
    
    [HttpGet]
    public IActionResult FacultyCreate()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View();
    }
    [HttpPost]
    public IActionResult FacultyCreate(string name, string? desc)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Faculties");
        }
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
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_facultyService.GetFacultyById(id));
    }
    [HttpPost]
    public IActionResult UpdateFaculty(int id, string name, string? desc)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Faculties");
        }
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
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/Faculties");
            return;
        }
        _facultyService.DeleteFaculty(id);
        Response.Redirect("/Home/Faculties");
    }
    
    [HttpGet]
    public IActionResult Specialities()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_specialityService.GetAllSpecialities());
    }
    
    [HttpGet]
    public IActionResult SpecialityCreate()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        ViewBag.Faculties = _facultyService.GetAllFaculties();
        return View();
    }
    [HttpPost]
    public IActionResult SpecialityCreate(string name, string? desc, string countStr, int? facultyId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Specialities");
        }
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
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        ViewBag.Faculties = _facultyService.GetAllFaculties();
        return View(_specialityService.GetSpecialityById(id));
    }
    [HttpPost]
    public IActionResult UpdateSpeciality(int id, string name, string? desc, string countStr, int? facultyId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Specialities");
        }
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
    
    [HttpGet]
    public IActionResult SpecialitySubjects(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        ViewBag.AllSubjects = _subjectService.GetAllSubjects();
        ViewBag.Subjects = _subjectService.GetSubjectsBySpecialityId(id);
        return View(_specialityService.GetSpecialityById(id));
    }

    [HttpPost]
    public IActionResult AddSpecialitySubject(int specialityId, int subjectId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("SpecialitySubjects", new { id = specialityId });
        }
        _specialityService.AddSpecialitySubject(specialityId, subjectId);
        return RedirectToAction("SpecialitySubjects", new { id = specialityId });
    }

    public void DeleteSpecialitySubject(int specialityId, int subjectId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/SpecialitySubjects/" + specialityId);
            return;
        }
        _specialityService.DeleteSpecialitySubject(specialityId, subjectId);
        Response.Redirect("/Home/SpecialitySubjects/" + specialityId);
    }
    
    [HttpGet]
    public IActionResult SpecialityApplicants(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        ViewBag.AllApplicants = _applicantService.GetAllApplicants();
        return View(_listService.GetListApplicantsForSpeciality(id));
    }
    
    [HttpPost]
    public IActionResult AddSpecialityApplicant(int specialityId, int applicantId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("SpecialityApplicants", new {id = specialityId});
        }
        _specialityService.AddSpecialityApplicant(specialityId, applicantId);
        return RedirectToAction("SpecialityApplicants", new {id = specialityId});
    }
    
    public void DeleteSpecialityApplicant(int specialityId, int applicantId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/SpecialityApplicants/" + specialityId);
            return;
        }
        _specialityService.DeleteSpecialityApplicant(specialityId, applicantId);
        Response.Redirect("/Home/SpecialityApplicants/" + specialityId);
    }

    public void DeleteSpeciality(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/Specialities");
            return;
        }
        _specialityService.DeleteSpeciality(id);
        Response.Redirect("/Home/Specialities");
    }
    
    [HttpGet]
    public IActionResult Applicants()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_applicantService.GetAllApplicants());
    }
    
    [HttpGet]
    public IActionResult ApplicantCreate()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View();
    }
    [HttpPost]
    public IActionResult ApplicantCreate(string name)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Applicants");
        }
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите имя";
            return View();
        }

        _applicantService.AddApplicant(name);
        return RedirectToAction("Applicants");
    }
    
    [HttpGet]
    public IActionResult ApplicantSetting(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_applicantService.GetApplicantById(id));
    }
    [HttpPost]
    public IActionResult UpdateApplicant(int id, string name)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Applicants");
        }
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите имя";
            return RedirectToAction("ApplicantSetting",  new { id });
        }
        _applicantService.UpdateApplicant(id, name);
        return RedirectToAction("Applicants");
    }
    
    [HttpGet]
    public IActionResult ApplicantSubjects(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        ViewBag.AllSubjects = _subjectService.GetAllSubjects();
        return View(_applicantService.GetApplicantById(id));
    }
    
    [HttpPost]
    public IActionResult AddApplicantSubject(int applicantId, int subjectId, int points)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("ApplicantSubjects", new { id = applicantId });
        }
        var maxPoints = _subjectService.GetSubjectById(subjectId).MaxPoints;
        if (points > maxPoints)
        {
            TempData["ErrorMessage"] = "Кол-во баллов не может быть больше максимального " + maxPoints;
            return RedirectToAction("ApplicantSubjects",  new { id = applicantId });
        }
        _applicantService.AddApplicantSubject(applicantId, subjectId, points);
        return RedirectToAction("ApplicantSubjects", new { id = applicantId });
    }
    
    public void DeleteApplicantSubject(int applicantId, int subjectId)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/ApplicantSubjects/" + applicantId);
            return;
        }
        _applicantService.DeleteApplicantSubject(applicantId, subjectId);
        Response.Redirect("/Home/ApplicantSubjects/" + applicantId);
    }

    public void DeleteApplicant(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/Applicants");
            return;
        }
        _applicantService.DeleteApplicant(id);
        Response.Redirect("/Home/Applicants");
    }
    
    [HttpGet]
    public IActionResult Subjects()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_subjectService.GetAllSubjects());
    }
    
    [HttpGet]
    public IActionResult SubjectCreate()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View();
    }
    [HttpPost]
    public IActionResult SubjectCreate(string name, string maxPointsStr)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Subjects");
        }
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите название";
            return View();
        }
        
        if (!int.TryParse(maxPointsStr, out int maxPoints) || maxPoints < 0)
        {
            TempData["ErrorMessage"] = "Максимальное количество баллов должно быть числом и не может быть отрицательным";
            return View();
        }

        _subjectService.AddSubject(name, maxPoints);
        return RedirectToAction("Subjects");
    }
    
    [HttpGet]
    public IActionResult SubjectSetting(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        return View(_subjectService.GetSubjectById(id));
    }
    [HttpPost]
    public IActionResult UpdateSubject(int id, string name, string maxPointsStr)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return RedirectToAction("Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            return RedirectToAction("Subjects");
        }
        if (string.IsNullOrEmpty(name))
        {
            TempData["ErrorMessage"] = "Введите название";
            return RedirectToAction("SubjectSetting",  new { id });
        }
        if (!int.TryParse(maxPointsStr, out int maxPoints) || maxPoints < 0)
        {
            TempData["ErrorMessage"] = "Максимальное количество баллов должно быть числом и не может быть отрицательным";
            return RedirectToAction("SubjectSetting",  new { id });
        }
        _subjectService.UpdateSubject(id, name, maxPoints);
        return RedirectToAction("Subjects");
    }

    public void DeleteSubject(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/Subjects");
            return;
        }
        _subjectService.DeleteSubject(id);
        Response.Redirect("/Home/Subjects");
    }
    
    [HttpGet]
    public IActionResult Users()
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            return Redirect("~/Home/Enter");
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Эта страница доступна только админам";
            return Redirect("~/Home/Index");
        }
        return View(_userService.GetAllUsers());
    }
    
    public void PromoteUser(int id)
    {
        if (ApiClient.Client == null || (ApiClient.Client.IsTwoFactor && !ApiClient.ValidCode))
        {
            Response.Redirect("/Home/Enter");
            return;
        }
        if (!ApiClient.Client.IsAdmin)
        {
            TempData["ErrorMessage"] = "Недостаточно прав для данного действия";
            Response.Redirect("/Home/Users");
            return;
        }
        _userService.PromoteUser(id);
        Response.Redirect("/Home/Users");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
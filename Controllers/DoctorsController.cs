using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prueba.Models;
using Prueba.Services;

namespace Prueba.Controllers;

public class DoctorsController : Controller
{
    private readonly DoctorService _doctorService;
    
    public DoctorsController(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }
    
    //GET doctors
    
    public async Task<IActionResult> Index(string? speciality)
    {
        var doctors = await _doctorService.GetDoctorsBySpecialityAsync(speciality);

        // Get all specialities for the dropdown menu
        var allSpecialities = await _doctorService.GetAllDoctorsAsync();
        var uniqueSpecialities = allSpecialities.Select(d => d.Speciality).Distinct().ToList();
        ViewBag.SpecialityList = new SelectList(uniqueSpecialities);

        return View(doctors);
    }
    
    // GET /Doctors/Create
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Doctor doctor)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _doctorService.CreateDoctorAsync(doctor);
                TempData["SuccessMessage"] = "Médico registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al registrar el médico.");
                Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
            }
        }
        return View(doctor);
    }
    
    // GET: /Doctors/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var doctor = await _doctorService.GetDoctorByIdAsync(id.Value);
        if (doctor == null)
        {
            return NotFound();
        }
        return View(doctor);
    }
    
    // POST: /Doctors/Edit/{id}
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Doctor doctor)
    {
        if (id != doctor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _doctorService.UpdateDoctorAsync(doctor);
                TempData["SuccessMessage"] = "Información del médico actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar la información del médico.");
                Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
            }
        }
        return View(doctor);
    }
    
    // GET: /Doctors/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var doctor = await _doctorService.GetDoctorByIdAsync(id.Value);
        if (doctor == null)
        {
            return NotFound();
        }
        return View(doctor);
    }
}
using Microsoft.AspNetCore.Mvc;
using Prueba.Models;
using Prueba.Services;

namespace Prueba.Controllers;

public class PacientsController : Controller
{
    private readonly PacientService _pacientService;
    
    public PacientsController(PacientService pacientService)
    {
        _pacientService = pacientService;
    }
    
    //GET pacients
    
    public async Task<IActionResult> Index()
    {
        var pacients = await _pacientService.GetAllPacientAsync();
        return View(pacients);
    }
    
    // GET /Pacients/Create
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Pacient pacient)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _pacientService.CreatePacientAsync(pacient);
                TempData["SuccessMessage"] = "Paciente registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al registrar el paciente.");
                Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
            }
        }
        return View(pacient);
    }
    
    // GET: /Pacients/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pacient = await _pacientService.GetPacientByIdAsync(id.Value);
        if (pacient == null)
        {
            return NotFound();
        }
        return View(pacient);
    }
    
    // POST: /Pacients/Edit/{id}
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Pacient pacient)
    {
        if (id != pacient.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _pacientService.UpdatePacientAsync(pacient);
                TempData["SuccessMessage"] = "Información del paciente actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar la información del paciente.");
                // Log the exception
            }
        }
        return View(pacient);
    }
    
    // GET: /Pacients/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pacient = await _pacientService.GetPacientByIdAsync(id.Value);
        if (pacient == null)
        {
            return NotFound();
        }
        return View(pacient);
    }
}
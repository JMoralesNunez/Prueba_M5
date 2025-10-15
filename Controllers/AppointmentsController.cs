using Prueba.Models;
using Prueba.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prueba.Controllers;

public class AppointmentsController : Controller
{
    private readonly AppointmentService _appointmentService;
    private readonly PacientService _pacientService; 
    private readonly DoctorService _doctorService;  

    public AppointmentsController(AppointmentService appointmentService, PacientService pacientService, DoctorService doctorService)
    {
        _appointmentService = appointmentService;
        _pacientService = pacientService;
        _doctorService = doctorService;
    }
    
    // GET: /Appointments
    public async Task<IActionResult> Index(int? pacientId, int? doctorId, string? state)
    {
        var appointments = await _appointmentService.GetFilteredAppointmentsAsync(pacientId, doctorId, state);
        
        ViewBag.PacientId = new SelectList(await _pacientService.GetAllPacientAsync(), "Id", "Name", pacientId);
        ViewBag.DoctorId = new SelectList(await _doctorService.GetAllDoctorsAsync(), "Id", "Name", doctorId);
        ViewBag.States = new SelectList(AppointmentService.AppointmentStates.AllStates, state);

        return View(appointments);
    }
    
    public async Task<IActionResult> Create()
    {
        ViewBag.PacientId = new SelectList(await _pacientService.GetAllPacientAsync(), "Id", "Name");
        ViewBag.DoctorId = new SelectList(await _doctorService.GetAllDoctorsAsync(), "Id", "Name");
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _appointmentService.CreateAppointmentAsync(
                    appointment.PacientID, 
                    appointment.DoctorID, 
                    appointment.AppointmentDate, 
                    appointment.AppointmentHour, 
                    appointment.Observations
                );
                TempData["SuccessMessage"] = "Cita creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al crear la cita.");
                Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
            }
        }
        ViewBag.PacientId = new SelectList(await _pacientService.GetAllPacientAsync(), "Id", "Name", appointment.PacientID);
        ViewBag.DoctorId = new SelectList(await _doctorService.GetAllDoctorsAsync(), "Id", "Name", appointment.DoctorID);
        return View(appointment);
    }
    
    // GET: /Appointments/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _appointmentService.GetAppointmentByIdAsync(id.Value);
        if (reservation == null)
        {
            return NotFound();
        }
        return View(reservation);
    }
    
    // POST: /Appointments/Cancel/{id}
    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var reservation = await _appointmentService.CancelAppointmentAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            TempData["SuccessMessage"] = "Cita cancelada exitosamente.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Ocurrió un error al cancelar la reserva.";
            Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
        }
        return RedirectToAction(nameof(Details), new { id = id });
    }
    
    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var reservation = await _appointmentService.CompleteAppointmentAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            TempData["SuccessMessage"] = "Cita marcada como completada exitosamente.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Ocurrió un error al completar la cita.";
            Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
        }
        return RedirectToAction(nameof(Details), new { id = id });
    }
}
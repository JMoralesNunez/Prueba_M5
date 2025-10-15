using Prueba.Data;
using Prueba.Models;
using Microsoft.EntityFrameworkCore;

namespace Prueba.Services;

public class AppointmentService
{
    private readonly MySqlContext _context; 
    private readonly IEmailService _emailService;
    
    public AppointmentService(MySqlContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    public static class AppointmentStates
    {
        public const string Programmed = "Programada";
        public const string Canceled = "Cancelada";
        public const string Attended = "Atentida";
        public static List<string> AllStates = new List<string> { Programmed, Canceled, Attended };
    }
    
    // Create appointment
    public async Task<Appointment> CreateAppointmentAsync(int pacientId, int doctorId, DateOnly date, TimeOnly hour, string? observations = null)
    {
        var pacient = await _context.Pacients.FindAsync(pacientId) ?? throw new InvalidOperationException("Paciente no encontrado.");
        var doctor = await _context.Doctors.FindAsync(doctorId) ?? throw new InvalidOperationException("Médico no encontrado.");

        //check doctor availability
        var doctorHasConflict = await _context.Appointments
            .AnyAsync(a => a.DoctorID == doctorId && 
                           a.AppointmentDate == date && 
                           a.AppointmentHour == hour && 
                           a.State == AppointmentStates.Programmed);
        
        if (doctorHasConflict)
        {
            throw new InvalidOperationException($"El médico {doctor.Name} ya tiene una cita programada para el {date.ToShortDateString()} a las {hour.ToShortTimeString()}.");
        }
        
        //check pacient availability
        var pacientHasConflict = await _context.Appointments
            .AnyAsync(a => a.PacientID == pacientId && 
                           a.AppointmentDate == date && 
                           a.AppointmentHour == hour && 
                           a.State == AppointmentStates.Programmed);

        if (pacientHasConflict)
        {
            throw new InvalidOperationException($"El paciente {pacient.Name} ya tiene una cita programada para el {date.ToShortDateString()} a las {hour.ToShortTimeString()}.");
        }
        
        //check date is not in the past
        if (date < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new InvalidOperationException("La fecha de la cita no puede ser en el pasado.");
        }

        // create appointment
        var appointment = new Appointment
        {
            PacientID = pacientId,
            DoctorID = doctorId,
            AppointmentDate = date,
            AppointmentHour = hour,
            State = AppointmentStates.Programmed,
            Observations = observations
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        
        //send email
        var subject = "Confirmación de Cita - Hospital San Vicente";
        var body = $@"
            <h2>¡Bienvenido a Hospital San Vicente!</h2>
            <p>Tu cita ha sido creada exitosamente.</p>
            <p>Documento: {pacient.NationalId}</p>
            <p>Teléfono: {pacient.Phone}</p>
            <p>Fecha de la cita: {appointment.AppointmentDate}</p>
            <p>Hora de la cita: {appointment.AppointmentHour}</p>
            <p>Estado de la cita: {appointment.State}</p>
            <p>Gracias por elegirnos 💙</p>"; 
        
        //record history
        string emailState = "Enviado";
        string emailMessage = "Correo de confirmación enviado exitosamente.";

        try
        {
            await _emailService.SendEmail(pacient.Email, subject, body);
        }
        catch (Exception ex)
        {
            emailState = "Fallo al Enviar";
            emailMessage = $"Error al enviar el correo: {ex.Message}";
            Console.WriteLine($" EXCEPCIÓN DETALLADA: {ex}");
            Console.WriteLine($"Error al enviar correo para cita {appointment.Id}: {ex}");
            throw new InvalidOperationException("Cita creada, pero el correo no se pudo enviar correctamente");
        }
        
        var emailHistory = new EmailHistory
        {
            AppointmentId = appointment.Id,
            DeliverDate = DateTime.Now, 
            State = emailState,
            Message = emailMessage
        };

        _context.EmailHistory.Add(emailHistory);
        await _context.SaveChangesAsync(); 


        return appointment;
    }
    
    // Cancel
    public async Task<Appointment?> CancelAppointmentAsync(int appointmentId)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
        {
            return null;
        }

        if (appointment.State != AppointmentStates.Programmed)
        {
            throw new InvalidOperationException("Solo se pueden cancelar citas activas.");
        }

        appointment.State = AppointmentStates.Canceled;

        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }
    
    
    // mark as completed
    public async Task<Appointment?> CompleteAppointmentAsync(int appointmentId)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
        {
            return null;
        }

        if (appointment.State != AppointmentStates.Programmed)
        {
            throw new InvalidOperationException("Solo se pueden completar citas activas.");
        }

        appointment.State = AppointmentStates.Attended;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }
    
    public async Task<Appointment?> GetAppointmentByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(r => r.Pacient)
            .Include(r => r.Doctor)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<List<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.Pacient) 
            .Include(a => a.Doctor)   
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentHour)
            .ToListAsync();
    }
    
    public async Task<List<Appointment>> GetFilteredAppointmentsAsync(int? pacientId, int? doctorId, string? state)
    {
        IQueryable<Appointment> query = _context.Appointments
            .Include(a => a.Pacient) 
            .Include(a => a.Doctor);

        if (pacientId.HasValue)
        {
            query = query.Where(a => a.PacientID == pacientId.Value);
        }

        if (doctorId.HasValue)
        {
            query = query.Where(a => a.DoctorID == doctorId.Value);
        }

        if (!string.IsNullOrEmpty(state))
        {
            query = query.Where(a => a.State == state);
        }

        return await query
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentHour)
            .ToListAsync();
    }
}
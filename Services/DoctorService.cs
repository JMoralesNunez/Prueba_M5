using Prueba.Data;
using Prueba.Models;
using Microsoft.EntityFrameworkCore;

namespace Prueba.Services;

public class DoctorService
{
    private readonly MySqlContext _context; 
    
    public DoctorService(MySqlContext context)
    {
        _context = context;
    }
    
    //Register
    
    public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
    {
        // Validar que el documento de identidad sea único
        if (await _context.Doctors.AnyAsync(p => p.NationalId == doctor.NationalId))
        {
            throw new InvalidOperationException($"Ya existe un doctor con el documento de identidad '{doctor.NationalId}'.");
        }
        //Validar que email sea único
        if (await _context.Doctors.AnyAsync(p => p.Email == doctor.Email))
        {
            throw new InvalidOperationException($"Ya existe un paciente con este correo electrónico '{doctor.Email}'.");
        }

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        
        return doctor;
    }
    
    //  Edit
    public async Task<Doctor?> UpdateDoctorAsync(Doctor updatedDoctor)
    {
        var existingDoctor = await _context.Doctors.FindAsync(updatedDoctor.Id);
        if (existingDoctor == null)
        {
            return null; 
        }

        // Validar que el documento de identidad sea único si ha cambiado
        if (existingDoctor.NationalId != updatedDoctor.NationalId &&
            await _context.Doctors.AnyAsync(p => p.NationalId == updatedDoctor.NationalId))
        {
            throw new InvalidOperationException($"El documento de identidad '{updatedDoctor.NationalId}' ya está registrado para otro médico.");
        }
        //Validar que el email sea único
        if (existingDoctor.Email != updatedDoctor.Email &&
            await _context.Doctors.AnyAsync(p => p.Email == updatedDoctor.Email))
        {
            throw new InvalidOperationException($"El correo '{updatedDoctor.Email}' ya está registrado para otro médico.");
        }

        existingDoctor.Name = updatedDoctor.Name;
        existingDoctor.NationalId = updatedDoctor.NationalId;
        existingDoctor.Speciality = updatedDoctor.Speciality;
        existingDoctor.Phone = updatedDoctor.Phone;
        existingDoctor.Email = updatedDoctor.Email;

        _context.Doctors.Update(existingDoctor);
        await _context.SaveChangesAsync();
        return existingDoctor;
    }
    
    //List
    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors.OrderBy(p => p.Name).ToListAsync();
    }
    
    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        return await _context.Doctors.FindAsync(id);
    }
    
    public async Task<List<Doctor>> GetDoctorsBySpecialityAsync(string? speciality)
    {
        if (string.IsNullOrEmpty(speciality))
        {
            return await _context.Doctors.OrderBy(d => d.Name).ToListAsync();
        }
        return await _context.Doctors
            .Where(d => d.Speciality.Contains(speciality))
            .OrderBy(d => d.Name)
            .ToListAsync();
    }
}
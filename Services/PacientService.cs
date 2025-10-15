using Microsoft.EntityFrameworkCore;
using Prueba.Data;
using Prueba.Models;

namespace Prueba.Services;

public class PacientService
{
    private readonly MySqlContext _context; 
    
    public PacientService(MySqlContext context)
    {
        _context = context;
    }
    
    //Register
    
    public async Task<Pacient> CreatePacientAsync(Pacient pacient)
    {
        if (await _context.Pacients.AnyAsync(p => p.NationalId == pacient.NationalId))
        {
            throw new InvalidOperationException($"Ya existe un paciente con el documento de identidad '{pacient.NationalId}'.");
        }
        if (await _context.Pacients.AnyAsync(p => p.Email == pacient.Email))
        {
            throw new InvalidOperationException($"Ya existe un paciente con este correo electrónico '{pacient.Email}'.");
        }

        _context.Pacients.Add(pacient);
        await _context.SaveChangesAsync();
        
        return pacient;
    }
    
    //  Edit
    public async Task<Pacient?> UpdatePacientAsync(Pacient updatedPacient)
    {
        var existingPacient = await _context.Pacients.FindAsync(updatedPacient.Id);
        if (existingPacient == null)
        {
            return null; 
        }
        
        if (existingPacient.NationalId != updatedPacient.NationalId &&
            await _context.Pacients.AnyAsync(p => p.NationalId == updatedPacient.NationalId))
        {
            throw new InvalidOperationException($"El documento de identidad '{updatedPacient.NationalId}' ya está registrado para otro paciente.");
        }
        if (existingPacient.Email != updatedPacient.Email &&
            await _context.Pacients.AnyAsync(p => p.Email == updatedPacient.Email))
        {
            throw new InvalidOperationException($"El correo '{updatedPacient.Email}' ya está registrado para otro paciente.");
        }

        existingPacient.Name = updatedPacient.Name;
        existingPacient.NationalId = updatedPacient.NationalId;
        existingPacient.Age = updatedPacient.Age;
        existingPacient.Phone = updatedPacient.Phone;
        existingPacient.Email = updatedPacient.Email;

        _context.Pacients.Update(existingPacient);
        await _context.SaveChangesAsync();
        return existingPacient;
    }
    
    //List
    public async Task<List<Pacient>> GetAllPacientAsync()
    {
        return await _context.Pacients.OrderBy(p => p.Name).ToListAsync();
    }
    
    public async Task<Pacient?> GetPacientByIdAsync(int id)
    {
        return await _context.Pacients.FindAsync(id);
    }
}
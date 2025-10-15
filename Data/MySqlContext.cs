using Microsoft.EntityFrameworkCore;
using Prueba.Models;

namespace Prueba.Data;

public class MySqlContext : DbContext
{
    public MySqlContext(DbContextOptions<MySqlContext> options)
        : base(options)
    {
    }
    
    public DbSet<Pacient> Pacients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<EmailHistory> EmailHistory { get; set; }
}
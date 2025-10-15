using System.ComponentModel.DataAnnotations;

namespace Prueba.Models;

public class Doctor
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "El documento es obligatorio")]
    [StringLength(20)]
    public string NationalId { get; set; }

    [Required(ErrorMessage = "La especialidad es obligatoria")]
    [StringLength(100)]
    public string Speciality { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "El Correo es obligatorio")]
    public string Email { get; set; }
    
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
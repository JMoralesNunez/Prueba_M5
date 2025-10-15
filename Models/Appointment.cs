using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prueba.Models;

public class Appointment
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Elegir un paciente es obligatorio")]
    public int PacientID { get; set; }

    [Required(ErrorMessage = "Elegir un médico es obligatorio")]
    public int DoctorID { get; set; }

    [Required(ErrorMessage = "Elegir una fecha es obligatorio")]
    [DataType(DataType.Date)]
    public DateOnly AppointmentDate { get; set; }

    [Required(ErrorMessage = "Elegir una hora es obligatorio")]
    [DataType(DataType.Time)]
    public TimeOnly AppointmentHour { get; set; }

    [Required]
    [StringLength(20)]
    public string? State { get; set; } = "Programada";

    public string? Observations { get; set; }
    
    public Pacient? Pacient { get; set; }
    public Doctor? Doctor { get; set; }

    public ICollection<EmailHistory> EmailHistories { get; set; } = new List<EmailHistory>();
}
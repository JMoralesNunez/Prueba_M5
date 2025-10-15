using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prueba.Models;

public class EmailHistory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    public DateTime DeliverDate { get; set; }

    [Required]
    [StringLength(20)]
    public string State { get; set; } = "Enviado";

    public string? Message { get; set; }
    
    public Appointment? Appointment { get; set; }
}
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace Models;

public class Booking
{
    [Required]
    [StringLength(50, ErrorMessage = "The ID length can't exceed 50 characters.")]
    public string Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The Flight ID length can't exceed 50 characters.")]
    public string FlightId { get; set; }

    [Required] [Name("ClassType")] public ClassType ClassType { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The Passenger ID length can't exceed 50 characters.")]
    public string PassengerId { get; set; }

    [Required] [Name("Status")] public Status Status { get; set; }
}
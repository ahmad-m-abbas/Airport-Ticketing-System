using System.ComponentModel.DataAnnotations;
using Data;

namespace Models;

public class Flight
{
    [Required]
    [StringLength(50, ErrorMessage = "The ID length can't exceed 50 characters.")]
    public string Id { get; set; }

    [Required]
    [ValidationDescription("Required")]
    [ValidationDescription("Free Text Doesn't Exceed 100 Characters")]
    [StringLength(100, ErrorMessage = "The Departure Country length can't exceed 100 characters.")]
    public string DepartureCountry { get; set; }

    [Required]
    [ValidationDescription("Required")]
    [ValidationDescription("Free Text Doesn't Exceed 100 Characters")]
    [StringLength(100, ErrorMessage = "The Destination Country length can't exceed 100 characters.")]
    public string DestinationCountry { get; set; }

    [Required]
    [ValidationDescription("Required")]
    [ValidationDescription("Date Time")]
    [DateRange("01/01/2000", "12/31/2100")]
    [ValidationDescription("Allowed Range: today → future")]
    [DataType(DataType.Date)]
    public DateTime DepartureDate { get; set; }

    [Required]
    [ValidationDescription("Required")]
    [ValidationDescription("Free Text Doesn't Exceed 100 Characters")]
    [StringLength(100, ErrorMessage = "The Departure Airport length can't exceed 100 characters.")]
    public string DepartureAirport { get; set; }

    [Required]
    [ValidationDescription("Required")]
    [ValidationDescription("Free Text Doesn't Exceed 100 Characters")]
    [StringLength(100, ErrorMessage = "The Arrival Airport length can't exceed 100 characters.")]
    public string ArrivalAirport { get; set; }

    public decimal PriceEconomy { get; set; }

    public decimal PriceBusiness { get; set; }

    public decimal PriceFirstClass { get; set; }

    [ValidationDescription("Required")]
    [ValidationDescription("Positive or zero Integer")]
    [Range(0, 200, ErrorMessage = "The number of available seats for Economy Class must be positive.")]
    public int AvailableSeatsEconomy { get; set; }

    [ValidationDescription("Required")]
    [ValidationDescription("Positive or zero Integer")]
    [Range(0, 20, ErrorMessage = "The number of available seats for Business Class must be positive.")]
    public int AvailableSeatsBusiness { get; set; }

    [ValidationDescription("Required")]
    [ValidationDescription("Positive or zero Integer")]
    [Range(0, 20, ErrorMessage = "The number of available seats for First Class must be positive.")]
    public int AvailableSeatsFirstClass { get; set; }

    public override string ToString()
    {
        return $"Flight: {DepartureCountry} => {ArrivalAirport} at \t {DepartureDate}";
    }
}
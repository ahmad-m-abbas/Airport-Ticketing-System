using System.ComponentModel.DataAnnotations;

namespace Data;

public class DateRangeAttribute : ValidationAttribute
{
    public DateRangeAttribute(string minDate, string maxDate)
    {
        MinDate = DateTime.Parse(minDate);
        MaxDate = DateTime.Parse(maxDate);
    }

    public DateTime MinDate { get; }
    public DateTime MaxDate { get; }

    public override bool IsValid(object value)
    {
        if (value is DateTime dateValue) return dateValue >= MinDate && dateValue <= MaxDate;
        return true;
    }
}
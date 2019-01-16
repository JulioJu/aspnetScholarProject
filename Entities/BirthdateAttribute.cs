namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class BirthdateAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
      Customer customer = (Customer)validationContext.ObjectInstance;

      if (customer.Birthdate != null)
      {
        if (customer.Birthdate <= new DateTime(DateTime.Today.Year - 122,
              DateTime.Today.Month,
              DateTime.Today.Day))
        {
            return new ValidationResult(BirthdateToOld());
        }

        if (customer.Birthdate >= new DateTime(DateTime.Today.Year - 13,
              DateTime.Today.Month,
              DateTime.Today.Day))
        {
            return new ValidationResult(TooYoung());
        }
      }

      return ValidationResult.Success;
    }

    private static string BirthdateToOld()
    {
      return $"Only Jeanne Calment lived 122 years!!";
    }

    private static string TooYoung()
    {
        return $"The Custmer must is more than 13 years old.";
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos

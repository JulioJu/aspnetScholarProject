namespace Videotheque.Data
{
  using System.ComponentModel.DataAnnotations;

  public class CustomerIsCompanyAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
      Customer customer = (Customer)validationContext.ObjectInstance;

      if (!string.IsNullOrEmpty(customer.Company))
      {
        if (value is bool && (bool)value)
        {
            return new ValidationResult(ShouldBeFalse());
        }
        if (value is System.DateTime)
        {
          return new ValidationResult(ShouldBeNull());
        }
      }

      return ValidationResult.Success;
    }

    private static string ShouldBeFalse()
    {
      return $"As Company is defined, this attributes should be false.";
    }

    private static string ShouldBeNull()
    {
      return $"As Company is defined, this attributes should be null.";
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos

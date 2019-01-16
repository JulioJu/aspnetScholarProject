namespace Videotheque.Data
{
  using System.ComponentModel.DataAnnotations;

  public class CustomerNameAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
      Customer customer = (Customer)validationContext.ObjectInstance;

      if ((string.IsNullOrEmpty(customer.Firstname)
                  || string.IsNullOrEmpty(customer.Lastname))
              && string.IsNullOrEmpty(customer.Society))
      {
          return new ValidationResult(GetMessage());
      }

      return ValidationResult.Success;
    }

    private static string GetMessage()
    {
      return $"Fill either Firtname and Lastname or Society";
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos

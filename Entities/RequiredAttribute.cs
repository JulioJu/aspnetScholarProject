namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  [AttributeUsage(AttributeTargets.Property,
      AllowMultiple = false,
      Inherited = true)]
    public class RequiredAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
      return ValidationResult.Success;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos

namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class FilmReleasedAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
      Film film = (Film)validationContext.ObjectInstance;

      if (DateTime.Compare(film.ReleaseDate, new DateTime(1895, 4, 22)) < 0
          || DateTime.Compare(film.ReleaseDate, DateTime.Today) > 0)
      {
        return new ValidationResult(GetErrorMessage());
      }

      return ValidationResult.Success;
    }

    private static string GetErrorMessage()
    {
      return $"Movie must have a release after " +
        new DateTime(1895, 4, 22).ToShortDateString()
        + " and " +
        DateTime.Today.ToShortDateString()
        + " (include).";
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos

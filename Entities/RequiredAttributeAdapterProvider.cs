// Inspired from https://blogs.msdn.microsoft.com/mvpawardprogram/2017/01/03/asp-net-core-mvc/
namespace Videotheque.Data
{
  using System.ComponentModel.DataAnnotations;
  using Microsoft.AspNetCore.Mvc.DataAnnotations;
  using Microsoft.Extensions.Localization;

  public class RequiredAttributeAdapterProvider :
    IValidationAttributeAdapterProvider
  {
    private readonly IValidationAttributeAdapterProvider baseProvider =
      new ValidationAttributeAdapterProvider();

    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute,
        IStringLocalizer stringLocalizer)
    {
      if (attribute is RequiredAttribute)
      {
        return new RequiredAttributeAdapter(attribute as RequiredAttribute,
            stringLocalizer);
      }
      else
      {
        return this.baseProvider
          .GetAttributeAdapter(attribute, stringLocalizer);
      }
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos

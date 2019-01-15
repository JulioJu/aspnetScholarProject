// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// source https://github.com/aspnet/AspNetCore/blob/afb92018f07bf81c7be092ce93ae36cf1aca5365/src/Mvc/src/Microsoft.AspNetCore.Mvc.DataAnnotations/RequiredAttributeAdapter.cs#L34,L35
// See also code added by me : https://github.com/aspnet/AspNetCore/issues/6329

namespace Videotheque.Data
{
  using System;
  using Microsoft.AspNetCore.Mvc.DataAnnotations;
  using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
  using Microsoft.Extensions.Localization;

  /// <summary>
  /// <see cref="AttributeAdapterBase{TAttribute}"/> for <see cref="RequiredAttribute"/>.
  /// </summary>
  public sealed class RequiredAttributeAdapter : AttributeAdapterBase<RequiredAttribute>
  {
    /// <summary>
    /// Initializes a new instance of <see cref="RequiredAttributeAdapter"/>.
    /// </summary>
    /// <param name="attribute">The <see cref="RequiredAttribute"/>.</param>
    /// <param name="stringLocalizer">The <see cref="IStringLocalizer"/>.</param>
    public RequiredAttributeAdapter(RequiredAttribute attribute,
        IStringLocalizer stringLocalizer)
      : base(attribute, stringLocalizer)
    {
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      MergeAttribute(context.Attributes, "data-val", "true");
      MergeAttribute(context.Attributes,
          "data-val-required",
          this.GetErrorMessage(context) + "(required)");
      MergeAttribute(context.Attributes, "required", "required"); // (at least)
      MergeAttribute(context.Attributes, "required-aria", "true");
    }

    /// <inheritdoc />
    public override string GetErrorMessage(ModelValidationContextBase
        validationContext)
    {
      if (validationContext == null)
      {
        throw new ArgumentNullException(nameof(validationContext));
      }

      return base.GetErrorMessage(validationContext.ModelMetadata,
          validationContext.ModelMetadata.GetDisplayName());
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos

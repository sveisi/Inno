using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Inno.Helper.ConventionalMetadataProviders {
/*
https://github.com/ridercz/Altairis.ConventionalMetadataProviders/wiki/Creating-Validation-Resource   
Key naming conventions

The validation metadata provider is based on conventions and tries to find metadata based on the following naming scheme:

[[[Namespace_]TypeName_]PropertyName_]ValidatorType

It's actually simple. Let's say you want to display error message for [Required] attribute of property with full name is Company.Project.Pages.IndexModel+Input.FullName. The provider will try to get values of the following keys, stopping on the first where it succeeds:

Company_Project_Pages_IndexModel_Input_FullName_Required
Project_Pages_IndexModel_Input_FullName_Required
Pages_IndexModel_Input_FullName_Required
IndexModel_Input_FullName_Required
Input_FullName_Required
FullName_Required
Required
*/
    public class ConventionalValidationMetadataProvider : IValidationMetadataProvider {
        private const string AttributeNameSuffix = "Attribute";

        private readonly ResourceManager resourceManager;
        private readonly Type resourceType;

        public ConventionalValidationMetadataProvider() : this(typeof(Resources.ValidationMessages)) {
        }

        public ConventionalValidationMetadataProvider(Type resourceType) {
            this.resourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
            this.resourceManager = new ResourceManager(resourceType);
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Add Required attribute to value types to simplify localization
            if (context.Key.ModelType.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(context.Key.ModelType.GetTypeInfo()) == null && !context.ValidationMetadata.ValidatorMetadata.OfType<RequiredAttribute>().Any()) {
                context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute());
            }

            foreach (var attribute in context.ValidationMetadata.ValidatorMetadata) {
                if (!(attribute is ValidationAttribute validationAttribute)) continue; // Not a validation attribute

                // Do nothing if custom error message or localization options are specified
                if (!(string.IsNullOrWhiteSpace(validationAttribute.ErrorMessage) || attribute is DataTypeAttribute)) continue;
                if (!string.IsNullOrWhiteSpace(validationAttribute.ErrorMessageResourceName) && validationAttribute.ErrorMessageResourceType != null) continue;

                // Get attribute name without the "Attribute" suffix
                var attributeName = validationAttribute.GetType().Name;
                if (attributeName.EndsWith(AttributeNameSuffix, StringComparison.Ordinal)) attributeName = attributeName.Substring(0, attributeName.Length - AttributeNameSuffix.Length);

                // Link to resource if exists
                var resourceKey = this.resourceManager.GetResourceKeyName(context.Key, attributeName, allowSuffixOnly: true);
                if (resourceKey != null) {
                    validationAttribute.ErrorMessageResourceType = this.resourceType;
                    if (string.IsNullOrEmpty(validationAttribute.ErrorMessageResourceName))
                        validationAttribute.ErrorMessageResourceName = resourceKey;
                    validationAttribute.ErrorMessage = null;
                } else {
                    validationAttribute.ErrorMessage = $"Missing resource key for '{attributeName}'.";
                }

            }
        }

    }
}

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

/*
https://github.com/ridercz/Altairis.ConventionalMetadataProviders
Key naming conventions
The display provider is based on conventions and tries to find metadata based on the following naming scheme:

[[Namespace_]TypeName_]PropertyName[_MetadataType]

It's actually simple. Let's say you want to display Description of property with full name is Company.Project.Pages.IndexModel+Input.FullName. The provider will try to get values of the following keys, stopping on the first where it succeeds:

    Company_Project_Pages_IndexModel_Input_FullName_Description
    Project_Pages_IndexModel_Input_FullName_Description
    Pages_IndexModel_Input_FullName_Description
    IndexModel_Input_FullName_Description
    Input_FullName_Description
    FullName_Description
 */
namespace Inno.Helper.ConventionalMetadataProviders {
    public class ConventionalDisplayMetadataProvider : IDisplayMetadataProvider {
        private readonly ResourceManager resourceManager;
        private readonly Type resourceType;

        public ConventionalDisplayMetadataProvider(Type resourceType) {
            this.resourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
            this.resourceManager = new ResourceManager(resourceType);
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.UpdateDisplayName(context);
            this.UpdateDescription(context);
            this.UpdatePlaceholder(context);
            this.UpdateNullDisplayText(context);
            this.UpdateDisplayFormatString(context);
            this.UpdateEditorFormatString(context);
        }

        private void UpdateDisplayName(DisplayMetadataProviderContext context)
        {
            var contextKeyName = context.Key.Name;
            // Special cases
            if (string.IsNullOrWhiteSpace(contextKeyName)) return;
            if (!string.IsNullOrWhiteSpace(context.DisplayMetadata.SimpleDisplayProperty)) return;
            if (context.Attributes.OfType<DisplayAttribute>().Any(x => !string.IsNullOrWhiteSpace(x.Name))) return;

            //change with Sirous
            //if use DisplayNameAttribute change key name to it
            string keyName;
            var diplayNameAttr = context.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (diplayNameAttr != null && !string.IsNullOrWhiteSpace(diplayNameAttr.DisplayName))
            {
                var resourceString = resourceManager.GetString(diplayNameAttr.DisplayName);
                if (resourceString != null)
                    context.DisplayMetadata.DisplayName = () => this.resourceManager.GetString(diplayNameAttr.DisplayName);
                else
                    context.DisplayMetadata.DisplayName = () => diplayNameAttr.DisplayName.SplitUpperCaseToString();
            }
            else
            {
                // Try get resource key name
                keyName = this.resourceManager.GetResourceKeyName(context.Key, null) ?? this.resourceManager.GetResourceKeyName(context.Key, "Name");
                if (keyName != null)
                    context.DisplayMetadata.DisplayName = () => this.resourceManager.GetString(keyName);
                else
                    context.DisplayMetadata.DisplayName = () => contextKeyName.SplitUpperCaseToString();
            }
        }

        private void UpdateDescription(DisplayMetadataProviderContext context) {
            // Special cases
            if (context.Attributes.OfType<DisplayAttribute>().Any(x => !string.IsNullOrWhiteSpace(x.Description))) return;

            // Try get resource key name
            var keyName = this.resourceManager.GetResourceKeyName(context.Key, "Description");
            if (keyName != null) context.DisplayMetadata.Description = () => this.resourceManager.GetString(keyName);
        }

        private void UpdatePlaceholder(DisplayMetadataProviderContext context) {
            // Special cases
            if (context.Attributes.OfType<DisplayAttribute>().Any(x => !string.IsNullOrWhiteSpace(x.Prompt))) return;

            // Try get resource key name
            var keyName = this.resourceManager.GetResourceKeyName(context.Key, "Placeholder");
            if (keyName != null) context.DisplayMetadata.Placeholder = () => this.resourceManager.GetString(keyName);
        }

        private void UpdateNullDisplayText(DisplayMetadataProviderContext context) {
            var keyName = this.resourceManager.GetResourceKeyName(context.Key, "Null");
            if (keyName != null) context.DisplayMetadata.NullDisplayTextProvider = () => this.resourceManager.GetString(keyName);
        }

        private void UpdateDisplayFormatString(DisplayMetadataProviderContext context) {
            var keyName = this.resourceManager.GetResourceKeyName(context.Key, "DisplayFormat");
            if (keyName != null) context.DisplayMetadata.DisplayFormatStringProvider = () => this.resourceManager.GetString(keyName);
        }

        private void UpdateEditorFormatString(DisplayMetadataProviderContext context) {
            var keyName = this.resourceManager.GetResourceKeyName(context.Key, "EditFormat");
            if (keyName != null) context.DisplayMetadata.EditFormatStringProvider = () => this.resourceManager.GetString(keyName);
        }


    }
}

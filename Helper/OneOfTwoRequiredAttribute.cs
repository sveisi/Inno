using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Inno.Helper
{
    //از سورس کامپیراتریبیوت استفاده شده ولی نشد که آذرپراپرتی لواکالایز شود و نامش نمایش داده شود
    //یک روز وقت صرف کردم. البته میتوانستم دستی انجام دهم
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OneOfTwoNumberRequiredAttribute : ValidationAttribute
    {
        public OneOfTwoNumberRequiredAttribute(string otherProperty)
            : base(Resources.ValidationMessages.OneOfTwoNumberRequired)
        {
            if (otherProperty == null)
            {
                throw new ArgumentNullException("otherProperty");
            }
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; private set; }

        public string OtherPropertyDisplayName { get; internal set; }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherPropertyDisplayName ?? OtherProperty);
        }

        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "UnknownProperty", OtherProperty));
            }

            object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (Equals(value, otherPropertyValue))
            {
                if (OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = GetDisplayNameForProperty(validationContext.ObjectType, OtherProperty);
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                var num1 = Convert.ToDecimal(value);
                var num2 = Convert.ToDecimal(otherPropertyValue);
                if ((num1 > 0 && num2 > 0) || (num1 == 0 && num2 == 0))
                {
                    if (OtherPropertyDisplayName == null)
                    {
                        OtherPropertyDisplayName = GetDisplayNameForProperty(validationContext.ObjectType, OtherProperty);
                    }
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return null;
        }

        private static string GetDisplayNameForProperty(Type containerType, string propertyName)
        {
            ICustomTypeDescriptor typeDescriptor = GetTypeDescriptor(containerType);
            PropertyDescriptor property = typeDescriptor.GetProperties().Find(propertyName, true);
            if (property == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                    "PropertyNotFound", containerType.FullName, propertyName));
            }
            IEnumerable<Attribute> attributes = property.Attributes.Cast<Attribute>();
            DisplayAttribute display = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (display != null)
            {
                return display.GetName();
            }
            DisplayNameAttribute displayName = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayName != null)
            {
                return displayName.DisplayName;
            }
            return propertyName;
        }

        private static ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class OneOfTwoNumberRequiredAttribute1 : ValidationAttribute
    {
        private readonly string property1Name;
        private readonly string property2Name;

        public OneOfTwoNumberRequiredAttribute1(string property1, string property2)
        {
            this.property1Name = property1;
            this.property2Name = property2;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property1 = validationContext.ObjectType.GetProperty(property1Name).GetValue(validationContext.ObjectInstance, null);

            var property2 = validationContext.ObjectType.GetProperty(property2Name).GetValue(validationContext.ObjectInstance, null);

            if ((property1 == null && property2 == null) || (property1 != null && property2 != null))
            {
                var num1 = Convert.ToDecimal(property1);
                var num2 = Convert.ToDecimal(property2);
                if ((num1 > 0 && num2 > 0) || (num1 == 0 && num2 == 0))
                {
                    System.Resources.ResourceManager a = new System.Resources.ResourceManager(ErrorMessageResourceType);
                    var p = a.GetString(property1Name);
                    return new ValidationResult(string.Format(ErrorMessageString, property1Name, property2Name));
                }
            }

            return ValidationResult.Success;
        }
    }
}
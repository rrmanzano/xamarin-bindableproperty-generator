﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Xamarin.BindableProperty.Generator.Helpers;
using Xamarin.BindableProperty.Generator.Core.BindableProperty.Implementation;
using Xamarin.BindableProperty.Generator.Core.BindableProperty.Implementation.Interfaces;

namespace Xamarin.BindableProperty.Generator.Core.BindableProperty
{

    [Generator]
    public class AutoBindablePropertyGenerator : ISourceGenerator
    {
        private TypedConstant NameProperty { get; set; }
        private readonly List<IImplementation> CustomImplementations = new() { new DefaultValue(), new PropertyChanged(), new DefaultBindingMode() };

        private const string attributeText = @"
        #pragma warning disable
        #nullable enable
        using System;
        namespace Xamarin.BindableProperty.Generator.Core
        {
            [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
            [System.Diagnostics.Conditional(""AutoBindableGenerator_DEBUG"")]
            public sealed class AutoBindableAttribute : Attribute
            {
                public AutoBindableAttribute(){}

                public string PropertyName { get; set; } = string.Empty;

                public string? OnChanged { get; set; }

                public string? DefaultValue { get; set; }

                public string? DefaultBindingMode { get; set; }

                public bool HidesUnderlyingProperty { get; set; } = false;
            }
        }";

        public void Execute(GeneratorExecutionContext context)
        {
            context.EachField<AutoBindableSyntaxReceiver>(AutoBindableConstants.AttrClassDisplayString, (attributeSymbol, group) => {
                var classSource = this.ProcessClass(group.Key, group.ToList(), attributeSymbol, context);
                context.AddSource($"{group.Key.Name}.generated.cs", SourceText.From(classSource, Encoding.UTF8));
            });
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; // TODO: issue a diagnostic that it must be top level
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            var w = new CodeWriter(CodeWriterSettings.CSharpDefault);
            w._("#pragma warning disable");
            w._("#nullable enable");
            using (w.B(@$"namespace {namespaceName}"))
            {
                w._(AutoBindableConstants.AttrGeneratedCodeString);
                using (w.B(@$"public partial class {classSymbol.Name}"))
                {
                    // Create properties for each field 
                    foreach (IFieldSymbol fieldSymbol in fields)
                    {
                        this.ProcessBindableProperty(w, fieldSymbol, attributeSymbol, classSymbol);
                    }
                }
            }

            return w.ToString();
        }

        private void ProcessBindableProperty(CodeWriter w, IFieldSymbol fieldSymbol, ISymbol attributeSymbol, INamedTypeSymbol classSymbol)
        {
            // Get the name and type of the field
            var fieldName = fieldSymbol.Name;
            var fieldType = fieldSymbol.Type;

            this.InitializeAttrProperties(fieldSymbol, attributeSymbol, classSymbol);

            var propertyName = this.ChooseName(fieldName, this.NameProperty);
            if (propertyName?.Length == 0 || propertyName == fieldName)
            {
                // TODO: issue a diagnostic that we can't process this field
                return;
            }

            var bindablePropertyName = $@"{propertyName}Property";
            var customParameters = this.ProcessBindableParameters();
            var applyHidesUnderlying = fieldSymbol.GetValue<bool>(attributeSymbol, AutoBindableConstants.AttrHidesUnderlyingProperty);
            var hidesUnderlying = applyHidesUnderlying ? " new" : string.Empty;
            var declaringType = fieldType.WithNullableAnnotation(NullableAnnotation.None);
            var parameters = $"nameof({propertyName}),typeof({declaringType}),typeof({classSymbol.Name}){customParameters}".Split(',');
            w._(AutoBindableConstants.AttrGeneratedCodeString);
            w._($@"public static{hidesUnderlying} readonly {AutoBindableConstants.FullNameXamarinControls}.BindableProperty {bindablePropertyName} =");
            w._($"{w.GetIndentString(6)}{AutoBindableConstants.FullNameXamarinControls}.BindableProperty.Create(");

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var ends = i < parameters.Length - 1 ? "," : ");";
                w._($"{w.GetIndentString(10)}{parameter}{ends}");
            }

            w._();
            w._(AutoBindableConstants.AttrGeneratedCodeString);
            using (w.B(@$"public{hidesUnderlying} {fieldType} {propertyName}"))
            {
                w._($@"get => ({fieldType})GetValue({bindablePropertyName});");
                if (this.ExistsBodySetter())
                {
                    using (w.B(@$"set"))
                    {
                        w._($@"SetValue({bindablePropertyName}, value);");
                        this.ProcessBodySetter(w);
                    }
                }
                else
                {
                    w._($@"set => SetValue({bindablePropertyName}, value);");
                }
            }

            this.ProcessImplementationLogic(w);
        }

        private void InitializeAttrProperties(IFieldSymbol fieldSymbol, ISymbol attributeSymbol, INamedTypeSymbol classSymbol)
        {
            this.NameProperty = fieldSymbol.GetTypedConstant(attributeSymbol, AutoBindableConstants.AttrPropertyName);
            this.CustomImplementations.ForEach(i => i.Initialize(this.NameProperty, fieldSymbol, attributeSymbol, classSymbol));
        }

        private string ProcessBindableParameters()
        {
            var parameters = this.CustomImplementations
                                    .Select(i => i.ProcessBindableParameters())
                                    .Where(x => !string.IsNullOrEmpty(x)).ToArray();
            
            return parameters.Length > 0 ? $@",{ string.Join(",", parameters) }" : string.Empty;
        }

        private void ProcessBodySetter(CodeWriter w)
        {
            this.CustomImplementations
                    .ForEach(i => i.ProcessBodySetter(w));
        }

        private void ProcessImplementationLogic(CodeWriter w)
        {
            this.CustomImplementations
                .ForEach(i => i.ProcessImplementationLogic(w));
        }

        private bool ExistsBodySetter()
        {
            return this.CustomImplementations.Any(i => i.SetterImplemented());
        }

        private string ChooseName(string fieldName, TypedConstant overridenNameOpt)
        {
            if (!overridenNameOpt.IsNull)
            {
                return overridenNameOpt.Value?.ToString();
            }

            fieldName = fieldName.TrimStart('_');
            if (fieldName.Length == 0)
                return string.Empty;

            if (fieldName.Length == 1)
                return fieldName.ToUpper();

            return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource(AutoBindableConstants.AttrName, attributeText));

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new AutoBindableSyntaxReceiver());
        }
    }
}

﻿using Xamarin.BindableProperty.Generator.Core.BindableProperty.Implementation.Interfaces;
using Xamarin.BindableProperty.Generator.Helpers;
using Microsoft.CodeAnalysis;

namespace Xamarin.BindableProperty.Generator.Core.BindableProperty.Implementation
{
    public class PropertyChanged : IImplementation
    {
        private TypedConstant NameProperty { get; set; }
        private TypedConstant OnChangedProperty { get; set; }
        private IFieldSymbol FieldSymbol { get; set; }
        private ISymbol AttributeSymbol { get; set; }
        private INamedTypeSymbol ClassSymbol { get; set; }

        public void Initialize(TypedConstant nameProperty, IFieldSymbol fieldSymbol, ISymbol attributeSymbol, INamedTypeSymbol classSymbol)
        {
            this.NameProperty = nameProperty;
            this.OnChangedProperty = fieldSymbol.GetTypedConstant(attributeSymbol, AutoBindableConstants.AttrOnChanged);
            this.FieldSymbol = fieldSymbol;
            this.AttributeSymbol = attributeSymbol;
            this.ClassSymbol = classSymbol;
        }

        public bool SetterImplemented()
        {
            return false;
        }

        public string ProcessBindableParameters()
        {
            return this.OnChangedProperty.GetValue<string>(methodName => {
                return $@"propertyChanged: __{methodName}";
            });
        }

        public void ProcessBodySetter(CodeWriter w)
        {
            // Not implemented
        }

        public void ProcessImplementationLogic(CodeWriter w)
        {
            this.OnChangedProperty.GetValue<string>(methodName => {
                var methodDefinition = @$"private static void __{methodName}({AutoBindableConstants.FullNameXamarinControls}.BindableObject bindable, object oldValue, object newValue)";

                if (w.ToString().Contains(methodDefinition))
                    return default;

                AttributeBuilder.WriteAllAttrGeneratedCodeStrings(w);
                using (w.B(methodDefinition))
                {
                    var methods = this.GetMethodsToCall(methodName);
                    if (methods.Any())
                    {
                        w._($@"var ctrl = ({this.ClassSymbol.Name})bindable;");
                        methods.ToList().ForEach(m => {
                            var count = m.Parameters.Count();
                            if (count == 0)
                                w._($@"ctrl.{methodName}();");
                            else if (count == 1)
                                w._($@"ctrl.{methodName}(({this.FieldSymbol.Type})newValue);");
                            else if (count == 2)
                                w._($@"ctrl.{methodName}(({this.FieldSymbol.Type})oldValue, ({this.FieldSymbol.Type})newValue);");
                        });
                    }
                }

                return default;
            });
        }

        private IEnumerable<IMethodSymbol> GetMethodsToCall(string methodName)
        {
            var typeSymbol = this.FieldSymbol.Type;
            var methods = this.ClassSymbol.GetMembers(methodName)
                            .OfType<IMethodSymbol>()
                            .Where(m => m != null && (m.Parameters.Count() == 0 || (m.Parameters.Count() <= 2 && m.Parameters.All(p => p.Type.Equals(typeSymbol, SymbolEqualityComparer.Default)))));

            return methods.OrderBy(m => m.Parameters.Count());
        }
    }
}

﻿using Microsoft.CodeAnalysis;

namespace Xamarin.BindableProperty.Generator.Core.BindableProperty.Implementation.Interfaces
{
    public interface IImplementation
    {
        void Initialize(TypedConstant nameProperty, IFieldSymbol fieldSymbol, ISymbol attributeSymbol, INamedTypeSymbol classSymbol);
        bool SetterImplemented();
        string ProcessBindableParameters();
        void ProcessBodySetter(CodeWriter w);
        void ProcessImplementationLogic(CodeWriter w);
    }
}

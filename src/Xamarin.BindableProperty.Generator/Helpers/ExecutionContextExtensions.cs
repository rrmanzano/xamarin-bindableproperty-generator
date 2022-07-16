﻿using Microsoft.CodeAnalysis;
using Xamarin.BindableProperty.Generator.Core;

namespace Xamarin.BindableProperty.Generator.Helpers
{
    internal static class ExecutionContextExtensions
    {
        public static void EachField<T>(
            this GeneratorExecutionContext context,
            string metadataName,
            Action<INamedTypeSymbol, IGrouping<INamedTypeSymbol, IFieldSymbol>> action) where T : IFieldSyntaxContextReceiver
        {
            // Retrieve the populated receiver 
            if (context.SyntaxContextReceiver is not T receiver)
                return;

            if (receiver is null)
                return;

            // Get the added attribute
            var attributeSymbol = context.Compilation?.GetTypeByMetadataName(metadataName);

            // Group the fields by class, and generate the source
#pragma warning disable RS1024 // Symbols should be compared for equality
            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.Fields?.GroupBy(f => f.ContainingType) ?? Enumerable.Empty<IGrouping<INamedTypeSymbol, IFieldSymbol>>())
            {
                action?.Invoke(attributeSymbol, group);
            }
#pragma warning restore RS1024 // Symbols should be compared for equality
        }
    }
}

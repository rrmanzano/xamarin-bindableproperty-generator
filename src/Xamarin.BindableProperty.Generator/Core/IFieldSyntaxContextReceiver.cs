using Microsoft.CodeAnalysis;

namespace Xamarin.BindableProperty.Generator.Core
{
    internal interface IFieldSyntaxContextReceiver
    {
        List<IFieldSymbol> Fields { get; }
    }
}

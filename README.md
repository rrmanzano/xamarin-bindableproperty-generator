[![NuGet](http://img.shields.io/nuget/vpre/X.BindableProperty.Generator.svg?label=NuGet)](https://www.nuget.org/packages/X.BindableProperty.Generator/) [![GitHub issues](https://img.shields.io/github/issues/rrmanzano/xamarin-bindableproperty-generator?style=flat-square)](https://github.com/rrmanzano/xamarin-bindableproperty-generator/) [![GitHub stars](https://img.shields.io/github/stars/rrmanzano/xamarin-bindableproperty-generator?style=flat-square)](https://github.com/rrmanzano/xamarin-bindableproperty-generator/stargazers) ![last commit](https://img.shields.io/github/last-commit/rrmanzano/xamarin-bindableproperty-generator?style=flat-square)

# Xamarin.BindableProperty.Generator

Source generator that automatically transforms fields into BindableProperties that can be used in Xamarin.<br />
Are you looking for the MAUI project? check this [link](https://github.com/rrmanzano/maui-bindableproperty-generator)

## Installation
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [X.BindableProperty.Generator](https://www.nuget.org/packages/X.BindableProperty.Generator/) from the package manager console:
````bash
PM> Install-Package X.BindableProperty.Generator
````

## Usage - Simple implementation
Just decorate field with the Bindable attribute.

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class CustomEntry : ContentView
    {
        [AutoBindable]
        private string _placeholder;
    }
```
the prevoius code will generate this:
```csharp
    public partial class CustomEntry
    {
        public static readonly Xamarin.Forms.BindableProperty PlaceholderProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(Placeholder),
                                                    typeof(string),
                                                    typeof(CustomEntry),
                                                    default(string));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
    }
```

## Usage - Custom property name
Just decorate field with the Bindable attribute.

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class CustomEntry : ContentView
    {
        [AutoBindable(PropertyName = "Text")]
        private string _t;
    }
```
the prevoius code will generate this:
```csharp
    public partial class CustomEntry
    {
        public static readonly Xamarin.Forms.BindableProperty TextProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(Text),
                                                    typeof(string),
                                                    typeof(CustomEntry),
                                                    default(string));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
```

## Usage - OnChanged method 

### Example 1 - No Parameters
Just decorate field with the Bindable attribute.

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(OnChanged = nameof(UpdateDisplayName))]
        private string _firstName;

        private void UpdateDisplayName()
        { 
            // Do stuff here
        }
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static readonly Xamarin.Forms.BindableProperty FirstNameProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(FirstName),
                                                    typeof(string),
                                                    typeof(HeaderControl),
                                                    defaultValue: default(string),
                                                    propertyChanged: __UpdateDisplayName);

        public string FirstName
        {
            get => (string)GetValue(FirstNameProperty);
            set => SetValue(FirstNameProperty, value);
        }

        private static void __UpdateDisplayName(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (HeaderControl)bindable;
            ctrl.UpdateDisplayName();
        }
    }
```

### Example 2 - One Parameter
Just decorate field with the Bindable attribute. The 'UpdateDisplayName' method must have only one parameter (must match the type of the field)

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(OnChanged = nameof(UpdateDisplayName))]
        private string _firstName;

        private void UpdateDisplayName(string newValue)
        { 
            // Do stuff here
        }
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static readonly Xamarin.Forms.BindableProperty FirstNameProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(FirstName),
                                                    typeof(string),
                                                    typeof(HeaderControl),
                                                    defaultValue: default(string),
                                                    propertyChanged: __UpdateDisplayName);

        public string FirstName
        {
            get => (string)GetValue(FirstNameProperty);
            set => SetValue(FirstNameProperty, value);
        }

        private static void __UpdateDisplayName(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (HeaderControl)bindable;
            ctrl.UpdateDisplayName((string)newValue);
        }
    }
```

### Example 3 - Two Parameters
Just decorate field with the Bindable attribute. The 'UpdateDisplayName' method must have two parameters (must match the type of the field)

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(OnChanged = nameof(UpdateDisplayName))]
        private string _firstName;

        private void UpdateDisplayName(string oldValue, string newValue)
        { 
            // Do stuff here
        }
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static readonly Xamarin.Forms.BindableProperty FirstNameProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(FirstName),
                                                    typeof(string),
                                                    typeof(HeaderControl),
                                                    defaultValue: default(string),
                                                    propertyChanged: __UpdateDisplayName);

        public string FirstName
        {
            get => (string)GetValue(FirstNameProperty);
            set => SetValue(FirstNameProperty, value);
        }

        private static void __UpdateDisplayName(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            var ctrl = (HeaderControl)bindable;
            ctrl.UpdateDisplayName((string)oldValue, (string)newValue);
        }
    }
```

## Usage - Set default value

### Example 1 - DateTime
Just decorate field with the Bindable attribute and add the "text/value" that you want to use as default value.

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(DefaultValue = "System.DateTime.Now")]
        private DateTime _birthDate;
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static readonly Xamarin.Forms.BindableProperty BirthDateProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(BirthDate),
                                                    typeof(System.DateTime),
                                                    typeof(HeaderControl),
                                                    defaultValue: System.DateTime.Now);

        public System.DateTime BirthDate
        {
            get => (System.DateTime)GetValue(BirthDateProperty);
            set => SetValue(BirthDateProperty, value);
        }
    }
```

### Example 2 - String
Just decorate field with the Bindable attribute and add the "text/value" that you want to use as default value.

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(DefaultValue = "USA")]
        private string _country;
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static readonly Xamarin.Forms.BindableProperty CountryProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(Country),
                                                    typeof(string),
                                                    typeof(HeaderControl),
                                                    defaultValue: "USA");

        public string Country
        {
            get => (string)GetValue(CountryProperty);
            set => SetValue(CountryProperty, value);
        }
    }
```

## Usage - Set default BindingMode
Just decorate field with the Bindable attribute and add the "BindingMode" that you want to use as default value.

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(DefaultBindingMode = nameof(BindingMode.TwoWay))]
        private string _firstName;
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static readonly Xamarin.Forms.BindableProperty FirstNameProperty =
                                    Xamarin.Forms.BindableProperty.Create(
                                                    nameof(FirstName),
                                                    typeof(string),
                                                    typeof(HeaderControl),
                                                    defaultValue: default(string),
                                                    defaultBindingMode: Xamarin.Forms.BindingMode.TwoWay);

        public string FirstName
        {
            get => (string)GetValue(FirstNameProperty);
            set => SetValue(FirstNameProperty, value);
        }
    }
```

## Usage - Hide existing BindableProperties
Just decorate field with the Bindable attribute and set "HidesUnderlyingProperty = true".

```csharp
    using Xamarin.BindableProperty.Generator.Core;

    public partial class HeaderControl : ContentView
    {
        [AutoBindable(HidesUnderlyingProperty = true)]
        private readonly Color _backgroundColor;
    }
```
the prevoius code will generate this:
```csharp
    public partial class HeaderControl
    {
        public static new readonly Xamarin.Forms.BindableProperty BackgroundColorProperty =
                                        Xamarin.Forms.BindableProperty.Create(
                                                        nameof(BackgroundColor),
                                                        typeof(Microsoft.Maui.Graphics.Color),
                                                        typeof(HeaderControl));

        public new Microsoft.Maui.Graphics.Color BackgroundColor
        {
            get => (Microsoft.Maui.Graphics.Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
    }
```

## Project status

- ✅ Simple implementation - Done
- ✅ Custom property name - Done
- ✅ Custom Parameters - Done
- ✅ OnChanged method - Done
- ✅ OnChanged method overloading - Done

## Extra info
This repo is using part of the code of [CodeWriter](https://github.com/SaladLab/CodeWriter "CodeWriter") to generate the CSharp files, thanks to the author.
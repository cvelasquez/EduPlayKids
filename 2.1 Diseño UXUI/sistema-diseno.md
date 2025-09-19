# Sistema de Diseño Técnico - EduPlayKids

## Información del Documento
- **Proyecto**: EduPlayKids - Aplicación Educativa Móvil
- **Versión**: 1.0
- **Fecha**: Septiembre 2025
- **Autor**: Equipo de Diseño UX/UI
- **Framework**: .NET MAUI (Android Primario)

---

## Arquitectura del Sistema de Diseño

### Estructura Modular

#### Tokens de Diseño (Design Tokens)
```json
{
  "color": {
    "primary": {
      "blue": { "value": "#4285F4" },
      "green": { "value": "#34A853" },
      "yellow": { "value": "#FBBC04" },
      "red": { "value": "#EA4335" }
    },
    "semantic": {
      "success": { "value": "{color.primary.green}" },
      "warning": { "value": "{color.primary.yellow}" },
      "error": { "value": "{color.primary.red}" },
      "info": { "value": "{color.primary.blue}" }
    }
  },
  "spacing": {
    "scale": {
      "0": { "value": "0dp" },
      "1": { "value": "4dp" },
      "2": { "value": "8dp" },
      "3": { "value": "12dp" },
      "4": { "value": "16dp" },
      "5": { "value": "20dp" },
      "6": { "value": "24dp" },
      "8": { "value": "32dp" },
      "10": { "value": "40dp" },
      "12": { "value": "48dp" }
    }
  },
  "typography": {
    "fontFamily": {
      "primary": { "value": "Nunito" },
      "secondary": { "value": "Roboto" }
    },
    "fontSize": {
      "xs": { "value": "10sp" },
      "sm": { "value": "12sp" },
      "base": { "value": "14sp" },
      "md": { "value": "16sp" },
      "lg": { "value": "18sp" },
      "xl": { "value": "20sp" },
      "2xl": { "value": "24sp" },
      "3xl": { "value": "28sp" },
      "4xl": { "value": "32sp" }
    }
  }
}
```

#### Implementación en .NET MAUI
```xml
<!-- Tokens/DesignTokens.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    
    <!-- Color Tokens -->
    <Color x:Key="TokenColorPrimaryBlue">#4285F4</Color>
    <Color x:Key="TokenColorPrimaryGreen">#34A853</Color>
    <Color x:Key="TokenColorPrimaryYellow">#FBBC04</Color>
    <Color x:Key="TokenColorPrimaryRed">#EA4335</Color>
    
    <!-- Semantic Color Tokens -->
    <Color x:Key="TokenColorSuccess">{StaticResource TokenColorPrimaryGreen}</Color>
    <Color x:Key="TokenColorWarning">{StaticResource TokenColorPrimaryYellow}</Color>
    <Color x:Key="TokenColorError">{StaticResource TokenColorPrimaryRed}</Color>
    <Color x:Key="TokenColorInfo">{StaticResource TokenColorPrimaryBlue}</Color>
    
    <!-- Spacing Tokens -->
    <x:Double x:Key="TokenSpacing0">0</x:Double>
    <x:Double x:Key="TokenSpacing1">4</x:Double>
    <x:Double x:Key="TokenSpacing2">8</x:Double>
    <x:Double x:Key="TokenSpacing3">12</x:Double>
    <x:Double x:Key="TokenSpacing4">16</x:Double>
    <x:Double x:Key="TokenSpacing5">20</x:Double>
    <x:Double x:Key="TokenSpacing6">24</x:Double>
    <x:Double x:Key="TokenSpacing8">32</x:Double>
    <x:Double x:Key="TokenSpacing10">40</x:Double>
    <x:Double x:Key="TokenSpacing12">48</x:Double>
    
    <!-- Typography Tokens -->
    <x:Double x:Key="TokenFontSizeXS">10</x:Double>
    <x:Double x:Key="TokenFontSizeSM">12</x:Double>
    <x:Double x:Key="TokenFontSizeBase">14</x:Double>
    <x:Double x:Key="TokenFontSizeMD">16</x:Double>
    <x:Double x:Key="TokenFontSizeLG">18</x:Double>
    <x:Double x:Key="TokenFontSizeXL">20</x:Double>
    <x:Double x:Key="TokenFontSize2XL">24</x:Double>
    <x:Double x:Key="TokenFontSize3XL">28</x:Double>
    <x:Double x:Key="TokenFontSize4XL">32</x:Double>
    
</ResourceDictionary>
```

---

## Componentes Atómicos

### Fundamentos (Atoms)

#### 1. Botones Base
```xml
<!-- Components/Atoms/Buttons.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Base Button Style -->
    <Style x:Key="ButtonBase" TargetType="Button">
        <Setter Property="FontFamily" Value="NunitoSemiBold" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="MinimumHeightRequest" Value="48" />
        <Setter Property="MinimumWidthRequest" Value="120" />
        <Setter Property="TextTransform" Value="Uppercase" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal">
                        <VisualState.Setters>
                            <Setter Property="Scale" Value="1" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Pressed">
                        <VisualState.Setters>
                            <Setter Property="Scale" Value="0.98" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="#E8EAED" />
                            <Setter Property="TextColor" Value="#5F6368" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <!-- Primary Button -->
    <Style x:Key="ButtonPrimary" TargetType="Button" BasedOn="{StaticResource ButtonBase}">
        <Setter Property="BackgroundColor" Value="{StaticResource TokenColorSuccess}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeMD}" />
        <Setter Property="Shadow">
            <Shadow Brush="Black" Opacity="0.3" Radius="8" Offset="0,4" />
        </Setter>
    </Style>

    <!-- Secondary Button -->
    <Style x:Key="ButtonSecondary" TargetType="Button" BasedOn="{StaticResource ButtonBase}">
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="TextColor" Value="{StaticResource TokenColorPrimaryBlue}" />
        <Setter Property="BorderColor" Value="{StaticResource TokenColorPrimaryBlue}" />
        <Setter Property="BorderWidth" Value="2" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeMD}" />
    </Style>

    <!-- Subject Button -->
    <Style x:Key="ButtonSubject" TargetType="Button" BasedOn="{StaticResource ButtonBase}">
        <Setter Property="WidthRequest" Value="140" />
        <Setter Property="HeightRequest" Value="130" />
        <Setter Property="CornerRadius" Value="16" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeLG}" />
        <Setter Property="Shadow">
            <Shadow Brush="Black" Opacity="0.15" Radius="12" Offset="0,4" />
        </Setter>
    </Style>

</ResourceDictionary>
```

#### 2. Tipografía Base
```xml
<!-- Components/Atoms/Typography.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Heading Styles -->
    <Style x:Key="HeadingHero" TargetType="Label">
        <Setter Property="FontFamily" Value="NunitoExtraBold" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSize4XL}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>

    <Style x:Key="HeadingSection" TargetType="Label">
        <Setter Property="FontFamily" Value="NunitoBold" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSize2XL}" />
        <Setter Property="TextColor" Value="#202124" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>

    <Style x:Key="HeadingActivity" TargetType="Label">
        <Setter Property="FontFamily" Value="NunitoSemiBold" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeXL}" />
        <Setter Property="TextColor" Value="#202124" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>

    <!-- Body Text Styles -->
    <Style x:Key="TextInstruction" TargetType="Label">
        <Setter Property="FontFamily" Value="NunitoMedium" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeLG}" />
        <Setter Property="TextColor" Value="#5F6368" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>

    <Style x:Key="TextBody" TargetType="Label">
        <Setter Property="FontFamily" Value="NunitoRegular" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeBase}" />
        <Setter Property="TextColor" Value="#202124" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>

    <Style x:Key="TextCaption" TargetType="Label">
        <Setter Property="FontFamily" Value="NunitoRegular" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeSM}" />
        <Setter Property="TextColor" Value="#5F6368" />
        <Setter Property="LineBreakMode" Value="WordWrap" />
    </Style>

</ResourceDictionary>
```

#### 3. Iconos Base
```xml
<!-- Components/Atoms/Icons.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Icon Base Style -->
    <Style x:Key="IconBase" TargetType="Image">
        <Setter Property="Aspect" Value="AspectFit" />
        <Setter Property="BackgroundColor" Value="Transparent" />
    </Style>

    <!-- Icon Sizes -->
    <Style x:Key="IconXS" TargetType="Image" BasedOn="{StaticResource IconBase}">
        <Setter Property="WidthRequest" Value="16" />
        <Setter Property="HeightRequest" Value="16" />
    </Style>

    <Style x:Key="IconSM" TargetType="Image" BasedOn="{StaticResource IconBase}">
        <Setter Property="WidthRequest" Value="20" />
        <Setter Property="HeightRequest" Value="20" />
    </Style>

    <Style x:Key="IconMD" TargetType="Image" BasedOn="{StaticResource IconBase}">
        <Setter Property="WidthRequest" Value="24" />
        <Setter Property="HeightRequest" Value="24" />
    </Style>

    <Style x:Key="IconLG" TargetType="Image" BasedOn="{StaticResource IconBase}">
        <Setter Property="WidthRequest" Value="32" />
        <Setter Property="HeightRequest" Value="32" />
    </Style>

    <Style x:Key="IconXL" TargetType="Image" BasedOn="{StaticResource IconBase}">
        <Setter Property="WidthRequest" Value="48" />
        <Setter Property="HeightRequest" Value="48" />
    </Style>

    <Style x:Key="Icon2XL" TargetType="Image" BasedOn="{StaticResource IconBase}">
        <Setter Property="WidthRequest" Value="64" />
        <Setter Property="HeightRequest" Value="64" />
    </Style>

</ResourceDictionary>
```

### Componentes Moleculares

#### 1. Cards Compuestas
```xml
<!-- Components/Molecules/Cards.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Base Card Style -->
    <Style x:Key="CardBase" TargetType="Frame">
        <Setter Property="BackgroundColor" Value="White" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="HasShadow" Value="True" />
        <Setter Property="Padding" Value="{StaticResource TokenSpacing4}" />
        <Setter Property="BorderColor" Value="#F1F3F4" />
        <Setter Property="Shadow">
            <Shadow Brush="Black" Opacity="0.08" Radius="8" Offset="0,2" />
        </Setter>
    </Style>

    <!-- Progress Card -->
    <Style x:Key="CardProgress" TargetType="Frame" BasedOn="{StaticResource CardBase}">
        <Setter Property="Padding" Value="{StaticResource TokenSpacing5}" />
        <Setter Property="BackgroundColor">
            <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                <GradientStop Color="White" Offset="0" />
                <GradientStop Color="#F8F9FA" Offset="1" />
            </LinearGradientBrush>
        </Setter>
    </Style>

    <!-- Stats Card -->
    <Style x:Key="CardStats" TargetType="Frame" BasedOn="{StaticResource CardBase}">
        <Setter Property="Padding" Value="{StaticResource TokenSpacing4}" />
        <Setter Property="MinimumHeightRequest" Value="80" />
    </Style>

</ResourceDictionary>
```

#### 2. Inputs Compuestos
```xml
<!-- Components/Molecules/Inputs.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Text Input Style -->
    <Style x:Key="InputText" TargetType="Entry">
        <Setter Property="FontFamily" Value="NunitoRegular" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeMD}" />
        <Setter Property="TextColor" Value="#202124" />
        <Setter Property="BackgroundColor" Value="White" />
        <Setter Property="PlaceholderColor" Value="#5F6368" />
        <Setter Property="HeightRequest" Value="48" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="White" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Focused">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="#F8F9FA" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <!-- Search Input Style -->
    <Style x:Key="InputSearch" TargetType="SearchBar">
        <Setter Property="FontFamily" Value="NunitoRegular" />
        <Setter Property="FontSize" Value="{StaticResource TokenFontSizeMD}" />
        <Setter Property="TextColor" Value="#202124" />
        <Setter Property="BackgroundColor" Value="#F8F9FA" />
        <Setter Property="PlaceholderColor" Value="#5F6368" />
        <Setter Property="CancelButtonColor" Value="{StaticResource TokenColorPrimaryBlue}" />
    </Style>

</ResourceDictionary>
```

#### 3. Navegación Compuesta
```xml
<!-- Components/Molecules/Navigation.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Navigation Bar Style -->
    <Style x:Key="NavigationBar" TargetType="Grid">
        <Setter Property="BackgroundColor" Value="White" />
        <Setter Property="HeightRequest" Value="56" />
        <Setter Property="Padding" Value="{StaticResource TokenSpacing4}" />
    </Style>

    <!-- Tab Bar Style -->
    <Style x:Key="TabBar" TargetType="TabBar">
        <Setter Property="BackgroundColor" Value="White" />
        <Setter Property="BarBackgroundColor" Value="White" />
    </Style>

    <!-- Tab Style -->
    <Style x:Key="Tab" TargetType="Tab">
        <Setter Property="Title" Value="Tab" />
    </Style>

</ResourceDictionary>
```

---

## Componentes Organizmicos

### 1. Header de Actividad
```xml
<!-- Components/Organisms/ActivityHeader.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="EduPlayKids.Components.ActivityHeader">
    
    <Frame Style="{StaticResource ActivityHeaderFrame}">
        <Grid ColumnDefinitions="Auto,*,Auto,Auto" 
              VerticalOptions="Center">
            
            <!-- Back Button -->
            <Button Grid.Column="0"
                    Style="{StaticResource ButtonIconOnly}"
                    Text="←"
                    Command="{Binding BackCommand}"
                    AutomationProperties.Name="Volver"
                    AutomationProperties.HelpText="Toca para volver al menú anterior" />
            
            <!-- Subject Title -->
            <Label Grid.Column="1"
                   Text="{Binding SubjectTitle}"
                   Style="{StaticResource HeadingActivity}"
                   TextColor="White"
                   VerticalOptions="Center"
                   HorizontalOptions="Start"
                   Margin="{StaticResource TokenSpacing3},0,0,0" />
            
            <!-- Lives Display -->
            <StackLayout Grid.Column="2"
                         Orientation="Horizontal"
                         VerticalOptions="Center"
                         Spacing="{StaticResource TokenSpacing1}">
                <Image Source="heart_full.png" 
                       Style="{StaticResource IconMD}"
                       IsVisible="{Binding Lives, Converter={StaticResource IntToBoolConverter}, ConverterParameter=1}" />
                <Image Source="heart_full.png" 
                       Style="{StaticResource IconMD}"
                       IsVisible="{Binding Lives, Converter={StaticResource IntToBoolConverter}, ConverterParameter=2}" />
                <Image Source="heart_full.png" 
                       Style="{StaticResource IconMD}"
                       IsVisible="{Binding Lives, Converter={StaticResource IntToBoolConverter}, ConverterParameter=3}" />
            </StackLayout>
            
            <!-- Timer -->
            <Label Grid.Column="3"
                   Text="{Binding TimeRemaining, StringFormat='{0:mm\\:ss}'}"
                   Style="{StaticResource TextTimer}"
                   TextColor="White"
                   VerticalOptions="Center"
                   Margin="{StaticResource TokenSpacing3},0,0,0"
                   AutomationProperties.Name="Tiempo restante"
                   AutomationProperties.HelpText="{Binding TimeRemaining, StringFormat='Tiempo restante: {0:mm} minutos {0:ss} segundos'}" />
                   
        </Grid>
    </Frame>
</ContentView>
```

```csharp
// Components/Organisms/ActivityHeader.xaml.cs
using Microsoft.Maui.Controls;

namespace EduPlayKids.Components;

public partial class ActivityHeader : ContentView
{
    public static readonly BindableProperty SubjectTitleProperty =
        BindableProperty.Create(nameof(SubjectTitle), typeof(string), typeof(ActivityHeader), default(string));

    public static readonly BindableProperty LivesProperty =
        BindableProperty.Create(nameof(Lives), typeof(int), typeof(ActivityHeader), 3);

    public static readonly BindableProperty TimeRemainingProperty =
        BindableProperty.Create(nameof(TimeRemaining), typeof(TimeSpan), typeof(ActivityHeader), TimeSpan.Zero);

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(ActivityHeader), default(ICommand));

    public string SubjectTitle
    {
        get => (string)GetValue(SubjectTitleProperty);
        set => SetValue(SubjectTitleProperty, value);
    }

    public int Lives
    {
        get => (int)GetValue(LivesProperty);
        set => SetValue(LivesProperty, value);
    }

    public TimeSpan TimeRemaining
    {
        get => (TimeSpan)GetValue(TimeRemainingProperty);
        set => SetValue(TimeRemainingProperty, value);
    }

    public ICommand BackCommand
    {
        get => (ICommand)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public ActivityHeader()
    {
        InitializeComponent();
        BindingContext = this;
    }
}
```

### 2. Subject Card Component
```xml
<!-- Components/Organisms/SubjectCard.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="EduPlayKids.Components.SubjectCard">
    
    <Button Style="{StaticResource ButtonSubject}"
            BackgroundColor="{Binding SubjectColor}"
            Command="{Binding SelectCommand}"
            IsEnabled="{Binding IsEnabled}"
            AutomationProperties.Name="{Binding AccessibilityName}"
            AutomationProperties.HelpText="{Binding AccessibilityDescription}">
        
        <Button.Content>
            <Grid RowDefinitions="Auto,Auto,Auto,Auto" 
                  VerticalOptions="Center">
                
                <!-- Subject Icon -->
                <Image Grid.Row="0"
                       Source="{Binding IconSource}"
                       Style="{StaticResource IconXL}"
                       VerticalOptions="Center"
                       HorizontalOptions="Center" />
                
                <!-- Subject Name -->
                <Label Grid.Row="1"
                       Text="{Binding SubjectName}"
                       Style="{StaticResource TextSubjectCard}"
                       TextColor="White"
                       HorizontalOptions="Center"
                       HorizontalTextAlignment="Center"
                       Margin="0,8,0,4" />
                
                <!-- Progress Stars -->
                <StackLayout Grid.Row="2"
                             Orientation="Horizontal"
                             HorizontalOptions="Center"
                             Spacing="2"
                             Margin="0,4">
                    <Image Source="{Binding Star1Source}" Style="{StaticResource IconXS}" />
                    <Image Source="{Binding Star2Source}" Style="{StaticResource IconXS}" />
                    <Image Source="{Binding Star3Source}" Style="{StaticResource IconXS}" />
                </StackLayout>
                
                <!-- Progress Text -->
                <Label Grid.Row="3"
                       Text="{Binding ProgressText}"
                       Style="{StaticResource TextProgressCard}"
                       TextColor="White"
                       HorizontalOptions="Center"
                       HorizontalTextAlignment="Center"
                       Margin="0,2,0,0" />
                       
            </Grid>
        </Button.Content>
        
        <!-- Visual States for Different States -->
        <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal">
                    <VisualState.Setters>
                        <Setter Property="Scale" Value="1" />
                        <Setter Property="Opacity" Value="1" />
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Pressed">
                    <VisualState.Setters>
                        <Setter Property="Scale" Value="0.98" />
                    </VisualState.Setters>
                </VisualState>
                <VisualState x:Name="Disabled">
                    <VisualState.Setters>
                        <Setter Property="BackgroundColor" Value="#E8EAED" />
                        <Setter Property="Opacity" Value="0.6" />
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>
    </Button>
</ContentView>
```

```csharp
// Components/Organisms/SubjectCard.xaml.cs
namespace EduPlayKids.Components;

public partial class SubjectCard : ContentView
{
    public static readonly BindableProperty SubjectNameProperty =
        BindableProperty.Create(nameof(SubjectName), typeof(string), typeof(SubjectCard));

    public static readonly BindableProperty IconSourceProperty =
        BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(SubjectCard));

    public static readonly BindableProperty SubjectColorProperty =
        BindableProperty.Create(nameof(SubjectColor), typeof(Color), typeof(SubjectCard));

    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(SubjectCard), 0.0, propertyChanged: OnProgressChanged);

    public static readonly BindableProperty SelectCommandProperty =
        BindableProperty.Create(nameof(SelectCommand), typeof(ICommand), typeof(SubjectCard));

    public static readonly BindableProperty IsLockedProperty =
        BindableProperty.Create(nameof(IsLocked), typeof(bool), typeof(SubjectCard), false, propertyChanged: OnIsLockedChanged);

    public string SubjectName
    {
        get => (string)GetValue(SubjectNameProperty);
        set => SetValue(SubjectNameProperty, value);
    }

    public ImageSource IconSource
    {
        get => (ImageSource)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public Color SubjectColor
    {
        get => (Color)GetValue(SubjectColorProperty);
        set => SetValue(SubjectColorProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public ICommand SelectCommand
    {
        get => (ICommand)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    // Computed Properties
    public string Star1Source => Progress >= 0.33 ? "star_filled.png" : "star_empty.png";
    public string Star2Source => Progress >= 0.66 ? "star_filled.png" : "star_empty.png";
    public string Star3Source => Progress >= 1.0 ? "star_filled.png" : "star_empty.png";
    public string ProgressText => IsLocked ? "Bloqueado" : $"{(int)(Progress * 100)}% ✓";
    public string AccessibilityName => IsLocked ? $"{SubjectName} bloqueado" : SubjectName;
    public string AccessibilityDescription => IsLocked 
        ? $"La materia {SubjectName} está bloqueada. Completa las materias anteriores para desbloquearla."
        : $"Materia {SubjectName}, progreso {(int)(Progress * 100)} por ciento completado. Toca para acceder a las actividades.";

    public SubjectCard()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SubjectCard card)
        {
            card.OnPropertyChanged(nameof(Star1Source));
            card.OnPropertyChanged(nameof(Star2Source));
            card.OnPropertyChanged(nameof(Star3Source));
            card.OnPropertyChanged(nameof(ProgressText));
            card.OnPropertyChanged(nameof(AccessibilityDescription));
        }
    }

    private static void OnIsLockedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SubjectCard card)
        {
            card.IsEnabled = !(bool)newValue;
            card.OnPropertyChanged(nameof(ProgressText));
            card.OnPropertyChanged(nameof(AccessibilityName));
            card.OnPropertyChanged(nameof(AccessibilityDescription));
        }
    }
}
```

### 3. Progress Summary Component
```xml
<!-- Components/Organisms/ProgressSummary.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="EduPlayKids.Components.ProgressSummary">
    
    <Frame Style="{StaticResource CardProgress}">
        <Grid RowDefinitions="Auto,Auto,*" 
              RowSpacing="{StaticResource TokenSpacing4}">
            
            <!-- Header -->
            <Label Grid.Row="0"
                   Text="📊 Progreso de {Binding ChildName}"
                   Style="{StaticResource HeadingSection}"
                   HorizontalOptions="Center" />
            
            <!-- Stats Row -->
            <Grid Grid.Row="1"
                  ColumnDefinitions="*,*"
                  ColumnSpacing="{StaticResource TokenSpacing4}">
                
                <!-- Left Stats -->
                <StackLayout Grid.Column="0" Spacing="{StaticResource TokenSpacing2}">
                    <Label Text="{Binding TodayTime, StringFormat='Tiempo hoy: {0} min'}"
                           Style="{StaticResource TextBody}" />
                    <Label Text="{Binding TotalActivities, StringFormat='Actividades: {0}'}"
                           Style="{StaticResource TextBody}" />
                </StackLayout>
                
                <!-- Right Stats -->
                <StackLayout Grid.Column="1" Spacing="{StaticResource TokenSpacing2}">
                    <Label Text="{Binding Streak, StringFormat='🔥 Racha: {0}'}"
                           Style="{StaticResource TextBody}" />
                    <Label Text="{Binding TotalStars, StringFormat='⭐ Total: {0}'}"
                           Style="{StaticResource TextBody}" />
                </StackLayout>
            </Grid>
            
            <!-- Weekly Chart Placeholder -->
            <Frame Grid.Row="2"
                   BackgroundColor="#F8F9FA"
                   CornerRadius="8"
                   HeightRequest="120"
                   Padding="{StaticResource TokenSpacing3}">
                <Label Text="📈 Gráfico Semanal"
                       Style="{StaticResource HeadingActivity}"
                       VerticalOptions="Center"
                       HorizontalOptions="Center" />
            </Frame>
            
        </Grid>
    </Frame>
</ContentView>
```

---

## Sistema de Theming

### 1. Light Theme (Default)
```xml
<!-- Themes/LightTheme.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Background Colors -->
    <Color x:Key="BackgroundPrimary">#FFFFFF</Color>
    <Color x:Key="BackgroundSecondary">#F8F9FA</Color>
    <Color x:Key="BackgroundTertiary">#F1F3F4</Color>
    
    <!-- Text Colors -->
    <Color x:Key="TextPrimary">#202124</Color>
    <Color x:Key="TextSecondary">#5F6368</Color>
    <Color x:Key="TextTertiary">#9E9E9E</Color>
    <Color x:Key="TextInverse">#FFFFFF</Color>
    
    <!-- Border Colors -->
    <Color x:Key="BorderPrimary">#E8EAED</Color>
    <Color x:Key="BorderSecondary">#DADCE0</Color>
    
    <!-- Surface Colors -->
    <Color x:Key="SurfaceElevated">#FFFFFF</Color>
    <Color x:Key="SurfaceDepressed">#F8F9FA</Color>
    
</ResourceDictionary>
```

### 2. High Contrast Theme
```xml
<!-- Themes/HighContrastTheme.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- High Contrast Background Colors -->
    <Color x:Key="BackgroundPrimary">#FFFFFF</Color>
    <Color x:Key="BackgroundSecondary">#000000</Color>
    <Color x:Key="BackgroundTertiary">#000000</Color>
    
    <!-- High Contrast Text Colors -->
    <Color x:Key="TextPrimary">#000000</Color>
    <Color x:Key="TextSecondary">#000000</Color>
    <Color x:Key="TextTertiary">#000000</Color>
    <Color x:Key="TextInverse">#FFFFFF</Color>
    
    <!-- High Contrast Border Colors -->
    <Color x:Key="BorderPrimary">#000000</Color>
    <Color x:Key="BorderSecondary">#000000</Color>
    
    <!-- High Contrast Surface Colors -->
    <Color x:Key="SurfaceElevated">#FFFFFF</Color>
    <Color x:Key="SurfaceDepressed">#E0E0E0</Color>
    
    <!-- Override Button Styles for High Contrast -->
    <Style x:Key="ButtonPrimaryHighContrast" TargetType="Button" BasedOn="{StaticResource ButtonPrimary}">
        <Setter Property="BackgroundColor" Value="#000000" />
        <Setter Property="TextColor" Value="#FFFFFF" />
        <Setter Property="BorderColor" Value="#FFFFFF" />
        <Setter Property="BorderWidth" Value="2" />
    </Style>

</ResourceDictionary>
```

### 3. Theme Manager Service
```csharp
// Services/ThemeService.cs
using Microsoft.Maui.Controls;

namespace EduPlayKids.Services;

public interface IThemeService
{
    void ApplyTheme(Theme theme);
    Theme GetCurrentTheme();
    bool IsHighContrastEnabled();
}

public enum Theme
{
    Light,
    HighContrast
}

public class ThemeService : IThemeService
{
    public void ApplyTheme(Theme theme)
    {
        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        
        // Remove existing theme
        mergedDictionaries.Clear();
        
        // Add base resources
        mergedDictionaries.Add(new Tokens.DesignTokens());
        mergedDictionaries.Add(new Components.Atoms.Buttons());
        mergedDictionaries.Add(new Components.Atoms.Typography());
        
        // Add theme-specific resources
        switch (theme)
        {
            case Theme.Light:
                mergedDictionaries.Add(new Themes.LightTheme());
                break;
            case Theme.HighContrast:
                mergedDictionaries.Add(new Themes.HighContrastTheme());
                break;
        }
        
        Preferences.Set("SelectedTheme", theme.ToString());
    }

    public Theme GetCurrentTheme()
    {
        var themeString = Preferences.Get("SelectedTheme", Theme.Light.ToString());
        return Enum.Parse<Theme>(themeString);
    }

    public bool IsHighContrastEnabled()
    {
        // Check system accessibility settings
        return GetCurrentTheme() == Theme.HighContrast;
    }
}
```

---

## Animaciones y Micro-interacciones

### 1. Animation Service
```csharp
// Services/AnimationService.cs
using Microsoft.Maui.Controls;

namespace EduPlayKids.Services;

public interface IAnimationService
{
    Task BounceAsync(VisualElement element, uint duration = 300);
    Task ShakeAsync(VisualElement element, uint duration = 500);
    Task PulseAsync(VisualElement element, uint duration = 1000);
    Task FadeInAsync(VisualElement element, uint duration = 300);
    Task FadeOutAsync(VisualElement element, uint duration = 300);
    Task SlideInFromRightAsync(VisualElement element, uint duration = 400);
    Task SlideOutToLeftAsync(VisualElement element, uint duration = 400);
    Task CelebrationAsync(VisualElement element, uint duration = 800);
    bool IsReduceMotionEnabled();
}

public class AnimationService : IAnimationService
{
    public bool IsReduceMotionEnabled()
    {
        return Preferences.Get("AccessibilityReduceMotion", false);
    }

    public async Task BounceAsync(VisualElement element, uint duration = 300)
    {
        if (IsReduceMotionEnabled()) return;
        
        await element.ScaleTo(0.95, duration / 3, Easing.CubicInOut);
        await element.ScaleTo(1.05, duration / 3, Easing.CubicInOut);
        await element.ScaleTo(1, duration / 3, Easing.CubicInOut);
    }

    public async Task ShakeAsync(VisualElement element, uint duration = 500)
    {
        if (IsReduceMotionEnabled()) return;
        
        var displacement = 10;
        var interval = duration / 8;
        
        await element.TranslateTo(-displacement, 0, interval);
        await element.TranslateTo(displacement, 0, interval);
        await element.TranslateTo(-displacement, 0, interval);
        await element.TranslateTo(displacement, 0, interval);
        await element.TranslateTo(-displacement, 0, interval);
        await element.TranslateTo(displacement, 0, interval);
        await element.TranslateTo(-displacement, 0, interval);
        await element.TranslateTo(0, 0, interval);
    }

    public async Task PulseAsync(VisualElement element, uint duration = 1000)
    {
        if (IsReduceMotionEnabled()) return;
        
        var originalOpacity = element.Opacity;
        
        await element.FadeTo(0.6, duration / 2, Easing.SinInOut);
        await element.FadeTo(originalOpacity, duration / 2, Easing.SinInOut);
    }

    public async Task FadeInAsync(VisualElement element, uint duration = 300)
    {
        element.Opacity = 0;
        element.IsVisible = true;
        await element.FadeTo(1, duration, Easing.CubicInOut);
    }

    public async Task FadeOutAsync(VisualElement element, uint duration = 300)
    {
        await element.FadeTo(0, duration, Easing.CubicInOut);
        element.IsVisible = false;
    }

    public async Task SlideInFromRightAsync(VisualElement element, uint duration = 400)
    {
        if (IsReduceMotionEnabled())
        {
            await FadeInAsync(element, duration);
            return;
        }
        
        element.TranslationX = element.Width;
        element.IsVisible = true;
        await element.TranslateTo(0, 0, duration, Easing.CubicOut);
    }

    public async Task SlideOutToLeftAsync(VisualElement element, uint duration = 400)
    {
        if (IsReduceMotionEnabled())
        {
            await FadeOutAsync(element, duration);
            return;
        }
        
        await element.TranslateTo(-element.Width, 0, duration, Easing.CubicIn);
        element.IsVisible = false;
    }

    public async Task CelebrationAsync(VisualElement element, uint duration = 800)
    {
        if (IsReduceMotionEnabled())
        {
            await PulseAsync(element, duration);
            return;
        }
        
        // Complex celebration animation
        var tasks = new List<Task>
        {
            element.RotateTo(10, duration / 8, Easing.CubicInOut),
            element.ScaleTo(1.1, duration / 8, Easing.CubicInOut)
        };
        
        await Task.WhenAll(tasks);
        
        tasks = new List<Task>
        {
            element.RotateTo(-10, duration / 4, Easing.CubicInOut),
            element.ScaleTo(1.2, duration / 8, Easing.CubicInOut)
        };
        
        await Task.WhenAll(tasks);
        
        tasks = new List<Task>
        {
            element.RotateTo(0, duration / 4, Easing.CubicInOut),
            element.ScaleTo(1.0, duration / 2, Easing.BounceOut)
        };
        
        await Task.WhenAll(tasks);
    }
}
```

### 2. Trigger-based Animations
```xml
<!-- Behaviors/AnimationTriggers.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Bounce on Tap Trigger -->
    <DataTrigger x:Key="BounceOnTap" TargetType="Button">
        <DataTrigger.Binding>
            <Binding Path="IsPressed" />
        </DataTrigger.Binding>
        <DataTrigger.Value>
            <x:Boolean>true</x:Boolean>
        </DataTrigger.Value>
        <DataTrigger.EnterActions>
            <TriggerAction x:TypeArguments="Button">
                <Setter Property="Scale" Value="0.95" />
            </TriggerAction>
        </DataTrigger.EnterActions>
        <DataTrigger.ExitActions>
            <TriggerAction x:TypeArguments="Button">
                <Setter Property="Scale" Value="1.0" />
            </TriggerAction>
        </DataTrigger.ExitActions>
    </DataTrigger>

    <!-- Pulse on Success Trigger -->
    <DataTrigger x:Key="PulseOnSuccess" TargetType="VisualElement">
        <DataTrigger.Binding>
            <Binding Path="IsSuccess" />
        </DataTrigger.Binding>
        <DataTrigger.Value>
            <x:Boolean>true</x:Boolean>
        </DataTrigger.Value>
        <DataTrigger.EnterActions>
            <TriggerAction x:TypeArguments="VisualElement">
                <!-- Custom animation trigger action would go here -->
            </TriggerAction>
        </DataTrigger.EnterActions>
    </DataTrigger>

</ResourceDictionary>
```

---

## Patrones de Composición

### 1. Layout Templates
```xml
<!-- Templates/LayoutTemplates.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Standard Page Template -->
    <DataTemplate x:Key="StandardPageTemplate">
        <Grid RowDefinitions="Auto,*,Auto">
            
            <!-- Header -->
            <ContentPresenter Grid.Row="0" 
                              Content="{Binding Header}" />
            
            <!-- Main Content -->
            <ScrollView Grid.Row="1" 
                        Padding="{StaticResource TokenSpacing4}">
                <ContentPresenter Content="{Binding MainContent}" />
            </ScrollView>
            
            <!-- Footer/Actions -->
            <ContentPresenter Grid.Row="2" 
                              Content="{Binding Footer}" />
                              
        </Grid>
    </DataTemplate>

    <!-- Activity Page Template -->
    <DataTemplate x:Key="ActivityPageTemplate">
        <Grid RowDefinitions="Auto,*,Auto">
            
            <!-- Activity Header -->
            <ContentPresenter Grid.Row="0" 
                              Content="{Binding ActivityHeader}" />
            
            <!-- Activity Content -->
            <Grid Grid.Row="1" 
                  Padding="{StaticResource TokenSpacing6}">
                <ContentPresenter Content="{Binding ActivityContent}" />
            </Grid>
            
            <!-- Activity Actions -->
            <ContentPresenter Grid.Row="2" 
                              Content="{Binding ActivityActions}" />
                              
        </Grid>
    </DataTemplate>

    <!-- Modal Template -->
    <DataTemplate x:Key="ModalTemplate">
        <Grid BackgroundColor="rgba(0,0,0,0.6)">
            <Frame BackgroundColor="White"
                   CornerRadius="16"
                   Padding="{StaticResource TokenSpacing6}"
                   Margin="{StaticResource TokenSpacing5}"
                   VerticalOptions="Center"
                   HorizontalOptions="Center"
                   MaximumWidthRequest="400">
                <ContentPresenter Content="{Binding ModalContent}" />
            </Frame>
        </Grid>
    </DataTemplate>

</ResourceDictionary>
```

### 2. Content Templates
```xml
<!-- Templates/ContentTemplates.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Subject Card Data Template -->
    <DataTemplate x:Key="SubjectCardTemplate">
        <components:SubjectCard 
            SubjectName="{Binding Name}"
            IconSource="{Binding IconSource}"
            SubjectColor="{Binding Color}"
            Progress="{Binding Progress}"
            IsLocked="{Binding IsLocked}"
            SelectCommand="{Binding SelectCommand}" />
    </DataTemplate>

    <!-- Progress Item Template -->
    <DataTemplate x:Key="ProgressItemTemplate">
        <Grid ColumnDefinitions="Auto,*,Auto" 
              Padding="{StaticResource TokenSpacing3}"
              ColumnSpacing="{StaticResource TokenSpacing3}">
            
            <Image Grid.Column="0"
                   Source="{Binding Icon}"
                   Style="{StaticResource IconMD}" />
            
            <Label Grid.Column="1"
                   Text="{Binding Title}"
                   Style="{StaticResource TextBody}"
                   VerticalOptions="Center" />
            
            <Label Grid.Column="2"
                   Text="{Binding Value}"
                   Style="{StaticResource TextCaption}"
                   VerticalOptions="Center" />
                   
        </Grid>
    </DataTemplate>

    <!-- Achievement Item Template -->
    <DataTemplate x:Key="AchievementItemTemplate">
        <Frame Style="{StaticResource CardBase}"
               Margin="{StaticResource TokenSpacing2}">
            <Grid ColumnDefinitions="Auto,*" 
                  ColumnSpacing="{StaticResource TokenSpacing3}">
                
                <Image Grid.Column="0"
                       Source="{Binding BadgeIcon}"
                       Style="{StaticResource IconLG}" />
                
                <StackLayout Grid.Column="1">
                    <Label Text="{Binding Title}"
                           Style="{StaticResource TextBody}" />
                    <Label Text="{Binding Description}"
                           Style="{StaticResource TextCaption}" />
                    <Label Text="{Binding DateEarned, StringFormat='Obtenido: {0:d}'}"
                           Style="{StaticResource TextCaption}" />
                </StackLayout>
                
            </Grid>
        </Frame>
    </DataTemplate>

</ResourceDictionary>
```

---

## Testing y Validación

### 1. Visual Regression Testing
```csharp
// Tests/VisualRegressionTests.cs
using Microsoft.Maui.Controls;
using Xunit;

namespace EduPlayKids.Tests;

public class VisualRegressionTests
{
    [Fact]
    public async Task SubjectCard_RendersCorrectly_WhenUnlocked()
    {
        // Arrange
        var subjectCard = new Components.SubjectCard
        {
            SubjectName = "Matemáticas",
            Progress = 0.8,
            IsLocked = false,
            SubjectColor = Colors.Blue
        };

        // Act
        var screenshot = await CaptureScreenshotAsync(subjectCard);

        // Assert
        await CompareWithBaseline(screenshot, "SubjectCard_Unlocked");
    }

    [Fact]
    public async Task SubjectCard_RendersCorrectly_WhenLocked()
    {
        // Arrange
        var subjectCard = new Components.SubjectCard
        {
            SubjectName = "Música",
            Progress = 0.0,
            IsLocked = true
        };

        // Act
        var screenshot = await CaptureScreenshotAsync(subjectCard);

        // Assert
        await CompareWithBaseline(screenshot, "SubjectCard_Locked");
    }

    [Theory]
    [InlineData(0.0, "star_empty.png", "star_empty.png", "star_empty.png")]
    [InlineData(0.33, "star_filled.png", "star_empty.png", "star_empty.png")]
    [InlineData(0.66, "star_filled.png", "star_filled.png", "star_empty.png")]
    [InlineData(1.0, "star_filled.png", "star_filled.png", "star_filled.png")]
    public void SubjectCard_DisplaysCorrectStars_ForProgress(double progress, 
        string expectedStar1, string expectedStar2, string expectedStar3)
    {
        // Arrange
        var subjectCard = new Components.SubjectCard
        {
            Progress = progress
        };

        // Assert
        Assert.Equal(expectedStar1, subjectCard.Star1Source);
        Assert.Equal(expectedStar2, subjectCard.Star2Source);
        Assert.Equal(expectedStar3, subjectCard.Star3Source);
    }

    private async Task<byte[]> CaptureScreenshotAsync(VisualElement element)
    {
        // Implementation would use platform-specific screenshot capabilities
        // This is a placeholder
        await Task.Delay(100);
        return new byte[0];
    }

    private async Task CompareWithBaseline(byte[] screenshot, string baselineName)
    {
        // Implementation would compare with stored baseline images
        // and fail test if differences exceed threshold
        await Task.Delay(100);
    }
}
```

### 2. Accessibility Testing
```csharp
// Tests/AccessibilityTests.cs
using Microsoft.Maui.Controls;
using Xunit;

namespace EduPlayKids.Tests;

public class AccessibilityTests
{
    [Fact]
    public void SubjectCard_HasAccessibilityProperties_WhenUnlocked()
    {
        // Arrange
        var subjectCard = new Components.SubjectCard
        {
            SubjectName = "Matemáticas",
            Progress = 0.8,
            IsLocked = false
        };

        // Assert
        Assert.Equal("Matemáticas", subjectCard.AccessibilityName);
        Assert.Contains("80 por ciento", subjectCard.AccessibilityDescription);
        Assert.Contains("Toca para acceder", subjectCard.AccessibilityDescription);
    }

    [Fact]
    public void SubjectCard_HasAccessibilityProperties_WhenLocked()
    {
        // Arrange
        var subjectCard = new Components.SubjectCard
        {
            SubjectName = "Música",
            IsLocked = true
        };

        // Assert
        Assert.Equal("Música bloqueado", subjectCard.AccessibilityName);
        Assert.Contains("está bloqueada", subjectCard.AccessibilityDescription);
        Assert.Contains("Completa las materias anteriores", subjectCard.AccessibilityDescription);
    }

    [Fact]
    public void ActivityHeader_HasAccessibilityProperties()
    {
        // Arrange
        var header = new Components.ActivityHeader
        {
            SubjectTitle = "Matemáticas",
            Lives = 2,
            TimeRemaining = TimeSpan.FromMinutes(3)
        };

        // Act & Assert
        var backButton = FindChildByAutomationId(header, "BackButton");
        Assert.Equal("Volver", AutomationProperties.GetName(backButton));
        Assert.Equal("Toca para volver al menú anterior", AutomationProperties.GetHelpText(backButton));

        var timer = FindChildByAutomationId(header, "Timer");
        Assert.Equal("Tiempo restante", AutomationProperties.GetName(timer));
        Assert.Contains("3 minutos", AutomationProperties.GetHelpText(timer));
    }

    private VisualElement FindChildByAutomationId(VisualElement parent, string automationId)
    {
        // Implementation would recursively search the visual tree
        // This is a placeholder
        return new Label();
    }
}
```

### 3. Performance Testing
```csharp
// Tests/PerformanceTests.cs
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Xunit;

namespace EduPlayKids.Tests;

public class PerformanceTests
{
    [Fact]
    public async Task SubjectGrid_RendersQuickly_With20Cards()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var grid = new Grid();
        
        // Act
        for (int i = 0; i < 20; i++)
        {
            var card = new Components.SubjectCard
            {
                SubjectName = $"Subject {i}",
                Progress = i * 0.05
            };
            grid.Children.Add(card);
        }

        // Force layout
        grid.Measure(400, 600);
        grid.Arrange(new Rect(0, 0, 400, 600));
        
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Grid rendering took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
    }

    [Fact]
    public async Task AnimationService_BounceAnimation_CompletesWithinExpectedTime()
    {
        // Arrange
        var animationService = new Services.AnimationService();
        var element = new Button();
        var stopwatch = Stopwatch.StartNew();

        // Act
        await animationService.BounceAsync(element, 300);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds >= 250 && stopwatch.ElapsedMilliseconds <= 350,
            $"Bounce animation took {stopwatch.ElapsedMilliseconds}ms, expected 250-350ms");
    }
}
```

---

## Documentación de Componentes

### 1. Component Documentation Template
```markdown
# ComponentName

## Description
Brief description of what the component does and when to use it.

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| PropertyName | Type | Default | Description of the property |

## Events

| Event | Description |
|-------|-------------|
| EventName | Description of when the event is triggered |

## Usage Examples

### Basic Usage
```xml
<components:ComponentName 
    Property1="Value1"
    Property2="Value2" />
```

### Advanced Usage
```xml
<components:ComponentName 
    Property1="{Binding ViewModelProperty}"
    Event1="HandleEvent">
    <components:ComponentName.Content>
        <!-- Child content -->
    </components:ComponentName.Content>
</components:ComponentName>
```

## Accessibility
- List of accessibility features
- ARIA labels and descriptions
- Keyboard navigation support
- Screen reader compatibility

## Design Guidelines
- When to use this component
- Visual specifications
- Interaction patterns
- Responsive behavior

## Related Components
- List of related components
- When to use alternatives
```

### 2. Auto-generated Documentation
```csharp
// Tools/DocumentationGenerator.cs
using System.Reflection;
using System.Text;

namespace EduPlayKids.Tools;

public class DocumentationGenerator
{
    public static string GenerateComponentDocs(Type componentType)
    {
        var sb = new StringBuilder();
        
        // Component name
        sb.AppendLine($"# {componentType.Name}");
        sb.AppendLine();
        
        // Description from XML docs
        var description = GetXmlDocDescription(componentType);
        sb.AppendLine($"## Description");
        sb.AppendLine(description);
        sb.AppendLine();
        
        // Properties
        sb.AppendLine("## Properties");
        sb.AppendLine();
        sb.AppendLine("| Property | Type | Default | Description |");
        sb.AppendLine("|----------|------|---------|-------------|");
        
        var bindableProperties = GetBindableProperties(componentType);
        foreach (var prop in bindableProperties)
        {
            sb.AppendLine($"| {prop.PropertyName} | {prop.ReturnType?.Name} | {prop.DefaultValue} | {prop.Description} |");
        }
        
        return sb.ToString();
    }
    
    private static string GetXmlDocDescription(Type type)
    {
        // Would parse XML documentation comments
        return "Component description from XML docs";
    }
    
    private static IEnumerable<BindablePropertyInfo> GetBindableProperties(Type type)
    {
        // Would use reflection to find BindableProperty fields
        return new List<BindablePropertyInfo>();
    }
}

public class BindablePropertyInfo
{
    public string PropertyName { get; set; }
    public Type ReturnType { get; set; }
    public object DefaultValue { get; set; }
    public string Description { get; set; }
}
```

---

## Deployment y Distribución

### 1. Design System Package
```xml
<!-- EduPlayKids.DesignSystem.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <PackageId>EduPlayKids.DesignSystem</PackageId>
    <PackageVersion>1.0.0</PackageVersion>
    <Authors>EduPlayKids Team</Authors>
    <Description>Design System components for EduPlayKids educational app</Description>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Maui.Controls" Version="7.0.92" />
  </ItemGroup>

  <ItemGroup>
    <MauiXaml Update="**/*.xaml" />
  </ItemGroup>

  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="" />
    <None Include="CHANGELOG.md" Pack="true" PackagePath="" />
  </ItemGroup>

</Project>
```

### 2. CI/CD Pipeline
```yaml
# .github/workflows/design-system.yml
name: Design System CI/CD

on:
  push:
    branches: [ main, develop ]
    paths: [ 'src/EduPlayKids.DesignSystem/**' ]
  pull_request:
    branches: [ main ]
    paths: [ 'src/EduPlayKids.DesignSystem/**' ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 7.0.x
        
    - name: Restore dependencies
      run: dotnet restore src/EduPlayKids.DesignSystem
      
    - name: Build
      run: dotnet build src/EduPlayKids.DesignSystem --no-restore
      
    - name: Test
      run: dotnet test src/EduPlayKids.DesignSystem.Tests --no-build --verbosity normal
      
    - name: Visual Regression Tests
      run: dotnet test src/EduPlayKids.DesignSystem.VisualTests --no-build
      
  publish:
    if: github.ref == 'refs/heads/main'
    needs: test
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 7.0.x
        
    - name: Build
      run: dotnet build src/EduPlayKids.DesignSystem --configuration Release
      
    - name: Pack
      run: dotnet pack src/EduPlayKids.DesignSystem --configuration Release --no-build
      
    - name: Publish to NuGet
      run: dotnet nuget push src/EduPlayKids.DesignSystem/bin/Release/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

---

## Roadmap y Versioning

### 1. Semantic Versioning
```
Version Format: MAJOR.MINOR.PATCH

MAJOR: Breaking changes to component APIs
MINOR: New components or non-breaking feature additions
PATCH: Bug fixes and minor improvements

Example:
1.0.0 - Initial stable release
1.1.0 - Added new ProgressChart component
1.1.1 - Fixed SubjectCard accessibility issue
2.0.0 - Breaking change: Renamed SubjectCard to TopicCard
```

### 2. Feature Roadmap
```markdown
## Design System Roadmap

### v1.0.0 (Current) - Foundation
- [x] Core design tokens
- [x] Basic component library
- [x] Typography system
- [x] Color system
- [x] Accessibility compliance

### v1.1.0 - Enhanced Interactions
- [ ] Advanced animation components
- [ ] Gesture-based interactions
- [ ] Audio feedback system
- [ ] Haptic feedback integration

### v1.2.0 - Advanced Components
- [ ] Chart and graph components
- [ ] Advanced form controls
- [ ] Data visualization widgets
- [ ] Interactive learning modules

### v2.0.0 - Multi-platform
- [ ] iOS-specific adaptations
- [ ] Windows platform support
- [ ] Web components (Blazor)
- [ ] Cross-platform theming
```

---

*Sistema de diseño técnico completo para EduPlayKids v1.0 - Septiembre 2025*
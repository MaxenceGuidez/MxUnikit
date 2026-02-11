# MxUnikit - Singleton

Provides professional singleton implementations for both normal C# classes and MonoBehaviour classes with explicit lifecycle control.

> WARN: This package is experimental and subject to frequent changes. It is not intended for public use.

## Features

- **MxSingleton<T>**: Thread-safe lazy singleton for normal C# classes using Lazy<T> pattern
- **MxMonoSingleton<T>**: Flexible MonoBehaviour singleton with:
  - Persistent mode (DontDestroyOnLoad) or Scene-scoped mode
  - Auto-creation or Manual-only mode
  - Automatic duplicate prevention
  - Lifecycle hooks: OnInitialize, OnDestroySingleton
  - Clear debug logging

## Usage

### Normal Class Singleton
```csharp
public class GameSettings : MxSingleton<GameSettings>
{
    public float MasterVolume { get; set; } = 1.0f;
}

// Access anywhere
var settings = GameSettings.Instance;
settings.MasterVolume = 0.8f;
```

### MonoBehaviour Singleton
```csharp
public class GameManager : MxMonoSingleton<GameManager>
{
    protected override MxSingletonMode Mode => MxSingletonMode.Persistent;
    protected override MxSingletonCreation Creation => MxSingletonCreation.AutoCreate;

    protected override void OnInitialize()
    {
        // Initialization logic
    }
}

// Access anywhere
GameManager.Instance.DoSomething();
```

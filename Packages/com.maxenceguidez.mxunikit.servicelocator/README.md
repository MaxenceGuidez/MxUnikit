# MxUnikit - Service Locator

Provides a professional service locator pattern for dependency management with explicit registration and resolution.

> WARN: This package is experimental and subject to frequent changes. It is not intended for public use.

## Features

- **Simple API**: Register and resolve services with type-safe methods
- **IService Interface**: All services must implement IService for type safety
- **MonoBehaviour Services**: Special handling for Unity components with DontDestroyOnLoad
- **Thread-Safe**: Lock-based synchronization for concurrent access
- **Debug Window**: Editor window to inspect registered services (Editor only)
- **Clear Lifecycle**: Explicit registration, resolution, and cleanup

## Usage

### Define a Service
```csharp
// Services must implement IService
public interface IAudioService : IService
{
    void PlaySound(string soundName);
}

public class AudioService : IAudioService
{
    public void PlaySound(string soundName)
    {
        Debug.Log($"Playing: {soundName}");
    }
}
```

### Basic Service Registration
```csharp
// Register an instance
var audioService = new AudioService();
MxServiceLocator.Register<IAudioService>(audioService);

// Resolve the service
var service = MxServiceLocator.Get<IAudioService>();
service.PlaySound("click");
```

### Check if Service Exists
```csharp
if (MxServiceLocator.TryGet<ISaveService>(out var saveService))
{
    saveService.SaveGame();
}
```

### Unregister Services
```csharp
// Unregister a specific service
MxServiceLocator.Unregister<IAudioService>();

// Clear all services
MxServiceLocator.Clear();
```

### MonoBehaviour Services
```csharp
// MonoBehaviour services must also implement IService
public class InputManager : MonoBehaviour, IService
{
    public void ProcessInput() { }
}

// MonoBehaviour services are automatically set to DontDestroyOnLoad
var inputManager = new GameObject("InputManager").AddComponent<InputManager>();
MxServiceLocator.Register(inputManager);
```

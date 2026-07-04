# MxUnikit - Provider

Lightweight provider (service locator) to register and resolve **services and managers** by type, with explicit registration and shutdown-aware resolution.

> WARN: This package is experimental and subject to frequent changes. It is not intended for public use.

## Features

- **Simple API**: `Register`, `Unregister`, `Get`, `TryGet`, `IsRegistered`, `GetAll`, `Clear`
- **No marker interface**: register any reference type — plain services or `MonoBehaviour` managers
- **Shutdown-aware**: `Get<T>` logs an error when a type is missing, but stays silent while the application is quitting (`MxProvider.IsQuitting`)
- **Play-mode reset**: state is cleared on each play session
- **Debug Window**: editor window to inspect registered instances (Editor only)

## Usage

### Register and resolve

```csharp
AudioService audioService = new AudioService();
MxProvider.Register<IAudioService>(audioService);

IAudioService service = MxProvider.Get<IAudioService>();
service.PlaySound("click");
```

### Safe check

```csharp
if (MxProvider.TryGet<ISaveService>(out ISaveService saveService))
{
    saveService.SaveGame();
}
```

### MonoBehaviour managers

```csharp
public class InputManager : MonoBehaviour
{
    public void ProcessInput() { }
}

InputManager inputManager = new GameObject("InputManager").AddComponent<InputManager>();
MxProvider.Register(inputManager);
```

### Cleanup

```csharp
MxProvider.Unregister<IAudioService>();
MxProvider.Clear();
```

## Notes

- `Register<T>` registers under `typeof(T)`, so register interfaces explicitly (`Register<IAudioService>(impl)`) when you want to resolve by interface.
- Not thread-safe: intended to be used from the main thread.

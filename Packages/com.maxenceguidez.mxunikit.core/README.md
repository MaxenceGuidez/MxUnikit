# MxUnikit - Core

Minimal game lifecycle foundation to bootstrap a project quickly, without copy-pasting the same boilerplate across games. Built on top of `MxUnikit.Provider` (no singletons).

It provides:

- **`MxCoreManager`** — an abstract `MonoBehaviour` that owns the asynchronous startup of your game. Inherit it, implement `Initialize()`, and it registers itself into `MxProvider` (under `MxCoreManager`) and exposes an `OnInitialized` event.
- **`MxBootstrapper`** — a `MonoBehaviour` for your bootstrap scene. It resolves the `MxCoreManager` from `MxProvider`, initializes it, then loads the next scene.

## Dependencies

- `com.maxenceguidez.mxunikit.provider`
- `com.maxenceguidez.mxunikit.log`

## Usage

Create your game manager by inheriting `MxCoreManager`:

```csharp
using System.Threading.Tasks;
using MxUnikit.Core;
using MxUnikit.Provider;

public class GameManager : MxCoreManager
{
    protected override Task Initialize()
    {
        // Register your services / managers, sign in, load databases...
        // MxProvider.Register<IAudioService>(new AudioService());
        return Task.CompletedTask;
    }
}
```

Set up a bootstrap scene:

1. Add a GameObject with your `GameManager` component.
2. Add a GameObject with `MxBootstrapper` and set its `Next Scene Name`.

On play, `MxBootstrapper` runs `InitializeAsync()` on the manager and loads the next scene once initialization completes (or fails). Resolve the manager anywhere with `MxProvider.Get<MxCoreManager>()`.

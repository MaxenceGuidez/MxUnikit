# MxUnikit - Core

Minimal game lifecycle foundation to bootstrap a project quickly, without copy-pasting the same boilerplate across games. Built on top of `MxUnikit.Provider` (no singletons).

> WARN: This package is experimental and subject to frequent changes. It is not intended for public use.

It provides a two-phase, event-driven startup, both living in the bootstrap scene:

- **`MxBootstrapper`** — an abstract `MonoBehaviour`. It registers itself into `MxProvider`, runs
  your `Preload()` (connect to backend services, sign in, fetch remote config...), fires
  `OnPreloaded`, then loads the next scene. It never needs to survive the scene switch — it's
  destroyed with the bootstrap scene once the next scene is loaded.
- **`MxCoreManager`** — an abstract `MonoBehaviour` that owns the actual game startup (UI, gameplay
  systems...). It registers itself into `MxProvider` and uses `DontDestroyOnLoad` in `Awake`, then
  subscribes to `MxBootstrapper.OnPreloaded` in `Start`. When preload completes it runs its own
  `InitializeAsync()` — without waiting for it, the bootstrapper has already moved on. Inherit it,
  implement `Initialize()`, and it exposes an `OnInitialized` event.

## Dependencies

- `com.maxenceguidez.mxunikit.provider`
- `com.maxenceguidez.mxunikit.log`

## Usage

Create your bootstrapper by inheriting `MxBootstrapper`:

```csharp
using System.Threading.Tasks;
using MxUnikit.Core;

public class GameBootstrapper : MxBootstrapper
{
    protected override async Task Preload()
    {
        // Connect to backend services: Unity Services, Firebase, remote config...
        await Task.CompletedTask;
    }
}
```

Create your game manager by inheriting `MxCoreManager`:

```csharp
using System.Threading.Tasks;
using MxUnikit.Core;
using MxUnikit.Provider;

public class GameManager : MxCoreManager
{
    protected override Task Initialize()
    {
        // Register your gameplay services, show the main menu...
        // MxProvider.Register<IAudioService>(new AudioService());
        return Task.CompletedTask;
    }
}
```

Set up the bootstrap scene:

1. Add a GameObject with your `GameBootstrapper` component and set its `Next Scene Name`.
2. Add a GameObject with your `GameManager` component.

On play, `MxCoreManager` subscribes to the bootstrapper's `OnPreloaded`. Once `Preload()`
completes, `MxBootstrapper` fires the event and loads the main scene right away — it doesn't wait
for `GameManager.InitializeAsync()`, which keeps running in the background thanks to
`DontDestroyOnLoad`. Resolve the manager anywhere with `MxProvider.Get<MxCoreManager>()`.

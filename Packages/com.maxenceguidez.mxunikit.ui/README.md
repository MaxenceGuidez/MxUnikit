# MxUnikit - UI

UI Toolkit foundations for a **layered UI architecture**, without copy-pasting the same
manager/menu/dialog logic across games. The reusable *logic* lives here; the game-specific
views, widgets and service wiring stay in your project.

## What it provides

- **`MxUiManager`** — abstract `MonoBehaviour` (requires a `PanelRenderer`) that owns one UI layer:
  resolves the panel `Root` on UI reload, calls `OnUiReady()`, and exposes `Show()`/`Hide()`/`IsVisible`
  for the whole layer, plus `FocusedElement`, `SetRootEnabled`, and `IsUiReady`/`WaitUntilUiReadyAsync()`
  to await the first UI load.
- **`MxView`** — abstract `VisualElement` (tagged `mx-view`) with `Show()`/`Hide()` (toggling the
  `mx-hidden` USS class) and `OnShow()`/`OnHide()` hooks.
- **`MxMenuUiManager<TKey>`** — a menu layer with **navigation history**: `ShowMenu(key, isRoot)`,
  `Back()`, `HideAllMenus()`, `HasHistory`, `HasMenuOpen`. Register menus by key in `OnUiReady()`.
- **`MxOverlayUiManager`** — an overlay layer with a **dialog stack**: `ShowDialog`, `CloseDialog`,
  `CloseAllDialogs`, `IsAnyDialogOpen`, `TopDialog`.
- **`MxDialog`** — an *abstract* `MxView` (tagged `mx-dialog`) defining the dialog contract:
  `Init(title, message)`, `Close()`, and a `CloseRequested` event. You build the visuals (labels,
  buttons, layout) in your concrete subclass.
- **`MxStyles`** — shared USS class-name constants (`mx-hidden`, `mx-view`, `mx-dialog`), plus a base
  stylesheet `Runtime/Styles/MxStyles.uss` (`.mx-hidden { display: none }`, `.mx-view`, and a
  `--space-*` spacing-token scale). Include it in your `PanelSettings` theme so hiding works.

### Decoupled from your game

The managers never reference game services (cursor, input, pause...). Instead they expose **virtual
hooks** you override:

- `MxMenuUiManager`: `OnMenuShown(key, menu)`, `OnAllMenusHidden()`
- `MxOverlayUiManager`: `OnFirstDialogOpened()`, `OnLastDialogClosed()`

## Recommended layering

Three independent layers, each its own `PanelRenderer` + manager:

- **HUD** — the in-game player HUD → extend `MxUiManager` (use `Show()`/`Hide()` for the layer).
- **Menus** — navigable menus → extend `MxMenuUiManager<YourMenuType>`.
- **Overlays** — dialogs, toasts, loading, splash → extend `MxOverlayUiManager`.

## Usage

### Menus

```csharp
using MxUnikit.UI;
using MxUnikit.Provider;

public enum MenuType { Home, Settings, Pause }

public class MenuUiManager : MxMenuUiManager<MenuType>
{
    protected override void Awake()
    {
        base.Awake();
        MxProvider.Register(this); // resolve later via MxProvider.Get<MenuUiManager>()
    }

    protected override void OnUiReady()
    {
        RegisterMenu(MenuType.Home, Root.Q<HomeMenu>());
        RegisterMenu(MenuType.Settings, Root.Q<SettingsMenu>());
        RegisterMenu(MenuType.Pause, Root.Q<PauseMenu>());

        ShowMenu(MenuType.Home, isRoot: true);
    }

    // Wire game concerns here instead of coupling the base.
    protected override void OnMenuShown(MenuType key, MxView menu)
    {
        MxProvider.Get<CursorManager>()?.Show();
    }
}
```

### Waiting for the UI

The `PanelRenderer` loads the UI *after* the first frame's `Start()` calls, so code driving a
manager from outside (e.g. `MxCoreManager.Initialize()`) must await readiness before using it:

```csharp
protected override async Task Initialize()
{
    MenuUiManager menus = MxProvider.Get<MenuUiManager>();

    await menus.WaitUntilUiReadyAsync();
    menus.ShowMenu(MenuType.Home, isRoot: true);
}
```

### Dialogs

Define a concrete dialog (visuals are yours):

```csharp
public class ConfirmDialog : MxDialog
{
    public ConfirmDialog(string title, string message, Action onConfirm, Action onCancel = null)
    {
        Init(title, message);
        // ...build your Label/Button hierarchy, focus in OnShow()...
    }

    public override void Init(string title, string message) { /* set labels */ }

    protected override void OnShow() { /* focus first button */ }
}
```

Then push it onto the overlay stack:

```csharp
public class OverlayUiManager : MxOverlayUiManager
{
    protected override void OnFirstDialogOpened() => MxProvider.Get<InputManager>()?.SetEnabled(false);
    protected override void OnLastDialogClosed()  => MxProvider.Get<InputManager>()?.SetEnabled(true);
}

// somewhere:
overlay.ShowDialog(new ConfirmDialog("Quit?", "Are you sure?", onConfirm: QuitGame));
```

The dialog calls `Close()` (raising `CloseRequested`); the manager pops it off the stack.

## Dependencies

- `com.maxenceguidez.mxunikit.log`

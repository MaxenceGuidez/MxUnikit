# MxUnikit

Monorepo of reusable Unity packages ("MxUnikit") extracted from real games to avoid
duplicating battle-tested code (logging, tests, service wiring, game lifecycle) across projects.

The repository root is itself a **Unity project** (Unity `6000.5.0f1`) that **embeds** every
package under `Packages/` so they can be developed and tested in place. `Assets/` only holds
scratch/test content and imported samples (mostly git-ignored).

> Experimental, not intended for public use. Global code-style and language rules from the
> user's personal `~/.claude/CLAUDE.md` apply on top of this file.

## Packages

Seven packages are **PROD** (maintained). The rest are **WIP** — do not touch unless asked.

| Package (`com.maxenceguidez.mxunikit.*`) | Version | Deps          | Key types                                                               | Editor             | Sample                                                |
|------------------------------------------|---------|---------------|-------------------------------------------------------------------------|--------------------|--------------------------------------------------------|
| `core`                                   | 0.2.0   | log, provider | `MxCoreManager`, `MxBootstrapper`                                       | —                  | Sample - Core Bootstrap                                |
| `debug`                                  | 0.1.0   | —             | `MxDebug`                                                               | —                  | —                                                      |
| `extensions`                             | 0.1.0   | —             | `MxExtensions`                                                          | —                  | —                                                      |
| `log`                                    | 0.4.0   | —             | `MxLog`, `MxLogConfig`, `MxLogCategory`                                 | ✅ config editor    | Sample - MxLog                                         |
| `provider`                               | 0.3.0   | log           | `MxProvider`                                                            | ✅ inspector window | Sample - MxProvider, Sample - Multi Key Registration   |
| `tests`                                  | 0.2.0   | —             | `RequiredFieldAttribute`, validators, `ProjectReferenceValidationTests` | ✅ validation       | Sample - Basic Validation                              |
| `ui`                                     | 0.2.0   | log           | `MxUiManager`, `MxView`                                                 | —                  | Sample - Menu And Dialog                               |

WIP (ignore): `i18n`, `singleton`, `timer`. Their samples (where present) already follow the
nomenclature below since it was applied repo-wide, but the packages themselves are still off-limits
unless asked.

## Naming conventions (keep everything consistent)

| Thing             | Pattern                                                   | Example                                  |
|-------------------|-----------------------------------------------------------|------------------------------------------|
| Package id        | `com.maxenceguidez.mxunikit.<lower>`                      | `com.maxenceguidez.mxunikit.provider`    |
| Display name      | `MxUnikit - <Title>`                                      | `MxUnikit - Provider`                    |
| Namespace         | `MxUnikit.<Pascal>`                                       | `MxUnikit.Provider`                      |
| Assembly (asmdef) | `MaxenceGuidez.MxUnikit.<Pascal>` (`.Editor`, `.Samples`) | `MaxenceGuidez.MxUnikit.Provider.Editor` |
| Public types      | `Mx` prefix                                               | `MxLog`, `MxProvider`, `MxCoreManager`   |

Author is always `Maxence Guidez <contact@maxenceguidez.com>`. Versions stay in `0.x.x` (no 1.0.0 yet).

### Sample nomenclature

Applies to every sample, in every package (PROD or WIP), regardless of how many samples a
package has — there is no shorthand for the single-sample case.

| Thing                                 | Pattern                                                                                    | Example                                                                              |
|---------------------------------------|--------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------|
| Folder (`Samples~/<x>`)               | `Sample<Name>`                                                                             | `Samples~/SampleMxProvider`                                                          |
| Assembly (asmdef)                     | `MaxenceGuidez.MxUnikit.<Pascal>.Samples.Sample<Name>` — always, even with a single sample | `MaxenceGuidez.MxUnikit.Provider.Samples.SampleMxProvider`                           |
| Namespace                             | `MxUnikit.<Pascal>.Samples.Sample<Name>`                                                   | `MxUnikit.Provider.Samples.SampleMxProvider`                                         |
| `package.json` `displayName`          | `Sample - <Human Name>`                                                                    | `Sample - Multi Key Registration`                                                    |
| `package.json` `description` (sample) | Starts with `Demonstrates`, 1 to 3 sentences                                               | `Demonstrates registration, resolution and MonoBehaviour managers using MxProvider.` |

- No sample script, class, method, scene, GameObject, asset, or CSS class may contain the word
  "Demo" — everything sample-specific is `Sample`-prefixed instead (e.g. `SampleProviderManager`,
  never `DemoManager` or `MxProviderDemoManager`).
- Scene-serialized fields that reference sample identity by string (`_nextSceneName`,
  `m_EditorClassIdentifier`, GameObject `m_Name`) must be updated by hand when renaming — Unity
  doesn't rewrite them until the scene is reopened and re-saved.

### Package description convention

`package.json` top-level `description` (not the sample one above): starts with `Provides a`, no
`:`, 2 to 6 sentences, and stays generic — describe what the package is for (e.g. "adds a timer
system", "adds validation tooling"), not what each individual script does.

## Package anatomy

```
com.maxenceguidez.mxunikit.<x>/
├── package.json (+ .meta)     # name, version, displayName, description, unity/unityRelease, dependencies, author, samples
├── README.md (+ .meta)
├── Runtime/                    # asmdef MaxenceGuidez.MxUnikit.<X> + sources
├── Editor/                     # optional; asmdef ...<X>.Editor, includePlatforms: [Editor]
└── Samples~/                   # optional; hidden from Unity; samples referenced from package.json "samples"
```

- Every file **and folder** has a `.meta`. Indentation: `.cs` = 4 spaces, `.json`/`.asmdef` = 2 spaces (`.editorconfig`).
- asmdef references are by **name** (e.g. `"MaxenceGuidez.MxUnikit.Log"`), not GUID.

### ⚠️ `Samples~` gotcha (recurring bug)

A folder ending in `~` is hidden from Unity and must **never** have a root `Samples~.meta` —
Unity warns ("folder can't be found, has been created") and keeps regenerating it. The
`.gitignore` has a `Samples~.meta` rule so it can never be committed again. The `.meta` files
*inside* `Samples~` are kept (they stabilize GUIDs when the sample is imported).

### ⚠️ `.gitignore` `*.log` gotcha

The Unity-template `*.log` rule (meant for `Editor.log`/`Player.log`) also matches the
`com.maxenceguidez.mxunikit.log` package **directory** by name, since gitignore patterns without
a slash match at any path depth. Without the explicit `!/Packages/com.maxenceguidez.mxunikit.log/`
+ `!/Packages/com.maxenceguidez.mxunikit.log/**` negation lines right after it, every new file
(mainly `.meta`) added under that package silently fails to track — don't remove those negations.

## Core architecture (provider + core)

Deliberate design: **no singletons**. Managers/services are wired through a service locator.

- **`MxProvider`** (provider) — static, type-keyed registry for *services and managers* alike.
  `Register<T>` / `Get<T>` / `TryGet<T>` / `Unregister<T>` / `IsRegistered<T>` / `GetAll` / `Clear`.
  No marker interface (`where T : class`). Resolution is strict: one key = `typeof(T)` at the
  registration call site; an instance needed under several types (e.g. base + concrete manager)
  is registered under each key explicitly. `Get<T>` logs an error when missing, but stays silent
  during shutdown (`_isQuitting`, hooked to `Application.quitting`). State resets on
  `SubsystemRegistration`. Logs via `MxLog`.
- **`MxBootstrapper`** (core) — abstract `MonoBehaviour`, sits in the bootstrap scene alongside
  `MxCoreManager`. Registers itself into `MxProvider` in `Awake` (`Awake`/`OnDestroy` are
  `protected virtual`). In `Start` runs the abstract
  `Preload()` (backend services, sign in...), fires `OnPreloaded`, then loads `_nextSceneName`. No
  `DontDestroyOnLoad` — it's disposable, destroyed with the bootstrap scene once the next scene
  loads; it never waits for `MxCoreManager` to finish. Inherit it, implement `Preload()`.
- **`MxCoreManager`** (core) — abstract `MonoBehaviour`. In `Awake` (`protected virtual`, as is
  `OnDestroy`): `DontDestroyOnLoad` + registers itself into `MxProvider`; subclasses override
  both to also register/unregister under their concrete type. In `Start`, resolves `MxBootstrapper` via `MxProvider` and
  subscribes to its `OnPreloaded`; when it fires, runs its own `InitializeAsync()` (wraps the
  abstract `Initialize()`, fires `OnInitialized`) without the bootstrapper waiting on it. Inherit
  it, implement `Initialize()` for the actual game startup (UI, gameplay systems...) — it survives
  into the main scene via `DontDestroyOnLoad`.

A strongly-typed facade over `MxProvider` (e.g. `ServiceProvider.MusicManager`) belongs in the
**consumer project**, not in these packages (it references game-specific types).

## Commit conventions

`[SCOPE] Imperative capitalized message` — scope is the package name in UPPERCASE (with a space,
e.g. `[SERVICE LOCATOR]`), or `[GLOBAL]` for cross-cutting changes. New packages use
`[SCOPE] Init <x> package`.

```
[PROVIDER] Refactor service locator into MxProvider
[CORE] Init core package
[GLOBAL] Fix samples warnings
[LOG] Upgrade category detection
```

End commit messages with the required `Co-Authored-By` trailer. Branch before committing on `main`.

## Working in the project

- Open in Unity `6000.5.0f1`; packages are embedded, so they compile with the project.
- Tests: Unity Test Framework (the `tests` package ships Editor test assemblies). No CLI build.
- Samples are imported via the Package Manager (copied into `Assets/Samples/…`, git-ignored).

## House rules / gotchas

- Only the seven PROD packages above are in scope by default.
- When bumping a package version, realign dependents' dependency constraints
  (e.g. `core` → `log`/`provider`, `provider` → `log`).
- After editing package files, re-focus Unity to recompile; re-import a sample to test its copy.
- `CLAUDE.md` is currently git-ignored (personal, not shared).

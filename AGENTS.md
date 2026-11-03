# AGENTS.md

## Overview

Windows-only WinForms baby game targeting .NET Framework 4.8. `LangVersion` is pinned to **7.3** in every csproj — C# 8+ features (nullable ref types, switch expressions, ranges) will not compile.

Three projects:
- `Egorka/` — the app. MVP: `FullScreenForm` (view; global hooks via MouseKeyHook) + `MainGameController` (all testable logic goes here) + `EgorkaSettings` (singleton `ConfigurationSection` read from `App.config`, key `egorkaSettings`; falls back to scanning configs next to the assembly when the section is absent — this is what makes it work under in-proc test runners).
- `EgorkaTestProject/` — **MSTest v2** 4.5.2 (NuGet `MSTest.TestFramework` + `MSTest.TestAdapter`; runs under `vstest.console` via the adapter deployed next to the test dll), Moq 4.21, Shouldly 4.3.
- `BuildDictionary/` — standalone WinForms key-logging tool used to generate the key dictionaries; not part of the game.

Packages use legacy `packages.config` + `packages/` folder (gitignored). Restore before building — the old `.nuget/NuGet.exe` bootstrap has been removed, so use a modern nuget.exe. Note: `nuget restore` skips transitive dependencies for packages.config, so all dependency packages (Castle.Core, DiffEngine, etc.) are listed explicitly in the packages.config files; if you bump a version, check its nuspec dependencies and add them too:

## Build & test (verified)

```powershell
Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $env:TEMP\nuget.exe
& $env:TEMP\nuget.exe restore EgorkaGame.sln
```

Build with VS MSBuild (`dotnet build` works on a single csproj, but MSBuild is canonical):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" EgorkaGame.sln /p:Configuration=Debug /v:m
```

Run tests with `vstest.console.exe` (the MSTest v2 adapter is deployed next to the test dll by the imported `MSTest.TestAdapter` props/targets; `dotnet test` is not wired up for this solution):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" EgorkaTestProject\bin\Debug\EgorkaTestProject.dll
```

## Tests

All 10 tests pass. `EgorkaTestProject/app.config` carries its own `egorkaSettings` section (`IsSpeechEnabled="false"` so tests exercise the beep path). MSTest v2 runs tests **in-proc** inside vstest.console, so `ConfigurationManager` sees the runner's config, not the test project's — that is why `EgorkaSettings.Load` has the scan-fallback. Don't remove it.

## Localization

Culture comes from the `culture` attribute of `egorkaSettings` in `Egorka/App.config` (currently `ru-RU`), applied to the thread in `Program.Main`. Key data is **embedded at compile time**, not read from disk: `Properties/Resources.resx` FileRefs `Resources/keys.txt` (EN), `Resources.ru.resx` → `keys.ru.txt` (RU). Line format is NUL-delimited (`KeysEnumValue\0DisplayChar\0KnownColorName`, e.g. `Q\0Й\0DarkCyan`) — NULs render as spaces in most editors, don't convert them. Adding a language = new `Resources.xx.resx` + `Resources/keys.xx.txt`.

## App behavior gotchas

- Running `Egorka.exe` opens a borderless fullscreen window that hooks **all** global keyboard/mouse input (Win key, Alt-Tab suppressed) and blocks the mouse. Exit is Alt+F4 only — don't launch it casually inside an agent session.
- TTS (System.Speech) vs. beep is set by `IsSpeechEnabled` in App.config; beep frequencies are piano keys from #25 (`MainGameController.CalcKeyFrequency`).

## Conventions

- Source files use `#region` blocks (Consts / Constructors / Properties / Public methods / Private methods / Fields) — follow that layout.
- No CI, lint, or formatter config exists in the repo; verification is build + vstest.console.

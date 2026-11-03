# AGENTS.md

## Overview

Windows-only WinForms baby game targeting .NET Framework 4.8 (README says 4.5 — stale; csproj is truth). No LangVersion is set, so classic-project defaults apply: **C# 7.3 max** — C# 8+ features (nullable ref types, switch expressions, ranges) will not compile.

Three projects:
- `Egorka/` — the app. MVP: `FullScreenForm` (view; global hooks via MouseKeyHook) + `MainGameController` (all testable logic goes here) + `EgorkaSettings` (singleton `ConfigurationSection` read from `App.config`, key `egorkaSettings`).
- `EgorkaTestProject/` — **MSTest v1** (`[TestClass]` via `Microsoft.VisualStudio.QualityTools.UnitTestFramework`, not MSTest v2/xUnit), Moq 4.2, Shouldly 2.5.
- `BuildDictionary/` — standalone WinForms key-logging tool used to generate the key dictionaries; not part of the game.

## Build & test (verified)

The bundled `.nuget/NuGet.exe` is v2.8 (2014) and **cannot reach nuget.org** (TLS failure). `packages/` is gitignored, so restore with a modern nuget.exe before building:

```powershell
Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $env:TEMP\nuget.exe
& $env:TEMP\nuget.exe restore EgorkaGame.sln
```

Build with VS MSBuild (`dotnet build` works on a single csproj, but MSBuild is canonical):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" EgorkaGame.sln /p:Configuration=Debug /v:m
```

Run tests with `vstest.console.exe` (`dotnet test` does not run MSTest v1):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" EgorkaTestProject\bin\Debug\EgorkaTestProject.dll
```

## Known failing tests

`ProcessKeyTest` and `CalculateFrequencyTest` fail with NullReferenceException (8/10 pass). Cause: `EgorkaTestProject/app.config` defines no `egorkaSettings` section, so `EgorkaSettings.Instance` (`ConfigurationManager.GetSection`) returns null and `MainGameController.ProcessKey` NREs. Fix: copy the `<configSections>` + `<egorkaSettings>` block from `Egorka/App.config` into the test app.config.

## Localization

Culture comes from the `culture` attribute of `egorkaSettings` in `Egorka/App.config` (currently `ru-RU`), applied to the thread in `Program.Main`. Key data is **embedded at compile time**, not read from disk: `Properties/Resources.resx` FileRefs `Resources/keys.txt` (EN), `Resources.ru.resx` → `keys.ru.txt` (RU). Line format is NUL-delimited (`KeysEnumValue\0DisplayChar\0KnownColorName`, e.g. `Q\0Й\0DarkCyan`) — NULs render as spaces in most editors, don't convert them. Adding a language = new `Resources.xx.resx` + `Resources/keys.xx.txt`.

## App behavior gotchas

- Running `Egorka.exe` opens a borderless fullscreen window that hooks **all** global keyboard/mouse input (Win key, Alt-Tab suppressed) and blocks the mouse. Exit is Alt+F4 only — don't launch it casually inside an agent session.
- TTS (System.Speech) vs. beep is set by `IsSpeechEnabled` in App.config; beep frequencies are piano keys from #25 (`MainGameController.CalcKeyFrequency`).

## Conventions

- Source files use `#region` blocks (Consts / Constructors / Properties / Public methods / Private methods / Fields) — follow that layout.
- No CI, lint, or formatter config exists in the repo; verification is build + vstest.console.

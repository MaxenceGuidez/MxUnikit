# MxUnikit - Log

Provides tools to easily implement a log system.

> WARN: This package is experimental and subject to frequent changes. It is not intended for public use.

## Installation

1. Open the Package Manager
2. Click **+** (top-left) > **Install package from git URL...**
3. Fill with [MxUnikit - Log git URL](https://github.com/MaxenceGuidez/MxUnikit.git?path=Packages/com.maxenceguidez.mxunikit.log/)
4. Click on **Install**

## Configuration

1. Create a `MxLogConfig` asset with: **Assets > Create > MxUnikit > Log > Log Config**
2. Place it in a `Resources` folder (e.g., `Assets/Resources/MxLogConfig.asset`)
3. Configure categories, colors, and keywords in the Inspector

## Quick Start

```csharp
using MxUnikit.Log;

// Simple logging
MxLog.L("This is a log");
MxLog.W("This is a warning");
MxLog.E("This is an error");

// Exception logging
try
{
    throw new Exception("Oops");
}
catch (Exception ex)
{
    MxLog.Ex(ex);
}
```

## Categories

Logs are automatically categorized based on class names, method names, and keywords.

Built-in categories:
- API
- Audio
- Debug
- Event
- Firebase
- Game
- Inputs
- Inventory
- Network
- Player
- Session
- UI

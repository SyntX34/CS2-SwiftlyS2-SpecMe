# SpecMe

<div align="center">

![License](https://img.shields.io/badge/License-MIT-blue.svg)
![SwiftlyS2](https://img.shields.io/badge/Framework-SwiftlyS2-orange.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)
![CS2](https://img.shields.io/badge/Game-Counter--Strike%202-yellow.svg)

**A flexible, high-performance spectator management plugin for Counter-Strike 2 powered by SwiftlyS2.**

[Features](#features) • [Installation](#installation) • [Commands](#commands) • [Configuration](#configuration) • [Localization](#localization) • [License](#license)

</div>

---

## Overview

**SpecMe** allows players to step away (AFK) or request spectator status cleanly without disrupting active rounds. 

- **Immediate spectator transfer:** If the player is already dead or has administrative permissions (`!spec`), they are moved to spectator immediately.
- **Fair play protection:** If a player is alive and executes `!specme` or `!afk`, they are safely queued to transition to Spectator either upon death or at round end (configurable). Toggling the command again cancels the request.
- **Timer safety & performance:** Built using cancellation token sources (`CancellationTokenSource`) and event cleanup so no orphaned timers or memory leaks degrade tick rates.
- **Auto-reload support:** Automatically reloads configuration and plugin state on map changes.

---

## Features

- **Dynamic Command Permissions:** Configure `!spec` to be admin-only with customizable permission flags (`@css/generic`, `@css/root`, etc.) or make it available to all players.
- **Safe Transitions:** Automatically moves dead players directly to spectator, queues living players until their death or round end.
- **Toggle Cancellation:** If a living player changes their mind, running `!specme` or `!afk` again cancels the pending move.
- **Map Change Automation:** Config and active state reload automatically across map cycles.
- **Bracket Chat Colors:** Fully supports SwiftlyS2 bracket color formatting tokens (`[green]`, `[red]`, `[default]`, etc.).
- **Multi-Language Translations:** Uses SwiftlyS2's official `ITranslationService` / `ILocalizer` APIs.

---

## Commands

| Command | Allowed Callers | Description |
| :--- | :--- | :--- |
| `!afk` | All Players | Queues the player to move to Spectator when dead or on round end. If already dead, moves immediately. |
| `!specme` | All Players | Alternative command to `!afk` for moving to spectator. |
| `!spec` | Admins / Configurable | Moves the player to Spectator immediately (or queues if restricted to admin flags). |
| `!specme_reload` | Admins | Reloads `config.jsonc` and clears active queues. |

> **Note:** Both chat triggers (e.g. `!spec`, `/spec`) and console commands (e.g. `sw_spec`) are supported by SwiftlyS2.

---

## Configuration

The configuration file is automatically created at `addons/swiftlys2/configs/plugins/SpecMe/config.jsonc`:

```jsonc
{
  // Whether the plugin functionality is enabled.
  "Enabled": true,

  // Chat message prefix with bracket color tokens.
  "ChatPrefix": "[green][SpecMe][default] ",

  // Whether !spec is restricted to admins with permission flags.
  "SpecAdminOnly": true,

  // Permission flag required for !spec when SpecAdminOnly is true.
  "SpecAdminFlag": "@css/generic",

  // Whether !afk command is enabled for players.
  "EnableAfkCommand": true,

  // Whether !specme command is enabled for players.
  "EnableSpecMeCommand": true,

  // When a player executes !specme or !afk while alive, move on round end instead of on death?
  "MoveOnRoundEndInsteadOfDeath": false,

  // Delay in seconds before moving player to spectator once death or round end trigger fires (0 = immediate).
  "MoveDelaySeconds": 0.5,

  // Whether to automatically reload the plugin and its configuration on map changes.
  "AutoReloadOnMapChange": true
}
```

---

## Localization

Translations are located in `resources/translations/en.jsonc` and can be customized with SwiftlyS2 chat color bracket tokens:

```jsonc
{
  "specme.prefix": "[green][SpecMe][default] ",
  "specme.disabled": "[red]Plugin features are currently disabled.[default]",
  "specme.command_disabled": "[red]This command is currently disabled.[default]",
  "specme.player_only": "[red]This command can only be executed by players.[default]",
  "specme.already_spec": "[yellow]You are already in the spectator team.[default]",
  "specme.moved_spec": "[green]You have been moved to Spectator.[default]",
  "specme.queued_death": "[lightred]You will be moved to Spectator upon your death.[default]",
  "specme.queued_round_end": "[lightred]You will be moved to Spectator at the end of the round.[default]",
  "specme.cancelled_queue": "[yellow]Your pending spectator request has been cancelled.[default]",
  "specme.no_permission": "[red]You do not have permission to use !spec. Use [lightred]!specme[red] or [lightred]!afk[red] instead.[default]",
  "specme.reloaded": "[lime]Configuration and state reloaded successfully.[default]"
}
```

---

## Installation

1. Install [SwiftlyS2](https://swiftlys2.net/) on your Counter-Strike 2 dedicated server.
2. Download the latest release `SpecMe.zip` from GitHub Releases.
3. Extract `SpecMe.zip` into `game/csgo/addons/swiftlys2/plugins/`.
4. Start your server or run `sw_reload SpecMe` in your server console.

---

## Contributing

Contributions and PRs are welcome! Check out [CONTRIBUTING.md](CONTRIBUTING.md) for details on development workflow, code standards, and guidelines.

---

## License

This project is licensed under the [MIT License](LICENSE).

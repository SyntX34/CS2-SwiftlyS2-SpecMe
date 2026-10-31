using System.ComponentModel;

namespace SpecMe;

public sealed class PluginConfig
{
  [Description("Whether the plugin functionality is enabled.")]
  public bool Enabled { get; set; } = true;

  [Description("Chat message prefix with bracket color tokens (e.g. [green][SpecMe][default] ).")]
  public string ChatPrefix { get; set; } = "[green][SpecMe][default] ";

  [Description("Whether !spec is restricted to admins with permission flags.")]
  public bool SpecAdminOnly { get; set; } = true;

  [Description("Permission flag required for !spec when SpecAdminOnly is true (empty string allows all admins/players).")]
  public string SpecAdminFlag { get; set; } = "@css/generic";

  [Description("Whether !afk command is enabled for players.")]
  public bool EnableAfkCommand { get; set; } = true;

  [Description("Whether !specme command is enabled for players.")]
  public bool EnableSpecMeCommand { get; set; } = true;

  [Description("When a player executes !specme or !afk while alive, should they be moved to spectator on round end instead of on death? (true = round end, false = on death)")]
  public bool MoveOnRoundEndInsteadOfDeath { get; set; } = false;

  [Description("Delay in seconds before moving player to spectator once death or round end trigger fires (0 = immediate).")]
  public float MoveDelaySeconds { get; set; } = 0.5f;

  [Description("Whether to automatically reload the plugin and its configuration on map changes.")]
  public bool AutoReloadOnMapChange { get; set; } = true;
}

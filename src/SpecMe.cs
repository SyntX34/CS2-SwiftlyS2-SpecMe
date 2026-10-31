using Microsoft.Extensions.Configuration;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;

namespace SpecMe;

[PluginMetadata(
  Id = "SpecMe", 
  Version = "1.0.0", 
  Name = "SpecMe", 
  Author = "SyntX34", 
  Description = "Let players go to Spectator when AFK, with configurable admin controls and death/round-end transitions."
)]
public partial class SpecMe : BasePlugin
{
  public PluginConfig Config { get; private set; } = new();
  public PluginLogger Logger { get; private set; }

  public SpecMe(ISwiftlyCore core) : base(core)
  {
    Logger = new PluginLogger(core);
  }

  public override void ConfigureSharedInterface(IInterfaceManager interfaceManager) {}
  public override void UseSharedInterface(IInterfaceManager interfaceManager) {}

  public override void Load(bool hotReload)
  {
    LoadConfig();
    RegisterCommands();
    RegisterEvents();
    Logger.LogSystem($"Plugin loaded (hotReload: {hotReload})");
  }

  public override void Unload()
  {
    Logger.LogSystem("Plugin unloaded");
    UnregisterEvents();
    UnregisterCommands();
    ClearAllPendingQueues();
  }

  /* Initializes or reloads the plugin configuration via official jsonc API */
  public void LoadConfig()
  {
    Core.Configuration.InitializeJsonWithModel<PluginConfig>("config.jsonc", "SpecMe", addMissingKeys: true);
    var loaded = Core.Configuration.Manager.GetSection("SpecMe").Get<PluginConfig>();
    Config = loaded ?? new PluginConfig();
  }
}

using SwiftlyS2.Shared.Commands;

namespace SpecMe;

public partial class SpecMe
{
  /* Registers chat and console commands for !afk, !spec, !specme, and !specme_reload */
  private void RegisterCommands()
  {
    Core.Command.RegisterCommand("afk", HandleAfkCommand);
    Core.Command.RegisterCommand("spec", HandleSpecCommand);
    Core.Command.RegisterCommand("specme", HandleSpecMeCommand);
    Core.Command.RegisterCommand("specme_reload", HandleReloadCommand);
  }

  /* Unregisters plugin commands */
  private void UnregisterCommands()
  {
    Core.Command.UnregisterCommand("afk");
    Core.Command.UnregisterCommand("spec");
    Core.Command.UnregisterCommand("specme");
    Core.Command.UnregisterCommand("specme_reload");
  }

  /* Handles !afk command */
  private void HandleAfkCommand(ICommandContext context)
  {
    if (!Config.Enabled)
    {
      SendReply(context, "specme.disabled");
      return;
    }

    if (!Config.EnableAfkCommand)
    {
      SendReply(context, "specme.command_disabled");
      return;
    }

    ProcessPlayerSpectatorRequest(context, allowImmediateWhenAlive: false);
  }

  /* Handles !spec command (allows immediate move if player is admin or if admin check is disabled) */
  private void HandleSpecCommand(ICommandContext context)
  {
    if (!Config.Enabled)
    {
      SendReply(context, "specme.disabled");
      return;
    }

    var player = context.Sender;
    if (player == null || !player.IsValid)
    {
      SendReply(context, "specme.player_only");
      return;
    }

    /* Verify admin privileges if SpecAdminOnly is configured */
    if (Config.SpecAdminOnly && !string.IsNullOrWhiteSpace(Config.SpecAdminFlag))
    {
      if (!Core.Permission.PlayerHasPermission(player.SteamID, Config.SpecAdminFlag))
      {
        SendReply(context, "specme.no_permission");
        return;
      }
    }

    /* Admin or open !spec allows immediate move to spectator */
    ProcessPlayerSpectatorRequest(context, allowImmediateWhenAlive: true);
  }

  /* Handles !specme command */
  private void HandleSpecMeCommand(ICommandContext context)
  {
    if (!Config.Enabled)
    {
      SendReply(context, "specme.disabled");
      return;
    }

    if (!Config.EnableSpecMeCommand)
    {
      SendReply(context, "specme.command_disabled");
      return;
    }

    ProcessPlayerSpectatorRequest(context, allowImmediateWhenAlive: false);
  }

  /* Handles admin reload command */
  private void HandleReloadCommand(ICommandContext context)
  {
    var player = context.Sender;
    if (player != null && player.IsValid && !string.IsNullOrWhiteSpace(Config.SpecAdminFlag))
    {
      if (!Core.Permission.PlayerHasPermission(player.SteamID, Config.SpecAdminFlag))
      {
        SendReply(context, "specme.no_permission");
        return;
      }
    }

    LoadConfig();
    ClearAllPendingQueues();
    SendReply(context, "specme.reloaded");
  }
}


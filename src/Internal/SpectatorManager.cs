using System.Collections.Concurrent;
using System.Threading;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Players;

namespace SpecMe;

public partial class SpecMe
{
  /* Active player spectator queues keyed by player slot */
  private readonly ConcurrentDictionary<int, bool> _pendingSpectators = new();

  /* Active scheduled timers keyed by player slot for proper cancellation and resource tracking */
  private readonly ConcurrentDictionary<int, CancellationTokenSource> _activeTimers = new();

  /* Core spectator evaluation pipeline */
  private void ProcessPlayerSpectatorRequest(ICommandContext context, bool allowImmediateWhenAlive)
  {
    var player = context.Sender;
    if (player == null || !player.IsValid)
    {
      SendReply(context, "specme.player_only");
      return;
    }

    /* Check if already in Spectator team */
    var currentTeam = player.Controller.Team;
    if (currentTeam == Team.Spectator)
    {
      SendReply(context, "specme.already_spec");
      return;
    }

    /* If the player is already queued, toggle off/cancel the pending move */
    if (_pendingSpectators.TryRemove(player.Slot, out _))
    {
      CancelPlayerTimer(player.Slot);
      Logger.LogPlayerAction(player, "Cancelled pending spectator request", $"Command: {context.CommandName}");
      SendReply(context, "specme.cancelled_queue");
      return;
    }

    /* If the player is dead or immediate move is granted, execute switch immediately */
    if (!player.IsAlive || allowImmediateWhenAlive)
    {
      ExecuteMoveToSpectator(player);
      Logger.LogPlayerAction(player, "Moved to Spectator immediately", $"Alive: {player.IsAlive}, AdminOverride: {allowImmediateWhenAlive}");
      SendReply(context, "specme.moved_spec");
      return;
    }

    /* Player is alive: queue them based on configuration */
    _pendingSpectators[player.Slot] = true;

    if (Config.MoveOnRoundEndInsteadOfDeath)
    {
      Logger.LogPlayerAction(player, "Queued to Spectator on round end", $"Command: {context.CommandName}");
      SendReply(context, "specme.queued_round_end");
    }
    else
    {
      Logger.LogPlayerAction(player, "Queued to Spectator on death", $"Command: {context.CommandName}");
      SendReply(context, "specme.queued_death");
    }
  }

  /* Dispatches player transition to spectator with scheduled cancellation control */
  private void QueuePlayerSpectatorTransition(int slot)
  {
    CancelPlayerTimer(slot);

    if (Config.MoveDelaySeconds <= 0f)
    {
      var target = Core.PlayerManager.GetPlayer(slot);
      if (target != null && target.IsValid)
      {
        ExecuteMoveToSpectator(target);
        Logger.LogPlayerAction(target, "Moved to Spectator via queued trigger");
        SendPlayerMessage(target, "specme.moved_spec");
      }
      return;
    }

    var cts = Core.Scheduler.DelayBySeconds(Config.MoveDelaySeconds, () =>
    {
      _activeTimers.TryRemove(slot, out _);
      var target = Core.PlayerManager.GetPlayer(slot);
      if (target != null && target.IsValid && target.Controller.Team != Team.Spectator)
      {
        ExecuteMoveToSpectator(target);
        Logger.LogPlayerAction(target, "Moved to Spectator via delayed queue timer");
        SendPlayerMessage(target, "specme.moved_spec");
      }
    });

    _activeTimers[slot] = cts;
  }

  /* Switches player team cleanly to Spectator */
  private void ExecuteMoveToSpectator(IPlayer player)
  {
    try
    {
      player.ChangeTeam(Team.Spectator);
    }
    catch
    {
      /* Fallback to switch team if pawn or controller state requires non-lethal change */
      try
      {
        player.SwitchTeam(Team.Spectator);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[SpecMe] Failed to switch player {player.Name} to spectator: {ex.Message}");
      }
    }
  }

  /* Cancels and disposes active player transition timer */
  private void CancelPlayerTimer(int slot)
  {
    if (_activeTimers.TryRemove(slot, out var cts))
    {
      try
      {
        cts.Cancel();
        cts.Dispose();
      }
      catch
      {
        /* Ignore disposal exceptions on cancelled timers */
      }
    }
  }

  /* Clears all pending spectator queues and active timers */
  private void ClearAllPendingQueues()
  {
    _pendingSpectators.Clear();

    foreach (var kvp in _activeTimers)
    {
      try
      {
        kvp.Value.Cancel();
        kvp.Value.Dispose();
      }
      catch
      {
        /* Ignore disposal errors */
      }
    }
    _activeTimers.Clear();
  }

  /* Sends localized message to command sender */
  private void SendReply(ICommandContext context, string translationKey)
  {
    var player = context.Sender;
    if (player != null && player.IsValid)
    {
      SendPlayerMessage(player, translationKey);
    }
    else
    {
      var prefix = !string.IsNullOrWhiteSpace(Config.ChatPrefix) ? Config.ChatPrefix : Core.Localizer["specme.prefix"];
      var message = Core.Localizer[translationKey];
      context.Reply($"{prefix}{message}");
    }
  }

  /* Sends localized chat message to player */
  private void SendPlayerMessage(IPlayer player, string translationKey)
  {
    var localizer = Core.Translation.GetPlayerLocalizer(player);
    var prefix = !string.IsNullOrWhiteSpace(Config.ChatPrefix) ? Config.ChatPrefix : localizer["specme.prefix"];
    var message = localizer[translationKey];
    player.SendChat($"{prefix}{message}");
  }
}

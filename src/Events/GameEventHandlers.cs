using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.GameEvents;
using SwiftlyS2.Shared.Misc;

namespace SpecMe;

public partial class SpecMe
{
  /* Hooks game and server lifecycle events */
  private void RegisterEvents()
  {
    Core.Event.OnMapLoad += HandleMapLoad;
    Core.Event.OnClientDisconnected += HandleClientDisconnected;

    Core.GameEvent.HookPre<EventPlayerDeath>(HandlePlayerDeath);
    Core.GameEvent.HookPre<EventRoundEnd>(HandleRoundEnd);
  }

  /* Unhooks game and server lifecycle events */
  private void UnregisterEvents()
  {
    Core.Event.OnMapLoad -= HandleMapLoad;
    Core.Event.OnClientDisconnected -= HandleClientDisconnected;

    Core.GameEvent.UnhookPre<EventPlayerDeath>();
    Core.GameEvent.UnhookPre<EventRoundEnd>();
  }

  /* Handles player death event */
  private HookResult HandlePlayerDeath(EventPlayerDeath @event)
  {
    if (!Config.Enabled || Config.MoveOnRoundEndInsteadOfDeath)
    {
      return HookResult.Continue;
    }

    var player = @event.UserIdPlayer;
    if (player == null || !player.IsValid)
    {
      return HookResult.Continue;
    }

    var slot = player.Slot;
    if (slot >= 0 && _pendingSpectators.TryRemove(slot, out _))
    {
      QueuePlayerSpectatorTransition(slot);
    }

    return HookResult.Continue;
  }

  /* Handles round end event */
  private HookResult HandleRoundEnd(EventRoundEnd @event)
  {
    if (!Config.Enabled)
    {
      return HookResult.Continue;
    }

    foreach (var kvp in _pendingSpectators)
    {
      var slot = kvp.Key;
      if (_pendingSpectators.TryRemove(slot, out _))
      {
        QueuePlayerSpectatorTransition(slot);
      }
    }

    return HookResult.Continue;
  }

  /* Handles map load: reloads config and reloads plugin if configured */
  private void HandleMapLoad(IOnMapLoadEvent @event)
  {
    ClearAllPendingQueues();

    if (Config.AutoReloadOnMapChange)
    {
      LoadConfig();
    }
  }

  /* Cleans up state when a client disconnects */
  private void HandleClientDisconnected(IOnClientDisconnectedEvent @event)
  {
    var slot = @event.PlayerId;
    _pendingSpectators.TryRemove(slot, out _);
    CancelPlayerTimer(slot);
  }
}

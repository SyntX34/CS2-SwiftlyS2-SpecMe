using System.IO;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;

namespace SpecMe;

public sealed class PluginLogger
{
  private readonly ISwiftlyCore _core;
  private readonly object _lock = new();

  public PluginLogger(ISwiftlyCore core)
  {
    _core = core;
  }

  /* Formats and writes player spectator transition actions to daily log files */
  public void LogPlayerAction(IPlayer player, string action, string details = "")
  {
    try
    {
      var now = DateTime.Now;
      var dateStamp = now.ToString("yyyy-MM-dd");
      var timeStamp = now.ToString("yyyy-MM-dd HH:mm:ss");

      var logsDir = Path.Combine(_core.PluginDataDirectory, "logs");
      if (!Directory.Exists(logsDir))
      {
        Directory.CreateDirectory(logsDir);
      }

      var filePath = Path.Combine(logsDir, $"specme_{dateStamp}.log");

      var playerName = player.IsValid ? player.Name : "Unknown";
      var playerIp = player.IsValid ? player.IPAddress : "Unknown";
      var steamId = player.IsValid ? player.SteamID.ToString() : "Unknown";

      var mapName = GetCurrentMapName();

      var line = $"[{timeStamp}] [Map: {mapName}] [Player: {playerName}] [SteamID: {steamId}] [IP: {playerIp}] - {action}";
      if (!string.IsNullOrWhiteSpace(details))
      {
        line += $" ({details})";
      }

      lock (_lock)
      {
        File.AppendAllText(filePath, line + Environment.NewLine);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"[SpecMe] Failed to write log: {ex.Message}");
    }
  }

  /* Logs general system and administrative events */
  public void LogSystem(string message)
  {
    try
    {
      var now = DateTime.Now;
      var dateStamp = now.ToString("yyyy-MM-dd");
      var timeStamp = now.ToString("yyyy-MM-dd HH:mm:ss");

      var logsDir = Path.Combine(_core.PluginDataDirectory, "logs");
      if (!Directory.Exists(logsDir))
      {
        Directory.CreateDirectory(logsDir);
      }

      var filePath = Path.Combine(logsDir, $"specme_{dateStamp}.log");
      var mapName = GetCurrentMapName();
      var line = $"[{timeStamp}] [Map: {mapName}] [System] - {message}";

      lock (_lock)
      {
        File.AppendAllText(filePath, line + Environment.NewLine);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"[SpecMe] Failed to write system log: {ex.Message}");
    }
  }

  /* Resolves current map name, preserving workshop map id when applicable */
  public string GetCurrentMapName()
  {
    try
    {
      var rawMap = _core.Engine.GlobalVars.MapName.ToString();
      var workshopId = _core.Engine.WorkshopId;

      if (!string.IsNullOrWhiteSpace(workshopId) && workshopId != "0")
      {
        if (!string.IsNullOrWhiteSpace(rawMap) && !rawMap.Contains(workshopId, StringComparison.OrdinalIgnoreCase))
        {
          return $"workshop/{workshopId}/{rawMap}";
        }
        return $"workshop/{workshopId}";
      }

      return !string.IsNullOrWhiteSpace(rawMap) ? rawMap : "unknown_map";
    }
    catch
    {
      return "unknown_map";
    }
  }
}

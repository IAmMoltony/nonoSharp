using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NonoSharp;

public class Replay
{
    public Dictionary<uint, ReplayMove> Moves { get; private set; }

    public static string GetReplayDirectory()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp", "Replays");
    }

    public static string GetReplayFullPath(string replayName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp", "Replays", $"{replayName}.rpy");
    }

    public Replay()
    {
        Moves = new Dictionary<uint, ReplayMove>();
    }

    public void AddMove(ReplayMove move, uint frame)
    {
        Moves[frame] = move;
    }

    public void Save(string name)
    {
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string replayDirectory = GetReplayDirectory();
        Directory.CreateDirectory(replayDirectory);
        string realName = GetReplayFullPath($"{name}_{dateTime}");

        // I understand that writing JSON manually isn't a very good idea
        // But it's the one that I could implement with the least amount of headaches.

        string json = "{\"Moves\":{";
        foreach (var (frame, move) in Moves)
        {
            json += $"\"{frame}\":{{\"Type\":\"{move.type.ToString()}\",\"X\":\"{move.tileX}\", \"Y\":\"{move.tileY}\"}},";
        }
        json = json.TrimEnd(',');
        json += "}}";

        File.WriteAllText(realName, json);
    }

    public bool Load(string name)
    {
        string replayDirectory = GetReplayDirectory();
        string realName = GetReplayFullPath(name);

        if (!File.Exists(realName))
            return false;

        string replayJson = File.ReadAllText(realName);

        JsonDocument replayDocument = JsonDocument.Parse(replayJson);

        return true;
    }
}

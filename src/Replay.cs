using System;
using System.Collections.Generic;
using System.IO;

namespace NonoSharp;

public class Replay
{
    public Dictionary<uint, ReplayMove> Moves { get; private set; }

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
        string replayDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp", "Replays");
        Directory.CreateDirectory(replayDirectory);
        string realName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + $"/nonoSharp/Replays/{name}_{dateTime}.rpy";

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
}

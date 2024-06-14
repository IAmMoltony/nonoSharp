using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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

        string json = JsonSerializer.Serialize(this);
        File.WriteAllText(realName, json);
    }
}
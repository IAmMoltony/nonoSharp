using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NonoSharp;

// TODO generic settings class that Settings and KeySettings will inherit from

public static class KeySettings
{
    private static Dictionary<string, Keys> _keys = null!;
    private static string _keySettingsFile = null!;

    private static readonly Dictionary<string, Keys> _defaultKeys = new()
    {
        {"placeTile", Keys.X}
    };

    public static void Initialize()
    {
        Log.Logger.Information("Initializing key settings");

        string keySettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp");
        Log.Logger.Information($"Key settings folder: {keySettingsFolder}");
        Directory.CreateDirectory(keySettingsFolder);

        _keySettingsFile = Path.Combine(keySettingsFolder, "keySettings.json");
        if (!File.Exists(_keySettingsFile))
        {
            _keys = new(_defaultKeys);
            Save();
        }
        else
            Load();

        _keys ??= new(_defaultKeys);

        Log.Logger.Information("Current keys:");
        foreach (KeyValuePair<string, Keys> pair in _keys)
            Log.Logger.Information($"{pair.Key}={pair.Value}");
    }

    public static void Save()
    {
        Log.Logger.Information("Saving key settings");
        string json = JsonSerializer.Serialize(_keys);
        File.WriteAllText(_keySettingsFile, json);
    }

    public static void Load()
    {
        Log.Logger.Information("Loading settings");
        string jsonString = File.ReadAllText(_keySettingsFile);

        try
        {
            _keys = JsonSerializer.Deserialize<Dictionary<string, Keys>>(jsonString) ?? new(_defaultKeys);
        }
        catch (JsonException exception)
        {
            Log.Logger.Error(exception, "Failed to load keySettings.json");
        }
    }

    public static void Set(string key, Keys val)
    {
        _keys[key] = val;
    }

    public static Keys Get(string key)
    {
        if (_keys.ContainsKey(key))
            return _keys[key];

        return _defaultKeys[key];
    }
}
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Serilog;

namespace NonoSharp;

public static class Settings
{
    private static Dictionary<string, string> _settings;
    private static string _settingsFile;

    private static Dictionary<string, string> _defaultSettings = new()
    {
        {"language", "System"}
    };

    public static void Initialize()
    {
        Log.Logger.Information("Initializing settings");
        
        string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nonoSharp");
        Log.Logger.Information($"Settings folder: {settingsFolder}");
        Directory.CreateDirectory(settingsFolder);

        _settingsFile = Path.Combine(settingsFolder, "settings.json");
        if (!File.Exists(_settingsFile))
        {
            _settings = new(_defaultSettings);
            Save();
        }
        else
            Load();

        Log.Logger.Information("Current settings:");
        foreach (KeyValuePair<string, string> pair in _settings)
            Log.Logger.Information($"{pair.Key}={pair.Value}");
    }

    public static void Save()
    {
        Log.Logger.Information("Saving settings");
        string json = JsonSerializer.Serialize(_settings);
        File.WriteAllText(_settingsFile, json);
    }

    public static void Load()
    {
        Log.Logger.Information("Loading settings");
        string jsonString = File.ReadAllText(_settingsFile);
        _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
    }

    public static void Set(string key, string val)
    {
        _settings[key] = val;
    }

    public static string Get(string key)
    {
        if (_settings.ContainsKey(key))
            return _settings[key];
        return _defaultSettings[key];
    }
}

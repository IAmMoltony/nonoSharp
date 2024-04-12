using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NonoSharp;

public static class Settings
{
    private static Dictionary<string, string> _settings;
    private static string _settingsFile;

    private static readonly Dictionary<string, string> _defaultSettings = new()
    {
        {"language", "System"},
        {"fullScreen", "no"}
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

        _settings ??= new(_defaultSettings);

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

        try
        {
            _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
        }
        catch (JsonException exception)
        {
            Log.Logger.Error(exception, "Failed to load settings.json");
        }
    }

    public static void Set(string key, string val)
    {
        _settings[key] = val;
    }

    public static void Set(string key, bool val)
    {
        _settings[key] = val ? "yes" : "no";
    }

    public static string Get(string key)
    {
        // If the setting is found in the settings dictionary, return it
        if (_settings.ContainsKey(key))
            return _settings[key];

        // If it's not found, look it up in default settings
        return _defaultSettings[key];
    }

    public static bool GetBool(string key)
    {
        return Get(key) == "yes";
    }
}

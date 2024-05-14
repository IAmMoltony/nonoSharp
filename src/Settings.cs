using Microsoft.Xna.Framework;
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
        {"fullScreen", "no"},
        {"accentColor", "0;128;0"}
    };

    public const float AccentColorDefaultDarkerAmount = 0.3f;
    public const float AccentColorDefaultLighterAmount = 0.5f;

    public static void Initialize()
    {
        Log.Logger.Information("Initializing settings");

        string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp");
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
        Set(key, val ? "yes" : "no");
    }

    public static void Set(string key, Color val)
    {
        Set(key, $"{val.R};{val.G};{val.B}");
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

    public static Color GetColor(string key)
    {
        string colorString = Get(key);
        string[] parts = colorString.Split(';');

        // Return white if there's not 3 parts
        if (parts.Length != 3)
            return Color.White;

        // Convert parts to integer array
        int[] intParts = new int[3];
        try
        {
            intParts = Array.ConvertAll(parts, int.Parse);
        }
        catch (ArgumentNullException)
        {
            // Just incase `parts` is null (which it probably shouldn't, but better be safe than sorry)
            return Color.White;
        }

        // Make sure the parts don't go over 255 or under 0
        for (int i = 0; i < 3; i++)
            if (intParts[i] > 255 || intParts[i] < 0)
                return Color.White;

        return new(intParts[0], intParts[1], intParts[2]);
    }

    // this is for convenience so i don't have to type "Settings.GetColor("accentColor")" all the time
    public static Color GetAccentColor()
    {
        return GetColor("accentColor");
    }

    public static Color GetDarkAccentColor(float darkAmount = AccentColorDefaultDarkerAmount)
    {
        return GetAccentColor().Darker(darkAmount);
    }

    public static Color GetLightAccentColor(float lightAmount = AccentColorDefaultLighterAmount)
    {
        return GetAccentColor().Lighter(lightAmount);
    }
}

using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NonoSharp;

public static class StringManager
{
    /// <summary>
    /// Dictionary of language strings
    /// </summary>
    private static Dictionary<string, string>? _strings;

    /// <summary>
    /// The current language
    /// </summary>
    private static string? _lang;

    /// <summary>
    /// List of supported languages
    /// </summary>
    public static readonly string[] SupportedLanguages = {
        "en", "ru"
    };

    /// <summary>
    /// Initialize string manager
    /// </summary>
    public static void Initialize()
    {
        Log.Logger.Information("Initializing string manager");
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        // step 1 get the language
        getLanguage();
        Log.Logger.Information($"Language is {_lang}");

        // step 2 check if it's supported
        if (!SupportedLanguages.Contains(_lang))
        {
            Log.Logger.Information("Language isn't supported, defaulting to 'en'");
            _lang = "en";
        }

        // step 3 load the language
        parseLanguage();

        stopwatch.Stop();
        Log.Logger.Information($"Initialized in {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Get a language string
    /// </summary>
    /// <param name="key">String key</param>
    /// <returns>The string translated to the current language</returns>
    public static string GetString(string key)
    {
        if (!_strings?.ContainsKey(key) == true)
            return $"{{ {key} }}";
        return _strings[key];
    }

    /// <summary>
    /// Parse a language file
    /// </summary>
    private static void parseLanguage()
    {
        string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "lang", $"{_lang}.json");
        string langString = File.ReadAllText(fileName);
        _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(langString);
    }

    /// <summary>
    /// Get the current language
    /// </summary>
    private static void getLanguage()
    {
        string langSetting = Settings.Get("language");

        if (langSetting == "System")
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            _lang = cultureInfo.TwoLetterISOLanguageName;
        }
        else
            _lang = langSetting;
    }
}

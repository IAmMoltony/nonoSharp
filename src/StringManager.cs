﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Serilog;

namespace NonoSharp;

public static class StringManager
{
    private static Dictionary<string, string> _strings;
    private static string _lang;

    public static readonly string[] SupportedLanguages = {
        "en", "ru"
    };

    public static void Initialize()
    {
        Log.Logger.Information("Initializing string manager");
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        // step 1 get the language
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        _lang = cultureInfo.TwoLetterISOLanguageName;
        string forceLang = Environment.GetEnvironmentVariable("NONOSHARP_FORCE_LANG");
        if (!string.IsNullOrEmpty(forceLang))
            _lang = forceLang;
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

    public static string GetString(string key)
    {
        return _strings[key];
    }

    private static void parseLanguage()
    {
        string fileName = $"{AppDomain.CurrentDomain.BaseDirectory}/Content/lang/{_lang}.json";
        string langString = File.ReadAllText(fileName);
        _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(langString);
    }
}
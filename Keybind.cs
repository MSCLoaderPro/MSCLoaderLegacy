using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// GNU GPL 3.0
#pragma warning disable CS1591
namespace MSCLoader
{
    [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
	public class Keybind
    {
        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public static List<Keybind> Keybinds = new List<Keybind>();
        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public static List<Keybind> DefaultKeybinds = new List<Keybind>();

        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public string ID, Name;
        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public KeyCode Key, Modifier;
        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public Mod Mod;
        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public object[] Vals { get; set; }

        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public SettingKeybind keybind;

        [Obsolete("Old Keybind is obsolete, use SettingKeybind instead"), EditorBrowsable(EditorBrowsableState.Never)]
        public bool noModifier = false;

        [Obsolete("Old Keybind is obsolete, use modSettings.AddKeybind() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void Add(Mod mod, Keybind key)
        {
            key.Mod = mod;
            Keybinds.Add(key);
            DefaultKeybinds.Add(new Keybind(key.ID, key.Name, key.Key, key.Modifier) { Mod = mod });

            if (key.noModifier)
                key.keybind = mod.modSettings.AddKeybind(key.ID, key.Name, key.Key);
            else
                key.keybind = mod.modSettings.AddKeybind(key.ID, key.Name, key.Key, key.Modifier);
        }

        [Obsolete("Old header is obsolete, use modSettings.AddHeader() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle) => 
            mod.modSettings.AddHeader(HeaderTitle);

        [Obsolete("Old header is obsolete, use modSettings.AddHeader() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor) => 
            mod.modSettings.AddHeader(HeaderTitle, backgroundColor, textColor);

        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public static List<Keybind> Get(Mod mod) => Keybinds.FindAll(x => x.Mod == mod);

        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public static List<Keybind> GetDefault(Mod mod) => DefaultKeybinds.FindAll(x => x.Mod == mod);

        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public Keybind(string id, string name, KeyCode key)
        {
            ID = id;
            Name = name;
            Key = key;
            noModifier = true;
        }

        [Obsolete("Old Keybind is obsolete, use modSettings.AddKeybind() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public Keybind(string id, string name, KeyCode key, KeyCode modifier)
        {
            ID = id;
            Name = name;
            Key = key;
            Modifier = modifier;
            noModifier = false;
        }

        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetKeybind() => keybind.GetKey();
        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetKeybindDown() => keybind.GetKeyDown();
        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetKeybindUp() => keybind.GetKeyUp();
        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsPressed() => keybind.GetKey();
        [Obsolete("Old Keybind is obsolete"), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDown() => keybind.GetKeyDown();
    }
}
using System;
using System.ComponentModel;
using UnityEngine;

// GNU GPL 3.0
#pragma warning disable CS1591, IDE1006
namespace MSCLoader
{
    public class ModUI
    {
        [Obsolete("Please use ModLoader.UICanvas instead."), EditorBrowsable(EditorBrowsableState.Never)]
        internal static GameObject canvas { get => ModLoader.UICanvas.gameObject; set => ModLoader.UICanvas = value.transform; }

        [Obsolete("Please use ModLoader.UICanvas instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static GameObject GetCanvas() => ModLoader.UICanvas.gameObject;

        [Obsolete("Please use ModPrompt.CreatePrompt() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void ShowMessage(string message, string title = "MESSAGE") => ModPrompt.CreatePrompt(message, title);

        [Obsolete("Please use ModPrompt.CreateYesNoPrompt() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void ShowYesNoMessage(string message, Action ifYes) => ShowYesNoMessage(message, "MESSAGE", ifYes);

        [Obsolete("Please use ModPrompt.CreateYesNoPrompt() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void ShowYesNoMessage(string message, string title, Action ifYes) => ModPrompt.CreateYesNoPrompt(message, title, () => { ifYes(); });

        [Obsolete(), EditorBrowsable(EditorBrowsableState.Never)]
        public static GameObject messageBox { get => ModPrompt.prompt; set => ModPrompt.prompt = value; }
    }
}

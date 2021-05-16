using HutongGames.PlayMaker;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591
namespace MSCLoader
{
    [Obsolete("Use MSCLoader.Helper.PlayMakerHelper instead."), EditorBrowsable(EditorBrowsableState.Never)]
    public static class PlayMakerExtensions
    {
        [Obsolete("Use MSCLoader.Helper.PlayMakerHelper.GetPlayMakerFSM() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static PlayMakerFSM GetPlayMaker(this GameObject obj, string playMakerName)
        {
            PlayMakerFSM[] pm = obj.GetComponents<PlayMakerFSM>();

            if (pm != null)
            {
                PlayMakerFSM fsm = pm.FirstOrDefault(x => x.FsmName == playMakerName);
                if (fsm != null) return fsm;
                else
                {
                    ModConsole.LogError($"GetPlayMaker: Cannot find <b>{playMakerName}</b> in GameObject <b>{obj.name}</b>");
                    return null;
                }

            }
            ModConsole.LogError($"GetPlayMaker: Cannot find any Playmakers in GameObject <b>{obj.name}</b>");
            return null;
        }

        [Obsolete("Use MSCLoader.Helper.PlayMakerHelper.GetPlayMakerFSM() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static PlayMakerFSM GetPlayMaker(this Transform t, string playMakerName)
        {
            return t.gameObject.GetPlayMaker(playMakerName);
        }

        public static FsmState GetPlayMakerState(this GameObject obj, string stateName)
        {
            PlayMakerFSM[] pm = obj.GetComponents<PlayMakerFSM>();
            if (pm != null)
            {
                for (int i = 0; i < pm.Length; i++)
                {
                    FsmState state = pm[i].FsmStates.FirstOrDefault(x => x.Name == stateName);
                    if (state != null)
                        return state;
                }
                ModConsole.LogError(string.Format("GetPlayMakerState: Cannot find <b>{0}</b> state in GameObject <b>{1}</b>", stateName, obj.name));
                return null;
            }
            ModConsole.LogError(string.Format("GetPlayMakerState: Cannot find any Playmakers in GameObject <b>{0}</b>", obj.name));
            return null;
        }

        public static void FsmInject(this GameObject gameObject, string stateName, Action hook)
        {
            FsmHook.FsmInject(gameObject, stateName, hook);
        }

    }
}
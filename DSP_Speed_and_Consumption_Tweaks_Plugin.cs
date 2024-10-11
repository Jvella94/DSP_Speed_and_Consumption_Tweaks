using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DSP_Speed_and_Consumption_Tweaks.Patches;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Diagnostics;
using UnityEngine;

namespace DSP_Speed_and_Consumption_Tweaks
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    [BepInProcess("DSPGAME.exe")]
    //[BepInDependency("dsp.galactic-scale.2")]
    public class DSP_Speed_and_Consumption_Tweaks_Plugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // com.mynameororg.pluginname
        // Version should be a valid version string.
        // e.g.
        // 1.0.0
        private const string MyGUID = "com.hiul.DSP_Speed_and_Consumption_Tweaks";
        private const string PluginName = "DSP_Speed_and_Consumption_Tweaks";
        public const string VersionString = "0.0.6";

        

        public static ManualLogSource Log;
        public void Awake()
        {
            Harmony harmony = new Harmony(MyGUID);

            Log = base.Logger;
            DSP_Speed_and_Consumption_Tweaks.Config.Init(Config);
            harmony.PatchAll(typeof(DSP_Speed_and_Consumption_Tweaks_Plugin));
            harmony.PatchAll(typeof(MyGameHistoryData));
            harmony.PatchAll(typeof(MyMecha));
            harmony.PatchAll(typeof(MyPlayerController));
            harmony.PatchAll(typeof(MyStationComponent));

        }
        public void Start()
        {
            
            Log.LogInfo("Loaded!");

            ModDebug.SetLogger(Log);

#if DEBUG
            Debugger.Break();
#endif
        }
    }
}

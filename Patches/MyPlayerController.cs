using HarmonyLib;
using System;
using System.Reflection;
using static GuideMissionStandardMode;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;
using System.IO;

namespace DSP_Speed_and_Consumption_Tweaks.Patches
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required

    /// <summary>
    /// Sample Harmony Patch class. Suggestion is to use one file per patched class
    /// though you can include multiple patch classes in one file.
    /// Below is included as an example, and should be replaced by classes and methods
    /// for your mod.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMove_Sail))]
    internal class MyPlayerController
    {
        /// <summary>
        /// Patches the PlayerController Init method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(PlayerMove_Sail.Init))]
        [HarmonyPostfix]
        public static void Init_Postfix(PlayerMove_Sail __instance)
        {
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In PlayerMove_Sail.Init method Postfix |");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.max_acc       : {__instance.max_acc}.");
            
            __instance.max_acc = Config.Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRate.Value;

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"New value of __instance.max_acc   : {__instance.max_acc}.");

        }
    }
}
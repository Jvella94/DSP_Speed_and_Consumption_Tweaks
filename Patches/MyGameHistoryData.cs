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
    [HarmonyPatch(typeof(GameHistoryData))]
    internal class MyGameHistoryData
    {

        /// <summary>
        /// Patches the GameHistoryData Import method with Postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(GameHistoryData.Import))]
        [HarmonyPostfix]
        public static void ImportPostfix(GameHistoryData __instance, BinaryReader r){
            
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("In GameHistoryData Import method Postfix.");

            applyPatch(ref __instance);
        }

        /// <summary>
        /// Patches the GameHistoryData SetForNewGame method with Postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(GameHistoryData.SetForNewGame))]
        [HarmonyPostfix]
        public static void SetForNewGamePostfix(GameHistoryData __instance)
        {

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("In GameHistoryData SetForNewGame method Postfix.");

            applyPatch(ref __instance);

        }

        /// <summary>
        ///     applyPatch
        public static void applyPatch(ref GameHistoryData __instance)
        {
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("In applyPatch");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|      CRUISE engine Configuration       |");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticDroneSpeed        : {__instance.logisticDroneSpeed}");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticDroneSpeedScale   : {__instance.logisticDroneSpeedScale}");

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipSailSpeed     : {__instance.logisticShipSailSpeed}");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipSpeedScale    : {__instance.logisticShipSpeedScale}");

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipWarpSpeed     : {__instance.logisticShipWarpSpeed}");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipSpeedScale    : {__instance.logisticShipSpeedScale}");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipWarpDrive     : {__instance.logisticShipWarpDrive}");

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticCourierSpeed      : {__instance.logisticCourierSpeed}");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticCourierSpeedScale : {__instance.logisticCourierSpeedScale}");
        }
    }
}
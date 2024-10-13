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
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("In GameHistoryData Import method Postfix.");
            }

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
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("In GameHistoryData SetForNewGame method Postfix.");
            }

            applyPatch(ref __instance);

        }

        

        /// <summary>
        ///     applyPatch
        public static void applyPatch(ref GameHistoryData __instance)
        {
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("In applyPatch");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|      Drones engine Configuration       |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticDroneSpeed        : {__instance.logisticDroneSpeed}");
            }

            __instance.logisticDroneSpeed = (float)Config.Logistic_DRONE_CONFIG.DroneMaxSpeed.Value;
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticDroneSpeed        : {__instance.logisticDroneSpeed}");
            }

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|   Ships CRUISE engine Configuration    |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipSailSpeed     : {__instance.logisticShipSailSpeed}");
            }
            __instance.logisticShipSailSpeed = 
                (float)(Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value != "M"
                ? Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value != "AU"
                ? Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value != "LY"
                ? __instance.logisticShipSailSpeed
                    : Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * Config.LY
                    : Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * Config.AU
                    : Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * Config.M);

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|    Ships WARP engine Configuration     |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticShipWarpSpeed     : {__instance.logisticShipWarpSpeed}");
            }
            __instance.logisticShipWarpSpeed = (float)(Config.Logistic_SHIP_CONFIG.ShipMaxWarpSpeedUnits.Value != "M"
                ? Config.Logistic_SHIP_CONFIG.ShipMaxWarpSpeedUnits.Value != "AU"
                ? Config.Logistic_SHIP_CONFIG.ShipMaxWarpSpeedUnits.Value != "LY"
                ? __instance.logisticShipWarpSpeed
                    : Config.Logistic_SHIP_CONFIG.ShipMaxWarpSpeed.Value * Config.LY
                    : Config.Logistic_SHIP_CONFIG.ShipMaxWarpSpeed.Value * Config.AU
                    : Config.Logistic_SHIP_CONFIG.ShipMaxWarpSpeed.Value * Config.M);


            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.logisticCourierSpeed      : {__instance.logisticCourierSpeed}");
            
        }
    }
}
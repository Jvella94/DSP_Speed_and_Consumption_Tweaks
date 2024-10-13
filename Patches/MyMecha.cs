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
    [HarmonyPatch(typeof(Mecha))]
    internal class MyMecha
    {
        /// <summary>
        /// Patches the Player Awake method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(Mecha.SetForNewGame))]
        [HarmonyPostfix]
        public static void SetForNewGame_Postfix(Mecha __instance)
        {
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|  In Mecha SetForNewGame method Postfix |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }
            applyPatch(ref __instance);
        }

        /// <summary>
        /// Patches the Mecha Import method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(Mecha.Import))]
        [HarmonyPostfix]
        public static void Import_Postfix(Mecha __instance, BinaryReader r)
        {
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|      In Mecha Import method Postfix    |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }
            applyPatch(ref __instance);
        }

        /// <summary>
        /// Method to apply the patch on the Mecha entity
        /// </summary>
        /// <param name="__instance"></param>
        public static void applyPatch(ref Mecha __instance)
        {
            //////////////////////////////////////////////////////////////////////////////
            // CRUISE CONFIG INIT
            //////////////////////////////////////////////////////////////////////////////
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|              In applyPatch             |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|       CRUISE engine Configuration      |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.maxSailSpeed         : {__instance.maxSailSpeed}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.thrustPowerPerAcc    : {__instance.thrustPowerPerAcc}");
            }
            __instance.maxSailSpeed = (float)(Config.Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit.Value != "M"
                    ? Config.Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit.Value != "AU"
                    ? Config.Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit.Value != "LY"
                    ? __instance.maxSailSpeed 
                        : Config.Mecha_CRUISE_CONFIG.MaxCruiseSpeed.Value * Config.LY 
                        : Config.Mecha_CRUISE_CONFIG.MaxCruiseSpeed.Value * Config.AU 
                        : Config.Mecha_CRUISE_CONFIG.MaxCruiseSpeed.Value * Config.M);
            
            __instance.thrustPowerPerAcc = Config.Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCost.Value;
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.maxSailSpeed         : {__instance.maxSailSpeed}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.thrustPowerPerAcc    : {__instance.thrustPowerPerAcc}");
            }
            ////////////////////////////////////////////////////////////////////////////////
            // WARP CONFIG INIT
            ////////////////////////////////////////////////////////////////////////////////
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|        WARP engine Configuration       |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.maxWarpSpeed             : {__instance.maxWarpSpeed}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.warpStartPowerPerSpeed   : {__instance.warpStartPowerPerSpeed}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.warpKeepingPowerPerSpeed : {__instance.warpKeepingPowerPerSpeed}");
            }
            __instance.maxWarpSpeed =
                      (float)(Config.Mecha_WARP_CONFIG.maxWarpSpeedUnit.Value != "M"
                    ? Config.Mecha_WARP_CONFIG.maxWarpSpeedUnit.Value != "AU"
                    ? Config.Mecha_WARP_CONFIG.maxWarpSpeedUnit.Value != "LY"
                    ? __instance.maxWarpSpeed
                        : Config.Mecha_WARP_CONFIG.maxWarpSpeed.Value * Config.LY
                        : Config.Mecha_WARP_CONFIG.maxWarpSpeed.Value * Config.AU
                        : Config.Mecha_WARP_CONFIG.maxWarpSpeed.Value * Config.M);

            __instance.warpStartPowerPerSpeed = Config.Mecha_WARP_CONFIG.warpStartPowerPerSpeed.Value;

            __instance.warpKeepingPowerPerSpeed = Config.Mecha_WARP_CONFIG.warpKeepingPowerPerSpeed.Value;
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.maxWarpSpeed             : {__instance.maxWarpSpeed}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.warpStartPowerPerSpeed   : {__instance.warpStartPowerPerSpeed}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"values of __instance.warpKeepingPowerPerSpeed : {__instance.warpKeepingPowerPerSpeed}");
            }

        }
    }
}
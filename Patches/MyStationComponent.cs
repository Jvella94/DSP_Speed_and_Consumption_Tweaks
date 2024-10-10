using HarmonyLib;

namespace DSP_Speed_and_Consumption_Tweaks.Patches
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required

    /// <summary>
    /// Sample Harmony Patch class. Suggestion is to use one file per patched class
    /// though you can include multiple patch classes in one file.
    /// Below is included as an example, and should be replaced by classes and methods
    /// for your mod.
    /// </summary>
    [HarmonyPatch(typeof(StationComponent))]
    internal class MyStationComponent
    {
        /// <summary>
        /// Patches the Player Awake method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(StationComponent.DetermineDispatch))]
        [HarmonyPrefix]
        public static bool DetermineDispatch_Prefix(StationComponent __instance)
        {
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+------------------------------------------------+");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("| In StationComponent Constructor method Prefix |");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+------------------------------------------------+");




            return true;
        }

        /// <summary>
        /// Patches the StationComponent CalcTripEnergyCost method with Prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(StationComponent.CalcTripEnergyCost))]
        [HarmonyPrefix]
        public static bool CalcTripEnergyCost_Prefix(StationComponent __instance)
        {
            return false;
        }

        /// <summary>
        /// Patches the StationComponent InternalTickLocal method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(StationComponent.InternalTickLocal))]
        [HarmonyPostfix]
        public static void InternalTickLocal_Postfix(StationComponent __instance)
        {
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+------------------------------------------------------+");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("| In StationComponent InternalTickLocal method Postfix |");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+------------------------------------------------------+");
        }
    }
}
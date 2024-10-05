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
        /// Patches the StationComponent Init method with prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(StationComponent.Init))]
        [HarmonyPrefix]
        public static bool Init_Prefix(int _id, int _entityId, int _pcId, PrefabDesc _desc, EntityData[] _entityPool, int _extraStorage, bool _logisticShipWarpDrive)
        {
            DSP_Speed_and_Consumption_TweaksPlugin.Log.LogDebug("In StationComponent Init method Prefix.");
            return true;
        }

        /// <summary>
        /// Patches the StationComponent Init method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(StationComponent.Init))]
        [HarmonyPostfix]
        public static void Init_Postfix(int _id, int _entityId, int _pcId, PrefabDesc _desc, EntityData[] _entityPool, int _extraStorage, bool _logisticShipWarpDrive)
        {
            DSP_Speed_and_Consumption_TweaksPlugin.Log.LogDebug("In StationComponent Init method Postfix.");
        }
    }
}
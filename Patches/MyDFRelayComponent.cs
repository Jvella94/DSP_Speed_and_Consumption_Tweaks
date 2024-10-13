using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace DSP_Speed_and_Consumption_Tweaks.Patches
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required

    /// <summary>
    /// Sample Harmony Patch class. Suggestion is to use one file per patched class
    /// though you can include multiple patch classes in one file.
    /// Below is included as an example, and should be replaced by classes and methods
    /// for your mod.
    /// </summary>
    [HarmonyPatch(typeof(DFRelayComponent))]
    internal class MyDFRelayComponent
    {
        /// <summary>
        /// Patches the Player Awake method with prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(DFRelayComponent.CarrierSailLogic))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CarrierSailLogic_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions, il);

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In CarrierSailLogic method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------+");
            }

            matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "1800"))
                );

            float maxCarrierCruiseSpeed = (float)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * 1.5f;
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("------------------  avant  ------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 10);

                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_R4, maxCarrierCruiseSpeed));

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("------------------  après  ------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 10);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("---------------------------------------------");
            }

            matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "1800"))
                );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("------------------  avant  ------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 4);

                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_R4, maxCarrierCruiseSpeed));

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("------------------  après  ------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 4);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("---------------------------------------------");
            }
            if (matcher.IsInvalid)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError("Failed to apply CarrierSailLogic patch");
                return codeInstructions;
            }
            return matcher.InstructionEnumeration();
        }

        /// <summary>
        /// Patches the Player Awake method with prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(DFRelayComponent.RelaySailLogic))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RelaySailLogic_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions, il);

            float maxRelayCruiseSpeed = (float)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * 1.5f;

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+-------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In RelaySailLogic method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+-------------------------------------+");
            }

            matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "1000"))
                );

            matcher.RemoveInstruction();
            matcher.Insert(new CodeInstruction(OpCodes.Ldc_R4, maxRelayCruiseSpeed));

            matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "1000"))
                );

            matcher.RemoveInstruction();
            matcher.Insert(new CodeInstruction(OpCodes.Ldc_R4, maxRelayCruiseSpeed));

            if (matcher.IsInvalid)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError("Failed to apply RelaySailLogic patch");
                return codeInstructions;
            }

            return matcher.InstructionEnumeration();
        }
    }
}
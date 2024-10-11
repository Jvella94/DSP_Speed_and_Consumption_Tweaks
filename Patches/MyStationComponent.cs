using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        static public bool showFirst = false;
        static public int position = 0;

        /// <summary>
        /// Patches the StationComponent InternalTickLocal method with Transpiler code.
        /// </summary>
        [HarmonyPatch(nameof(StationComponent.InternalTickLocal))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> InternalTickLocal_Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator il
        )
        {
            // get the orginal instructions
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent DetermineDispatch method Transpiler |");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");

            var matcher = new CodeMatcher(codeInstructions, il)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(OpCodes.Ble),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(i => i.opcode == OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldc_I4)
                    );

            if (matcher.IsValid)
            {
                for (int i = -2; i < 2; i++)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo(
                        $"N° instruction: {matcher.Pos + i} ||| " +
                        $"instruction: {matcher.InstructionAt(i).opcode.ToString()}" +
                        (matcher.InstructionAt(i).operand != null ? $" ||| operand: {matcher.InstructionAt(i).operand.ToString()}" : "")
                        );
                }
                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_I4, (int)Config.Logistic_DRONE_CONFIG.DroneEnergyTakeOff.Value));
                for (int i = -2; i < 2; i++)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo(
                        $"N° instruction: {matcher.Pos + i} ||| " +
                        $"instruction: {matcher.InstructionAt(i).opcode.ToString()}" +
                        (matcher.InstructionAt(i).operand != null ? $" ||| operand: {matcher.InstructionAt(i).operand.ToString()}" : "")
                        );
                }
            }
            else
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                return codeInstructions;
            }
            int patches = 0;
            while (matcher.Pos < instructions.Count() && patches < 2)
            {
                matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_S && (i.operand.ToString() == "System.Double (37)" || i.operand.ToString() == "System.Double (56)")),
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_S && (i.operand.ToString() == "System.Double (35)" || i.operand.ToString() == "System.Double (54)")),
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_S && (i.operand.ToString() == "System.Double (37)" || i.operand.ToString() == "System.Double (56)")),
                    new CodeMatch(i => i.opcode == OpCodes.Mul),
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_S && (i.operand.ToString() == "System.Double (38)" || i.operand.ToString() == "System.Double (57)")),
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_S && (i.operand.ToString() == "System.Double (38)" || i.operand.ToString() == "System.Double (57)"))
                );
                if(matcher.IsValid){
                    patches++;
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Second match");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                    for (int i = 0; i < 20; i++)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo(
                            $"N° instruction: {matcher.Pos + i} ||| " +
                            $"instruction: {matcher.InstructionAt(i).opcode.ToString()}" +
                            (matcher.InstructionAt(i).operand != null ? $" ||| operand: {matcher.InstructionAt(i).operand.ToString()}" : "")
                            );
                    }
                    matcher.Advance(1);
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_R8, Config.Logistic_DRONE_CONFIG.DroneEnergyPerMeter.Value));
                    matcher.Advance(4);
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_R8, Config.Logistic_DRONE_CONFIG.DroneEnergyTakeOff.Value));
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
                    for (int i = -10; i < 10; i++)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo(
                            $"N° instruction: {matcher.Pos + i} ||| " +
                            $"instruction: {matcher.InstructionAt(i).opcode.ToString()}" +
                            (matcher.InstructionAt(i).operand != null ? $" ||| operand: {matcher.InstructionAt(i).operand.ToString()}" : "")
                            );
                    }
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
                }
                else
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                    return codeInstructions;
                }
            }
            


            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("Match found and patched");
            return matcher.InstructionEnumeration();
            
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
            if( ! showFirst)
            {
                showFirst = true;
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent InternalTickLocal method Postfix |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+------------------------------------------------------+");
            }
            
        }
    }
}
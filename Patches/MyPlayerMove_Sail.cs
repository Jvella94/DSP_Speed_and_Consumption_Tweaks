using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using UnityEngine;
using static DSP_Speed_and_Consumption_Tweaks.Config;
using static DSP_Speed_and_Consumption_Tweaks.DSP_Speed_and_Consumption_Tweaks_Plugin;

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
    internal class MyPlayerMove_Sail
    {
        /// <summary>
        /// Patches the PlayerMove_Sail Init method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(PlayerMove_Sail.Init))]
        [HarmonyPostfix]
        public static void Init_Postfix(PlayerMove_Sail __instance)
        {
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In PlayerMove_Sail.Init method Postfix |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Value of __instance.max_acc       : {__instance.max_acc}.");
            }
            __instance.max_acc = (float)Config.Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRate.Value;
            
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"New value of __instance.max_acc   : {__instance.max_acc}.");
            }
        }

        /// <summary>
        /// Patches the PlayerMove_Sail GameTick method with postfix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(PlayerMove_Sail.GameTick))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GameTick_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            // get the orginal instructions
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions, il);


            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In PlayerMove_Sail.GameTick_Transpiler method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------+");

                if (generateCsvFile)
                {
                    try
                    {
                        using (FileStream FS = new FileStream(pluginPath + "/expectedInstructionsNew.csv", FileMode.Append, FileAccess.Write))
                        {
                            //DSP_Speed_and_Consumption_Tweaks_Plugin.LogError($"this is the content we want to write to file {pluginPath + "/expectedInstructions.csv"}\n{strexpectedInstructions}");

                            StringCollection strexpectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 1000000);
                            byte[] strexpectedInstructionsBytes;
                            foreach (string expectedInstruction in strexpectedInstructions)
                            {
                                strexpectedInstructionsBytes = Encoding.UTF8.GetBytes(
                                    expectedInstruction
                                );
                                strexpectedInstructionsBytes = strexpectedInstructionsBytes.Concat(Encoding.UTF8.GetBytes("\n")).ToArray();

                                //DSP_Speed_and_Consumption_Tweaks_Plugin.LogError($"The length of the Bytes array is {strexpectedInstructionsBytes.Length} and it's count is {strexpectedInstructionsBytes.Count()}");
                                FS.Write(strexpectedInstructionsBytes, 0, strexpectedInstructionsBytes.Length);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        //DSP_Speed_and_Consumption_Tweaks_Plugin.LogError();
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"Error writing to file {pluginPath + "/expectedInstructions.csv"}");

                        //DSP_Speed_and_Consumption_Tweaks_Plugin.LogError();
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError(e.ToString());

                    }

                }


                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Acceleration rate multiplier       : 2 % / s .");
            }
            double acceleration_rate_multiplier = (double)Config.Mecha_CRUISE_CONFIG.CruiseAccelerationRateMultiplier.Value;

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S && i.operand.ToString() == "System.Double (37)"),
                new CodeMatch(OpCodes.Ldc_R8, (object)0.02),
                new CodeMatch(OpCodes.Mul)
                );

            if (matcher.IsValid)
            {
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------ Acceleration rate multiplier -------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-----------------  before  ----------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 709))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");

                }

                Console.WriteLine($"Found! at {matcher.Pos - 1}");
                matcher.Advance(-1);
                matcher.Set(OpCodes.Ldc_R8, acceleration_rate_multiplier * 0.02);

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  after  ------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 709))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");

                }

                ;
            }

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S && i.operand.ToString() == "System.Double (45)"),
                new CodeMatch(OpCodes.Ldc_R8, (object)0.02),
                new CodeMatch(OpCodes.Mul)
                );

            if (matcher.IsValid)
            {
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------ Acceleration rate multiplier -------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-----------------  before  ----------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: matcher.Pos - 1))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");

                }

                Console.WriteLine($"Found! at {matcher.Pos - 1}");
                matcher.Advance(-1);
                matcher.Set(OpCodes.Ldc_R8, acceleration_rate_multiplier * 0.02);

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  after  ------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: matcher.Pos - 1))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");

                }

            }
            
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_R8, (object)0.008),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S && i.operand.ToString() == "VectorLF3 (50)")
            );

            if (matcher.IsValid)
            {
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------ Acceleration rate multiplier -------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-----------------  before  ----------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: matcher.Pos - 2))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");

                }

                Console.WriteLine($"Found! at {matcher.Pos - 2}");
                matcher.Advance(-2);
                matcher.Set(OpCodes.Ldc_R8, acceleration_rate_multiplier * 0.008);

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  after  ------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: matcher.Pos))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");

                }

                return matcher.InstructionEnumeration();
            }

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Acceleration rate multiplier       : {(int)(acceleration_rate_multiplier*100)} % / s .");
            }
            return instructions;
        }

    }
}
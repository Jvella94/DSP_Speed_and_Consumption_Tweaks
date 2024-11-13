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
    [HarmonyPatch(typeof(DFTinderComponent))]
    internal class MyDFTinderComponent
    {
        /// <summary>
        /// Patches the Player Awake method with prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(DFTinderComponent.TinderSailLogic))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TinderSailLogic_Transpiler(
            IEnumerable<CodeInstruction> instructions, 
            ILGenerator il
        )
        {
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            StringCollection expectedInstructions;

            var matcher = new CodeMatcher(codeInstructions, il);

            double maxSeedSailSpeed = (double)Config.Dark_Fog_CONFIG.maxSeedSpeed.Value * (
                Config.Dark_Fog_CONFIG.maxSeedSpeedUnit.Value == "LY" ? Config.LY
                : Config.Dark_Fog_CONFIG.maxSeedSpeedUnit.Value == "AU" ? Config.AU
                : 1.0);

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+--------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+ In DFTinderComponent.TinderSailLogic method Transpiler +");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogDebug("+--------------------------------------------------------+");

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
            }

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"----- seed acceleration and deceleration -----");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-----------------  before  -------------------");
                foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 114))
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
            }



            matcher.MatchForward(true,
                new CodeMatch(opcode: OpCodes.Ldc_R4, operand: (float)240.0),
                new CodeMatch(opcode: OpCodes.Stloc_0),
                new CodeMatch(opcode: OpCodes.Ldc_R4, operand: (float)360.0),
                new CodeMatch(opcode: OpCodes.Stloc_1)
            );

            if (matcher.IsValid)
            {
                matcher.Advance(-3);
                matcher.Set(opcode: OpCodes.Ldc_R4, operand: (float)(maxSeedSailSpeed * 0.2));
                matcher.Advance(2);
                matcher.Set(opcode: OpCodes.Ldc_R4, operand: (float)(maxSeedSailSpeed * 0.3));

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"----- seed sail speed -----");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------  after   --------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 114))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                }

                matcher.MatchForward(true,
                    new CodeMatch(opcode: OpCodes.Ldloc_S),
                    new CodeMatch(opcode: OpCodes.Ldc_R4, operand: (float)1200.0),
                    new CodeMatch(opcode: OpCodes.Ble_Un),
                    new CodeMatch(opcode: OpCodes.Ldc_R4, operand: (float)1200.0)
                );

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"----- seed sail speed -----");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------  before  --------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 114))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                }

                if (matcher.IsValid)
                {
                    
                    matcher.Advance(-2);
                    matcher.Set(opcode: OpCodes.Ldc_R4, operand: (float)(maxSeedSailSpeed));
                    matcher.Advance(2);
                    matcher.Set(opcode: OpCodes.Ldc_R4, operand: (float)(maxSeedSailSpeed));
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------  after  ---------");
                        foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 114))
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                    }
                }
            }


            if (!matcher.IsValid)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in TinderSailLogic_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20000, expectedInstructionPosition: 114))
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"{expectedInstruction}");
                return codeInstructions;
            }

            return matcher.Instructions();
        }
    }
}
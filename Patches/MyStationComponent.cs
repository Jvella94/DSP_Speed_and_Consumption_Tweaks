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
    [HarmonyPatch(typeof(StationComponent))]
    internal class MyStationComponent
    {
        static public bool showFirst = false;
        static public int position = 0;
        static public double maxDroneTaxiSpeed = 1.0;

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
            StringCollection debugInstructions = new StringCollection();
            StringCollection expectedInstructions = new StringCollection();

            // replace takeoff cost and flight cost with custom values
            var matcher = new CodeMatcher(codeInstructions, il);
                

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent InternalTickLocal method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");

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


            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ble),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_I4)
            );

            if (matcher.IsValid)
            {
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"----- drone energy take off 1------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  before  ------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 114))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                }
                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_I4, (int)Config.Logistic_DRONE_CONFIG.DroneEnergyTakeOff.Value));
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  after  -------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 114))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}"); 
                }
            }
            else
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20, expectedInstructionPosition: 114))
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"{expectedInstruction}");
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
                if (matcher.IsValid)
                {
                    patches++;
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"----- drone energy per meter ------");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------ drone takeoff energy { patches + 1} ---");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  before  ------------");
                        foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: (patches == 1) ? 365 : 846 ))
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                    }
                    matcher.Advance(1);
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_R8, Config.Logistic_DRONE_CONFIG.DroneEnergyPerMeter.Value));
                    matcher.Advance(4);
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_R8, Config.Logistic_DRONE_CONFIG.DroneEnergyTakeOff.Value));
                    
                    matcher.Advance(-5);
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  after  -------------");
                        foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: (patches == 1) ? 365 : 846))
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                    }
                }
                else
                {
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20, expectedInstructionPosition: (patches == 1) ? 365 : 846))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.LogError($"{expectedInstruction}");
                    
                    return codeInstructions;
                }
            }

            // set max taxi speed with custom values
            matcher.Start();
            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToDouble(i.operand) == 8)),
                new CodeMatch(i => i.opcode == OpCodes.Div),
                new CodeMatch(OpCodes.Call)
                );
            matcher.Advance(-2);
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------ max drone taxi speed -------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  before  ------------");
                foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 39))
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
            }
            if (matcher.IsValid){
                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_R4, (float)maxDroneTaxiSpeed));
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  after  -------------");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 39))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"{expectedInstruction}");
                }
            }
            else
            {
                
                foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20, expectedInstructionPosition: 39))
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"{expectedInstruction}");

                return codeInstructions;
            }
            return matcher.InstructionEnumeration();

        }

        private delegate double setMaxTaxiSpeed(double _currentSpeed);

        [HarmonyPatch(nameof(StationComponent.DetermineDispatch))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DetermineDispatch_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {


            // get the orginal instructions
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions, il);

            StringCollection expectedInstructions;
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent DetermineDispatch method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
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

            
            int patches = 0;
            while (matcher.Pos < instructions.Count() && patches < 2)
            {
                matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldfld && (i.operand.ToString() == "System.Int64 energy")),
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_I4 && ( Convert.ToInt32(i.operand) == 6000000))
                );
                if (matcher.IsValid)
                {
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------- vessels energy takeoff -------");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"--------------- before ---------------");
                        expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: (patches == 0) ? 393 : 762);
                        
                        foreach (string expectedInstruction in expectedInstructions)
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                    }
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_I4, (int)Config.Logistic_SHIP_CONFIG.ShipEnergyCostTakeOff.Value));
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------  after ---------------");
                        
                        foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: (patches == 0) ? 393 : 762))
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                    }
                    patches++;
                }
                else
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                    foreach (string expectedInstruction in DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20, expectedInstructionPosition: (patches == 0) ? 393 : 762))
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");

                    return codeInstructions;
                }
            }
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("Match found and patched");
            }
            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(StationComponent.CalcLocalSingleTripTime))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CalcLocalSingleTripTime_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            StringCollection expectedInstructions;

            var matcher = new CodeMatcher(codeInstructions, il);
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent CalcLocalSingleTripTime method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------------+");
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

            
            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R8 && (Convert.ToDouble(i.operand) == 1.5)),
                new CodeMatch(OpCodes.Ldarg_2)
                );
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---- vessels max Drone Taxi Speed ----");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"--------------- before ---------------");
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 28);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
            }

            if (matcher.IsValid)
            {

                
                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_R4, (float)maxDroneTaxiSpeed));
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------  après  ---------------");
                    expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 28);
                    foreach (string expectedInstruction in expectedInstructions)
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("Match found and patched");
                }
                return matcher.InstructionEnumeration();
            }
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
            expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20, expectedInstructionPosition: 28);
            foreach (string expectedInstruction in expectedInstructions)
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($" {expectedInstruction}");
            return codeInstructions;

        }


        /// <summary>
        /// Patches the StationComponent CalcTripEnergyCost method with Prefix code.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(nameof(StationComponent.CalcTripEnergyCost))]
        [HarmonyPrefix]
        public static bool CalcTripEnergyCost_Prefix(ref long __result, StationComponent __instance, double trip, float maxSpeed, bool canWarp)
        {
            double num = trip * 0.03 + 100.0;
            if (num > (double)maxSpeed)
            {
                num = maxSpeed;
            }
            if (num > 3000.0)
            {
                num = 3000.0;
            }
            double num2 = num * Config.Logistic_SHIP_CONFIG.ShipEnergyCostForMaxSpeed.Value;
            if (canWarp && trip > __instance.warpEnableDist)
            {
                num2 += Config.Logistic_SHIP_CONFIG.ShipEnergyCostPerWarp.Value;
            }

            __result = (long)(Config.Logistic_SHIP_CONFIG.ShipEnergyCostTakeOff.Value + trip * Config.Logistic_SHIP_CONFIG.ShipEneregyCostPerMeter.Value + num2);

            return false;
        }

        /// <summary>
        /// Patches the StationComponent internalTickRemote method with Transpiler code.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPatch(nameof(StationComponent.InternalTickRemote))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> InternalTickRemote_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions, il);
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent InternalTickRemote method Transpiler |");
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
            }


            
            double maxCruiseShipSpeed = (double)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * (
                Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value == "LY" ? Config.LY
                : Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value == "AU" ? Config.AU
                : 1.0);
            StringCollection expectedInstructions;

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldelema),
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.03f)),
                new CodeMatch(i => i.opcode == OpCodes.Mul)
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"--------------  max Ship Atmo Speed  --------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(
                    ref matcher, 
                    5,
                    expectedInstructionPosition: 138
                );
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipAtmoSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipAtmoSpeed.Value;
            matcher.Advance(-2);
            matcher.RemoveInstructions(3);
            matcher.Insert(
                    new CodeInstruction(OpCodes.Nop), 
                    new CodeInstruction(OpCodes.Nop), 
                    new CodeInstruction(OpCodes.Ldc_R4, (float)maxShipAtmoSpeed)
                );
            matcher.Advance(2);

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5,
                    expectedInstructionPosition: 138);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),                                   //ldarg.3
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.12f)),     //ldc.r4
                new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),                                   //ldloc.s
                new CodeMatch(i => i.opcode == OpCodes.Mul)                                        //mul
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------------- max Cruise Ship Speed  --------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 144);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipNearSpeed = (float)(
                Config.Logistic_SHIP_CONFIG.maxShipNearSpeed.Value > maxCruiseShipSpeed 
                ? maxCruiseShipSpeed * 0.5 : 
                Config.Logistic_SHIP_CONFIG.maxShipNearSpeed.Value
            );

            matcher.Advance(-4);
            matcher.RemoveInstructions(5);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Ldc_R4, (float)maxShipNearSpeed)
            );
            matcher.Advance(-1);

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 144);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),                                   //ldarg.3
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.4f)),   //ldc.r4 
                new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul    
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),                                   //ldloc.s
                new CodeMatch(i => i.opcode == OpCodes.Mul)                                        //mul    
            );

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.006f)),
                new CodeMatch(i => i.opcode == OpCodes.Mul),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4),
                new CodeMatch(i => i.opcode == OpCodes.Add)
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"-------------  max Ship Taxi Speed  ---------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 156);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipTaxiSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipTaxiSpeed.Value;

            matcher.Advance(-4);
            matcher.RemoveInstructions(5);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Ldc_R4, (float)maxShipTaxiSpeed)
            );
            matcher.Advance(-1);

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 156);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            if (!matcher.IsValid)
            {
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 20, expectedInstructionPosition: 156);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($" {expectedInstruction}");
                return codeInstructions;
            }
            return matcher.InstructionEnumeration();

        }

        /// <summary>
        /// Patches the StationComponent internalTickRemote method with Transpiler code.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPatch(nameof(StationComponent.CalcRemoteSingleTripTime))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> calcRemoteSingleTripTime_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            float shipSailSpeed = (float)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value;
            StringCollection expectedInstructions;
            
            var matcher = new CodeMatcher(codeInstructions, il);
            
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent CalcRemoteSingleTripTime method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------------+");
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




            ////////////////////////////////////
            ///taxi speed
            ///
            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.006f)),
                new CodeMatch(i => i.opcode == OpCodes.Mul),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 1E-05f)),
                new CodeMatch(i => i.opcode == OpCodes.Add),
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S)
                );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"--------------  max Ship Taxi Speed  --------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 81);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipTaxiSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipTaxiSpeed.Value;
            if (matcher.IsValid)
            {
                matcher.Advance(-2);
                matcher.RemoveInstructions(2);
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Pop, 0),
                    new CodeInstruction(OpCodes.Ldc_R4, (float)maxShipTaxiSpeed)
                );

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 81);
                    foreach (string expectedInstruction in expectedInstructions)
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
                }
            

                ////////////////////////////////////
                ///atmo speed
                ///

                matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.03f)),
                    new CodeMatch(i => i.opcode == OpCodes.Mul),
                    new CodeMatch(i => i.opcode == OpCodes.Stloc_S)
                    );

                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"--------------  max Ship Atmo Speed  --------------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                    expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 85);
                    foreach (string expectedInstruction in expectedInstructions)
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
                }
                
                if (matcher.IsValid)
                {
                    float maxShipAtmoSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipAtmoSpeed.Value;
                    matcher.Advance(-2);
                    matcher.RemoveInstructions(2);
                    matcher.InsertAndAdvance(
                        new CodeInstruction(OpCodes.Pop, 0),
                        new CodeInstruction(OpCodes.Ldc_R4, (float)maxShipAtmoSpeed)
                    );

                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 85);
                        foreach (string expectedInstruction in expectedInstructions)
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
                    }
                    ////////////////////////////////////
                    ///near speed
                    ///

                    matcher.MatchForward(true,
                        new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),                                   //ldarg.3
                        new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (Convert.ToSingle(i.operand) == 0.12f)),//ldc.r4
                        new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul
                        new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),                                   //ldloc.s
                        new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul
                        new CodeMatch(i => i.opcode == OpCodes.Stloc_S)                                    //stloc.s
                    );

                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"------------  max Cruise Ship Speed ---------------");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                        expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 91);
                        foreach (string expectedInstruction in expectedInstructions)
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
                    }
                    if (matcher.IsValid)
                    {
                        double maxCruiseShipSpeed = (double)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value * (
                            Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value == "LY" ? Config.LY
                            : Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits.Value == "AU" ? Config.AU
                            : 1.0);

                        float maxShipNearSpeed = (float)(
                            Config.Logistic_SHIP_CONFIG.maxShipNearSpeed.Value > maxCruiseShipSpeed
                            ? maxCruiseShipSpeed * 0.5 :
                            Config.Logistic_SHIP_CONFIG.maxShipNearSpeed.Value
                        );


                        matcher.Advance(-2);
                        matcher.RemoveInstructions(2);
                        matcher.InsertAndAdvance(
                            new CodeInstruction(OpCodes.Pop, 0),
                            new CodeInstruction(OpCodes.Ldc_R4, (float)maxShipNearSpeed)
                        );

                        if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                        {
                            expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 5, expectedInstructionPosition: 91);
                            foreach (string expectedInstruction in expectedInstructions)
                                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($" {expectedInstruction}");
                            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
                        }
                    }
                }
            }

            
            


            if (!matcher.IsValid)
            {
                expectedInstructions = DSP_Speed_and_Consumption_Tweaks_Plugin.returnInstructions(ref matcher, 10000);
                foreach (string expectedInstruction in expectedInstructions)
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($" {expectedInstruction}");
                return codeInstructions;
            }
            else return matcher.InstructionEnumeration();
        }
    }
}
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

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent DetermineDispatch method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
            }


            // replace takeoff cost and flight cost with custom values
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
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 2);
                }
                matcher.RemoveInstruction();
                matcher.Insert(new CodeInstruction(OpCodes.Ldc_I4, (int)Config.Logistic_DRONE_CONFIG.DroneEnergyTakeOff.Value));
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 2);
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
                if (matcher.IsValid)
                {
                    patches++;
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Second match");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                        DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 10);
                    }
                    matcher.Advance(1);
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_R8, Config.Logistic_DRONE_CONFIG.DroneEnergyPerMeter.Value));
                    matcher.Advance(4);
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_R8, Config.Logistic_DRONE_CONFIG.DroneEnergyTakeOff.Value));
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
                    }
                    matcher.Advance(-5);
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 10);
                        DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
                    }
                }
                else
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                    return codeInstructions;
                }
            }

            // set max taxi speed with custom values
            matcher.Start();
            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "8")),
                new CodeMatch(i => i.opcode == OpCodes.Div),
                new CodeMatch(OpCodes.Call)
                );
            matcher.Advance(-2);
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"Matched taxi speed");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5);
            }

            matcher.InsertAndAdvance(new CodeInstruction(HarmonyLib.Transpilers.EmitDelegate<setMaxTaxiSpeed>(_currentSpeed =>
                    {
                        return (
                            _currentSpeed > maxDroneTaxiSpeed)
                                ? maxDroneTaxiSpeed
                                : _currentSpeed;
                    }
                )));


            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5);

                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("Match found and patched");
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

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent DetermineDispatch method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------+");
            }

            var matcher = new CodeMatcher(codeInstructions, il);
            int patches = 0;
            while (matcher.Pos < instructions.Count() && patches < 2)
            {
                matcher.MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldfld && (i.operand.ToString() == "System.Int64 energy")),
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_I4 && (i.operand.ToString() == "6000000"))
                );
                if (matcher.IsValid)
                {
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5);
                    }
                    matcher.RemoveInstruction();
                    matcher.Insert(new CodeInstruction(OpCodes.Ldc_I4, (int)Config.Logistic_SHIP_CONFIG.ShipEnergyCostTakeOff.Value));
                    if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                    {
                        DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5);
                    }
                    patches++;
                }
                else
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
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
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent CalcLocalSingleTripTime method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------------------------------------+");
            }

            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

            var matcher = new CodeMatcher(codeInstructions, il);
            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R8 && (i.operand.ToString() == "1,5")),
                new CodeMatch(OpCodes.Ldarg_2)
                );
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5);
            }

            if (matcher.IsValid)
            {

                matcher.Advance(1);
                matcher.InsertAndAdvance(new CodeInstruction(HarmonyLib.Transpilers.EmitDelegate<setMaxTaxiSpeed>(_currentSpeed =>
                {
                    return (
                        _currentSpeed > maxDroneTaxiSpeed)
                            ? maxDroneTaxiSpeed
                            : _currentSpeed;
                }
                )));
                matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Conv_R4));
                if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
                {
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 54);
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
                    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("Match found and patched");
                }
                return matcher.InstructionEnumeration();
            }
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in InternalTickLocal_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
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
            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent InternalTickRemote method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------+");
            }


            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions, il);

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldelema),
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,03")),
                new CodeMatch(i => i.opcode == OpCodes.Mul)
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipAtmoSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipAtmoSpeed.Value;
            float shipSailSpeed = (float)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value;
            matcher.Advance(-2);
            matcher.RemoveInstructions(3);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_R4, (float)(maxShipAtmoSpeed < shipSailSpeed * 0.03 ? maxShipAtmoSpeed : shipSailSpeed * 0.03)));

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),                                   //ldarg.3
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,12")),//ldc.r4
                new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),                                   //ldloc.s
                new CodeMatch(i => i.opcode == OpCodes.Mul)                                        //mul
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipNearSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipNearSpeed.Value;

            matcher.Advance(-4);
            matcher.RemoveInstructions(5);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_R4, (float)(maxShipNearSpeed < shipSailSpeed * 0.12 * Mathf.Pow(shipSailSpeed / 600.0f, 0.4f) ? maxShipNearSpeed : shipSailSpeed * 0.12 * Mathf.Pow(shipSailSpeed / 600.0f, 0.4f))));

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),                                   //ldarg.3
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,4")), //ldc.r4 
                new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul    
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),                                   //ldloc.s
                new CodeMatch(i => i.opcode == OpCodes.Mul)                                        //mul    
            );

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,006")),
                new CodeMatch(i => i.opcode == OpCodes.Mul),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4),
                new CodeMatch(i => i.opcode == OpCodes.Add)
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 15);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipTaxiSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipTaxiSpeed.Value;

            matcher.Advance(-4);
            matcher.RemoveInstructions(5);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_R4, (float)(maxShipTaxiSpeed < Mathf.Pow(shipSailSpeed / 600.0f, 0.4f) * 0.006 + 1E-05f ? maxShipTaxiSpeed : Mathf.Pow(shipSailSpeed / 600.0f, 0.4f) * 0.006 + 1E-05f)));

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 15);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            if (!matcher.IsValid)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in InternalTickRemote_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
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

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("| In StationComponent CalcRemoteSingleTripTime method Transpiler |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------------------------------+");
            }


            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            float shipSailSpeed = (float)Config.Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed.Value;

            var matcher = new CodeMatcher(codeInstructions, il);


            ////////////////////////////////////
            ///taxi speed
            ///

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,006")),
                new CodeMatch(i => i.opcode == OpCodes.Mul),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "1E-05")),
                new CodeMatch(i => i.opcode == OpCodes.Add),
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S)
                );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5000);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipTaxiSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipTaxiSpeed.Value;

            matcher.Advance(-2);
            matcher.RemoveInstructions(2);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Pop, 0),
                new CodeInstruction(OpCodes.Ldc_R4, (float)(maxShipTaxiSpeed < Mathf.Pow(shipSailSpeed / 600.0f, 0.4f) * 0.006 + 1E-05f ? maxShipTaxiSpeed : Mathf.Pow(shipSailSpeed / 600.0f, 0.4f) * 0.006 + 1E-05f))
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 5000);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            ////////////////////////////////////
            ///atmo speed
            ///

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,03")),
                new CodeMatch(i => i.opcode == OpCodes.Mul),
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S)
                );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipAtmoSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipAtmoSpeed.Value;
            matcher.Advance(-2);
            matcher.RemoveInstructions(2);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Pop, 0),
                new CodeInstruction(OpCodes.Ldc_R4, (float)(maxShipAtmoSpeed < shipSailSpeed * 0.03 ? maxShipAtmoSpeed : shipSailSpeed * 0.03)));

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            }

            ////////////////////////////////////
            ///near speed
            ///

            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_3),                                   //ldarg.3
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand.ToString() == "0,12")),//ldc.r4
                new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),                                   //ldloc.s
                new CodeMatch(i => i.opcode == OpCodes.Mul),                                       //mul
                new CodeMatch(i => i.opcode == OpCodes.Stloc_S)                                    //stloc.s
            );

            if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  avant  ---------------------");
                DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------  après  ---------------------");
            }
            float maxShipNearSpeed = (float)Config.Logistic_SHIP_CONFIG.maxShipNearSpeed.Value;

            matcher.Advance(-2);
            matcher.RemoveInstructions(2);
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Pop, 0), 
                new CodeInstruction(OpCodes.Ldc_R4, (float)(maxShipNearSpeed < shipSailSpeed * 0.12 * Mathf.Pow(shipSailSpeed / 600.0f, 0.4f) ? maxShipNearSpeed : shipSailSpeed * 0.12 * Mathf.Pow(shipSailSpeed / 600.0f, 0.4f))));

            //if (DSP_Speed_and_Consumption_Tweaks_Plugin.DEBUG)
            //{
            //    DSP_Speed_and_Consumption_Tweaks_Plugin.printInstructions(ref matcher, 7);
            //    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"---------------------------------------------------");
            //}


            if (!matcher.IsValid)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogError($"No match found in calcRemoteSingleTripTime_Transpiler code incompatible with mod Version {DSP_Speed_and_Consumption_Tweaks_Plugin.VersionString}");
                return codeInstructions;
            }
            
            return matcher.InstructionEnumeration();
            

        }
    }
}
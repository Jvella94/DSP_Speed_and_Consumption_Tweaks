using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DSP_Speed_and_Consumption_Tweaks.Patches;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DSP_Speed_and_Consumption_Tweaks
{
    // TODO Review this file and update to your own requirements.
    


    [BepInPlugin(MyGUID, PluginName, VersionString)]
    [BepInProcess("DSPGAME.exe")]
    //[BepInDependency("dsp.galactic-scale.2")]
    public class DSP_Speed_and_Consumption_Tweaks_Plugin : BaseUnityPlugin
    {

        

        /// <summary>
        /// Prints the Neibouring instructions
        /// </summary>
        /// <param name="codeMatcher"></param>
        /// <param name="NumberOfNeibouringInstructions"></param>
        public static void printInstructions(ref CodeMatcher codeMatcher, int NumberOfNeibouringInstructions = 2)
        {
            int min = (
                codeMatcher.Pos > NumberOfNeibouringInstructions
                    ? -NumberOfNeibouringInstructions
                    : -codeMatcher.Pos
            );
            int max = (
                NumberOfNeibouringInstructions + codeMatcher.Pos > codeMatcher.Length - 1
                    ? codeMatcher.Length - codeMatcher.Pos
                    : NumberOfNeibouringInstructions
            );
            
            
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"codeMatcher.Pos = {codeMatcher.Pos} and codeMatcher.Length = {codeMatcher.Length}");
            DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo($"            min = {min}                                max = {max} ");

            for (int i = min; i < max; i++)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo(
                    $"N° instruction: {codeMatcher.Pos + i,5}"
                    +" ||| " +
                    $"instruction: {codeMatcher.InstructionAt(i).opcode.ToString(),-20}" +
                    (
                        codeMatcher.InstructionAt(i).operand != null 
                            ? $" ||| operand: {codeMatcher.InstructionAt(i).operand.ToString(),-30}" 
                            : "                                            "
                    ) +(i == 0 ? " <<<<<<<<< Current instruction" : "")

                );
            }
        }

        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // com.mynameororg.pluginname
        // Version should be a valid version string.
        // e.g.
        // 1.0.0
        private const string MyGUID = "com.hiul.DSP_Speed_and_Consumption_Tweaks";
        private const string PluginName = "DSP_Speed_and_Consumption_Tweaks";
        public const string VersionString = "1.0.0";
        public static bool DEBUG = true;


        public static ManualLogSource Log;
        public void Awake()
        {
            Harmony harmony = new Harmony(MyGUID);

            Log = base.Logger;
            DSP_Speed_and_Consumption_Tweaks.Config.Init(Config);
            //if (DEBUG)
            //{
            //    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            //    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|    Ships consumption Configuration     |");
            //    DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            //}
            //harmony.PatchAll(typeof(DSP_Speed_and_Consumption_Tweaks_Plugin));
            if (DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|    Patching GameHistoryData     |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+---------------------------------+");
            }
            harmony.PatchAll(typeof(MyGameHistoryData));
            if (DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|             Patching Mecha             |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }
            harmony.PatchAll(typeof(MyMecha));

            if (DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|        Pacthing PlayerControler        |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }
            harmony.PatchAll(typeof(MyPlayerController));
            if (DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|        Patching StationComponent       |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }
            harmony.PatchAll(typeof(MyStationComponent));

            if (DEBUG)
            {
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("|        Patching DFRelayComponent       |");
                DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("+----------------------------------------+");
            }
            harmony.PatchAll(typeof(MyDFRelayComponent));
            

        }
        public void Start()
        {
            
            Log.LogInfo("Loaded!");

            ModDebug.SetLogger(Log);

#if DEBUG
            Debugger.Break();
#endif
        }
    }
}

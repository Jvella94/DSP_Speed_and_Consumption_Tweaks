using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSP_Speed_and_Consumption_Tweaks
{
    using BepInEx.Configuration;


    

    public static class Config
    {
        private static readonly string MECHA_CRUISE_CONFIG = "ICARUS Cruise Configuration";
        private static readonly string MECHA_WARP_CONFIG = "ICARUS Warp Configuration";
        //private static readonly string UTILITY_SECTION = "Utility";

        public static readonly float LY = 2400000f;
        public static readonly float AU = 40000f;
        public static readonly float M = 1f;

        public static class Mecha_CRUISE_CONFIG
        {
            // mecha settings
            // cruise

            public static ConfigEntry<float> MaxCruiseSpeed;
            public static ConfigEntry<string> MaxCruiseSpeedUnit;
            public static ConfigEntry<bool> MaxCruiseSpeedUseCoef;
            public static ConfigEntry<float> MaxCruiseSpeedCoef;

            public static ConfigEntry<float> CruiseMaxAccelerationRate;
            public static ConfigEntry<bool> CruiseAccelerationUseCoef;
            public static ConfigEntry<float> CruiseMaxAccelerationRateCoef;

            public static ConfigEntry<float> CruiseAccelerationEnergyCost;
            public static ConfigEntry<bool> CruiseAccelerationEnergyCostUseCoef;
            public static ConfigEntry<float> CruiseAccelerationEnergyCostCoef;
        }

        public static class Mecha_WARP_CONFIG
        {
            // warp
            public static ConfigEntry<float> maxWarpSpeed;
            public static ConfigEntry<string> maxWarpSpeedUnit;
            public static ConfigEntry<bool> maxWarpSpeedUseCoef;
            public static ConfigEntry<float> maxWarpSpeedCoef;

            public static ConfigEntry<float> warpKeepingPowerPerSpeed;
            public static ConfigEntry<bool> warpKeepingPowerPerSpeedUseCoef;
            public static ConfigEntry<float> warpKeepingPowerPerSpeedCoef;

            public static ConfigEntry<float> warpStartPowerPerSpeed;
            public static ConfigEntry<bool> warpStartPowerPerSpeedUseCoef;
            public static ConfigEntry<float> warpStartPowerPerSpeedCoef;

        }

            
            
            

            //// ship settings
            //public static ConfigEntry<double> ShipMaxCruiseSpeed;
            //public static ConfigEntry<double> ShipMaxWarpSpeed;
            //public static ConfigEntry<double> ShipEneregyCost;

            //// drone settings
            //public static ConfigEntry<double> DroneMaxSpeed;
            //public static ConfigEntry<double> DroneEnergyCost;

        //public static class Utility
        //{
        //    public static ConfigEntry<bool> DisableMod;
        //    public static ConfigEntry<bool> UninstallMod;
        //}
        internal static void Init(ConfigFile config)
        {
            ////////////////////
            // General Config //
            ////////////////////

            Mecha_CRUISE_CONFIG.MaxCruiseSpeed = config.Bind(MECHA_CRUISE_CONFIG, "Maximum absolute cruise speed", 2000.0f,
                new ConfigDescription("Changes the base max cruise speed for Icarus. (Vanilla is 2000.0 m/s)\nIn game :\n40000 m is 1AU\n60 AU is 1 light year (2400000 m)",
                new AcceptableValueRange<float>(0.0f, 2400000.0f), Array.Empty<object>()));

            Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit = config.Bind(MECHA_CRUISE_CONFIG, "Max cruise speed modifier units", "Meters",
                new ConfigDescription("Unit to use for MaxCruiseSpeed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Mecha_CRUISE_CONFIG.MaxCruiseSpeedUseCoef = config.Bind(MECHA_CRUISE_CONFIG, "Use Coeficient ?", false,
                new ConfigDescription("Use cruise speed coefficient ? (ignores abslute max cruise speed)",
                    new AcceptableValueList<bool>(true, false)));

            Mecha_CRUISE_CONFIG.MaxCruiseSpeedCoef = config.Bind(MECHA_CRUISE_CONFIG, "Coeficient to use instead of absolute max speed", 1.0f,
                new ConfigDescription("max cruise speed coeficient for Icarus if activated. (Vanilla is 1)",
                    new AcceptableValueRange<float>(1.0f, 100.0f), Array.Empty<object>()));


            Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRate = config.Bind(MECHA_CRUISE_CONFIG, "Absolute acceleration rate in m/s^2", 20.0f,
                new ConfigDescription("Nax cruise acceleration rate for Icarus. (Vanilla is 20 m/s^2)",
                    new AcceptableValueRange<float>(0.1f, 10000.0f), Array.Empty<object>()));

            Mecha_CRUISE_CONFIG.CruiseAccelerationUseCoef = config.Bind(MECHA_CRUISE_CONFIG, "Use Coeficient ?", false,
                new ConfigDescription("Use cruise acceleration coeficient ? (ignores cruise max acceleration rate)",
                    new AcceptableValueList<bool>(true, false)));

            Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRateCoef = config.Bind(MECHA_CRUISE_CONFIG, "max acceleration rate coeficient", 1.0f,
                new ConfigDescription("Coeficient for the acceleration rate for Icarus. (Vanilla is 1)" +
                "\nThis value can't be upgraded in vanilla game.",
                    new AcceptableValueRange<float>(0.1f, 1000.0f), Array.Empty<object>()));


            Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCost = config.Bind(MECHA_CRUISE_CONFIG, "Energy used to maintain cruise acceleration", 24000.0f,
                new ConfigDescription("Cruise Acceleration Energy Cost for Icarus. (Vanilla is 24000 )" +
                "\nThis value is the base consumption per second at an acceleration of 20 m/s^2." +
                "\nThe faster you accelerate the bigger the consumption" +
                "\nWhile testing I had set the max speed to 100000 m/s and max acceleration to 1000" +
                "\nI depleted Icarus at Level 50 core energy infinit research in seconds",
                    new AcceptableValueRange<float>(0.1f, 240000.0f), Array.Empty<object>()));
            
            Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCostUseCoef = config.Bind(MECHA_CRUISE_CONFIG, "Use Coeficient ?", false,
                new ConfigDescription("Use cruise energy cost coeficient ? (ignores cruise max acceleration rate)",
                    new AcceptableValueList<bool>(true, false)));

            Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCostCoef = config.Bind(MECHA_CRUISE_CONFIG, "Energy Coeficient to use", 1.0f,
                new ConfigDescription("Cruise acceleration cost Coeficient for Icarus. (Vanilla is 1)",
                    new AcceptableValueRange<float>(0.01f, 10.0f), Array.Empty<object>()));




            Mecha_WARP_CONFIG.maxWarpSpeed = config.Bind(MECHA_WARP_CONFIG, "Maximum absolute warp speed", 480000.0f,
                new ConfigDescription("Base max warp speed of Icarus. (Vanilla is 480000)",
                new AcceptableValueRange<float>(0.0f, 5000000.0f), new { }));

            Mecha_WARP_CONFIG.maxWarpSpeedUnit = config.Bind(MECHA_WARP_CONFIG, "Warp speed modifier units", "Meters",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Mecha_WARP_CONFIG.maxWarpSpeedUseCoef = config.Bind(MECHA_WARP_CONFIG, "Use Coeficient ?", false,
                new ConfigDescription("Use warp speed coefficient ? (ignores abslute max warp speed)",
                    new AcceptableValueList<bool>(true, false)));

            Mecha_WARP_CONFIG.maxWarpSpeedCoef = config.Bind(MECHA_WARP_CONFIG, "Coeficient to use instead of absolute max speed", 1.0f,
                new ConfigDescription("warp speed coeficient for Icarus if activated. (Vanilla is 1)",
                    new AcceptableValueRange<float>(0.0f, 1000.0f), Array.Empty<object>()));

            Mecha_WARP_CONFIG.warpStartPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp start power per speed", 400f,
                new ConfigDescription("Warp start power per speed for Icarus. (Vanilla is 400)",
                    new AcceptableValueRange<float>(0.0f, 50000.0f), Array.Empty<object>()));

            Mecha_WARP_CONFIG.warpStartPowerPerSpeedUseCoef = config.Bind(MECHA_WARP_CONFIG, "Use Coeficient ?", false,
                new ConfigDescription("Use warp start power coeficient ? (ignores warp start power per speed)",
                    new AcceptableValueList<bool>(true, false), Array.Empty<object>()));

            Mecha_WARP_CONFIG.warpStartPowerPerSpeedCoef = config.Bind(MECHA_WARP_CONFIG, "Coeficient to use instead of absolute max speed", 1.0f,
                new ConfigDescription("warp start power coeficient for Icarus if activated. (Vanilla is 1)",
                    new AcceptableValueRange<float>(0.0f, 1000.0f), Array.Empty<object>()));


            Mecha_WARP_CONFIG.warpKeepingPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp keeping power per speed", 80f,
                new ConfigDescription("Warp keeping power per speed for Icarus. (Vanilla is 80)",
                    new AcceptableValueRange<float>(0.0f,800f), Array.Empty<object>()));

            Mecha_WARP_CONFIG.warpKeepingPowerPerSpeedUseCoef = config.Bind(MECHA_WARP_CONFIG, "Use Coeficient ?", false,
                new ConfigDescription("Use warp keeping power coeficient ? (ignores warp keeping power per speed)",
                    new AcceptableValueList<bool>(true, false), Array.Empty<object>()));

            Mecha_WARP_CONFIG.warpKeepingPowerPerSpeedCoef = config.Bind(MECHA_WARP_CONFIG, "Coeficient to use instead of absolute max speed", 1.0f,
                new ConfigDescription("warp keeping power coeficient for Icarus if activated. (Vanilla is 1)",
                    new AcceptableValueRange<float>(0.0f, 100f), Array.Empty<object>()));


            //General.CruiseAccelerationRate = config.Bind(GENERAL_SECTION, "CruiseAccelerationRate", 1.0,
            //    new ConfigDescription("Base cruise acceleration rate of Icarus. (Vanilla is 1.0)",
            //    new AcceptableValueRange<double>(0.0, 1000.0), new { }));

            //General.CruiseAccelerationEnergyCost = config.Bind(GENERAL_SECTION, "CruiseAccelerationEnergyCost", 30.0,
            //    new ConfigDescription("Base cruise acceleration cost of Icarus. (Vanilla is 30.0)",
            //    new AcceptableValueRange<double>(0.0, 1000.0), new { }));

            //General.ShipMaxCruiseSpeed = config.Bind(GENERAL_SECTION, "ShipMaxCruiseSpeed", 800.0,
            //    new ConfigDescription("Base max cruise speed of ships. (Vanilla is 800)",
            //    new AcceptableValueRange<double>(0.0, 100000.0), new { }));

            //General.ShipMaxWarpSpeed = config.Bind(GENERAL_SECTION, "ShipMaxWarpSpeed", 100000000.0,
            //    new ConfigDescription("Base max warp speed of ships. (Vanilla is 100000000)",
            //    new AcceptableValueRange<double>(0.0, 1000000000.0), new { }));

            //General.DroneMaxSpeed = config.Bind(GENERAL_SECTION, "DroneMaxSpeed", 100.0,
            //    new ConfigDescription("Base max speed of drones. (Vanilla is 100)",
            //    new AcceptableValueRange<double>(0.0, 100000.0), new { }));

            //General.DroneCruiseAccelerationRate = config.Bind(GENERAL_SECTION, "DroneCruiseAccelerationRate", 1.0,
            //    new ConfigDescription(  "Base cruise acceleration rate of drones. (Vanilla is 1.0)",
            //    new AcceptableValueRange<double>(0.0, 1000.0), new { }));



            ////////////////////
            // Utility Config //
            ////////////////////

            //Utility.DisableMod = config.Bind(UTILITY_SECTION, "DisableMod", false,
            //    "While true this will disable all mod effects but will not remove additional slot from ILS. " +
            //    "Useful if uninstalling mod failed for some reason.");

            //Utility.UninstallMod = config.Bind(UTILITY_SECTION, "UninstallMod", false,
            //    "WARNING!!! BACKUP YOUR SAVE BEFORE DOING THIS!!! This will not work if mod cannot load properly! " +
            //    "If this is true, mod will remove additional slot from all current ILS. " +
            //    "This will destroy any items in additional slot " +
            //    "To correctly uninstall mod and get vanilla save please follow this steps. " +
            //    "Step #1: Set UninstallMod to true. " +
            //    "Step #2: Load your save. " +
            //    "Step #3: Save your game. " +
            //    "Step #4: Exit the game and remove this mod."
            //    );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DSP_Speed_and_Consumption_Tweaks.DSP_Speed_and_Consumption_Tweaks_Plugin;

namespace DSP_Speed_and_Consumption_Tweaks
{
    using BepInEx;
    using BepInEx.Configuration;
    using BepInEx.Logging;

    

    public static class Config
    {
        private static readonly string MECHA_CRUISE_CONFIG = "ICARUS Cruise Configuration";
        private static readonly string MECHA_WARP_CONFIG = "ICARUS Warp Configuration";
        private static readonly string LOGISTIC_SHIP_CONFIG = "Logistic Ships Configuration";
        private static readonly string LOGISTIC_SHIP_WARP = "Logistic Ships Warp Configuration";
        private static readonly string LOGISTIC_SHIP_SAIL = "Logistic Ships Cruise Configuration";
        private static readonly string LOGISTIC_DRONE_CONFIG = "Logistic Drones Configuration";
        private static readonly string DARK_FOG_CONFIG = "Dark Fog Configuration";
        private static readonly string DEBUG_CONFIG = "Activate DEBUG messages";
        //private static readonly string UTILITY_SECTION = "Utility";

        public static readonly double LY = 2400000;
        public static readonly double AU = 40000;
        public static readonly double M = 1;

        public static class Mecha_CRUISE_CONFIG
        {
            // mecha settings
            // cruise

            public static ConfigEntry<double> MaxCruiseSpeed;
            public static ConfigEntry<string> MaxCruiseSpeedUnit;
            public static ConfigEntry<double> CruiseMaxAccelerationRate;
            public static ConfigEntry<double> CruiseAccelerationEnergyCost;

        }

        public static class Mecha_WARP_CONFIG
        {
            // warp
            public static ConfigEntry<double> maxWarpSpeed;
            public static ConfigEntry<string> maxWarpSpeedUnit;
            public static ConfigEntry<double> warpKeepingPowerPerSpeed;
            public static ConfigEntry<double> warpStartPowerPerSpeed;

        }

            
            
        public static class Logistic_SHIP_CONFIG
        {
            //// ship settings
            public static ConfigEntry<double> approchSpeed;
       
            public static ConfigEntry<double> ShipMaxCruiseSpeed;
            public static ConfigEntry<string> ShipMaxCruiseSpeedUnits;
       
            public static ConfigEntry<double> ShipMaxWarpSpeed;
            public static ConfigEntry<string> ShipMaxWarpSpeedUnits;
            
            public static ConfigEntry<double> ShipEneregyCostPerMeter;
            public static ConfigEntry<double> ShipEnergyCostTakeOff;
            public static ConfigEntry<double> ShipEnergyCostForMaxSpeed;
            
            public static ConfigEntry<double> ShipEnergyCostPerWarp;
        }

        public static class Logistic_DRONE_CONFIG
        {
            //// drone settings
            public static ConfigEntry<double> DroneMaxSpeed;
            public static ConfigEntry<double> DroneMaxTaxiSpeed;
            public static ConfigEntry<double> DroneEnergyPerMeter;
            public static ConfigEntry<double> DroneEnergyTakeOff;
        }
        
        public static class Dark_Fog_CONFIG
        {
            public static ConfigEntry<double> maxSeedSpeed;
            public static ConfigEntry<string> maxSeedSpeedUnit;
            public static ConfigEntry<double> maxRelaySpeed;
            public static ConfigEntry<string> maxRelaySpeedUnit;
            public static ConfigEntry<double> maxCarrierSpeed;
            public static ConfigEntry<string> maxCarrierSpeedUnit;
        }


        public static class Debug_CONFIG
        {
            public static ConfigEntry<bool> DEBUG;
        }

        //public static class Utility
        //{
        //    public static ConfigEntry<bool> DisableMod;
        //    public static ConfigEntry<bool> UninstallMod;
        //}
        internal static void Init(ConfigFile config)
        {
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("Loading Config");
            /////////////////////////////////
            // ICARUS Cruise Configuration //
            /////////////////////////////////
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Mecha_CRUISE_CONFIG.MaxCruiseSpeed = config.Bind(MECHA_CRUISE_CONFIG, "Maximum absolute cruise speed (Vanilla is 1000.0 M/s)", 0.025,
                new ConfigDescription("Changes the base max cruise speed for Icarus. " +
                "\nIn game : " +
                "\n40000 m is 1AU " +
                "\n60 AU is 1 light year (2400000 m)",
                new AcceptableValueRange<double>(0.0, 10000.0), null));

            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit = config.Bind(MECHA_CRUISE_CONFIG, "Max cruise speed modifier units", "AU",
                new ConfigDescription("Unit to use for MaxCruiseSpeed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRate = config.Bind(MECHA_CRUISE_CONFIG, "Acceleration rate in m/s2 (Vanilla is 20 M/s2)", 20.0,
                new ConfigDescription("Max cruise acceleration rate for Icarus.",
                    new AcceptableValueRange<double>(0.0, 10000.0), null));

            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCost = config.Bind(MECHA_CRUISE_CONFIG, "Energy used to maintain cruise acceleration (Vanilla is 24000 )", 24000.0,
                new ConfigDescription("Cruise Acceleration Energy Cost for Icarus." +
                "\nThis value is the base consumption per second at an acceleration of 20 m/s^2." +
                "\nThe faster you accelerate the bigger the consumption" +
                "\nWhile testing I had set the max speed to 100000 m/s and max acceleration to 1000" +
                "\nI depleted Icarus at Level 50 core energy infinit research in seconds",
                    new AcceptableValueRange<double>(0.0, 240000.0), null));


            ///////////////////////////////
            // ICARUS Warp Configuration //
            ///////////////////////////////
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Mecha_WARP_CONFIG.maxWarpSpeed = config.Bind(MECHA_WARP_CONFIG, "Maximum absolute warp speed (Vanilla is 480000 M/s)", 0.20,
                new ConfigDescription("Base max warp speed of Icarus.",
                new AcceptableValueRange<double>(0.0, 1000000.0), null));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("2");
            Mecha_WARP_CONFIG.maxWarpSpeedUnit = config.Bind(MECHA_WARP_CONFIG, "Warp speed modifier units", "LY",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("3");
            Mecha_WARP_CONFIG.warpStartPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp start power per speed (Vanilla is 400)", 400.0,
                new ConfigDescription("Warp start power per speed for Icarus.",
                    new AcceptableValueRange<double>(0.0, 50000.0), null));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("4");
            Mecha_WARP_CONFIG.warpKeepingPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp keeping power per speed (Vanilla is 80)", 80.0,
                new ConfigDescription("Warp keeping power per speed for Icarus. ",
                    new AcceptableValueRange<double>(0.0,800), null));


            ///////////////////////////////////
            // Logistic Drones Configuration //
            ///////////////////////////////////
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("5");
            Logistic_DRONE_CONFIG.DroneMaxSpeed = config.Bind(LOGISTIC_DRONE_CONFIG, "Maximum speed (Vanilla is 8 M/s)", 8.0,
                new ConfigDescription("Base max speed of Logistic Drones. ",
                new AcceptableValueRange<double>(0.0, 1000.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("6");
            Logistic_DRONE_CONFIG.DroneMaxTaxiSpeed = config.Bind(LOGISTIC_DRONE_CONFIG, "Maximum taxi speed (Vanilla is sqrt(droneMaxSpeed / 8)).", 50.0,
                new ConfigDescription("max taxi speed of Logistic Drones. (The taxi speed is manoeuvering speed at the Station)"
                + "\nIt was just too unnatural to see the Drones or Ships docking at ligth speed and it removed a lot of the game s aesthics."
                + "\nI made it be 0.5 M/s (50 cm/s here since the config file only increment with whole numbers) here By Default it s half the default smallest value but I find it cooler." +
                "\nset it to 1 if you want the vailla speed.",
                new AcceptableValueRange<double>(1.0, 200.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("7");
            Logistic_DRONE_CONFIG.DroneEnergyPerMeter = config.Bind(LOGISTIC_DRONE_CONFIG, "Energy consumption per meter (Vanilla is 20000)", 20000.0,
                new ConfigDescription("Energy consumption per meter for Logistic Drones. ",
                new AcceptableValueRange<double>(0.0, 1000000.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("8");
            Logistic_DRONE_CONFIG.DroneEnergyTakeOff = config.Bind(LOGISTIC_DRONE_CONFIG, "Energy consumption on take off (Vanilla is 800000)", 800000.0,
                new ConfigDescription("Energy consumption on take off for Logistic Drones. ",
                new AcceptableValueRange<double>(0.0, 10000000.0), null ));


            //////////////////////////////////
            // Logistic Ships Configuration //
            //////////////////////////////////
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("9");
            Logistic_SHIP_CONFIG.approchSpeed = config.Bind(LOGISTIC_SHIP_SAIL, "Maximum approch speed (Vanilla is a fraction of the sailing speed)", 400.0,
                new ConfigDescription("This value will replace the sailing speed for the computations.\n" +
                "This speed will affect all phases of approching vessels.\n" +
                "From entering planet controled space all the way to last landing manoevre.\n" +
                "With this value the transitions between each phases is garantied to be smooth.\n" +
                "The value is capped between 100 (garantied) and sailing speed (up to 10000 M/s).",
                new AcceptableValueRange<double>(100.0, 10000.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed = config.Bind(LOGISTIC_SHIP_SAIL, "Base cruise speed (Vanilla is 400 M/s)", 400.0,
                new ConfigDescription("Max cruise speed for Logistic Ships. guaranteed to be at least 400 M/s",
                new AcceptableValueRange<double>(0.01, 10000.0), null));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("2");
            Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits = config.Bind(LOGISTIC_SHIP_SAIL, "Warp speed modifier units", "M",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("3");
            Logistic_SHIP_CONFIG.ShipMaxWarpSpeed = config.Bind(LOGISTIC_SHIP_WARP, "Maximum warp speed (Vanilla is 4000000 M/s)", 0.1667,
                new ConfigDescription("Base max warp speed of Logistic Ships. (Base as in before research modifier)",
                new AcceptableValueRange<double>(0.0, 4000000.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("4");
            Logistic_SHIP_CONFIG.ShipMaxWarpSpeedUnits = config.Bind(LOGISTIC_SHIP_WARP, "Warp speed modifier units", "LY",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("5");
            Logistic_SHIP_CONFIG.ShipEnergyCostPerWarp = config.Bind(LOGISTIC_SHIP_WARP, "Energy consumption per warp", 100000000.0,
                new ConfigDescription("Energy consumption per warp for Logistic Ships. (Vanilla is 100000000)",
                new AcceptableValueRange<double>(0.0, 200000000.0), null));


            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("6");
            Logistic_SHIP_CONFIG.ShipEneregyCostPerMeter = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption per Meter (Vanilla is 30 )", 30.0,
                new ConfigDescription("Energy consumption per meter of travel for Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 60.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("7");
            Logistic_SHIP_CONFIG.ShipEnergyCostForMaxSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption For max speed (Vanilla is 200000)", 200000.0,
                new ConfigDescription("Energy consumption for atteining max speed for Logistic Ships."
                + "\nThis value is multiplied by the max cruise speed of the ship durring the trip (max speed value capped at 3000 for the calculation).",
                new AcceptableValueRange<double>(0.0, 400000.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("8");
            Logistic_SHIP_CONFIG.ShipEnergyCostTakeOff = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption on take off", 6000000.0,
                new ConfigDescription("Energy consumption on take off for Logistic Ships. (Vanilla is 6000000)",
                new AcceptableValueRange<double>(0.0, 12000000.0), null ));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("9");


            ////////////////////////////
            // Dark Fog Configuration //
            ////////////////////////////
            Dark_Fog_CONFIG.maxSeedSpeed = config.Bind(DARK_FOG_CONFIG, "Seed speed (Vanilla is 1200 M/s)", 1200.0,
                new ConfigDescription("Max seed speed.",
                new AcceptableValueRange<double>(0.01, 10000.0), null));
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("2");
            Dark_Fog_CONFIG.maxSeedSpeedUnit = config.Bind(DARK_FOG_CONFIG, "Seed speed modifier units", "M",
                new ConfigDescription("Unit to use for Seed speed",
                new AcceptableValueList<string>("M", "AU", "LY")));
            Dark_Fog_CONFIG.maxRelaySpeed = config.Bind(DARK_FOG_CONFIG, "Relay speed (Vanilla is 1000 M/s)", 1000.0,
                new ConfigDescription("Max relay speed.",
                new AcceptableValueRange<double>(0.01, 10000.0), null));
            Dark_Fog_CONFIG.maxRelaySpeedUnit = config.Bind(DARK_FOG_CONFIG, "Relay speed modifier units", "M",
                new ConfigDescription("Unit to use for Relay speed",
                new AcceptableValueList<string>("M", "AU", "LY")));
            Dark_Fog_CONFIG.maxCarrierSpeed = config.Bind(DARK_FOG_CONFIG, "Carrier speed (Vanilla is 1800 M/s)", 1800.0,
                new ConfigDescription("Max carrier speed.",
                new AcceptableValueRange<double>(0.01, 10000.0), null));
            Dark_Fog_CONFIG.maxCarrierSpeedUnit = config.Bind(DARK_FOG_CONFIG, "Carrier speed modifier units", "M",
                new ConfigDescription("Unit to use for Carrier speed",
                new AcceptableValueList<string>("M", "AU", "LY")));


            //////////////////////////////////
            // DEBUG CONFIG                 //
            //////////////////////////////////
            //DSP_Speed_and_Consumption_Tweaks_Plugin.Log.LogInfo("1");
            Debug_CONFIG.DEBUG = config.Bind(DEBUG_CONFIG, "Debug messages :", false,
                new ConfigDescription("Enable debug messages",
                new AcceptableValueList<bool>(true, false), null));

        }
    }
}
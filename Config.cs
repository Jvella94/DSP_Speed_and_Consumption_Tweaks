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
        private static readonly string LOGISTIC_DRONE_CONFIG = "Logistic Drones Configuration";
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
            public static ConfigEntry<float> CruiseMaxAccelerationRate;
            public static ConfigEntry<float> CruiseAccelerationEnergyCost;

        }

        public static class Mecha_WARP_CONFIG
        {
            // warp
            public static ConfigEntry<float> maxWarpSpeed;
            public static ConfigEntry<string> maxWarpSpeedUnit;
            public static ConfigEntry<float> warpKeepingPowerPerSpeed;
            public static ConfigEntry<float> warpStartPowerPerSpeed;

        }

            
            
            

            //// ship settings
            //public static ConfigEntry<double> ShipMaxCruiseSpeed;
            //public static ConfigEntry<double> ShipMaxWarpSpeed;
            //public static ConfigEntry<double> ShipEneregyCost;


        public static class Logistic_DRONE_CONFIG
        {
            //// drone settings
            public static ConfigEntry<double> DroneMaxSpeed;
            public static ConfigEntry<double> DroneEnergyPerMeter;
            public static ConfigEntry<double> DroneEnergyTakeOff;
        }
            

        //public static class Utility
        //{
        //    public static ConfigEntry<bool> DisableMod;
        //    public static ConfigEntry<bool> UninstallMod;
        //}
        internal static void Init(ConfigFile config)
        {
            /////////////////////////////////
            // ICARUS Cruise Configuration //
            /////////////////////////////////

            Mecha_CRUISE_CONFIG.MaxCruiseSpeed = config.Bind(MECHA_CRUISE_CONFIG, "Maximum absolute cruise speed", 2000.0f,
                new ConfigDescription("Changes the base max cruise speed for Icarus. (Vanilla is 2000.0 m/s)\nIn game :\n40000 m is 1AU\n60 AU is 1 light year (2400000 m)",
                new AcceptableValueRange<float>(0.0f, 2400000.0f), Array.Empty<object>()));

            Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit = config.Bind(MECHA_CRUISE_CONFIG, "Max cruise speed modifier units", "Meters",
                new ConfigDescription("Unit to use for MaxCruiseSpeed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRate = config.Bind(MECHA_CRUISE_CONFIG, "Absolute acceleration rate in m/s^2", 20.0f,
                new ConfigDescription("Max cruise acceleration rate for Icarus. (Vanilla is 20 m/s^2)",
                    new AcceptableValueRange<float>(0.0f, 10000.0f), Array.Empty<object>()));

            Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCost = config.Bind(MECHA_CRUISE_CONFIG, "Energy used to maintain cruise acceleration", 24000.0f,
                new ConfigDescription("Cruise Acceleration Energy Cost for Icarus. (Vanilla is 24000 )" +
                "\nThis value is the base consumption per second at an acceleration of 20 m/s^2." +
                "\nThe faster you accelerate the bigger the consumption" +
                "\nWhile testing I had set the max speed to 100000 m/s and max acceleration to 1000" +
                "\nI depleted Icarus at Level 50 core energy infinit research in seconds",
                    new AcceptableValueRange<float>(0.0f, 240000.0f), Array.Empty<object>()));


            /////////////////////////////////
            // ICARUS Warp Configuration //
            /////////////////////////////////

            Mecha_WARP_CONFIG.maxWarpSpeed = config.Bind(MECHA_WARP_CONFIG, "Maximum absolute warp speed", 480000.0f,
                new ConfigDescription("Base max warp speed of Icarus. (Vanilla is 480000)",
                new AcceptableValueRange<float>(0.0f, 5000000.0f), new { }));

            Mecha_WARP_CONFIG.maxWarpSpeedUnit = config.Bind(MECHA_WARP_CONFIG, "Warp speed modifier units", "Meters",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Mecha_WARP_CONFIG.warpStartPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp start power per speed", 400f,
                new ConfigDescription("Warp start power per speed for Icarus. (Vanilla is 400)",
                    new AcceptableValueRange<float>(0.0f, 50000.0f), Array.Empty<object>()));


            Mecha_WARP_CONFIG.warpKeepingPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp keeping power per speed", 80f,
                new ConfigDescription("Warp keeping power per speed for Icarus. (Vanilla is 80)",
                    new AcceptableValueRange<float>(0.0f,800f), Array.Empty<object>()));


            ///////////////////////////////////
            // Logistic Drones Configuration //
            ///////////////////////////////////

            Logistic_DRONE_CONFIG.DroneMaxSpeed = config.Bind(LOGISTIC_DRONE_CONFIG, "Maximum absolute speed", 8.0,
                new ConfigDescription("Base max speed of Logistic Drones. (Vanilla is 8)",
                new AcceptableValueRange<double>(0.0, 1000.0), Array.Empty<object>()));

            Logistic_DRONE_CONFIG.DroneEnergyPerMeter = config.Bind(LOGISTIC_DRONE_CONFIG, "Energy consumption per meter", 20000.0,
                new ConfigDescription("Energy consumption per meter for Logistic Drones. (Vanilla is 20000)",
                new AcceptableValueRange<double>(0.0, 1000000.0), Array.Empty<object>()));

            Logistic_DRONE_CONFIG.DroneEnergyTakeOff = config.Bind(LOGISTIC_DRONE_CONFIG, "Energy consumption on take off", 800000.0,
                new ConfigDescription("Energy consumption on take off for Logistic Drones. (Vanilla is 800000)",
                new AcceptableValueRange<double>(0.0, 10000000.0), Array.Empty<object>()));
        }
    }
}
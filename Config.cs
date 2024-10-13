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
        private static readonly string LOGISTIC_SHIP_CONFIG = "Logistic Ships Configuration";
        private static readonly string LOGISTIC_DRONE_CONFIG = "Logistic Drones Configuration";
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
            public static ConfigEntry<double> maxShipNearSpeed;
            public static ConfigEntry<double> maxShipAtmoSpeed; 
            public static ConfigEntry<double> maxShipTaxiSpeed; 
       
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

            Mecha_CRUISE_CONFIG.MaxCruiseSpeed = config.Bind(MECHA_CRUISE_CONFIG, "Maximum absolute cruise speed (Vanilla is 2000.0 M/s)", 2000.0,
                new ConfigDescription("Changes the base max cruise speed for Icarus.\nIn game :\n40000 m is 1AU\n60 AU is 1 light year (2400000 m)",
                new AcceptableValueRange<double>(0.0, 2400000.0), Array.Empty<object>()));

            Mecha_CRUISE_CONFIG.MaxCruiseSpeedUnit = config.Bind(MECHA_CRUISE_CONFIG, "Max cruise speed modifier units", "Meters",
                new ConfigDescription("Unit to use for MaxCruiseSpeed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Mecha_CRUISE_CONFIG.CruiseMaxAccelerationRate = config.Bind(MECHA_CRUISE_CONFIG, "Acceleration rate in m/s^2 (Vanilla is 20 M/s^2)", 20.0,
                new ConfigDescription("Max cruise acceleration rate for Icarus.",
                    new AcceptableValueRange<double>(0.0, 10000.0), Array.Empty<object>()));

            Mecha_CRUISE_CONFIG.CruiseAccelerationEnergyCost = config.Bind(MECHA_CRUISE_CONFIG, "Energy used to maintain cruise acceleration (Vanilla is 24000 )", 24000.0,
                new ConfigDescription("Cruise Acceleration Energy Cost for Icarus." +
                "\nThis value is the base consumption per second at an acceleration of 20 m/s^2." +
                "\nThe faster you accelerate the bigger the consumption" +
                "\nWhile testing I had set the max speed to 100000 m/s and max acceleration to 1000" +
                "\nI depleted Icarus at Level 50 core energy infinit research in seconds",
                    new AcceptableValueRange<double>(0.0, 240000.0), Array.Empty<object>()));


            /////////////////////////////////
            // ICARUS Warp Configuration //
            /////////////////////////////////

            Mecha_WARP_CONFIG.maxWarpSpeed = config.Bind(MECHA_WARP_CONFIG, "Maximum absolute warp speed (Vanilla is 480000 M/s)", 480000.0,
                new ConfigDescription("Base max warp speed of Icarus.",
                new AcceptableValueRange<double>(0.0, 5000000.0), new { }));

            Mecha_WARP_CONFIG.maxWarpSpeedUnit = config.Bind(MECHA_WARP_CONFIG, "Warp speed modifier units", "Meters",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Mecha_WARP_CONFIG.warpStartPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp start power per speed (Vanilla is 400)", 400.0,
                new ConfigDescription("Warp start power per speed for Icarus.",
                    new AcceptableValueRange<double>(0.0, 50000.0), Array.Empty<object>()));

            Mecha_WARP_CONFIG.warpKeepingPowerPerSpeed = config.Bind(MECHA_WARP_CONFIG, "Warp keeping power per speed (Vanilla is 80)", 80.0,
                new ConfigDescription("Warp keeping power per speed for Icarus. ",
                    new AcceptableValueRange<double>(0.0,800), Array.Empty<object>()));


            ///////////////////////////////////
            // Logistic Drones Configuration //
            ///////////////////////////////////

            Logistic_DRONE_CONFIG.DroneMaxSpeed = config.Bind(LOGISTIC_DRONE_CONFIG, "Maximum speed (Vanilla is 8 M/s)", 8.0,
                new ConfigDescription("Base max speed of Logistic Drones. ",
                new AcceptableValueRange<double>(0.0, 1000.0), Array.Empty<object>()));

            Logistic_DRONE_CONFIG.DroneMaxTaxiSpeed = config.Bind(LOGISTIC_DRONE_CONFIG, "Maximum taxi speed (Vanilla is sqrt(droneMaxSpeed / 8)).", 0.5,
                new ConfigDescription("max taxi speed of Logistic Drones. (The taxi speed is manoeuvering speed at the Station)\n"
                +"It was just too unnatural to see the Drones or Ships docking at ligth speed and it removed a lot of the game's aesthics."
                +"I made it be 1 By Default Since it's the result for the base DroneMaxSpeed of 8.",
                new AcceptableValueRange<double>(0.0, 1000.0), Array.Empty<object>()));

            Logistic_DRONE_CONFIG.DroneEnergyPerMeter = config.Bind(LOGISTIC_DRONE_CONFIG, "Energy consumption per meter (Vanilla is 20000)", 20000.0,
                new ConfigDescription("Energy consumption per meter for Logistic Drones. ",
                new AcceptableValueRange<double>(0.0, 1000000.0), Array.Empty<object>()));

            Logistic_DRONE_CONFIG.DroneEnergyTakeOff = config.Bind(LOGISTIC_DRONE_CONFIG, "Energy consumption on take off (Vanilla is 800000)", 800000.0,
                new ConfigDescription("Energy consumption on take off for Logistic Drones. ",
                new AcceptableValueRange<double>(0.0, 10000000.0), Array.Empty<object>()));


            //////////////////////////////////
            // Logistic Ships Configuration //
            //////////////////////////////////
            
            Logistic_SHIP_CONFIG.maxShipNearSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Maximum near planete speed (Vanilla is 400000 M/s)", 400.0,
                new ConfigDescription("max near planete speed of Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 1000.0), new { }));

            Logistic_SHIP_CONFIG.maxShipAtmoSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Maximum atmospheric speed (Vanilla is 400000 M/s)", 40.0,
                new ConfigDescription("max atmospheric speed of Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 100.0), new { }));

            Logistic_SHIP_CONFIG.maxShipTaxiSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Maximum taxi speed (Vanilla is 400000 M/s)", 0.5,
                new ConfigDescription("max taxi speed of Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 10.0), new { }));

            Logistic_SHIP_CONFIG.ShipMaxWarpSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Maximum warp speed (Vanilla is 400000 M/s)", 400000.0,
                new ConfigDescription("Base max warp speed of Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 4000000.0), new { }));

            Logistic_SHIP_CONFIG.ShipMaxWarpSpeedUnits = config.Bind(MECHA_WARP_CONFIG, "Warp speed modifier units", "M",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Logistic_SHIP_CONFIG.ShipMaxCruiseSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Maximum cruise speed (Vanilla is 400 M/s)", 400.0,
                new ConfigDescription("Warp start power per speed for Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 10000.0), new { }));

            Logistic_SHIP_CONFIG.ShipMaxCruiseSpeedUnits = config.Bind(MECHA_WARP_CONFIG, "Warp speed modifier units", "M",
                new ConfigDescription("Unit to use for warp speed",
                new AcceptableValueList<string>("M", "AU", "LY")));

            Logistic_SHIP_CONFIG.ShipEneregyCostPerMeter = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption per Meter (Vanilla is 30 )", 30.0,
                new ConfigDescription("Energy consumption per meter of travel for Logistic Ships. ",
                new AcceptableValueRange<double>(0.0, 60.0), new { }));

            Logistic_SHIP_CONFIG.ShipEnergyCostForMaxSpeed = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption For max speed (Vanilla is 200000)", 200000.0,
                new ConfigDescription("Energy consumption for atteining max speed for Logistic Ships.\n"
                +"This value is multiplied by the max cruise speed of the ship durring the trip (max speed value capped at 3000 for the calculation).\n",
                new AcceptableValueRange<double>(0.0, 400000.0), new { }));

            Logistic_SHIP_CONFIG.ShipEnergyCostTakeOff = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption on take off", 6000000.0,
                new ConfigDescription("Energy consumption on take off for Logistic Ships. (Vanilla is 6000000)",
                new AcceptableValueRange<double>(0.0, 12000000.0), new { }));

            Logistic_SHIP_CONFIG.ShipEnergyCostPerWarp = config.Bind(LOGISTIC_SHIP_CONFIG, "Energy consumption per warp", 100000000.0,
                new ConfigDescription("Energy consumption per warp for Logistic Ships. (Vanilla is 100000000)",
                new AcceptableValueRange<double>(0.0, 200000000.0), new { }));


        }
    }
}
using System.Collections.Generic;
using GTA.Math;

namespace RealLifeAccessMod
{
    public enum AccessLevel
    {
        Public = 0,
        Vip = 1,
        Security = 2,
        Operations = 3,
        Classified = 4
    }

    public class VehicleEntry
    {
        public string Label;
        public string Model;      // spawn do addon (se existir)
        public string Fallback;   // vanilla se addon não existir
        public string Category;
        public AccessLevel Access;
    }

    public class AccessZone
    {
        public string Name;
        public Vector3 Position;
        public float Radius;
        public AccessLevel Required;
        public string Description;
        public bool Restricted; // true = área normalmente bloqueada
    }

    public class BuildingEntry
    {
        public string Name;
        public Vector3 Entrance;
        public Vector3 Interior;
        public AccessLevel Required;
        public string Hint;
    }

    public static class Config
    {
        public static AccessLevel PlayerAccess = AccessLevel.Public;

        public static readonly List<VehicleEntry> Vehicles = new List<VehicleEntry>
        {
            new VehicleEntry { Label = "Porsche 911 GT3", Model = "911gt3", Fallback = "comet2", Category = "Esportivos", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "Ferrari F8 Tributo", Model = "f8trib", Fallback = "entityxf", Category = "Esportivos", Access = AccessLevel.Security },
            new VehicleEntry { Label = "Lamborghini Huracán", Model = "huracan", Fallback = "tempesta", Category = "Esportivos", Access = AccessLevel.Security },
            new VehicleEntry { Label = "McLaren 720S", Model = "720s", Fallback = "t20", Category = "Esportivos", Access = AccessLevel.Operations },
            new VehicleEntry { Label = "Nissan GT-R R35", Model = "gtr", Fallback = "elegy2", Category = "Esportivos", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "BMW M4 Competition", Model = "rmodm4", Fallback = "cypher", Category = "Esportivos", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "Audi R8 V10", Model = "r8v10", Fallback = "ninef", Category = "Esportivos", Access = AccessLevel.Security },
            new VehicleEntry { Label = "Mercedes-AMG GT", Model = "amggt", Fallback = "schafter3", Category = "Esportivos", Access = AccessLevel.Security },

            new VehicleEntry { Label = "Range Rover Sport", Model = "rrsport", Fallback = "baller2", Category = "SUV", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "Mercedes G-Class", Model = "g65", Fallback = "dubsta2", Category = "SUV", Access = AccessLevel.Security },
            new VehicleEntry { Label = "BMW X6 M", Model = "x6m", Fallback = "rebla", Category = "SUV", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "Toyota Land Cruiser", Model = "lc200", Fallback = "seminole2", Category = "SUV", Access = AccessLevel.Public },

            new VehicleEntry { Label = "Toyota Corolla", Model = "corolla", Fallback = "asea", Category = "Sedãs", Access = AccessLevel.Public },
            new VehicleEntry { Label = "Honda Civic Type R", Model = "fk8", Fallback = "blista", Category = "Sedãs", Access = AccessLevel.Public },
            new VehicleEntry { Label = "Tesla Model S", Model = "models", Fallback = "raiden", Category = "Elétricos", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "Tesla Model X", Model = "modelx", Fallback = "voltic2", Category = "Elétricos", Access = AccessLevel.Vip },

            new VehicleEntry { Label = "Ford Mustang GT", Model = "mgt", Fallback = "dominator3", Category = "Muscle", Access = AccessLevel.Public },
            new VehicleEntry { Label = "Chevrolet Camaro ZL1", Model = "zl1", Fallback = "gauntlet4", Category = "Muscle", Access = AccessLevel.Vip },
            new VehicleEntry { Label = "Dodge Charger Hellcat", Model = "hellcat", Fallback = "dukes", Category = "Muscle", Access = AccessLevel.Vip },

            new VehicleEntry { Label = "Blindado SWAT", Model = "bearcat", Fallback = "riot", Category = "Restritos", Access = AccessLevel.Operations },
            new VehicleEntry { Label = "Insurgent Blindado", Model = "insurgent2", Fallback = "insurgent2", Category = "Restritos", Access = AccessLevel.Operations },
            new VehicleEntry { Label = "Buzzard Tático", Model = "buzzard2", Fallback = "buzzard2", Category = "Aéreos", Access = AccessLevel.Operations },
            new VehicleEntry { Label = "Helicóptero Executivo", Model = "swift2", Fallback = "swift2", Category = "Aéreos", Access = AccessLevel.Security },
            new VehicleEntry { Label = "Kuruma Blindado Classificado", Model = "kuruma2", Fallback = "kuruma2", Category = "Classificado", Access = AccessLevel.Classified },
        };

        public static readonly string[] Categories =
        {
            "Esportivos", "SUV", "Sedãs", "Elétricos", "Muscle", "Restritos", "Aéreos", "Classificado"
        };

        // Zonas restritas / liberadas
        public static readonly List<AccessZone> Zones = new List<AccessZone>
        {
            new AccessZone {
                Name = "Heliporto Downtown",
                Position = new Vector3(-736.15f, -1457.89f, 5.00f),
                Radius = 40f,
                Required = AccessLevel.Vip,
                Description = "Acesso VIP a helipontos civis",
                Restricted = true
            },
            new AccessZone {
                Name = "Telhado Maze Bank",
                Position = new Vector3(-75.01f, -818.83f, 326.18f),
                Radius = 25f,
                Required = AccessLevel.Vip,
                Description = "Mirante / heliponto do Maze Bank",
                Restricted = true
            },
            new AccessZone {
                Name = "Base Militar Fort Zancudo",
                Position = new Vector3(-2360.0f, 3249.0f, 92.9f),
                Radius = 180f,
                Required = AccessLevel.Operations,
                Description = "Área militar restrita",
                Restricted = true
            },
            new AccessZone {
                Name = "Hangar Zancudo",
                Position = new Vector3(-1850.0f, 3140.0f, 32.96f),
                Radius = 50f,
                Required = AccessLevel.Operations,
                Description = "Hangar de operações",
                Restricted = true
            },
            new AccessZone {
                Name = "FIB Building",
                Position = new Vector3(136.0f, -761.0f, 45.75f),
                Radius = 30f,
                Required = AccessLevel.Security,
                Description = "Prédio FIB — segurança",
                Restricted = true
            },
            new AccessZone {
                Name = "IAA Office",
                Position = new Vector3(117.0f, -621.0f, 206.0f),
                Radius = 25f,
                Required = AccessLevel.Security,
                Description = "Escritórios IAA",
                Restricted = true
            },
            new AccessZone {
                Name = "Secret Room Vault",
                Position = new Vector3(215.0f, -999.0f, -98.0f),
                Radius = 20f,
                Required = AccessLevel.Classified,
                Description = "Sala secreta classificada",
                Restricted = true
            },
        };

        // Edifícios com entrada/interior
        public static readonly List<BuildingEntry> Buildings = new List<BuildingEntry>
        {
            new BuildingEntry {
                Name = "Escritório Executivo",
                Entrance = new Vector3(-1581.0f, -558.0f, 34.95f),
                Interior = new Vector3(-141.29f, -620.97f, 168.82f),
                Required = AccessLevel.Vip,
                Hint = "Cartão VIP necessário"
            },
            new BuildingEntry {
                Name = "Garagem Privada",
                Entrance = new Vector3(-72.0f, -814.0f, 243.38f),
                Interior = new Vector3(228.0f, -986.0f, -99.0f),
                Required = AccessLevel.Security,
                Hint = "Chave de segurança"
            },
            new BuildingEntry {
                Name = "Laboratório Subterrâneo",
                Entrance = new Vector3(1391.0f, 3606.0f, 38.94f),
                Interior = new Vector3(997.0f, -3200.7f, -36.39f),
                Required = AccessLevel.Operations,
                Hint = "Credencial de operações"
            },
            new BuildingEntry {
                Name = "Secret Room",
                Entrance = new Vector3(127.0f, -1297.0f, 29.27f),
                Interior = new Vector3(215.0f, -999.0f, -98.0f),
                Required = AccessLevel.Classified,
                Hint = "Complete a missão da secret room"
            },
            new BuildingEntry {
                Name = "Cobertura Eclipse Towers",
                Entrance = new Vector3(-777.0f, 313.0f, 85.70f),
                Interior = new Vector3(-786.87f, 315.74f, 187.91f),
                Required = AccessLevel.Vip,
                Hint = "Acesso VIP residencial"
            },
        };

        // Missão secret room
        public static readonly Vector3 MissionStart = new Vector3(127.0f, -1297.0f, 29.27f);
        public static readonly Vector3 MissionKeycard = new Vector3(127.32f, -768.63f, 242.15f); // FIB upper
        public static readonly Vector3 MissionHackPoint = new Vector3(136.0f, -761.0f, 242.15f);
        public static readonly Vector3 MissionVault = new Vector3(215.0f, -999.0f, -98.0f);
        public static readonly Vector3 MissionRewardSpawn = new Vector3(220.0f, -1000.0f, -99.0f);
    }
}

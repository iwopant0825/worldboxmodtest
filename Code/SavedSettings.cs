using System;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod
{
    [Serializable]
    public class SavedSettings
    {
        public string settingVersion = "0.6.5";
        public Dictionary<string, InputOption> inputOptions = new Dictionary<string, InputOption>
        {
            {"LevelRate", new InputOption{active = true, value = "1" }},
            {"levelLimit", new InputOption{active = false, value = "10" }},
            {"healthLimit", new InputOption{active = false, value = "999999" }},
            {"armorLimit", new InputOption{active = false, value = "5000" }},
            {"damageLimit", new InputOption{active = false, value = "999999" }},
            {"attack_speedLimit", new InputOption{active = false, value = "300" }},
            {"expRateOption", new InputOption{active = false, value = "0" }},
            {"WorldLawElectionsOption", new InputOption{active = false, value = "1" }},
            {"CRPG-expGapOption", new InputOption{active = false, value = "20" }},
            {"CRPG-kingExpGainOption", new InputOption{active = false, value = "20" }},
            {"CRPG-leaderExpGainOption", new InputOption{active = false, value = "10" }},
            {"CRPG-levelAgeIncreaseOption", new InputOption{active = false, value = "10" }},
            {"CRPG-VisibleDigitTitlesOption", new InputOption{active = true, value = "4" }},
            {"CRPG-LevelDMGSuppressionOption", new InputOption{active = true, value = "50" }},
            {"CRPG-InitialStartForLevelDMGSuppresionOption", new InputOption{active = true, value = "10" }},
            {"KingdomBaseCityLimitOption", new InputOption{active = false, value = "1" }},
            {"CRPG-AttributeEvolutionsLimitOption", new InputOption{active = false, value = "1" }}
        };
        public Dictionary<string, bool> boolOptions = new Dictionary<string, bool>
        {
            {$"{SK.human}Filter", true},
            {$"{SK.orc}Filter", true},
            {$"{SK.elf}Filter", true},
            {$"{SK.dwarf}Filter", true},
            {"SortKill", true},
            {"SortAge", true},
            {"SortDmg", true},
            {"SortLevel", true},
            {"SortTalent", false},
            {$"SSSFilter", false},
            {$"SSFilter", false},
            {$"SFilter", false},
            {$"AFilter", false},
            {$"BFilter", false},
            {$"CFilter", false},
            {$"DFilter", false},
            {$"EFilter", false},
            {$"FFilter", false},
            {"navalWarfareOption", false},
            {"imperialThinkingOption", false},
            {"CRPGOption", false},
            {"IgnoreWarriorLimitOption", false},
            {"IgnoreWarriorLimitOnlyForWarriorPowerOption", false},
            {"NoLavaWhenApplyingHeatOption", false},
            {"FasterWarPlotsOption", true}
        };
        public Dictionary<string, Dictionary<string, InputOption>> multipleInputOptions = new Dictionary<string, Dictionary<string, InputOption>>
        {
                {"CRPG-StatBoostOption", new Dictionary<string, InputOption>{
                    {S.damage, new InputOption{active = false, value = "1.0" }},
                    {S.health, new InputOption{active = false, value = "20.0" }},
                    {S.attack_speed, new InputOption{active = false, value = "1.0" }},
                    {S.armor, new InputOption{active = false, value = "0.0" }},
                }},
                {"CRPG-MoreStatBoostOption", new Dictionary<string, InputOption>{
                    {S.diplomacy, new InputOption{active = false, value = "0.0" }},
                    {S.stewardship, new InputOption{active = false, value = "0.0" }},
                    {S.intelligence, new InputOption{active = false, value = "0.0" }},
                    {S.warfare, new InputOption{active = false, value = "0.0" }},
                    {S.cities, new InputOption{active = false, value = "0.0" }}
                }},
                {"Level Filter", new Dictionary<string, InputOption>{
                    {"Min Level", new InputOption{active = false, value = "-1" }},
                    {"Max Level", new InputOption{active = false, value = "-1" }}
                }},
                {"Magnet Level Filter", new Dictionary<string, InputOption>{
                    {"Min Level", new InputOption{active = false, value = "-1" }},
                    {"Max Level", new InputOption{active = false, value = "-1" }}
                }}

        };
        public float titlesCheckTime = 0f;
        public Dictionary<string, CRPGTrait> crpgTraits = new Dictionary<string, CRPGTrait>
        {
            {"SSS", new CRPGTrait{
                expGain = 200,
                expGainHit = 50,
                expGainHurt = 10,
                birthRate = 0.01f,
                expGainKill = 45,
                EXPRequirement = -50f,
                talentLevelCap = 55,
                leaderBoardPriority = 9
            }},
            {"SS", new CRPGTrait{
                expGain = 150,
                expGainHit = 40,
                expGainHurt = 10,
                birthRate = 0.05f,
                expGainKill = 40,
                EXPRequirement = -30f,
                talentLevelCap = 50,
                leaderBoardPriority = 8
            }},
            {"S", new CRPGTrait{
                expGain = 120,
                expGainHit = 30,
                expGainHurt = 5,
                birthRate = 0.1f,
                expGainKill = 35,
                EXPRequirement = -30f,
                talentLevelCap = 45,
                leaderBoardPriority = 7
            }},
            {"A", new CRPGTrait{
                expGain = 70,
                expGainHit = 10,
                expGainHurt = 3,
                birthRate = 20f,
                expGainKill = 30,
                EXPRequirement = -20f,
                talentLevelCap = 40,
                leaderBoardPriority = 6
            }},
            {"B", new CRPGTrait{
                expGain = 50,
                expGainHit = 10,
                expGainHurt = 2,
                birthRate = 40f,
                expGainKill = 25,
                EXPRequirement = 0f,
                talentLevelCap = 30,
                leaderBoardPriority = 5
            }},
            {"C", new CRPGTrait{
                expGain = 40,
                expGainHit = 8,
                expGainHurt = 1,
                birthRate = 50f,
                expGainKill = 20,
                EXPRequirement = 5f,
                talentLevelCap = 25,
                leaderBoardPriority = 4
            }},
            {"D", new CRPGTrait{
                expGain = 20,
                expGainHit = 8,
                expGainHurt = 1,
                birthRate = 70f,
                expGainKill = 15,
                EXPRequirement = 10f,
                talentLevelCap = 20,
                leaderBoardPriority = 3
            }},
            {"E", new CRPGTrait{
                expGain = 15,
                expGainHit = 5,
                expGainHurt = 1,
                birthRate = 80f,
                expGainKill = 10,
                EXPRequirement = 15f,
                talentLevelCap = 15,
                leaderBoardPriority = 2
            }},
            {"F", new CRPGTrait{
                expGain = 10,
                expGainHit = 5,
                expGainHurt = 1,
                birthRate = 101f,
                expGainKill = 10,
                EXPRequirement = 20f,
                talentLevelCap = 10,
                leaderBoardPriority = 1
            }}
        };
        public Dictionary<string, AttributeTrait> crpgAttributes = new Dictionary<string, AttributeTrait>
        {
            {"FireAttribute1", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "fireAction1",
                damageReq = 1,
                ageReq = 1,
                levelReq = 1,
                killsReq = 1
                }
            },
            {"WaterAttribute1", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "waterAction1",
                damageReq = 1,
                ageReq = 1,
                levelReq = 1,
                killsReq = 1
                }
            },
            {"LightningAttribute1", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "lightningAction1",
                damageReq = 1,
                ageReq = 1,
                levelReq = 1,
                killsReq = 1
                }
            },
            {"EarthAttribute1", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "earthAction1",
                damageReq = 1,
                ageReq = 1,
                levelReq = 1,
                killsReq = 1
                }
            },

            {"FireAttribute2", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "fireAction2",
                damageReq = 20,
                ageReq = 18,
                levelReq = 10,
                killsReq = 20
                }
            },
            {"WaterAttribute2", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "waterAction2",
                damageReq = 20,
                ageReq = 18,
                levelReq = 10,
                killsReq = 20
                }
            },
            {"LightningAttribute2", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "lightningAction2",
                damageReq = 20,
                ageReq = 18,
                levelReq = 10,
                killsReq = 20
                }
            },
            {"EarthAttribute2", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "earthAction2",
                damageReq = 20,
                ageReq = 18,
                levelReq = 10,
                killsReq = 20
                }
            },

            {"FireAttribute3", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "fireAction3",
                damageReq = 40,
                ageReq = 18,
                levelReq = 20,
                killsReq = 50
                }
            },
            {"WaterAttribute3", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "waterAction3",
                damageReq = 40,
                ageReq = 18,
                levelReq = 20,
                killsReq = 50
                }
            },
            {"LightningAttribute3", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "lightningAction3",
                damageReq = 40,
                ageReq = 18,
                levelReq = 20,
                killsReq = 50
                }
            },
            {"EarthAttribute3", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "earthAction3",
                damageReq = 40,
                ageReq = 18,
                levelReq = 20,
                killsReq = 50
                }
            },

            {"FireAttribute4", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "fireAction4",
                damageReq = 60,
                ageReq = 18,
                levelReq = 30,
                killsReq = 100
                }
            },
            {"WaterAttribute4", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "waterAction4",
                damageReq = 60,
                ageReq = 18,
                levelReq = 30,
                killsReq = 100
                }
            },
            {"LightningAttribute4", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "lightningAction4",
                damageReq = 60,
                ageReq = 18,
                levelReq = 30,
                killsReq = 100
                }
            },
            {"EarthAttribute4", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "earthAction4",
                damageReq = 60,
                ageReq = 18,
                levelReq = 30,
                killsReq = 100
                }
            },

            {"FireAttribute5", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "fireAction5",
                damageReq = 60,
                ageReq = 18,
                levelReq = 40,
                killsReq = 150
                }
            },
            {"WaterAttribute5", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "waterAction5",
                damageReq = 60,
                ageReq = 18,
                levelReq = 40,
                killsReq = 150
                }
            },
            {"LightningAttribute5", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "lightningAction5",
                damageReq = 60,
                ageReq = 18,
                levelReq = 40,
                killsReq = 150
                }
            },
            {"EarthAttribute5", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "earthAction5",
                damageReq = 60,
                ageReq = 18,
                levelReq = 40,
                killsReq = 150
                }
            },

            {"FireAttribute6", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "fireAction6",
                damageReq = 60,
                ageReq = 18,
                levelReq = 50,
                killsReq = 200
                }
            },
            {"WaterAttribute6", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "waterAction6",
                damageReq = 60,
                ageReq = 18,
                levelReq = 50,
                killsReq = 200
                }
            },
            {"LightningAttribute6", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "lightningAction6",
                damageReq = 60,
                ageReq = 18,
                levelReq = 50,
                killsReq = 200
                }
            },
            {"EarthAttribute6", new AttributeTrait{
                attackChance = 0.3f,
                attackAction = "earthAction6",
                damageReq = 60,
                ageReq = 18,
                levelReq = 50,
                killsReq = 200
                }
            }
        };
        public List<Title> titles = new List<Title>();
        public Dictionary<string, Dictionary<string, string>> CRPGTitles = new Dictionary<string, Dictionary<string, string>>
        {
            {"1", new Dictionary<string, string>
            {
                {"0", "Nascent Circle"},
                {"1", "1st Circle"},
                {"2", "2nd Circle"},
                {"3", "3rd Circle"},
                {"4", "4th Circle"},
                {"5", "5th Circle"},
                {"6", "6th Circle"},
                {"7", "7th Circle"},
                {"8", "8th Circle"},
                {"9", "9th Circle"}
            }},
            {"2", new Dictionary<string, string>
            {
                {"0", "Acolyte"},
                {"1", "Novice Warrior"},
                {"2", "Intermediate Warrior"},
                {"3", "Advanced Warrior"},
                {"4", "Master Warrior"},
                {"5", "Grandmaster Warrior"},
                {"6", "Nascent Ascendant"},
                {"7", "Enlightened Ascendant"},
                {"8", "Martial Saint"},
                {"9", "Empyrean Paragon"}
            }},
            {"3", new Dictionary<string, string>
            {
                {"0", "Divine"},
                {"1", "Dingus"},
                {"2", "Supreme"},
                {"3", "Doge"},
                {"4", "El Gato"},
                {"5", "Nintendo"},
                {"6", "Mario & Luigi"},
                {"7", "Sakura Haruna"},
                {"8", "Bob The Builder"},
                {"9", "Goku"}
            }},
            {"4", new Dictionary<string, string>
            {
                {"0", "Harambe"},
                {"1", "DINGUS SUPREME"},
                {"2", "DINGUS ULTIMATE SUPREME"},
                {"3", "DINGUS GODLY SUPREME"},
                {"4", "DINGUS HEAVENLY SUPREME"},
                {"5", "DINGUS AMAZING SUPREME"},
                {"6", "DINGUS SUPREME SUPREME"},
                {"7", "DINGUS SLAYER SUPREME"},
                {"8", "DINGUS LOVER SUPREME"},
                {"9", "DINGUS SON SUPREME"}
            }}
        };
        public Dictionary<string, SavedItems> savedItems = new Dictionary<string, SavedItems>();
    }

    [Serializable]
    public class InputOption
    {
        public bool active = true;
        public string value;
    }

    [Serializable]
    public class Title
    {
        public string name;
        public Dictionary<string, bool> titleBools = new Dictionary<string, bool>();
        public Dictionary<string, int> titleStats = new Dictionary<string, int>();
        public List<string> titleTraits = new List<string>();
        public List<string> titleNames = new List<string>();
        public Dictionary<string, string> titleResults = new Dictionary<string, string>();
        public int currentNumber = 0;
        public int currentNumberName = 0;
        public int previousNumber = 0;
    }

    public class CRPGTrait
    {
        public int expGain = 0;
        public int expGainHit = 0;
        public int expGainHurt = 0;
        public float birthRate = 0f;
        public int expGainKill = 0;
        public float EXPRequirement = 0f;
        public int talentLevelCap = 0;
        public int leaderBoardPriority = 0;
    }

    [Serializable]
    public class AttributeTrait
    {
        public float attackChance = 0f;
        public string attackAction = "";
        public int damageReq = 1;
        public int ageReq = 1;
        public int levelReq = 1;
        public int killsReq = 1;
    }

    [Serializable]
    public class SavedItems
    {
        public Dictionary<string, ItemOption> itemAssets = new Dictionary<string, ItemOption>();
        public Dictionary<string, List<ItemAsset>> itemModifiers = new Dictionary<string, List<ItemAsset>>();
        public Dictionary<string, string> itemNames = new Dictionary<string, string>();
    }
}

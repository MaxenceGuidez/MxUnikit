using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MxUnikit.Log
{
    [CreateAssetMenu(fileName = "MxLogConfig", menuName = "MxUnikit/Log/Log Config")]
    public class MxLogConfig : ScriptableObject
    {
        [Serializable]
        public struct CategoryData
        {
            public string CategoryId;
            public string Color;
            public bool IsEnabled;
            public List<string> Keywords;
        }

        private const string ResourcePath = "MxLogConfig";

        private static MxLogConfig _instance;

        public static MxLogConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<MxLogConfig>(ResourcePath);
                }
                return _instance;
            }
        }

        public bool IsEnabled = true;
        public bool LogStackTraceForExceptions = true;

        public List<CategoryData> Categories = new List<CategoryData>
        {
            new CategoryData
            {
                CategoryId = "Default",
                Color = "#FFFFFF",
                IsEnabled = true,
                Keywords = new List<string>()
            },
            new CategoryData
            {
                CategoryId = "API",
                Color = "#00CED1",
                IsEnabled = true,
                Keywords = new List<string> { "api", "backend", "http", "request" }
            },
            new CategoryData
            {
                CategoryId = "Audio",
                Color = "#9370DB",
                IsEnabled = true,
                Keywords = new List<string> { "audio", "music", "sound" }
            },
            new CategoryData
            {
                CategoryId = "Debug",
                Color = "#FF6347",
                IsEnabled = true,
                Keywords = new List<string> { "debug" }
            },
            new CategoryData
            {
                CategoryId = "Event",
                Color = "#DA70D6",
                IsEnabled = true,
                Keywords = new List<string> { "event", "eventbus", "publish", "subscribe" }
            },
            new CategoryData
            {
                CategoryId = "Firebase",
                Color = "#FFA500",
                IsEnabled = true,
                Keywords = new List<string> { "firebase" }
            },
            new CategoryData
            {
                CategoryId = "Game",
                Color = "#32CD32",
                IsEnabled = true,
                Keywords = new List<string> { "game" }
            },
            new CategoryData
            {
                CategoryId = "Inputs",
                Color = "#1E90FF",
                IsEnabled = true,
                Keywords = new List<string> { "device", "input", "inputs", "joystick", "key", "keyboard", "mouse", "touch" }
            },
            new CategoryData
            {
                CategoryId = "Inventory",
                Color = "#FFD700",
                IsEnabled = true,
                Keywords = new List<string> { "accessory", "accessories", "inventory", "item", "items", "pickup", "pickups", "supply", "supplies" }
            },
            new CategoryData
            {
                CategoryId = "Network",
                Color = "#4A90D9",
                IsEnabled = true,
                Keywords = new List<string> { "client", "host", "lobby", "mirror", "network", "relay", "server", "session" }
            },
            new CategoryData
            {
                CategoryId = "Player",
                Color = "#ADFF2F",
                IsEnabled = true,
                Keywords = new List<string> { "character", "player" }
            },
            new CategoryData
            {
                CategoryId = "UI",
                Color = "#FF69B4",
                IsEnabled = true,
                Keywords = new List<string> { "dialog", "hud", "menu", "overlay", "ui" }
            }
        };

        private Dictionary<MxLogCategory, CategoryData> _categoryDataCache;
        private Dictionary<string, MxLogCategory> _keywordToCategoryCache;

        public bool IsCategoryEnabled(MxLogCategory category)
        {
            BuildCacheIfNeeded();
            if (_categoryDataCache.TryGetValue(category, out var data))
            {
                return data.IsEnabled;
            }
            return true;
        }

        public string GetCategoryColor(MxLogCategory category)
        {
            BuildCacheIfNeeded();
            if (_categoryDataCache.TryGetValue(category, out var data))
            {
                return data.Color;
            }
            return "#FFFFFF";
        }

        public MxLogCategory DetectCategoryFromKeyword(string keyword)
        {
            BuildCacheIfNeeded();
            if (_keywordToCategoryCache.TryGetValue(keyword.ToLowerInvariant(), out var category))
            {
                return category;
            }
            return MxLogCategory.Default;
        }

        public void RegisterCustomCategory(MxLogCategory category, string color = null, List<string> keywords = null)
        {
            if (Categories.Any(c => c.CategoryId == category.Id))
            {
                return;
            }

            Categories.Add(new CategoryData
            {
                CategoryId = category.Id,
                Color = color ?? "#FFFFFF",
                IsEnabled = true,
                Keywords = keywords ?? new List<string>()
            });

            ClearCache();
        }

        private void BuildCacheIfNeeded()
        {
            if (_categoryDataCache != null) return;

            _categoryDataCache = new Dictionary<MxLogCategory, CategoryData>();
            _keywordToCategoryCache = new Dictionary<string, MxLogCategory>();

            foreach (CategoryData data in Categories)
            {
                MxLogCategory category = GetBuiltInCategory(data.CategoryId) ?? MxLogCategoryRegistry.Register(data.CategoryId);

                _categoryDataCache[category] = data;

                foreach (string keyword in data.Keywords)
                {
                    _keywordToCategoryCache[keyword.ToLowerInvariant()] = category;
                }
            }
        }

        private static MxLogCategory GetBuiltInCategory(string id)
        {
            return id switch
            {
                "Default" => MxLogCategory.Default,
                "API" => MxLogCategory.API,
                "Audio" => MxLogCategory.Audio,
                "Debug" => MxLogCategory.Debug,
                "Event" => MxLogCategory.Event,
                "Firebase" => MxLogCategory.Firebase,
                "Game" => MxLogCategory.Game,
                "Inputs" => MxLogCategory.Inputs,
                "Inventory" => MxLogCategory.Inventory,
                "Network" => MxLogCategory.Network,
                "Player" => MxLogCategory.Player,
                "Session" => MxLogCategory.Session,
                "UI" => MxLogCategory.UI,
                _ => null
            };
        }

        public void ClearCache()
        {
            _categoryDataCache = null;
            _keywordToCategoryCache = null;
        }

        private void OnValidate()
        {
            ClearCache();
        }
    }
}

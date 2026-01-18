using System;
using System.Collections.Generic;
using UnityEngine;

namespace MxUnikit.Log
{
    [CreateAssetMenu(fileName = "MxLogConfig", menuName = "MxUnikit/Log/Log Config")]
    public class MxLogConfig : ScriptableObject
    {
        private const string RESOURCE_PATH = "MxLogConfig";

        private static MxLogConfig _instance;

        public static MxLogConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<MxLogConfig>(RESOURCE_PATH);
                }
                return _instance;
            }
        }

        public bool IsEnabled = true;
        public bool LogStackTraceForExceptions = true;

        public List<MxLogCategory> DisabledCategories = new List<MxLogCategory>();

        public List<CategoryColor> CategoryColors = new List<CategoryColor>
        {
            new CategoryColor { category = MxLogCategory.API, color = "#00CED1" },
            new CategoryColor { category = MxLogCategory.Audio, color = "#9370DB" },
            new CategoryColor { category = MxLogCategory.Debug, color = "#FF6347" },
            new CategoryColor { category = MxLogCategory.Event, color = "#DA70D6" },
            new CategoryColor { category = MxLogCategory.Firebase, color = "#FFA500" },
            new CategoryColor { category = MxLogCategory.Game, color = "#32CD32" },
            new CategoryColor { category = MxLogCategory.Inputs, color = "#1E90FF" },
            new CategoryColor { category = MxLogCategory.Inventory, color = "#FFD700" },
            new CategoryColor { category = MxLogCategory.Network, color = "#4A90D9" },
            new CategoryColor { category = MxLogCategory.Player, color = "#ADFF2F" },
            new CategoryColor { category = MxLogCategory.UI, color = "#FF69B4" }
        };

        public List<CategoryKeyword> CategoryKeywords = new List<CategoryKeyword>
        {
            // API
            new CategoryKeyword { keyword = "api", category = MxLogCategory.API },
            new CategoryKeyword { keyword = "backend", category = MxLogCategory.API },
            new CategoryKeyword { keyword = "http", category = MxLogCategory.API },
            new CategoryKeyword { keyword = "request", category = MxLogCategory.API },

            // Audio
            new CategoryKeyword { keyword = "audio", category = MxLogCategory.Audio },
            new CategoryKeyword { keyword = "music", category = MxLogCategory.Audio },
            new CategoryKeyword { keyword = "sound", category = MxLogCategory.Audio },

            // Debug
            new CategoryKeyword { keyword = "debug", category = MxLogCategory.Debug },

            // Event
            new CategoryKeyword { keyword = "event", category = MxLogCategory.Event },
            new CategoryKeyword { keyword = "eventbus", category = MxLogCategory.Event },
            new CategoryKeyword { keyword = "publish", category = MxLogCategory.Event },
            new CategoryKeyword { keyword = "subscribe", category = MxLogCategory.Event },

            // Firebase
            new CategoryKeyword { keyword = "firebase", category = MxLogCategory.Firebase },

            // Game
            new CategoryKeyword { keyword = "game", category = MxLogCategory.Game },

            // Inputs
            new CategoryKeyword { keyword = "device", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "input", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "inputs", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "joystick", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "key", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "keyboard", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "mouse", category = MxLogCategory.Inputs },
            new CategoryKeyword { keyword = "touch", category = MxLogCategory.Inputs },

            // Inventory
            new CategoryKeyword { keyword = "accessory", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "accessories", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "inventory", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "item", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "items", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "pickup", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "pickups", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "supply", category = MxLogCategory.Inventory },
            new CategoryKeyword { keyword = "supplies", category = MxLogCategory.Inventory },

            // Network
            new CategoryKeyword { keyword = "client", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "host", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "lobby", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "mirror", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "network", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "relay", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "server", category = MxLogCategory.Network },
            new CategoryKeyword { keyword = "session", category = MxLogCategory.Network },

            // Player
            new CategoryKeyword { keyword = "character", category = MxLogCategory.Player },
            new CategoryKeyword { keyword = "player", category = MxLogCategory.Player },

            // UI
            new CategoryKeyword { keyword = "dialog", category = MxLogCategory.UI },
            new CategoryKeyword { keyword = "hud", category = MxLogCategory.UI },
            new CategoryKeyword { keyword = "menu", category = MxLogCategory.UI },
            new CategoryKeyword { keyword = "overlay", category = MxLogCategory.UI },
            new CategoryKeyword { keyword = "ui", category = MxLogCategory.UI }
        };

        private HashSet<MxLogCategory> _disabledCategoriesCache;
        private Dictionary<MxLogCategory, string> _colorsCache;
        private Dictionary<string, MxLogCategory> _keywordsCache;

        public HashSet<MxLogCategory> DisabledCategoriesSet
        {
            get
            {
                _disabledCategoriesCache ??= new HashSet<MxLogCategory>(DisabledCategories);
                return _disabledCategoriesCache;
            }
        }

        public Dictionary<MxLogCategory, string> ColorsDict
        {
            get
            {
                if (_colorsCache != null) return _colorsCache;

                _colorsCache = new Dictionary<MxLogCategory, string>();
                foreach (CategoryColor cc in CategoryColors)
                {
                    _colorsCache[cc.category] = cc.color;
                }
                return _colorsCache;
            }
        }

        public Dictionary<string, MxLogCategory> KeywordsDict
        {
            get
            {
                if (_keywordsCache != null) return _keywordsCache;

                _keywordsCache = new Dictionary<string, MxLogCategory>();
                foreach (CategoryKeyword ck in CategoryKeywords)
                {
                    _keywordsCache[ck.keyword.ToLowerInvariant()] = ck.category;
                }
                return _keywordsCache;
            }
        }

        public void ClearCache()
        {
            _disabledCategoriesCache = null;
            _colorsCache = null;
            _keywordsCache = null;
        }

        private void OnValidate()
        {
            ClearCache();
        }

        [Serializable]
        public class CategoryColor
        {
            public MxLogCategory category;
            public string color = "#FFFFFF";
        }

        [Serializable]
        public class CategoryKeyword
        {
            public string keyword;
            public MxLogCategory category;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MxUnikit.Log
{
    [CreateAssetMenu(fileName = "MxLogConfig", menuName = "MxUnikit/Log/Log Config")]
    public class MxLogConfig : ScriptableObject
    {
        [Serializable]
        public struct CategoryColor
        {
            public MxLogCategory Category;
            public string Color;
        }

        [Serializable]
        public struct CategoryKeyword
        {
            public MxLogCategory Category;
            public string Keyword;
        }

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
            new CategoryColor { Category = MxLogCategory.API, Color = "#00CED1" },
            new CategoryColor { Category = MxLogCategory.Audio, Color = "#9370DB" },
            new CategoryColor { Category = MxLogCategory.Debug, Color = "#FF6347" },
            new CategoryColor { Category = MxLogCategory.Event, Color = "#DA70D6" },
            new CategoryColor { Category = MxLogCategory.Firebase, Color = "#FFA500" },
            new CategoryColor { Category = MxLogCategory.Game, Color = "#32CD32" },
            new CategoryColor { Category = MxLogCategory.Inputs, Color = "#1E90FF" },
            new CategoryColor { Category = MxLogCategory.Inventory, Color = "#FFD700" },
            new CategoryColor { Category = MxLogCategory.Network, Color = "#4A90D9" },
            new CategoryColor { Category = MxLogCategory.Player, Color = "#ADFF2F" },
            new CategoryColor { Category = MxLogCategory.UI, Color = "#FF69B4" }
        };

        public List<CategoryKeyword> CategoryKeywords = new List<CategoryKeyword>
        {
            // API
            new CategoryKeyword { Keyword = "api", Category = MxLogCategory.API },
            new CategoryKeyword { Keyword = "backend", Category = MxLogCategory.API },
            new CategoryKeyword { Keyword = "http", Category = MxLogCategory.API },
            new CategoryKeyword { Keyword = "request", Category = MxLogCategory.API },

            // Audio
            new CategoryKeyword { Keyword = "audio", Category = MxLogCategory.Audio },
            new CategoryKeyword { Keyword = "music", Category = MxLogCategory.Audio },
            new CategoryKeyword { Keyword = "sound", Category = MxLogCategory.Audio },

            // Debug
            new CategoryKeyword { Keyword = "debug", Category = MxLogCategory.Debug },

            // Event
            new CategoryKeyword { Keyword = "event", Category = MxLogCategory.Event },
            new CategoryKeyword { Keyword = "eventbus", Category = MxLogCategory.Event },
            new CategoryKeyword { Keyword = "publish", Category = MxLogCategory.Event },
            new CategoryKeyword { Keyword = "subscribe", Category = MxLogCategory.Event },

            // Firebase
            new CategoryKeyword { Keyword = "firebase", Category = MxLogCategory.Firebase },

            // Game
            new CategoryKeyword { Keyword = "game", Category = MxLogCategory.Game },

            // Inputs
            new CategoryKeyword { Keyword = "device", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "input", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "inputs", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "joystick", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "key", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "keyboard", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "mouse", Category = MxLogCategory.Inputs },
            new CategoryKeyword { Keyword = "touch", Category = MxLogCategory.Inputs },

            // Inventory
            new CategoryKeyword { Keyword = "accessory", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "accessories", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "inventory", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "item", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "items", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "pickup", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "pickups", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "supply", Category = MxLogCategory.Inventory },
            new CategoryKeyword { Keyword = "supplies", Category = MxLogCategory.Inventory },

            // Network
            new CategoryKeyword { Keyword = "client", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "host", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "lobby", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "mirror", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "network", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "relay", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "server", Category = MxLogCategory.Network },
            new CategoryKeyword { Keyword = "session", Category = MxLogCategory.Network },

            // Player
            new CategoryKeyword { Keyword = "character", Category = MxLogCategory.Player },
            new CategoryKeyword { Keyword = "player", Category = MxLogCategory.Player },

            // UI
            new CategoryKeyword { Keyword = "dialog", Category = MxLogCategory.UI },
            new CategoryKeyword { Keyword = "hud", Category = MxLogCategory.UI },
            new CategoryKeyword { Keyword = "menu", Category = MxLogCategory.UI },
            new CategoryKeyword { Keyword = "overlay", Category = MxLogCategory.UI },
            new CategoryKeyword { Keyword = "ui", Category = MxLogCategory.UI }
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
                    _colorsCache[cc.Category] = cc.Color;
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
                    _keywordsCache[ck.Keyword.ToLowerInvariant()] = ck.Category;
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
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hospital_System
{
    public enum ItemCategory { Furniture, Medical, Cleaning, Food }

    internal class StorageItem
    {
        public string       Name     { get; set; }
        public ItemCategory Category { get; set; }
        public int          Quantity { get; set; }

        public StorageItem() { }

        public StorageItem(string name, ItemCategory category, int quantity)
        {
            Name     = name;
            Category = category;
            Quantity = quantity;
        }
    }

    internal class InventoryManager
    {
        public List<StorageItem> Items { get; set; } = new List<StorageItem>();

        private static readonly string _filePath = DataStore.Inventory;
        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters    = { new JsonStringEnumConverter() }
        };

        // Default seed items (only used when no saved file exists)
        private static readonly List<StorageItem> _defaults = new List<StorageItem>
        {
            new StorageItem("Blankets",     ItemCategory.Furniture, 25),
            new StorageItem("Pillows",      ItemCategory.Furniture, 15),
            new StorageItem("BedSheets",    ItemCategory.Furniture, 40),
            new StorageItem("Injections",   ItemCategory.Medical,    4),
            new StorageItem("Bandages",     ItemCategory.Medical,  100),
            new StorageItem("Gloves",       ItemCategory.Medical,   80),
            new StorageItem("OxygenMasks",  ItemCategory.Medical,   10),
            new StorageItem("Thermometers", ItemCategory.Medical,    2),
            new StorageItem("Antiseptics",  ItemCategory.Cleaning,  20),
            new StorageItem("Soap",         ItemCategory.Cleaning,  30),
            new StorageItem("Water",        ItemCategory.Food,     200),
            new StorageItem("Juice",        ItemCategory.Food,      50),
        };

        public InventoryManager()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var loaded = JsonSerializer.Deserialize<List<StorageItem>>(File.ReadAllText(_filePath), _jsonOpts);
                    Items = loaded ?? new List<StorageItem>(_defaults);
                }
                catch { Items = new List<StorageItem>(_defaults); }
            }
            else
            {
                Items = new List<StorageItem>(_defaults);
                Save(); // create the file from defaults on first run
            }
        }

        public void Save()
        {
            try { File.WriteAllText(_filePath, JsonSerializer.Serialize(Items, _jsonOpts)); }
            catch { /* non-critical */ }
        }

        public StorageItem SearchItem(string name)
        {
            return Items.Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

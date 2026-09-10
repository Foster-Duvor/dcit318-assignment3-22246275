using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventorySystem
{
    // ==========================================================
    // (a) Marker interface for all inventory items
    // ==========================================================
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // ==========================================================
    // (b) Electronic product
    // ==========================================================
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"[Electronic] ID: {Id} | {Name} | Qty: {Quantity} | Brand: {Brand} | Warranty: {WarrantyMonths} mo";
        }
    }

    // ==========================================================
    // (c) Grocery product
    // ==========================================================
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"[Grocery] ID: {Id} | {Name} | Qty: {Quantity} | Expires: {ExpiryDate:d}";
        }
    }

    // ==========================================================
    // (e) Custom exceptions
    // ==========================================================
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // ==========================================================
    // (d) Generic repository, constrained to IInventoryItem
    // ==========================================================
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"An item with ID {item.Id} already exists.");
            }
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
            {
                throw new ItemNotFoundException($"No item found with ID {id}.");
            }
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Cannot remove — no item found with ID {id}.");
            }
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return _items.Values.ToList();
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException($"Quantity cannot be negative (received {newQuantity}).");
            }
            var item = GetItemById(id); // throws ItemNotFoundException if missing
            item.Quantity = newQuantity;
        }
    }

    // ==========================================================
    // (f) Manages both electronic and grocery inventories
    // ==========================================================
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 40, "Sony", 6));

            _groceries.AddItem(new GroceryItem(101, "Rice (5kg)", 50, DateTime.Now.AddMonths(8)));
            _groceries.AddItem(new GroceryItem(102, "Milk (1L)", 30, DateTime.Now.AddDays(14)));
            _groceries.AddItem(new GroceryItem(103, "Canned Beans", 60, DateTime.Now.AddMonths(12)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine("  " + item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased. '{item.Name}' now has quantity {item.Quantity}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void AddElectronic(ElectronicItem item)
        {
            try
            {
                _electronics.AddItem(item);
                Console.WriteLine($"Added electronic item '{item.Name}'.");
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void AddGrocery(GroceryItem item)
        {
            try
            {
                _groceries.AddItem(item);
                Console.WriteLine($"Added grocery item '{item.Name}'.");
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateQuantity<T>(InventoryRepository<T> repo, int id, int newQuantity) where T : IInventoryItem
        {
            try
            {
                repo.UpdateQuantity(id, newQuantity);
                Console.WriteLine($"Quantity for ID {id} updated to {newQuantity}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // v. Demonstrates each custom exception being thrown and caught gracefully
        public void RunErrorDemo()
        {
            Console.WriteLine("\n--- Demo: Adding a duplicate item ---");
            AddElectronic(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 12)); // ID 1 already exists

            Console.WriteLine("\n--- Demo: Removing a non-existent item ---");
            RemoveItemById(_groceries, 999); // does not exist

            Console.WriteLine("\n--- Demo: Updating with an invalid (negative) quantity ---");
            UpdateQuantity(_electronics, 2, -5); // negative quantity
        }
    }

    // ==========================================================
    // Interactive console driver
    // ==========================================================
    public class Program
    {
        private static readonly WareHouseManager Manager = new WareHouseManager();

        public static void Main(string[] args)
        {
            Console.WriteLine("=== Warehouse Inventory Management System ===");
            Manager.SeedData();

            Console.WriteLine("\n=== Grocery Items ===");
            Manager.PrintAllItems(Manager.Groceries);

            Console.WriteLine("\n=== Electronic Items ===");
            Manager.PrintAllItems(Manager.Electronics);

            // Built-in demo of the required error scenarios
            Manager.RunErrorDemo();

            bool keepGoing = true;
            while (keepGoing)
            {
                ShowMenuAndHandleChoice();
                keepGoing = AskToContinue();
            }

            Console.WriteLine("\nThank you for using the Warehouse Inventory System. Goodbye!");
        }

        private static void ShowMenuAndHandleChoice()
        {
            Console.WriteLine("\n--- Menu ---");
            Console.WriteLine("1. View all electronics");
            Console.WriteLine("2. View all groceries");
            Console.WriteLine("3. Add an electronic item");
            Console.WriteLine("4. Add a grocery item");
            Console.WriteLine("5. Remove an item by ID");
            Console.WriteLine("6. Update an item's quantity");
            Console.WriteLine("7. Increase stock for an item");
            Console.Write("Enter choice (1-7): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n=== Electronic Items ===");
                    Manager.PrintAllItems(Manager.Electronics);
                    break;

                case "2":
                    Console.WriteLine("\n=== Grocery Items ===");
                    Manager.PrintAllItems(Manager.Groceries);
                    break;

                case "3":
                    {
                        int id = ReadInt("Enter new electronic ID: ");
                        Console.Write("Enter name: ");
                        string name = Console.ReadLine();
                        int qty = ReadInt("Enter quantity: ");
                        Console.Write("Enter brand: ");
                        string brand = Console.ReadLine();
                        int warranty = ReadInt("Enter warranty (months): ");
                        Manager.AddElectronic(new ElectronicItem(id, name, qty, brand, warranty));
                        break;
                    }

                case "4":
                    {
                        int id = ReadInt("Enter new grocery ID: ");
                        Console.Write("Enter name: ");
                        string name = Console.ReadLine();
                        int qty = ReadInt("Enter quantity: ");
                        int daysToExpiry = ReadInt("Enter days until expiry: ");
                        Manager.AddGrocery(new GroceryItem(id, name, qty, DateTime.Now.AddDays(daysToExpiry)));
                        break;
                    }

                case "5":
                    {
                        string type = ReadItemType();
                        int id = ReadInt("Enter ID to remove: ");
                        if (type == "e") Manager.RemoveItemById(Manager.Electronics, id);
                        else Manager.RemoveItemById(Manager.Groceries, id);
                        break;
                    }

                case "6":
                    {
                        string type = ReadItemType();
                        int id = ReadInt("Enter ID to update: ");
                        int newQty = ReadInt("Enter new quantity: ");
                        if (type == "e") Manager.UpdateQuantity(Manager.Electronics, id, newQty);
                        else Manager.UpdateQuantity(Manager.Groceries, id, newQty);
                        break;
                    }

                case "7":
                    {
                        string type = ReadItemType();
                        int id = ReadInt("Enter ID to restock: ");
                        int amount = ReadInt("Enter quantity to add: ");
                        if (type == "e") Manager.IncreaseStock(Manager.Electronics, id, amount);
                        else Manager.IncreaseStock(Manager.Groceries, id, amount);
                        break;
                    }

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private static string ReadItemType()
        {
            while (true)
            {
                Console.Write("Item type — (E)lectronic or (G)rocery? ");
                string input = Console.ReadLine()?.Trim().ToLower();
                if (input == "e" || input == "g") return input;
                Console.WriteLine("Invalid input. Please type 'E' or 'G'.");
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }
                Console.WriteLine("Invalid input. Please enter a whole number.");
            }
        }

        private static bool AskToContinue()
        {
            while (true)
            {
                Console.Write("\nWould you like to (C)ontinue or (Q)uit? ");
                string input = Console.ReadLine()?.Trim().ToLower();

                if (input == "c" || input == "continue") return true;
                if (input == "q" || input == "quit") return false;

                Console.WriteLine("Invalid input. Please type 'C' to continue or 'Q' to quit.");
            }
        }
    }
}

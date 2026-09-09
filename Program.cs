using System;
using System.Collections.Generic;

// ==========================================
// RECORD: Transaction
// ==========================================
public record Transaction(
    int Id,
    DateTime Date,
    decimal Amount,
    string Category
);

// ==========================================
// INTERFACE: ITransactionProcessor
// ==========================================
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// ==========================================
// MOBILE MONEY PROCESSOR
// ==========================================
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Mobile Money: Processing GHC {transaction.Amount:F2} for {transaction.Category}."
        );
    }
}

// ==========================================
// BANK TRANSFER PROCESSOR
// ==========================================
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Bank Transfer: Processing GHC {transaction.Amount:F2} for {transaction.Category}."
        );
    }
}

// ==========================================
// CRYPTO WALLET PROCESSOR
// ==========================================
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Crypto Wallet: Processing GHC {transaction.Amount:F2} for {transaction.Category}."
        );
    }
}

// ==========================================
// BASE ACCOUNT CLASS
// ==========================================
public class Account
{
    public string AccountNumber { get; }

    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;

        Console.WriteLine(
            $"Transaction applied. New balance: GHC {Balance:F2}"
        );
    }
}

// ==========================================
// SEALED SAVINGS ACCOUNT CLASS
// ==========================================
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;

            Console.WriteLine(
                $"Transaction successful. Updated balance: GHC {Balance:F2}"
            );
        }
    }
}

// ==========================================
// FINANCE APPLICATION
// ==========================================
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    private readonly SavingsAccount _account;

    public FinanceApp()
    {
        _account = new SavingsAccount("ACC-1001", 1000m);
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("==============================================");
            Console.WriteLine("          FINANCE MANAGEMENT SYSTEM");
            Console.WriteLine("==============================================");

            Console.WriteLine($"\nAccount Number: {_account.AccountNumber}");
            Console.WriteLine($"Current Balance: GHC {_account.Balance:F2}");

            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Create and process a transaction");
            Console.WriteLine("2. View all transactions");
            Console.WriteLine("3. View account balance");
            Console.WriteLine("4. Quit application");

            Console.Write("\nEnter your choice: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    CreateTransaction();
                    break;

                case "2":
                    DisplayTransactions();
                    break;

                case "3":
                    DisplayBalance();
                    break;

                case "4":
                    Console.WriteLine("\nThank you for using the Finance Management System.");
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please select 1, 2, 3, or 4.");
                    Pause();
                    break;
            }
        }
    }

    // ==========================================
    // CREATE TRANSACTION
    // ==========================================
    private void CreateTransaction()
    {
        Console.Clear();

        Console.WriteLine("==============================================");
        Console.WriteLine("             NEW TRANSACTION");
        Console.WriteLine("==============================================");

        int id;

        while (true)
        {
            Console.Write("\nEnter Transaction ID: ");

            if (int.TryParse(Console.ReadLine(), out id) && id > 0)
            {
                break;
            }

            Console.WriteLine("Invalid ID. Please enter a positive number.");
        }

        decimal amount;

        while (true)
        {
            Console.Write("Enter Transaction Amount (GHC): ");

            if (decimal.TryParse(Console.ReadLine(), out amount) && amount > 0)
            {
                break;
            }

            Console.WriteLine("Invalid amount. Please enter a positive number.");
        }

        Console.Write("Enter Transaction Category: ");
        string category = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("Category cannot be empty.");
            Pause();
            return;
        }

        Transaction transaction = new Transaction(
            id,
            DateTime.Now,
            amount,
            category
        );

        Console.WriteLine("\nSelect payment processor:");
        Console.WriteLine("1. Mobile Money");
        Console.WriteLine("2. Bank Transfer");
        Console.WriteLine("3. Crypto Wallet");

        Console.Write("\nEnter processor: ");
        string processorChoice = Console.ReadLine() ?? "";

        ITransactionProcessor processor;

        switch (processorChoice)
        {
            case "1":
                processor = new MobileMoneyProcessor();
                break;

            case "2":
                processor = new BankTransferProcessor();
                break;

            case "3":
                processor = new CryptoWalletProcessor();
                break;

            default:
                Console.WriteLine("\nInvalid processor selected.");
                Pause();
                return;
        }

        Console.WriteLine("\n----------------------------------------------");
        Console.WriteLine("TRANSACTION PROCESSING");
        Console.WriteLine("----------------------------------------------");

        processor.Process(transaction);

        Console.WriteLine("\nApplying transaction to account...");

        _account.ApplyTransaction(transaction);

        _transactions.Add(transaction);

        Console.WriteLine("\nTransaction added successfully.");

        Pause();
    }

    // ==========================================
    // DISPLAY ALL TRANSACTIONS
    // ==========================================
    private void DisplayTransactions()
    {
        Console.Clear();

        Console.WriteLine("==============================================");
        Console.WriteLine("             ALL TRANSACTIONS");
        Console.WriteLine("==============================================");

        if (_transactions.Count == 0)
        {
            Console.WriteLine("\nNo transactions have been recorded.");
        }
        else
        {
            foreach (Transaction transaction in _transactions)
            {
                Console.WriteLine($"\nTransaction ID: {transaction.Id}");
                Console.WriteLine($"Date: {transaction.Date}");
                Console.WriteLine($"Amount: GHC {transaction.Amount:F2}");
                Console.WriteLine($"Category: {transaction.Category}");
                Console.WriteLine("----------------------------------------------");
            }
        }

        Pause();
    }

    // ==========================================
    // DISPLAY BALANCE
    // ==========================================
    private void DisplayBalance()
    {
        Console.Clear();

        Console.WriteLine("==============================================");
        Console.WriteLine("              ACCOUNT BALANCE");
        Console.WriteLine("==============================================");

        Console.WriteLine($"\nAccount Number: {_account.AccountNumber}");
        Console.WriteLine($"Current Balance: GHC {_account.Balance:F2}");

        Pause();
    }

    // ==========================================
    // PAUSE
    // ==========================================
    private void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}

// ==========================================
// MAIN PROGRAM
// ==========================================
class Program
{
    static void Main(string[] args)
    {
        FinanceApp app = new FinanceApp();

        app.Run();
    }
}
namespace WtLab2.Models;

public abstract class Account : BaseEntity
{
    public string AccountNumber { get; }
    public string OwnerName { get; set; }
    public decimal Balance { get; private set; }
    protected decimal InterestRate;

    protected Account(int id, string accountNumber, string ownerName, decimal initialBalance)
        : base(id)
    {
        if (initialBalance < 0)
            throw new ArgumentException("Начальный баланс не может быть отрицательным.");

        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = initialBalance;
    }

    protected void AdjustBalance(decimal delta)
    {
        Balance += delta;
    }

    public virtual bool Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Ошибка: сумма пополнения должна быть положительной.");
            return false;
        }

        Balance += amount;
        return true;
    }

    public virtual bool Deposit(decimal amount, string comment)
    {
        bool success = Deposit(amount);
        if (success)
            Console.WriteLine($"Комментарий к операции: {comment}");
        return success;
    }

    public virtual bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Ошибка: сумма снятия должна быть положительной.");
            return false;
        }

        if (amount > Balance)
        {
            Console.WriteLine($"Ошибка: недостаточно средств. Доступно: {Balance:N2}");
            return false;
        }

        Balance -= amount;
        return true;
    }

    public abstract decimal CalculateInterest();

    public override void PrintInfo()
    {
        Console.WriteLine($"{FormatHeader()}");
        Console.WriteLine($"  Счёт {AccountNumber} | Владелец: {OwnerName} | Баланс: {Balance:N2} BYN");
    }
}
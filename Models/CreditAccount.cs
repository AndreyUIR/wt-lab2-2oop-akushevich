namespace WtLab2.Models;

public class CreditAccount : Account
{
    public decimal CreditLimit { get; }

    public CreditAccount(int id, string accountNumber, string ownerName,
                          decimal initialBalance, decimal creditLimit)
        : base(id, accountNumber, ownerName, initialBalance)
    {
        CreditLimit = creditLimit;
        InterestRate = 0.19m;
    }

    public override bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Ошибка: сумма снятия должна быть положительной.");
            return false;
        }

        if (Balance - amount < -CreditLimit)
        {
            Console.WriteLine($"Ошибка: превышен кредитный лимит {CreditLimit:N2} BYN.");
            return false;
        }

        AdjustBalance(-amount);
        return true;
    }

    public override decimal CalculateInterest()
    {
        return Balance < 0 ? Math.Abs(Balance) * InterestRate / 12 : 0;
    }

    public override void PrintInfo()
    {
        base.PrintInfo();
        Console.WriteLine($"  Тип: Кредитный счёт | Лимит: {CreditLimit:N2} BYN | " +
                           $"Доступно: {(CreditLimit + Balance):N2} BYN");
    }
}
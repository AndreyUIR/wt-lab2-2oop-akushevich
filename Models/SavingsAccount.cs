namespace WtLab2.Models;

public class SavingsAccount : Account
{
    public SavingsAccount(int id, string accountNumber, string ownerName, decimal initialBalance)
        : base(id, accountNumber, ownerName, initialBalance)
    {
        InterestRate = 0.03m;
    }

    public override decimal CalculateInterest()
    {
        return Balance * InterestRate / 12;
    }

    public override void PrintInfo()
    {
        base.PrintInfo();
        Console.WriteLine($"  Тип: Накопительный счёт | Ставка: {InterestRate:P0} годовых | " +
                           $"Начислено за месяц: {CalculateInterest():N2} BYN");
    }
}
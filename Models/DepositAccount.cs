namespace WtLab2.Models;

public class DepositAccount : Account
{
    public int TermMonths { get; }
    public DateTime MaturityDate { get; }

    public DepositAccount(int id, string accountNumber, string ownerName,
                           decimal initialBalance, int termMonths)
        : base(id, accountNumber, ownerName, initialBalance)
    {
        TermMonths = termMonths;
        MaturityDate = DateTime.Now.AddMonths(termMonths);
        InterestRate = 0.08m;
    }

    public override bool Withdraw(decimal amount)
    {
        if (DateTime.Now < MaturityDate)
        {
            Console.WriteLine($"Ошибка: срочный вклад заблокирован до {MaturityDate:dd.MM.yyyy}.");
            return false;
        }

        return base.Withdraw(amount);
    }

    public override decimal CalculateInterest()
    {
        return Balance * InterestRate / 12;
    }

    public override void PrintInfo()
    {
        base.PrintInfo();
        Console.WriteLine($"  Тип: Срочный вклад | Срок: {TermMonths} мес. | " +
                           $"Погашение: {MaturityDate:dd.MM.yyyy} | Ставка: {InterestRate:P0}");
    }
}
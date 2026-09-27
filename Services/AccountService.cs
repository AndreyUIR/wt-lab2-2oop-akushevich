using WtLab2.Interfaces;
using WtLab2.Models;

namespace WtLab2.Services;

public class AccountService : IItemService<Account>
{
    private readonly List<Account> _accounts = new();

    public void Add(Account item)
    {
        if (_accounts.Any(a => a.Id == item.Id))
            throw new InvalidOperationException($"Счёт с Id={item.Id} уже существует.");

        _accounts.Add(item);
    }

    public Account? GetById(int id)
        => _accounts.FirstOrDefault(a => a.Id == id);

    public IEnumerable<Account> GetAll()
        => _accounts;

    public bool Remove(int id)
    {
        var account = GetById(id);
        if (account is null) return false;
        return _accounts.Remove(account);
    }

    public bool Update(Account item)
    {
        var index = _accounts.FindIndex(a => a.Id == item.Id);
        if (index == -1) return false;

        _accounts[index] = item;
        return true;
    }

    public Account? GetByAccountNumber(string accountNumber)
        => _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
}
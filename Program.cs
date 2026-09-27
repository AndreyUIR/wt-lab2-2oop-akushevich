using WtLab2.Models;
using WtLab2.Interfaces;
using WtLab2.Services;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=========================================");
Console.WriteLine("   БАНКОВСКАЯ СИСТЕМА — консольный клиент  ");
Console.WriteLine("   Лабораторная работа №2. Классы, ООП     ");
Console.WriteLine("=========================================");
Console.WriteLine();

bool exit = false;
while (!exit)
{
    Console.WriteLine("Выберите операцию:");
    Console.WriteLine("1 — Создать и показать счета (Блок 1)");
    Console.WriteLine("2 — Работа с коллекциями и LINQ (Блок 2)");
    Console.WriteLine("3 — Асинхронные операции (Блок 3)");
    Console.WriteLine("0 — Выход");
    Console.Write("> ");

    var choice = Console.ReadLine();

    switch (choice)
    {
                         case "1":
        {
            Account[] accounts =
            {
                new SavingsAccount(1, "BY01SAV0001", "Иванов Иван Иванович", 2000m),
                new CreditAccount(2, "BY02CRD0002", "Петрова Анна Сергеевна", 0m, 1000m),
                new DepositAccount(3, "BY03DEP0003", "Сидоров Пётр Петрович", 5000m, 6)
            };

            Console.WriteLine("--- Полиморфный вызов PrintInfo() ---");
            foreach (var acc in accounts)
            {
                acc.PrintInfo();
                Console.WriteLine();
            }

            Console.WriteLine("--- Проверка инкапсуляции и валидации ---");
            accounts[0].Deposit(300m, "Зарплата за сентябрь");
            accounts[1].Withdraw(500m);
            accounts[2].Withdraw(1000m);

            Console.WriteLine();
            foreach (var acc in accounts)
                acc.PrintInfo();

            Console.WriteLine();
            Console.WriteLine("--- Работа через IItemService<Account> ---");

            IItemService<Account> service = new AccountService();
            foreach (var acc in accounts)
                service.Add(acc);

            Console.WriteLine("Все счета через интерфейсную ссылку:");
            foreach (var acc in service.GetAll())
                acc.PrintInfo();

            var found = service.GetById(2);
            Console.WriteLine(found is not null
                ? $"Найден счёт #{found.Id}: {found.AccountNumber}"
                : "Счёт не найден");

            bool removed = service.Remove(99);
            Console.WriteLine($"Попытка удалить несуществующий счёт: {(removed ? "успех" : "не найден")}");
            break;
        }
                               case "2":
        {
            List<Account> accounts = CreateTestAccounts();

            Console.WriteLine($"Всего счетов в системе: {accounts.Count}");
            Console.WriteLine();

            foreach (var acc in accounts)
            {
                acc.PrintInfo();
                Console.WriteLine();
            }

            Dictionary<string, Account> accountsByNumber = accounts
                .ToDictionary(a => a.AccountNumber, a => a);

            Console.WriteLine($"Проиндексировано счетов: {accountsByNumber.Count}");
            Console.WriteLine();

            string searchKey1 = "BY03CRD0003";
            if (accountsByNumber.TryGetValue(searchKey1, out Account? found1))
            {
                Console.WriteLine($"Найден счёт по номеру {searchKey1}:");
                found1.PrintInfo();
            }

            Console.WriteLine();

            string searchKey2 = "BY99UNKNOWN";
            if (!accountsByNumber.TryGetValue(searchKey2, out Account? found2))
            {
                Console.WriteLine($"Счёт с номером {searchKey2} не найден — обработано без исключения.");
            }

            Console.WriteLine();
            Console.WriteLine("=== Фильтрация (Where): счета с балансом выше 1000 ===");
            var richAccounts = accounts.Where(a => a.Balance > 1000m);
            foreach (var a in richAccounts)
                Console.WriteLine($"  {a.AccountNumber}: {a.Balance:N2} BYN");

            Console.WriteLine();
            Console.WriteLine("=== Сортировка (OrderByDescending): по убыванию баланса ===");
            var sorted = accounts.OrderByDescending(a => a.Balance);
            foreach (var a in sorted)
                Console.WriteLine($"  {a.AccountNumber}: {a.Balance:N2} BYN");

            Console.WriteLine();
            Console.WriteLine("=== Проекция (Select): номер счёта и владелец ===");
            var projection = accounts.Select(a => new { a.AccountNumber, a.OwnerName });
            foreach (var p in projection)
                Console.WriteLine($"  {p.AccountNumber} — {p.OwnerName}");

            Console.WriteLine();
            Console.WriteLine("=== Агрегатные операции ===");
            Console.WriteLine($"  Количество счетов:     {accounts.Count()}");
            Console.WriteLine($"  Сумма всех балансов:   {accounts.Sum(a => a.Balance):N2} BYN");
            Console.WriteLine($"  Средний баланс:        {accounts.Average(a => a.Balance):N2} BYN");
            Console.WriteLine($"  Минимальный баланс:    {accounts.Min(a => a.Balance):N2} BYN");
            Console.WriteLine($"  Максимальный баланс:   {accounts.Max(a => a.Balance):N2} BYN");

            Console.WriteLine();
            Console.WriteLine("=== Группировка (GroupBy) по типу счёта ===");
            var grouped = accounts
                .GroupBy(a => a.GetType().Name)
                .Select(g => new
                {
                    TypeName = g.Key,
                    Count = g.Count(),
                    TotalBalance = g.Sum(a => a.Balance),
                    AverageBalance = g.Average(a => a.Balance)
                })
                .OrderByDescending(g => g.TotalBalance);

            foreach (var group in grouped)
            {
                Console.WriteLine($"  {group.TypeName,-15} | счетов: {group.Count} | " +
                                   $"сумма: {group.TotalBalance,10:N2} | среднее: {group.AverageBalance,10:N2}");
            }

            break;
        }
                               case "3":
        {
            Console.WriteLine("--- Уровень 1: одиночная асинхронная операция ---");
            await ProcessPaymentAsync("BY01SAV0001", 250m);
            Console.WriteLine("Main продолжает выполнение только после завершения await.");

            Console.WriteLine();
            Console.WriteLine("--- Уровень 2 ---");

            var accounts1 = await GetAccountsFromDatabaseAsync("client-001");
            Console.WriteLine($"Получено счетов: {accounts1.Count}");

            Console.WriteLine();

            var accounts2 = await GetAccountsFromDatabaseAsync("UNKNOWN");
            Console.WriteLine($"Получено счетов: {accounts2.Count}");

            // --- Уровень 3: Task.WhenAll ---
            Console.WriteLine();
            Console.WriteLine("--- Уровень 3: Task.WhenAll ---");
            await RunParallelInterestCalculationAsync();

            // --- Уровень 3: CancellationToken ---
            Console.WriteLine();
            Console.WriteLine("--- Уровень 3: CancellationToken ---");

            var accountsForStatements = CreateTestAccounts();
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(1500));

            try
            {
                await GenerateMonthlyStatementsAsync(accountsForStatements, cts.Token);
                Console.WriteLine("Все выписки сформированы без отмены.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Операция была отменена по истечении времени ожидания.");
            }

            break;
        }
        case "0":
            exit = true;
            break;
        default:
            Console.WriteLine("Неизвестный пункт меню, попробуйте снова.");
            break;
    }

    Console.WriteLine();
}

Console.WriteLine("Работа завершена.");
static List<Account> CreateTestAccounts() => new()
{
    new SavingsAccount(1, "BY01SAV0001", "Иванов И. И.", 2000m),
    new SavingsAccount(2, "BY02SAV0002", "Козлова М. А.", 750m),
    new CreditAccount(3, "BY03CRD0003", "Петрова А. С.", 200m, 1000m),
    new CreditAccount(4, "BY04CRD0004", "Волков Д. Н.", 0m, 500m),
    new DepositAccount(5, "BY05DEP0005", "Сидоров П. П.", 5000m, 6),
    new DepositAccount(6, "BY06DEP0006", "Морозова Е. В.", 12000m, 12),
    new SavingsAccount(7, "BY07SAV0007", "Кузнецов А. И.", 3400m),
};
static async Task ProcessPaymentAsync(string accountNumber, decimal amount)
{
    Console.WriteLine($"Начата обработка платежа {amount:N2} BYN по счёту {accountNumber}...");
    await Task.Delay(1000);
    Console.WriteLine($"Платёж по счёту {accountNumber} обработан.");
}
static async Task<List<Account>> GetAccountsFromDatabaseAsync(string clientId)
{
    Console.WriteLine($"Подключение к базе данных для клиента {clientId}...");

    try
    {
        await Task.Delay(800);

        if (clientId == "UNKNOWN")
            throw new InvalidOperationException($"Клиент с Id={clientId} не найден в базе.");

        Console.WriteLine("Данные успешно получены из СУБД.");
        return CreateTestAccounts();
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Ошибка при получении данных: {ex.Message}");
        return new List<Account>();
    }
    finally
    {
        Console.WriteLine("Соединение с базой данных закрыто.");
    }
}
static async Task<decimal> CalculateInterestAsync(Account account)
{
    await Task.Delay(500);
    return account.CalculateInterest();
}

static async Task RunParallelInterestCalculationAsync()
{
    var accounts = CreateTestAccounts();

    Console.WriteLine($"Запущен параллельный расчёт процентов по {accounts.Count} счетам...");
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    var tasks = accounts.Select(a => CalculateInterestAsync(a)).ToList();
    decimal[] results = await Task.WhenAll(tasks);

    stopwatch.Stop();

    for (int i = 0; i < accounts.Count; i++)
        Console.WriteLine($"  {accounts[i].AccountNumber}: начислено {results[i]:N2} BYN");

    Console.WriteLine($"Расчёт завершён за {stopwatch.ElapsedMilliseconds} мс " +
                       $"(последовательно заняло бы ~{accounts.Count * 500} мс).");
}

static async Task GenerateMonthlyStatementsAsync(List<Account> accounts, CancellationToken token)
{
    foreach (var account in accounts)
    {
        token.ThrowIfCancellationRequested();

        Console.WriteLine($"Формируется выписка по счёту {account.AccountNumber}...");
        await Task.Delay(700, token);

        Console.WriteLine($"Выписка по {account.AccountNumber} готова.");
    }
}
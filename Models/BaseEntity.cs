namespace WtLab2.Models;

public abstract class BaseEntity
{
    public int Id { get; }
    public DateTime CreatedAt { get; protected set; }

    protected BaseEntity(int id)
    {
        Id = id;
        CreatedAt = DateTime.Now;
    }

    public abstract void PrintInfo();

    protected virtual string FormatHeader()
        => $"[#{Id}] создано {CreatedAt:dd.MM.yyyy HH:mm}";
}
namespace WtLab2.Interfaces;

public interface IItemService<T>
{
    void Add(T item);
    T? GetById(int id);
    IEnumerable<T> GetAll();
    bool Remove(int id);
    bool Update(T item);
}
using Command;

namespace Spacebattle;

public interface IQueue
{
    void Add(Hwdtech.ICommand cmd);  //добавляет объект в конец очереди
    ICommand Take();                 //извлекает объект из очереди
}

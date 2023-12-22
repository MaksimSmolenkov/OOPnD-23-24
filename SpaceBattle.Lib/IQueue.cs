using _ICommand;

namespace SpaceBattle;

public interface IQueue
{
    void Add(Hwdtech.ICommand cmd);  //добавляет объект в конец очереди
    ICommand Take();                 //извлекает объект из очереди
}
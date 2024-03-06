using Command;

namespace SpaceBattle;

public interface IQueue
{
    void Add(Hwdtech.ICommand cmd);  
    ICommand Take();                 
}

using System.Collections.Concurrent;
using Hwdtech;

namespace Spacebattle;
public class ServerThread
{
    private readonly BlockingCollection<ICommand> _q;
    private Action _behaviour;

    private readonly Thread _t;
    private bool _stop = false;

    public ServerThread(BlockingCollection<ICommand> q)
    {
        _q = q;

        _behaviour = () =>
        {
            var cmd = _q.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
            }
        };

        _t = new Thread(() =>
        {

            while (!_stop)
            {
                _behaviour();
            }
        });
    }

    public void Start()
    {
        _t.Start();
    }

    internal void Stop()
    {
        _stop = true;
    }

    internal void UpdateBehaviour(Action behaviour)
    {
        _behaviour = behaviour;
    }
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        if (obj.GetType() == typeof(Thread))
        {
            return _t == (Thread)obj;
        }

        return false;
    }

    public override int GetHashCode()
    {
        try
        {
            return base.GetHashCode();
        }
        catch (System.NotImplementedException e)
        {
            Console.WriteLine("Ошибка: {0}", e.Message);
            throw new System.NotImplementedException();
        }
    }

    public class HardStopCommand : ICommand
    {
        private readonly ServerThread _st;
        public HardStopCommand(ServerThread st)
        {
            _st = st;
        }

        public void Execute()
        {
            if (_st.Equals(Thread.CurrentThread))
            {
                _st.Stop();
            }

            else
            {
                throw new Exception("Wrong Thread");
            }
        }
    }
}
public class SoftStop : ICommand
{
    private readonly ServerThread _st;
    private readonly BlockingCollection<ICommand> _q;
    public Action action = () => { };

    public SoftStop(ServerThread thread, BlockingCollection<ICommand> queue)
    {
        _st = thread;
        _q = queue;
    }
    public void Execute()
    {
        if (_st.Equals(Thread.CurrentThread))
        {
            _st.UpdateBehaviour(() =>
            {
                if (_q.Count > 0)
                {
                    var cmd = _q.Take();
                    try
                    {
                        cmd.Execute();
                    }
                    catch (Exception e)
                    {
                        IoC.Resolve<ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
                    }
                }
                else
                {
                    _st.Stop();
                }
            });
        }
        else
        {
            throw new Exception("Wrong Thread");
        }
    }
}

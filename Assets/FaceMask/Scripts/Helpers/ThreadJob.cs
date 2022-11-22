using System.Threading;

public class ThreadedJob
{
    Thread thread = null;

    const int idlingSleepTime = 10;

    public delegate void ThreadFunctionDeledate();
    ThreadFunctionDeledate m_threadFunction;

    public bool IsDone
    {
        get;
        private set;
    }

    public bool IsRunning
    {
        get;
        private set;
    }

    public ThreadedJob(ThreadFunctionDeledate threadFunction)
    {
        m_threadFunction = threadFunction;
    }

    public void Start()
    {
        IsDone = false;

        if (thread == null)
        {
            thread = new Thread(Run);
            thread.Start();
        }

        IsRunning = true;
    }

    public void Wait()
    {
        if (thread != null)
            IsDone = true;
    }

    public void Abort()
    {
        if (thread != null)
        {
            IsDone = false;
            IsRunning = false;
            thread.Join();

            thread = null;
        }
    }

    void Run()
    {
        while (IsRunning)
        {
            if (!IsDone)
            {
                m_threadFunction();
                IsDone = true;
            }
            else
                Thread.Sleep(idlingSleepTime);
        }
    }
}
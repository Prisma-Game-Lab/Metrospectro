public class Lock
{
    private int _locks = 0;
    
    public bool IsLocked()
    {
        return _locks != 0;
    }
    public void AddLock()
    {
        ++_locks;
    } 
  
    public void RemoveLock()
    {
        --_locks;
    }


}

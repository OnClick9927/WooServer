namespace WS.Core.WebSockets;

public class BinaryArrayPool<T>
{
    public Dictionary<int, Queue<T[]>> map = new Dictionary<int, Queue<T[]>>();

    int GetSize(int size)
    {
        int _base = 8;
        while (_base < size)
            _base *= 2;
        return _base;
    }

    public T[] Allocate(int size)
    {
        T[] result = null;
        size = GetSize(size);
        Queue<T[]> queue;
        if (!map.TryGetValue(size, out queue))
        {
            queue = new Queue<T[]>();
            map.Add(size, queue);
        }
        if (queue.Count == 0)
            result = new T[size];
        else
            result = queue.Dequeue();
        return result;
    }
    public void Free(T[] array)
    {
        int size = array.Length;
        Queue<T[]> queue;
        if (!map.TryGetValue(size, out queue))
        {
            queue = new Queue<T[]>();
            map.Add(size, queue);
        }
        queue.Enqueue(array);
    }
}


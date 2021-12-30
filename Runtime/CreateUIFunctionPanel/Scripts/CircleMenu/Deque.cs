using System.Collections;
using System.Collections.Generic;
using System;


public class Deque<T>
{
    private LinkedList<T> m_queue = new LinkedList<T>();

    public int Count
    {
        get
        {
            return m_queue.Count;
        }
    }

    public void Clear()
    {
        m_queue.Clear();
    }

    public bool Contains(T value)
    {
        return m_queue.Contains(value);
    }

    public T Front
    {
        get
        {
            if (Count == 0)
            {
                return default(T);
            }

            return m_queue.First.Value;
        }
    }

    public T Back
    {
        get
        {
            if (Count == 0)
            {
                return default(T);
            }

            return m_queue.Last.Value;
        }
    }

    public void PushFront(T value)
    {
        m_queue.AddFirst(value);
    }

    public void PushBack(T value)
    {
        m_queue.AddLast(value);
    }

    public T PopFront()
    {
        if (Count == 0)
        {
            return default(T);
        }

        T t = Front;
        m_queue.RemoveFirst();
        return t;
    }

    public T PopBack()
    {
        if (Count == 0)
        {
            return default(T);
        }

        T t = Back;
        m_queue.RemoveLast();
        return t;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return m_queue.GetEnumerator();
    }
}


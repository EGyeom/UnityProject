using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> _list = new List<T>();

    public void Push(T data)
    {
        _list.Add(data);
        int now = _list.Count - 1;

        while (now > 0)
        {
            int next = (now - 1) / 2;

            if (_list[now].CompareTo(_list[next]) < 0)
                break;

            T temp = _list[now];
            _list[now] = _list[next];
            _list[next] = temp;

            now = next;
        }

    }
    public T Pop()
    {
        T ret = _list[0];
        int lastIndex = _list.Count - 1;
        _list[0] = _list[lastIndex];
        _list.RemoveAt(lastIndex);
        lastIndex--;

        int now = 0;

        while (now <= lastIndex)
        {
            int next = now;

            int left = now * 2 + 1;
            int right = now * 2 + 2;

            if (left <= lastIndex && _list[next].CompareTo(_list[left]) < 0)
                next = left;

            if (right <= lastIndex && _list[next].CompareTo(_list[right]) < 0)
                next = right;

            if (next == now)
                break;

            T temp = _list[now];
            _list[now] = _list[next];
            _list[next] = temp;

            now = next;
        }
        return ret;
    }

    public int Count { get { return _list.Count; } }
}

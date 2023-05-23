using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Underall.Collections;

public class SortedLinkedList<TKey, TValue>
    where TKey: IComparable
{
    public SortedLinkedListNode<TKey, TValue> First;
    public SortedLinkedListNode<TKey, TValue> Last;
    public int Count;
    private bool IsChanged = true;
    private TValue[] _values;
    
    public TValue[] Values
    {
        get
        {
            if (IsChanged) GatherValues();
            return _values;
        }
        set
        {
            _values = value;
        }
    }

    public SortedLinkedList()
    {
    }

    public SortedLinkedListNode<TKey, TValue> Add(TKey key, TValue value)
    {
        var node = new SortedLinkedListNode<TKey, TValue>(key, value);
        Add(node);
        return node;
    }

    public void Add(SortedLinkedListNode<TKey, TValue> newNode)
    {
        IsChanged = true;
        Count++;
        if (Last is null)
        {
            Last = newNode;
            First = Last;
            return;
        }
        for (var node = First; node != null; node = node.Next)
            if (node.Key.CompareTo(newNode.Key) >= 0)
            {
                if (node == First) First = newNode;
                else newNode.Previous.Next = newNode;
                newNode.Next = node;
                newNode.Previous = node.Previous;
                newNode.Next.Previous = newNode;
                return;
            }
        

        Last.Next = newNode;
        newNode.Previous = Last;
        Last = newNode;
    }

    public void RemoveFirst(TValue value)
    {
        if (FindFirst(value) is not SortedLinkedListNode<TKey, TValue> node)
            return;
        Remove(node);
        IsChanged = true;
    }

    public void Remove(SortedLinkedListNode<TKey, TValue> node)
    {
        if (Count == 1)
        {
            First = null;
            Last = null;
        }
        else if (node == Last)
        {
            node.Previous.Next = null;
            Last = node.Previous;
        } 
        else if (node == First)
        {
            node.Next.Previous = null;
            First = node.Next;
        }
        else
        {
            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
        }
        Count--;
    }

    public SortedLinkedListNode<TKey, TValue>? FindFirst(TValue value)
    {
        for (var node = First; node != null; node = node.Next)
            if (node.Value.Equals(value))
                return node;
        return null;
    }

    private void GatherValues()
    {
        if (!IsChanged) return;
        IsChanged = false;
        
        _values = new TValue[Count];
        var i = 0;
        for (var node = First; node != null; node = node.Next)
        {
            _values[i] = node.Value;
            i++;
        }
    }

    public void Clear()
    {
        First = null;
        Last = null;
        Count = 0;
    }
}

public class SortedLinkedListNode<TKey, TValue>
    where TKey: IComparable
{
    public TKey Key;
    public TValue Value;
    public SortedLinkedListNode<TKey, TValue> Previous;
    public SortedLinkedListNode<TKey, TValue> Next;

    public SortedLinkedListNode(TKey key, TValue value, SortedLinkedListNode<TKey, TValue> previous = null, 
        SortedLinkedListNode<TKey, TValue> next = null)
    {
        Key = key;
        Value = value;
        Previous = previous;
        Next = next;
    }
}
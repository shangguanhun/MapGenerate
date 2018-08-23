using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCoroutine : IEnumerator
{
    private Stack<IEnumerator> executionStack;

    public EditorCoroutine(IEnumerator iterator)
    {
        this.executionStack = new Stack<IEnumerator>();
        this.executionStack.Push(iterator);
    }

    public bool MoveNext()
    {
        IEnumerator i = this.executionStack.Peek();

        if (i.MoveNext())
        {
            object result = i.Current;
            if (result != null && result is IEnumerator)
            {
                this.executionStack.Push((IEnumerator)result);
            }

            return true;
        }
        else
        {
            if (this.executionStack.Count > 1)
            {
                this.executionStack.Pop();
                return true;
            }
        }

        return false;
    }

    public void Reset()
    {
        throw new System.NotSupportedException("This Operation Is Not Supported.");
    }

    public object Current
    {
        get { return this.executionStack.Peek().Current; }
    }

    public bool Find(IEnumerator iterator)
    {
        return this.executionStack.Contains(iterator);
    }
}

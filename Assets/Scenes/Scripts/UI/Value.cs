using UnityEngine;
using System;

[Serializable]
public class Value : MonoBehaviour
{
    public string name;

    public virtual double Val
    {
        get
        {
            throw new Exception("����� �� ����������");
            return 0;
        }
        set
        {
            throw new Exception("����� �� ����������");
        }
    }

    public static explicit operator Double(Value counter)
    {
        return counter.Val;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Value : MonoBehaviour
{
    public double Val
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

    public int ValI
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
}
using Assets.Scenes.Scripts.UI;
using Program;
using System.Collections.Generic;
using System.Linq;
using Types;
using UnityEngine;

public class CollectData : MonoBehaviour
{
    private List<ValuesGroup> valuesGroups;
    private List<ValuesGroupArray> valuesGroupArrays;

    private Dictionary<string, Dictionary<string, object>> datas = new();

    private void Start()
    {
        SetInitData(new InitData());
    }

    public InitData GetInitData()
    {
        valuesGroups = new List<ValuesGroup>(GetComponentsInChildren<ValuesGroup>());
        valuesGroupArrays = new List<ValuesGroupArray>(GetComponentsInChildren<ValuesGroupArray>());

        //Debug.Log(valuesGroups.Count);
        //Debug.Log(valuesGroupArrays.Count);
        foreach(ValuesGroupArray val in valuesGroupArrays)
        {
            //Debug.Log(val.name);
        }

        InitData initData = new InitData(false);
        InitValuesGroupArray(ref initData.cyl, "Types.CYLINDER");
        InitValuesGroupArray(ref initData.sect, "Types.SECT");
        InitValuesGroupArray(ref initData.S_K.S, "Types.SECTIONS");
        initData.S_K.Num_S = initData.S_K.S.Length;
        InitValuesGroup(ref initData.data);
        initData.data.nS_1 = initData.sect.Count((SECT sec) => sec.S_Type == 1);
        initData.data.nS_2 = initData.sect.Count((SECT sec) => sec.S_Type == 2);
        initData.data.nS_korp = initData.cyl.Count();
        InitValuesGroup(ref initData.dop);
        InitValuesGroup(ref initData.fluxData);
        InitValuesGroup(ref initData.train);

        return initData;
    }

    public void SetInitData(InitData initData)
    {
        List<ValuesGroupArray> arrays = new List<ValuesGroupArray>(GetComponentsInChildren<ValuesGroupArray>());
        AbjastValuesGroupArray(arrays, initData.cyl);
        AbjastValuesGroupArray(arrays, initData.sect);
        AbjastValuesGroupArray(arrays, initData.S_K.S);

        valuesGroups = new List<ValuesGroup>(GetComponentsInChildren<ValuesGroup>());
        valuesGroupArrays = new List<ValuesGroupArray>(GetComponentsInChildren<ValuesGroupArray>());

        SetValuesGroupArrays(initData.cyl);
        SetValuesGroupArrays(initData.sect);
        SetValuesGroupArrays(initData.S_K.S);
        SetValuesGroup(initData.data);
        SetValuesGroup(initData.dop);
        SetValuesGroup(initData.fluxData);
        SetValuesGroup(initData.train);
    }

    void AbjastValuesGroupArray<T>(List<ValuesGroupArray> arrays, T[] vals)
    {
        var name = typeof(T).ToString();
        foreach (var arr in arrays)
        {
            if (arr.name == name)
            {
                arr.SetSize(vals.Length);
            }
        }
    }

    List<ValuesGroup> GetValuesGroups(string name)
    {
        //
        List<ValuesGroup> grs = new List<ValuesGroup>();
        for (int i = 0; i < valuesGroups.Count; ++i)
        {
            if (valuesGroups[i] is ValuesGroup &&
                valuesGroups[i].name == name)
            {
                grs.Add(valuesGroups[i]);
            }
        }

        if (grs.Count == 0)
        {
            //Debug.Log($"На форме нет группы {name}");
        }

        return grs;
    }

    ValuesGroupArray GetValuesGroupArray(string name)
    {
        for (int i = 0; i < valuesGroupArrays.Count; ++i)
        {
            if (valuesGroupArrays[i].name == name)
            {
                return valuesGroupArrays[i];
            }

            //Debug.Log("Value name = " + valuesGroupArrays[i].name + $" {i} {valuesGroupArrays.Count}");
        }

        return null;
    }

    public void InitValuesGroupArray<T>(ref T[] valuesGroup, string name)
    {
        //Debug.Log(name);
        ValuesGroupArray grArrs = GetValuesGroupArray(name);
        List<ValuesGroup> grs = grArrs.GetGroups();
        if (grs.Count == 0)
        {
            //Debug.Log("На форме не найден тип массива" + typeof(T).ToString());
            return;
        }

        valuesGroup = new T[grs.Count];
        for (int i = 0; i < grs.Count; ++i)
        {
            InitValuesGroupFromGroup(ref valuesGroup[i], grs.GetRange(i, 1));
        }
    }

    public void InitValuesGroup<T>(ref T valuesGroup)
    {
        List<ValuesGroup> grs = GetValuesGroups(typeof(T).ToString());
        if (grs.Count == 0)
        {
            //Debug.Log("На форме не найден тип массива" + typeof(T).ToString());
            return;
        }

        InitValuesGroupFromGroup(ref valuesGroup, grs);
    }

    public void InitValuesGroupFromGroup<T>(ref T valuesGroup, List<ValuesGroup> grs)
    {
        var filds = typeof(T).GetFields();
        for (int i = 0; i < grs.Count; ++i)
        {
            var vals = grs[i].GetVals();
            foreach (var fild in filds)
            {
                if (fild.Name == "Order")
                {
                    object obj1 = valuesGroup;
                    fild.SetValue(obj1, i + 1);
                    valuesGroup = (T)obj1;
                }

                if (vals.ContainsKey(fild.Name))
                {
                    object obj = valuesGroup;
                    fild.SetValue(obj, vals[fild.Name].Val);
                    valuesGroup = (T)obj;
                }
            }
        }
    }

    public void SetValuesGroupArrays<T>(T[] valuesGroup)
    {
        ValuesGroupArray grArrs = GetValuesGroupArray(typeof(T).ToString());
        List<ValuesGroup> grs = grArrs.GetGroups();
        if (grs.Count == 0)
        {
            //Debug.Log("На форме не найден тип массива" + typeof(T).ToString());
            return;
        }

        for (int i = 0; i < grs.Count(); ++i)
        {
            SetValuesGroupFromGroup(valuesGroup[i], grs.GetRange(i, 1));
        }
    }

    public void SetValuesGroup<T>(T valuesGroup)
    {
        List<ValuesGroup> grs = GetValuesGroups(typeof(T).ToString());
        if (grs.Count == 0)
        {
            //Debug.Log("На форме не найден тип массива" + typeof(T).ToString());
            return;
        }

        SetValuesGroupFromGroup(valuesGroup, grs);
    }

    public void SetCurrentValuesGroup<T>(T valuesGroup)
    {
        ValuesGroupArray grArrs = GetValuesGroupArray(typeof(T).ToString());
        List<ValuesGroup> grs = grArrs.GetCurrentGroups();
        if (grs.Count == 0)
        {
            //Debug.Log("На форме не найден тип массива" + typeof(T).ToString());
            return;
        }

        SetValuesGroupFromGroup(valuesGroup, grs);
    }

    public void SetValuesGroupFromGroup<T>(T valuesGroup, List<ValuesGroup> grs)
    {
        var filds = typeof(T).GetFields();
        for (int i = 0; i < grs.Count; ++i)
        {
            var vals = grs[i].GetVals();
            foreach (var fild in filds)
            {
                if (vals.ContainsKey(fild.Name))
                {
                    vals[fild.Name].Val = fild.GetValue(valuesGroup);
                }
            }
        }
    }
}

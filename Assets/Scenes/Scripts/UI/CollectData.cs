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
        //SetInitData(new InitData());
    }

    public InitData GetInitData()
    {
        valuesGroups = new List<ValuesGroup>(GetComponentsInChildren<ValuesGroup>());
        valuesGroupArrays = new List<ValuesGroupArray>(GetComponentsInChildren<ValuesGroupArray>());

        InitData initData = new InitData(false);
        initData.model = GetModel();

        if (initData.model == ExtruderModel.Dual)
        {
            GetInitDataDual(initData);
        }
        else
        {
            GetInitDataSingle(initData);
        }

        InitValuesGroup(ref initData.train);

        return initData;
    }

    void GetInitDataSingle(InitData initData)
    {
        InitValuesGroupArray(ref initData.cyl, "BarrelConfiguration");
        InitValuesGroupArray(ref initData.sect, "ScrewConfiguration");
        InitValuesGroupArray(ref initData.S_K.S, "DieConfiguration");
        initData.S_K.Num_S = initData.S_K.S.Length;
        InitValuesGroup(ref initData.data);
        initData.data.nS_1 = initData.sect.Count((SECT sec) => sec.S_Type == 1);
        initData.data.nS_2 = initData.sect.Count((SECT sec) => sec.S_Type == 2);
        initData.data.nS_korp = initData.cyl.Count();
        InitValuesGroup(ref initData.dop);
        InitValuesGroup(ref initData.fluxData);
    }

    void GetInitDataDual(InitData initData)
    {
        InitValuesGroupArray(ref initData.cylinderDual, "BarrelConfigurationDual");
        InitValuesGroupArray(ref initData.sectDual, "ScrewConfigurationDual");
        InitValuesGroupArray(ref initData.sect_aDual, "ScrewConfigurationDual_a");
        InitValuesGroupArray(ref initData.s_kDual.S, "DieConfigurationDual");
        initData.s_kDual.Num_S = initData.s_kDual.S.Length;
        InitValuesGroup(ref initData.dataDual, "DATA_Dual");
        initData.dataDual.nS_1 = initData.sectDual.Count((TypesDual.SECT sec) => sec.S_Type == 1);
        initData.dataDual.nS_2 = initData.sectDual.Count((TypesDual.SECT sec) => sec.S_Type == 2);
        initData.dataDual.nS_1a = initData.sect_aDual.Count((TypesDual.SECT_a sec) => sec.S_Type == 1);
        initData.dataDual.nS_2a = initData.sect_aDual.Count((TypesDual.SECT_a sec) => sec.S_Type == 2);
        initData.dataDual.nS_korp = initData.cylinderDual.Count();
        InitValuesGroup(ref initData.dop_dataDual, "DOP_DATADual");
        InitValuesGroup(ref initData.datDual, "FluxDataDual");
    }

    public void SetInitData(InitData initData)
    {
        List<ValuesGroupArray> arrays = new List<ValuesGroupArray>(GetComponentsInChildren<ValuesGroupArray>());
        AbjastValuesGroupArray(arrays, initData.cyl);
        AbjastValuesGroupArray(arrays, initData.sect);
        AbjastValuesGroupArray(arrays, initData.S_K.S);
        AbjastValuesGroupArray(arrays, initData.cylinderDual, "BarrelConfigurationDual");
        AbjastValuesGroupArray(arrays, initData.sectDual, "ScrewConfigurationDual");
        AbjastValuesGroupArray(arrays, initData.sect_aDual, "ScrewConfigurationDual_a");
        AbjastValuesGroupArray(arrays, initData.s_kDual.S, "DieConfigurationDual");

        valuesGroups = new List<ValuesGroup>(GetComponentsInChildren<ValuesGroup>());
        valuesGroupArrays = new List<ValuesGroupArray>(GetComponentsInChildren<ValuesGroupArray>());

        // Модель (initData.model) не устанавливается отдельно: она автоматически
        // определяется выбранной маркой экструдера, значение которой
        // восстанавливает сам SetValuesGroup для поля "Extruder" сценария.
        SetValuesGroupArrays(initData.cyl);
        SetValuesGroupArrays(initData.sect);
        SetValuesGroupArrays(initData.S_K.S);
        SetValuesGroup(initData.data);
        SetValuesGroup(initData.dop);
        SetValuesGroup(initData.fluxData);
        SetValuesGroup(initData.train);

        SetValuesGroupArrays(initData.cylinderDual, "BarrelConfigurationDual");
        SetValuesGroupArrays(initData.sectDual, "ScrewConfigurationDual");
        SetValuesGroupArrays(initData.sect_aDual, "ScrewConfigurationDual_a");
        SetValuesGroupArrays(initData.s_kDual.S, "DieConfigurationDual");
        SetValuesGroup(initData.dataDual, "DATA_Dual");
        SetValuesGroup(initData.dop_dataDual, "DOP_DATADual");
        SetValuesGroup(initData.datDual, "FluxDataDual");
    }

    // Модель расчёта определяется выбранной маркой экструдера (поле "Extruder"
    // на вкладке сценария) через связь Extruder -> ExtruderType -> MathModel.
    private static readonly string DualModelName = "Двухшнековый";

    Value GetExtruderValue()
    {
        foreach (var v in GetComponentsInChildren<Value>())
        {
            if (v.name == "Extruder")
            {
                return v;
            }
        }

        return null;
    }

    public ExtruderModel GetModel()
    {
        Value v = GetExtruderValue();
        if (v == null) return ExtruderModel.Single;

        string brand = v.Val?.ToString();
        if (string.IsNullOrEmpty(brand)) return ExtruderModel.Single;

        string modelName = DropdownDatas.DB.GetMathModelName(brand);
        return modelName == DualModelName ? ExtruderModel.Dual : ExtruderModel.Single;
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

    void AbjastValuesGroupArray<T>(List<ValuesGroupArray> arrays, T[] vals, string name)
    {
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
            //Debug.Log($"�� ����� ��� ������ {name}");
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
        }

        return null;
    }

    public void InitValuesGroupArray<T>(ref T[] valuesGroup, string name)
    {
        ValuesGroupArray grArrs = GetValuesGroupArray(name);
        List<ValuesGroup> grs = grArrs.GetGroups();
        if (grs.Count == 0)
        {
            //Debug.Log("�� ����� �� ������ ��� �������" + typeof(T).ToString());
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
        List<ValuesGroup> grs = GetValuesGroups(typeof(T).ToString()[6..]);
        if (grs.Count == 0)
        {
            Debug.Log("�� ����� �� ������ ��� �������" + typeof(T).ToString());
            return;
        }

        InitValuesGroupFromGroup(ref valuesGroup, grs);
    }

    public void InitValuesGroup<T>(ref T valuesGroup, string name)
    {
        List<ValuesGroup> grs = GetValuesGroups(name);
        if (grs.Count == 0)
        {
            Debug.Log("�� ����� �� ������ ��� �������" + name);
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
            //Debug.Log("�� ����� �� ������ ��� �������" + typeof(T).ToString());
            return;
        }

        for (int i = 0; i < grs.Count(); ++i)
        {
            SetValuesGroupFromGroup(valuesGroup[i], grs.GetRange(i, 1));
        }
    }

    public void SetValuesGroupArrays<T>(T[] valuesGroup, string name)
    {
        ValuesGroupArray grArrs = GetValuesGroupArray(name);
        List<ValuesGroup> grs = grArrs.GetGroups();
        if (grs.Count == 0)
        {
            //Debug.Log("�� ����� �� ������ ��� �������" + name);
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
            //Debug.Log("�� ����� �� ������ ��� �������" + typeof(T).ToString());
            return;
        }

        SetValuesGroupFromGroup(valuesGroup, grs);
    }

    public void SetValuesGroup<T>(T valuesGroup, string name)
    {
        List<ValuesGroup> grs = GetValuesGroups(name);
        if (grs.Count == 0)
        {
            //Debug.Log("�� ����� �� ������ ��� �������" + name);
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
            //Debug.Log("�� ����� �� ������ ��� �������" + typeof(T).ToString());
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

using Assets.Scenes.Scripts.UI;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DropdownDatas : Value
{
    private bool isInit = false;

    static DatabaseCollectData DB = new();

    private int currOptIndex = 0;
    private string CurrOptText
    {
        get
        {
            return dropdown.options.Count > currOptIndex ? 
                        dropdown.options[currOptIndex].text :
                        "";
        }
        set
        {
            if (dropdown.options.Count > currOptIndex)
            {
                dropdown.options[currOptIndex].text = value;
            }
        }
    }

    private string designValName = string.Empty;

    private Dictionary<string, Value> currVals = new();
    private Dictionary<string, Dictionary<string, object>> optVals = new();

    [SerializeField] private string newOptionText = "Новый";

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button addBtn;
    [SerializeField] private Button saveBtn;

    [SerializeField] private List<ValuesGroup> valGrps;
    [SerializeField] private List<ValuesGroupArray> valGrpsArr;
    [SerializeField] private List<DropdownDatas> valDropdowns;

    void Awake()
    {
        StringValue strVal = inputField.GetComponentInParent<StringValue>();
        designValName = strVal.name;

        currVals[designValName] = strVal;

        foreach (var valGrp in valGrps)
        {
            var vals = valGrp.GetVals();
            currVals.AddRange(vals);
        }

        foreach (var valArrGrp in valGrpsArr)
        {
            currVals[valArrGrp.name] = valArrGrp;
        }

        foreach (var dd in valDropdowns)
        {
            currVals[dd.name] = dd;
        }

        addBtn.onClick.AddListener(() => AddData());
        saveBtn.onClick.AddListener(() => SaveVals(dropdown.captionText.text));
        SubscribeDDEvents();

        dropdown.onValueChanged.AddListener((int _) => InitVals());
        dropdown.ClearOptions();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (!isInit)
        {
            //SetData(DB.GetDatas(dataFields));
            SetData(DB.GetDatas(name));
            isInit = true;
        }
    }

    public override object Val
    {
        get
        {
            Init();
            if (IsValsChanged())
                return "";
            return inputField.text;
        }
        set
        {
            Init();
            string v = value.ToString();
            int i = dropdown.options.FindIndex((TMP_Dropdown.OptionData o) => o.text == v);
            if (i > -1)
            {
                dropdown.value = i;
            }
        }
    }

    public override object DefaultVal
    {
        get
        {
            return Val;
        }
    }

    //public void SetData(List<Dictionary<string, object>> data)
    //{
    //    foreach (var vals in data)
    //    {
    //        string designName = (string)vals[designValName];
    //        AddOption(designName);
    //        var optVal = optVals[designName];
    //        foreach (var key in currVals.Keys)
    //        {
    //            if (vals.ContainsKey(key))
    //            {
    //                optVal[key] = vals[key];
    //            }
    //        }
    //    }

    //    if (optVals.Count > 0)
    //    {
    //        SetVals(optVals.First().Value);
    //    }
    //}

    public void SetData(Dictionary<string, List<IValue>> datas)
    {
        if (datas == null)
            return;

        foreach (var vals in datas)
        {
            AddOption(vals.Key);
            var optVal = optVals[vals.Key];
            foreach(var val in vals.Value)
            {
                if (currVals.ContainsKey(val.ValName))
                {
                    optVal[val.ValName] = val.Val;
                }
            }
        }

        if (optVals.Count > 0)
        {
            SetVals(optVals.First().Value);
        }
    }

    private void AddData()
    {
        if (IsValsChanged())
        {
            if (ResetChangesMsg())
            {
                ResetChanges();
            }
            else
            {
                return;
            }
        }

        var newOpt = AddOption(newOptionText);

        dropdown.value = dropdown.options.Count - 1;
        if (dropdown.options.Count == 1)
        {
            InitVals();
        }

        inputField.Select();

        if (newOpt != null)
        {
            DropdownDatasEvents.AddDataEvent.Invoke(this, name, newOpt);
        }
        else
        {
            Debug.Log("При AddData не получилось добавить option, скорее всего переданное в AddOption имя empty");
        }
    }

    private Dictionary<string, object> AddOption(string name)
    {
        if (name != string.Empty)
        {
            name = ValidName(name);
            dropdown.AddOptions(new List<string> { name });

            return ResetOptVals(name);
        }

        return null;
    }

    private void InitVals()
    {
        if (currOptIndex != dropdown.value && IsValsChanged())
        {
            if (!ResetChangesMsg())
            {
                dropdown.SetValueWithoutNotify(currOptIndex);
                return;
            }
        }

        currOptIndex = dropdown.value;
        SetVals(optVals[dropdown.captionText.text]);
    }
    
    public bool IsValsChanged()
    {
        if (!optVals.ContainsKey(CurrOptText))
            return false;

        var optVal = optVals[CurrOptText];
        foreach (var val in currVals)
        {
            if (!EqualsObjs(val.Value.Val, optVal[val.Key]))
                return true;
        }

        return false;
    }

    bool EqualsObjs(object o1,  object o2)
    {
        if (o1 is string || o1 is MyList<string> || o1 is List<string>)
        {
            return o1.Equals(o2);
        }

        double v1 = 0;
        double v2 = 0;
        if (o1 is int)
        {
            v1 = (int)o1;
        }
        else if (o1 is double)
        {
            v1 = (double)o1;
        }

        if (o2 is int)
        {
            v2 = (int)o2;
        }
        else if (o2 is double)
        {
            v2 = (double)o2;
        }

        return v1 == v2;
    }

    private void SaveVals(string optName)
    {
        string newOptName = (string)currVals[designValName].Val;
        if (newOptName != optName && optVals.ContainsKey(newOptName))
        {
            //EditorUtility.DisplayDialog("Предупреждение", $"Название {newOptName} уже используется!\n Попробуйте другое.", "Хорошо");
            return;
        }

        var optVal = optVals[optName];
        foreach (var valName in currVals.Keys)
        {
            optVal[valName] = currVals[valName].Val;
        }

        if (optName != newOptName)
        {
            optVals.Remove(optName);
            optVals[newOptName] = optVal;
        }

        dropdown.options[dropdown.value].text = newOptName;
        dropdown.captionText.text = newOptName;

        DropdownDatasEvents.SaveDataEvent.Invoke(this, name, optName, optVal);
    }

    private Dictionary<string, object> ResetOptVals(string optName)
    {
        if (!optVals.ContainsKey(optName))
        {
            optVals[optName] = new Dictionary<string, object>();
        }

        var optVal = optVals[optName];
        List<string> keys = new List<string>(currVals.Keys);
        foreach (var valKey in keys)
        {
            optVal[valKey] = currVals[valKey].DefaultVal;
        }

        optVal[designValName] = optName;
        
        return optVal;
    }

    private void ResetChanges()
    {
        var optVal = optVals[CurrOptText];
        List<string> keys = new List<string>(currVals.Keys);
        foreach (var valKey in keys)
        {
            currVals[valKey].Val = optVal[valKey];
        }
    }

    private void SetVals(Dictionary<string, object> newVals)
    {
        foreach (var val in newVals)
        {
            if (currVals.ContainsKey(val.Key))
            {
                currVals[val.Key].Val = val.Value;
            }
        }
    }

    public void RemoveData(Button remBtn)
    {
        string optText = remBtn.transform.parent.GetComponentInChildren<TMP_Text>().text;        
        RemoveData(optText);
    }

    public void RemoveData(string optText)
    {
        RemoveOption(optText);
        if (dropdown.options.Count == 0)
        {
            addBtn.onClick.Invoke();
        }

        RefreshOptions();

        DropdownDatasEvents.RemoveDataEvent.Invoke(this, name, optText);
    }

    public void RemoveOption(string optText)
    {
        int index = dropdown.options.FindIndex((TMP_Dropdown.OptionData od) => od.text == optText);
        optVals.Remove(optText);
        dropdown.options.RemoveAt(index);

        if (index <= currOptIndex)
        {
            currOptIndex = Math.Max(currOptIndex - 1, 0);
        }
        if (index <= dropdown.value)
        {
            dropdown.value -= 1;
        }
    }

    private void SubscribeDDEvents()
    {
        DropdownDatasEvents.SaveDataEvent += SaveDataEvent;
        DropdownDatasEvents.AddDataEvent += AddDataEvent;
        DropdownDatasEvents.RemoveDataEvent += RemoveDataEvent;
    }

    private void SaveDataEvent(object sender, string nameDataGroup, string oldDataName, Dictionary<string, object> dataFields)
    {
        if (name != nameDataGroup || sender == this || !gameObject.activeInHierarchy)
            return;

        string designName = dataFields[designValName].ToString();
        if (oldDataName != designName)
        {
            optVals.Remove(oldDataName);
            dropdown.options.Find((TMP_Dropdown.OptionData data) => data.text == oldDataName).text = designName;
        }

        optVals[designName] = dataFields;
        if (currVals[designValName].Val.ToString() == oldDataName)
        {
            dropdown.captionText.text = designName;
            SetVals(dataFields);
        }
    }

    private void AddDataEvent(object sender, string nameDataGroup, Dictionary<string, object> dataFields)
    {
        if (name == nameDataGroup && sender != this && gameObject.activeInHierarchy)
        {
            AddOption(dataFields[designValName].ToString());
        }
    }

    private void RemoveDataEvent(object sender, string nameDataGroup, string optText)
    {
        if (name == nameDataGroup && sender != this && gameObject.activeInHierarchy)
        {
            RemoveOption(optText);
        }
    }

    private void OnDestroy()
    {
        DropdownDatasEvents.SaveDataEvent -= SaveDataEvent;
        DropdownDatasEvents.AddDataEvent -= AddDataEvent;
        DropdownDatasEvents.RemoveDataEvent -= RemoveDataEvent;
    }

    public void RefreshOptions()
    {
        dropdown.enabled = false;
        dropdown.enabled = true;
        dropdown.Show();
    }

    private string ValidName(string name)
    {
        string postfix = "";
        int postfixVal = 0;
        while (optVals.ContainsKey(name + postfix))
        {
            postfix = (++postfixVal).ToString();
        }

        return name +postfix;
    }

    private bool ResetChangesMsg()
    {
        return true;
    }
}

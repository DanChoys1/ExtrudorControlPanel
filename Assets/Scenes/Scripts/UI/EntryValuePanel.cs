using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EntryValuePanel : MonoBehaviour
{
    public delegate void ChangeVal(TMP_Text message);
    public event ChangeVal OnChangeVal;

    Regex inReg = new Regex("^-?(\\d){0,3}(,(\\d){0,2})?$");

    [SerializeField] VrButton[] valBtns = new VrButton[10];
    [SerializeField] VrButton minusBtn;
    [SerializeField] VrButton commaBtn;
    [SerializeField] VrButton delBtn;
    [SerializeField] VrButton entrBtn;

    [SerializeField] TMP_Text valText;

    TMP_Text targetField;
    double _min = 0;
    double _max = 0;

    void Start()
    {
        for (int i = 0; i < valBtns.Length; ++i)
        {
            int elem = i;
            valBtns[i].down.AddListener(() =>
            {
                string resStr = valText.text + elem.ToString();
                if (inReg.IsMatch(resStr))
                {
                    if(double.TryParse(resStr, out double val))
                        if (_min <= val &&  val <= _max)
                            valText.text = resStr;
                }                
            });
        }

        minusBtn.down.AddListener(() =>
        {
            if (valText.text.Length == 0)
            {
                if (_min < 0)
                    valText.text = "-";
            }
        });

        commaBtn.down.AddListener(() =>
        {
            string resStr = valText.text + ",";
            if (inReg.IsMatch(resStr))
            {
                valText.text = resStr;
            }
        });

        delBtn.down.AddListener(() =>
        {
            if (valText.text.Length != 0)
            {
                valText.text = valText.text.Substring(0, valText.text.Length - 1);
            }
        });

        entrBtn.down.AddListener(() =>
        {
            if (valText.text.Length == 0 ||
                (valText.text.Length == 1 && valText.text == "-"))
            {
                valText.text = "0";
            }

            double val = double.Parse(valText.text);
            if (_min > val)
                val = _min;
            if (_max < val)
                val = _max;

            targetField.text = string.Format("{0:f}", val);

            OnChangeVal?.Invoke(targetField);
            gameObject.SetActive(false);
        });

        this.gameObject.SetActive(false);
    }

    public void Open(MinMaxTextData d)
    {
        if (gameObject.activeSelf) return;

        _min = d.min;
        _max = d.max;

        targetField = d.t;
        valText.text = d.t.text;
        gameObject.SetActive(true);
    }
}

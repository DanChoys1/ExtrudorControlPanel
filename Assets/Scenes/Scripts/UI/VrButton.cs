using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VrButton : MonoBehaviour
{
    public bool isChangeColor = false;
    private bool _active;
    private Color _color_start;
    private Image _button;
    [Serializable] public class ButtonEvent : UnityEvent { }

    public ButtonEvent down;
    public ButtonEvent press;
    public ButtonEvent up;

    private void OnEnable()
    {
        TryGetComponent(out _button);
        if (_button && isChangeColor)
            _color_start = _button.color;
    }

    private void OnDisable()
    {
        _active = false;
        if (_button && isChangeColor)
            _button.color = _color_start;
    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "hand")
        {
            down?.Invoke();
            _active = true;
            if (_button && isChangeColor)
                _button.color = Color.gray;
            StartCoroutine(Stay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "hand")
        {
            _active = false;
            up?.Invoke();
            if (_button && isChangeColor)
                _button.color = _color_start;
        }
    }

    private IEnumerator Stay()
    {
        while (_active)
        {
            press?.Invoke();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

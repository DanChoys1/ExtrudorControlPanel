using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitScript : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;

    void Start()
    {
        _button.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        _panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _panel.SetActive(!_panel.active);
        }
    }
}

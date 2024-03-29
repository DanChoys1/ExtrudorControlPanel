using UnityEngine;

public class ValuesGroupArray : MonoBehaviour
{
    public string name;
    private TabsManager tabsManager;

    void OnEnable()
    {
        if (tabsManager == null) tabsManager = GetComponent<TabsManager>();
    }

    public void SetSize(int size)
    {
        while (tabsManager.TabsCount() < size)
        {
            tabsManager.AddTab();
        }
        while (tabsManager.TabsCount() > size)
        {
            tabsManager.RemoveLast();
        }
    }
}

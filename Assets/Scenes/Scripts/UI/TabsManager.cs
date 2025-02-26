using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    public class Tab
    {
        public Tab(Button btn, GameObject tab)
        {
            this.btn = btn;
            this.tab = tab;
        }

        public Button btn;
        public GameObject tab;
    }

    [SerializeField] private Button addBtn;
    [SerializeField] private Button removeBtn;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject tabPrefab;
    [SerializeField] private Button tabBtnPrefab;

    private List<Tab> tabs = new List<Tab>();
    private Tab currTab = null;

    void Awake()
    {
        tabPrefab.SetActive(false);
        tabBtnPrefab.gameObject.SetActive(false);

        addBtn.onClick.AddListener(() =>
        {
            AddTab();
        });

        removeBtn.onClick.AddListener(() =>
        {
            RemoveLast();
        });
    }

    private void Start()
    {
        if (tabs.Count == 0)
        {
            AddTab();
        }
    }

    public void AddTab()
    {
        GameObject newTab = Instantiate(tabPrefab);
        newTab.transform.SetParent(tabPrefab.transform.parent.transform, false);
        newTab.SetActive(true);

        GameObject newTabBtn = Instantiate(tabBtnPrefab.gameObject);
        newTabBtn.transform.SetParent(tabBtnPrefab.gameObject.transform.parent.transform, false);
        newTabBtn.SetActive(true);
        int index = tabs.Count;
        newTabBtn.GetComponentInChildren<TMP_Text>().text = (index + 1).ToString();

        Button btn = newTabBtn.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            SetActiveTab(index);
        });

        tabs.Add(new Tab(btn, newTab));

        SetActiveTab(index);
    }

    public void RemoveLast()
    {
        if (tabs.Count > 0) RemoveTab(tabs.Count - 1);
    }

    public void RemoveTab(int index)
    {
        if (index <= 0) return;

        if (currTab == tabs[index])
        {
            SetActiveTab(index - 1);
        }

        tabs[index].tab.SetActive(false);
        tabs[index].btn.gameObject.SetActive(false);
        Destroy(tabs[index].tab);
        Destroy(tabs[index].btn.gameObject);
        tabs.RemoveAt(index);
    }

    public void SetActiveTab(int index)
    {
        if (currTab != null)
        {
            HideTab(currTab.tab);
        }
        if (tabs.Count > index && index >= 0)
        {
            ShowTab(tabs[index]);
        }
    }

    public List<Tab> GetTabs()
    {
        return tabs;
    }

    public int TabsCount()
    {
        return tabs.Count;
    }

    public Tab GetCurrentTab()
    {
        return currTab;
    }

    void HideTab(GameObject tab)
    {
        if (tab == null)
            return;

        RectTransform rectTr = tab.GetComponent<RectTransform>();
        rectTr.anchoredPosition = new Vector2(0, 1000);
        currTab.btn.GetComponent<Image>().color = Color.gray;
    }

    void ShowTab(Tab tab)
    {
        currTab = tab;
        scrollRect.content = currTab.tab.GetComponent<RectTransform>();
        RectTransform rectTr = currTab.tab.GetComponent<RectTransform>();
        rectTr.anchoredPosition = new Vector2(0, 0);

        currTab.btn.GetComponent<Image>().color = Color.white;
    }
}

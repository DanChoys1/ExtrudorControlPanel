﻿using UnityEngine;
using Vector = System.Numerics.Vector2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;

public class WindowGraph : MonoBehaviour
{
    public string graphName = string.Empty;
    public string xName = string.Empty;
    public string yName = string.Empty;

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateHor;
    private RectTransform dashTemplateVer;
    private List<GameObject> gameObjectList;

    private bool isMaxMin = false;

    private List<Vector> values = new List<Vector>();

    private float? yMaxLineVal = null;
    private float? yMinLineVal = null;

    private void Awake()
    {
        gameObjectList = new List<GameObject>();

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateHor = graphContainer.Find("dashTemplateHor").GetComponent<RectTransform>();
        dashTemplateVer = graphContainer.Find("dashTemplateVer").GetComponent<RectTransform>();
        if (graphName != string.Empty)
        {
            transform.Find("graphName").GetComponent<Text>().text = graphName;
        }
    }

    public void SetYMaxMinLine(float max, float min)
    {
        SetYMaxLine(max);
        SetYMinLine(min);
    }
    public void SetYMaxLine(float val)
    {
        yMaxLineVal = val;
    }
    public void SetYMinLine(float val)
    {
        yMinLineVal = val;
    }

    public void AddData(Vector value)
    {
        values.Add(value);
        SetData(values);
    }

    public void AddData(List<Vector> valueList)
    {
        values.AddRange(valueList);
        SetData(values);
    }

    public void SetData(List<Vector> valueList)
    {
        values = valueList;
        foreach (GameObject gameObject in gameObjectList) 
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        if (valueList.Count == 0) return;

        float yMax = valueList.Max((Vector vec) => { return vec.Y; });
        float yMin = valueList.Min((Vector vec) => { return vec.Y; });
        float xMax = valueList.Max((Vector vec) => { return vec.X; });
        float xMin = valueList.Min((Vector vec) => { return vec.X; });

        Vector2? lastCircle = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            lastCircle = ShowValue(valueList[i], lastCircle, xMax, xMin, yMax, yMin);
        }

        if (yMaxLineVal.HasValue && yMax > yMaxLineVal)
        {
            isMaxMin = true;
            lastCircle = ShowValue(new Vector(xMin, yMaxLineVal.Value), null, xMax, xMin, yMax, yMin);
            lastCircle = ShowValue(new Vector(xMax, yMaxLineVal.Value), lastCircle, xMax, xMin, yMax, yMin);
            isMaxMin = false;
        }
        if (yMinLineVal.HasValue && yMax > yMaxLineVal)
        {
            isMaxMin = true;
            lastCircle = ShowValue(new Vector(xMin, yMinLineVal.Value), null, xMax, xMin, yMax, yMin);
            lastCircle = ShowValue(new Vector(xMax, yMinLineVal.Value), lastCircle, xMax, xMin, yMax, yMin);
            isMaxMin = false;
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++) 
        {
            float normalizedValue = i * 1f / separatorCount;

            RectTransform labelY = CreateLabel(labelTemplateY, -10f, normalizedValue * Height());
            labelY.GetComponent<Text>().text = $"{yMin + (normalizedValue * (yMax - yMin)):f2}";

            RectTransform labelX = CreateLabel(labelTemplateX, normalizedValue * Width(), -15f);
            labelX.GetComponent<Text>().text = $"{xMin + (normalizedValue * (xMax - xMin)):f2}";

            if (i == separatorCount)
            {
                if (yName != string.Empty) labelY.GetComponent<Text>().text = yName;
                if (xName != string.Empty) labelX.GetComponent<Text>().text = xName;
            }

            if (i == 0) continue;

            RectTransform dashHor = CreateLabel(dashTemplateHor, -4f, normalizedValue * Height());
            RectTransform dashVer = CreateLabel(dashTemplateVer, normalizedValue * Width(), -4f);
        }
    }

    private Vector2 ShowValue(Vector val, Vector2? lastPos, float xMax, float xMin, float yMax, float yMin)
    {
        float xPos = ((val.X - xMin) / (xMax - xMin)) * Width();
        float yPos = ((val.Y - yMin) / (yMax - yMin)) * Height();
        RectTransform circle = CreateCircle(new Vector2(xPos, yPos));
        gameObjectList.Add(circle.gameObject);
        if (lastPos.HasValue)
        {
            GameObject connection = CreateConnection(lastPos.Value, circle.anchoredPosition);
            gameObjectList.Add(connection);
        }

        return circle.anchoredPosition;
    }

    private float Height()
    {
        return graphContainer.rect.height;
    }

    private float Width()
    {
        return graphContainer.rect.width;
    }

    private RectTransform CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        if (!isMaxMin) gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return rectTransform;
    }

    private GameObject CreateConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = isMaxMin ? Color.red : Color.black;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));

        return gameObject;
    }

    private RectTransform CreateLabel(RectTransform tr, float x, float y)
    {
        RectTransform dashVer = Instantiate(tr);
        dashVer.SetParent(graphContainer, false);
        dashVer.gameObject.SetActive(true);
        dashVer.anchoredPosition = new Vector2(x, y);
        gameObjectList.Add(dashVer.gameObject);

        return dashVer;
    }

    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}

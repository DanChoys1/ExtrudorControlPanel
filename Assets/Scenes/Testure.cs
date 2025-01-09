using UnityEngine;

public class Testure : MonoBehaviour
{
    public Texture texture1;
    public Texture texture2;
    public Texture texture3;
    public float transitionValue;

    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float normalizedValue = Mathf.Clamp01(transitionValue / 100f);

        if (normalizedValue <= 0.5f)
        {
            // Transition between texture1 and texture2
            float blendValue = normalizedValue * 2f;
            material.SetTexture("_MainTex", texture1);
            material.SetTexture("_BlendTex", texture2);
            material.SetFloat("_Blend", blendValue);
        }
        else
        {
            // Transition between texture2 and texture3
            float blendValue = (normalizedValue - 0.5f) * 2f;
            material.SetTexture("_MainTex", texture2);
            material.SetTexture("_BlendTex", texture3);
            material.SetFloat("_Blend", blendValue);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;


// Credit to this YouTube tutorial for the basic setup: https://www.youtube.com/watch?v=mOqHVMS7-Nw
public class ObjectFader : MonoBehaviour
{
    private const int MATERIAL_OPAQUE = 0;
    private const int MATERIAL_TRANSPARENT = 1;

    public float fadeSpeed = 10.0f;
    public float fadeOpacity = 0.5f;
    private List<(Material material, float originalOpacity, bool isOpaque)> materialProperties = new List<(Material, float, bool)>();
    private bool isFading = false;
    private bool isUnfading = false;
    private float curTime = 0.0f;
    Material[] materials;

    void Start()
    {
        materials = GetComponent<Renderer>().materials;

        foreach(var material in materials)
            materialProperties.Add((material, material.color.a, material.GetFloat("_Surface") == MATERIAL_OPAQUE));
    }

    void Update()
    {
        if (isFading || isUnfading)
            UpdateFade();

        if (Input.GetKeyDown(KeyCode.N))
            Fade();
        if (Input.GetKeyDown(KeyCode.M))
            Unfade();
    }

    public void Fade()
    {
        foreach (var (material, originalOpacity, isOpaque) in materialProperties)
            if (isOpaque) SetMaterialTransparent(material, true);
        isUnfading = false;
        isFading = true;
    }

    public void Unfade()
    {
        isUnfading = true;
        isFading = false;
    }

    private void UpdateFade()
    {
        curTime += Time.deltaTime;

        bool done = false;

        foreach (var (material, originalOpacity, isOpaque) in materialProperties)
        {
            Color curColor = material.color;
            float opacity = isFading ? Mathf.Lerp(originalOpacity, fadeOpacity, fadeSpeed * curTime) : Mathf.Lerp(fadeOpacity, originalOpacity, fadeSpeed * curTime);
            material.color = new Color(curColor.r, curColor.g, curColor.b, opacity);

            if (isFading && opacity == fadeOpacity || isUnfading && opacity == originalOpacity)
                done = true;
        }

        if (done)
        {
            curTime = 0.0f;
            if (isFading)
            {
                isFading = false;
            } 
            else if (isUnfading)
            {
                isUnfading = false;
                foreach (var (material, originalOpacity, isOpaque) in materialProperties)
                {
                    if (isOpaque) SetMaterialTransparent(material, false);
                }
            }
        }
    }

    // source: https://discussions.unity.com/t/changing-lightweight-shader-surface-type-in-c/720000/5
    private void SetMaterialTransparent(Material material, bool enabled)
    {
        material.SetFloat("_Surface", enabled ? MATERIAL_TRANSPARENT : MATERIAL_OPAQUE);
        material.renderQueue = enabled ? 3000 : 2000;
        material.SetFloat("_DstBlend", enabled ? 10 : 0);
        material.SetFloat("_SrcBlend", enabled ? 5 : 1);
        material.SetFloat("_ZWrite", enabled ? 0 : 1);
    }
}

using UnityEngine;
using System.Collections;

public class OVRScreenFade2 : MonoBehaviour
{
    public static OVRScreenFade2 oVRScreenFade;
    [SerializeField] private Material m_Material;
    // Use this for initialization
    void Start()
    {
        if(oVRScreenFade==null)
        {
            oVRScreenFade = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m_Material);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AwakeScreenEffect : MonoBehaviour
{
    public Shader shader;

    public static AwakeScreenEffect awakeScreenEffect;

    [Range(0f, 1f)]
    [Tooltip("苏醒进度")]
    public float progress;

    [SerializeField]
    Material material;

    [Range(0, 4)]
    [Tooltip("模糊迭代次数")]
    public int blurIterations = 3;
    [Range(.2f, 3f)]
    [Tooltip("每次模糊迭代时的模糊大小扩散")]
    public float blurSpread = .6f;

    private void Awake()
    {
        if(awakeScreenEffect==null)
        {
            awakeScreenEffect = this;
        }
        
    }
    Material Material
    {
        get
        {
            if (material == null)
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.DontSave;
            }
            return material;
        }
    }

    void OnDisable()
    {
        if (material)
        {
            DestroyImmediate(material);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Material.SetFloat("_Progress", progress);
        if (progress < 1)
        {
            // 由于降采样会影响模糊到清晰的连贯性，这里没有使用
            int rtW = src.width;
            int rtH = src.height;
            var buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;
            Graphics.Blit(src, buffer0, Material, 0);   // 眼皮Pass
                                                        // 模糊
            float blurSize;
            for (int i = 0; i < blurIterations; i++)
            {
                // 将progress(0~1)映射到blurSize(blurSize~0)
                blurSize = 1f + i * blurSpread;
                blurSize = blurSize - blurSize * progress;
                Material.SetFloat("_BlurSize", blurSize);
                // 竖直方向的Pass
                var buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(buffer0, buffer1, Material, 1);
                RenderTexture.ReleaseTemporary(buffer0);
                // 竖直方向的Pass
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(buffer0, buffer1, Material, 2);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }
            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            // 完全苏醒则无需处理，直接blit
            Graphics.Blit(src, dest);
        }
    }
}

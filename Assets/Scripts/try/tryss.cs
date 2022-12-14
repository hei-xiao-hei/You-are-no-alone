using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class tryss : MonoBehaviour
{
    public Animator CameraPanel;//获取控制相机闪烁的的Panel
    public int PanelSpeed;//相机闪烁的速度

    public Animator LightAni;
    public int LightSpeed;

    //控制PostProcessVolume的值
    public Animator PostProcessAni;
    public int PostProcessSpeed;

    // Start is called before the first frame update
    void Start()
    {
        PanelSpeed = 1;
        LightSpeed = 1;
        PostProcessSpeed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        CameraPanel.speed = PanelSpeed;
        LightAni.speed = LightSpeed;
        PostProcessAni.speed = PostProcessSpeed;
    }
}

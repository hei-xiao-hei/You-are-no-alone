using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    [SerializeField] GameObject StartUI;

    [SerializeField] Animator teacherAni;//玩家动画状态机
    [SerializeField] Animator cameraAni;//相机的动画状态机
    [SerializeField] Camera cameras;//相机

    public Animator LightAni;
    public int LightSpeed;

    //控制PostProcessVolume的值
    public Animator PostProcessAni;
    public int PostProcessSpeed;

    //呼吸和心跳的Audio Source
    public AudioSource BreathSource;
    public AudioSource HeartSource;

    //三种不同程度的呼吸和心跳
    public AudioClip[] BreathClip;
    public AudioClip[] HeartClip;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        StartUI.SetActive(true);//显示开始UI
        teacherAni.SetBool("isSpeeding", true);//切换玩家状态
        StartCoroutine(StartScene());

        LightSpeed = 1;
        PostProcessSpeed = 1;

        
        //InvokeRepeating("CameraInterval", 2.0f,0.0f);//每间隔两秒调用一次眨眼效果
    }

    // Update is called once per frame
    void Update()
    {
        LightAni.speed = LightSpeed;
        PostProcessAni.speed = PostProcessSpeed;
    }
    IEnumerator StartScene()
    {
        yield return new WaitForSeconds(5.0f);
        
        StartUI.SetActive(false);
        teacherAni.SetBool("isSpeeding", false);//关闭教师动画
        teacherAni.gameObject.SetActive(false);//隐藏教师模型
    }
    IEnumerator CountTime(float time,GameObject gameObject)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    //相机眨眼效果
    public void  CameraInterval()
    {
        cameraAni.SetTrigger("isBlink");//开始眨眼效果
    }

    //改变呼吸和心跳的函数
    public void BreathAndHeart(int index)
    {
        BreathSource.clip = BreathClip[index];
        HeartSource.clip = HeartClip[index];

        //改变灯光闪烁速度和场景曝光速度
        LightSpeed = index;
        PostProcessSpeed = index;
    }
}

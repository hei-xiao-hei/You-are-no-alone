using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject StartUI;

    [SerializeField] Animator teacherAni;//玩家动画状态机
    [SerializeField] Animator cameraAni;//相机的动画状态机
    [SerializeField] Camera cameras;//相机
    
    // Start is called before the first frame update
    void Start()
    {
        StartUI.SetActive(true);//显示开始UI
        teacherAni.SetBool("isSpeeding", true);//切换玩家状态
        StartCoroutine(StartScene());

        //InvokeRepeating("CameraInterval", 2.0f,0.0f);//每间隔两秒调用一次眨眼效果
    }

    // Update is called once per frame
    void Update()
    {
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
    //场景开始时的对话界面
    public void StartTalk()
    {
        
        //OVRScreenFade2.oVRScreenFade.OnRenderImage();
    }
}

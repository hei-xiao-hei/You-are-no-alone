using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController uiController;
    [Header("专注力")]
    public Slider Concentration;//专注力Slider
    public int scorces;//专注力分值
    [Header("通关UI")]
    public GameObject CleaanceUI;

    //咋眼效果
    public Animator CameraAwake;
    // Start is called before the first frame update
    void Start()
    {
        if(uiController==null)
        {
            uiController = this;
        }
        scorces = 100;
        CleaanceUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*//抓取物体，选对跳转场景，选错减分
    public void GrabObject()
    {
        //如果抓对了物体直接条跳转场景
        if()
        //如果选错了就扣分

    }*/

    //扣专注力
    public void DeductConcentration()
    {
        scorces -= 10;
        Concentration.value=scorces;
        /*if(scorces<=80&&scorces>50)
        {
            //调用改变呼吸和心跳的函数
            GameManager.Instance.BreathAndHeart(0);
        }
        else */
        if(scorces<=50)
        {
            GameManager.Instance.BreathAndHeart(1);
        }
        else if(scorces<=30)
        {
            GameManager.Instance.BreathAndHeart(2);
            //闭上眼睛
            CameraAwake.SetBool("isClose", true);
        }
    }
    //通关UI
    public void Clearace()
    {

        CleaanceUI.SetActive(true);
    }
}

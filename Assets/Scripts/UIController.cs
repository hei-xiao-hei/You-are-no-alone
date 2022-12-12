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
    }
    //通关UI
    public void Clearace()
    {
        CleaanceUI.SetActive(true);
    }
}

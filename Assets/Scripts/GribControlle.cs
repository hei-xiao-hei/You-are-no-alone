using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GribControlle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*//抓取到球，则显示通过UI
    public void isBalls()
    {
        //显示完成专注力训练UI
        UIController.uiController.Clearace();
    }*/
    //抓取到的不是球，则扣分
    public void NoBalls()
    {
        //减少专注力值
        UIController.uiController.DeductConcentration();
    }
}

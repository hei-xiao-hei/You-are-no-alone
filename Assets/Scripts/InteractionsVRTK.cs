using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class InteractionsVRTK : VRTK_ControllerEvents
{

    VRTK_InteractGrab VRTK_InteractGrab;
    public override void OnGripPressed(ControllerInteractionEventArgs e)
    {
        base.OnGripPressed(e);
    }
    // Start is called before the first frame update
    void Start()
    {
        VRTK_InteractGrab = this.gameObject.GetComponent<VRTK_InteractGrab>();
        VRTK_InteractGrab.ControllerGrabInteractableObject += VRTK_InteractGrab_ControllerGrabInteractableObject;
    }

    private void VRTK_InteractGrab_ControllerGrabInteractableObject(object sender, ObjectInteractEventArgs e)
    {
        Debug.Log("当前抓取的物体是："+sender);
        string gameobjecttype = sender.GetType().ToString();
        //如果抓取的物体是球，则跳转场景
        if(gameobjecttype=="Balls")
        {
            //显示完成专注力训练UI
            //UIController.uiController.Clearace();
        }
        else
        {
            //减少专注力值
            UIController.uiController.DeductConcentration();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

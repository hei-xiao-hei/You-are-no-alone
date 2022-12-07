using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class Speech : MonoBehaviour
{
    private const string app_id = "appid = 90211c66";//存储讯飞平台的ID

    /*public AudioSource audio;//存储录制的音频
    private int frequency = 16000;//采样率*/
    
    


    //登录
    private bool login(string my_appid)
    {
        //用户名、密码，登录信息，前两个均可空
        int res = MSCDLL.MSPLogin(null, null, my_appid);//登录my_appid的应用。
        if (res!=(int)Errors.MSP_SUCCESS)
        {
            Debug.Log("登录失败");
            Debug.Log(my_appid);
            Debug.Log("错误编号：" + res);
            return false;
        }
        Debug.Log("登录成功");
        return true;
    }
    /*      * QISRSessionBegin（）；
        * 功能：开始一次语音识别
        * 参数一：定义关键词识别||语法识别||连续语音识别（null）
        * 参数2：设置识别的参数：语言、领域、语言区域。。。。
        * 参数3：带回语音识别的结果，成功||错误代码
        * 返回值intPtr类型,后面会用到这个返回值  */

    private const string session_begin_params = "sub = iat, domain = iat, language = zh_cn, accent = mandarin, sample_rate = 16000, result_type = plain, result_encoding = utf-8";
    private IntPtr session_id;

    //建立会话
    private void sessionBegin(string session_begin_params)
    {
        int errcode = (int)Errors.MSP_SUCCESS;

        session_id = MSCDLL.QISRSessionBegin(null, session_begin_params, ref errcode);
        if (errcode != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("建立会话失败！");
            Debug.Log("错误编号: " + errcode);
        }
    }

    //写入音频
    /*
         QISRAudioWrite（）；
         功能：写入本次识别的音频
         参数1：之前已经得到的sessionID
         参数2：音频数据缓冲区起始地址
         参数3：音频数据长度,单位字节。
          参数4：用来告知MSC音频发送是否完成     MSP_AUDIO_SAMPLE_FIRST = 1第一块音频
                                                  MSP_AUDIO_SAMPLE_CONTINUE = 2还有后继音频
                                                   MSP_AUDIO_SAMPLE_LAST = 4最后一块音频
         参数5：端点检测（End-point detected）器所处的状态
                                                MSP_EP_LOOKING_FOR_SPEECH = 0还没有检测到音频的前端点。
                                                 MSP_EP_IN_SPEECH = 1已经检测到了音频前端点，正在进行正常的音频处理。
                                                 MSP_EP_AFTER_SPEECH = 3检测到音频的后端点，后继的音频会被MSC忽略。
                                                  MSP_EP_TIMEOUT = 4超时。
                                                 MSP_EP_ERROR = 5出现错误。
                                                 MSP_EP_MAX_SPEECH = 6音频过大。
         参数6：识别器返回的状态，提醒用户及时开始\停止获取识别结果
                                       MSP_REC_STATUS_SUCCESS = 0识别成功，此时用户可以调用QISRGetResult来获取（部分）结果。
                                        MSP_REC_STATUS_NO_MATCH = 1识别结束，没有识别结果。
                                      MSP_REC_STATUS_INCOMPLETE = 2正在识别中。
                                      MSP_REC_STATUS_COMPLETE = 5识别结束。
         返回值：函数调用成功则其值为MSP_SUCCESS，否则返回错误代码。
           本接口需不断调用，直到音频全部写入为止。上传音频时，需更新audioStatus的值。具体来说:
           当写入首块音频时,将audioStatus置为MSP_AUDIO_SAMPLE_FIRST
           当写入最后一块音频时,将audioStatus置为MSP_AUDIO_SAMPLE_LAST
           其余情况下,将audioStatus置为MSP_AUDIO_SAMPLE_CONTINUE
           同时，需定时检查两个变量：epStatus和rsltStatus。具体来说:
           当epStatus显示已检测到后端点时，MSC已不再接收音频，应及时停止音频写入
           当rsltStatus显示有识别结果返回时，即可从MSC缓存中获取结果*/
   /* private bool audio_iat(byte[] AudioData)
    {
        var aud_stat = AudioStatus.MSP_AUDIO_SAMPLE_CONTINUE;//音频状态
        var ep_stat = EpStatus.MSP_EP_LOOKING_FOR_SPEECH;//端点状态
        var rec_stat = RecogStatus.MSP_REC_STATUS_SUCCESS;//识别状态
        byte[] audio_content = AudioData;

        int res = MSCDLL.QISRAudioWrite(session_id, audio_content, (uint)audio_content.Length, aud_stat, ref ep_stat, ref rec_stat);
        if (res != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("写入音频失败！");
            Debug.Log("错误编号: " + res);
            return false;
        }
        //告知识别结束
        res = MSCDLL.QISRAudioWrite(session_id, null, 0, AudioStatus.MSP_AUDIO_SAMPLE_LAST, ref ep_stat, ref rec_stat);
        if (res != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("写入音频结束失败！");

            Debug.Log("错误编号: " + res);
             return false;
        }
        int errcode = (int)Errors.MSP_SUCCESS;
        StringBuilder result = new StringBuilder();//存储最终识别的结果
        int totalLength = 0;//用来记录总的识别后的结果的长度，判断是否超过缓存最大值


        while (RecogStatus.MSP_REC_STATUS_COMPLETE != rec_stat)
        {
            //如果没有完成就一直继续获取结果
            IntPtr now_result = MSCDLL.QISRGetResult(session_id, ref rec_stat, 0, ref errcode);
            if (errcode != (int)Errors.MSP_SUCCESS)
            {
                Debug.Log("获取结果失败：");
                Debug.Log("错误编号: " + errcode);
                     return false;
            }
            if (now_result != null)
            {
                int length = now_result.ToString().Length;
                totalLength += length;
                if (totalLength > 4096)
                {
                    Debug.Log("缓存空间不够" + totalLength);
                    return false;
                }
                result.Append(Marshal.PtrToStringAnsi(now_result));
            }
            Thread.Sleep(150);//防止频繁占用cpu
        }
        Debug.Log("语音听写结束，结果为： \n ");
        Debug.Log(result.ToString());
        //添加获取结果代码
        return true;
    }*/
    /*      
        QISRGetResult（）；
         功能：获取识别结果
         参数1：session，之前已获得
         参数2：识别结果的状态
         参数3：waitTime[in]此参数做保留用
         参数4：错误编码||成功
         返回值：函数执行成功且有识别结果时，返回结果字符串指针；其他情况(失败或无结果)返回NULL。
    */
    
    //会话结束
    private bool sessionEnd()
    {
        string hints = "hiahiahia";
        int res;

        res = MSCDLL.QISRSessionEnd(session_id, hints);
        if (res != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("会话结束失败！");
            Debug.Log("错误编号:" + res);
            return false;
        }
        return true;
    }
    //退出登录
    private bool logOut()
    {
        int res;

        res = MSCDLL.MSPLogout();
        if (res != (int)Errors.MSP_SUCCESS)
        {//说明登陆失败
            Debug.Log("退出登录失败！");
            Debug.Log("错误编号:" + res);
            return false;
        }
        Debug.Log("退出登录成功！");
        return true;
    }
    //public AudioSource audio; //音频源

    void Start()
    {
        login(app_id);
        
    }

    //按下手柄扳机键触发该函数
    public void StartGrab()
    {
        Debug.Log("我按下了扳机键");
        sessionBegin(session_begin_params);//建立会话
    }

    //松开手柄扳机键触发该函数
    public void EndGrab()
    {
        Debug.Log("松开了按键");
        sessionEnd();//结束会话
    }
    private void OnDestroy()
    {
        logOut();
    }
}

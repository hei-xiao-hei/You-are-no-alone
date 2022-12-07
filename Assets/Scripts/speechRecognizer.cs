using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

public class speechRecognizer : MonoBehaviour
{
    private const string app_id = "appid = 90211c66";
    private const string session_begin_params = "sub = iat, domain = iat, language = zh_cn, accent = mandarin, sample_rate = 16000, result_type = plain, result_encoding = utf-8";
    private IntPtr session_id;

    public AudioSource audios; //存储录制的音频
    private int frequency = 16000;//采样率

    // Start is called before the first frame update
    void Start()
    {
        login(app_id);//登录
    }

    private void OnDestroy()
    {
        logOut();//退出登录
    }
    
    //登录
    private bool login(string my_appid)
    {
        //用户名，密码，登陆信息，前两个均为空
        int res = MSCDLL.MSPLogin(null, null, my_appid);
        if (res != (int)Errors.MSP_SUCCESS) //说明登陆失败
        {
            Debug.Log("登陆失败！");
            Debug.Log(my_appid);
            Debug.Log("错误编号: " + res);
            return false;
        }

        Debug.Log("登陆成功！");
        return true;
    }


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

    //结束对话
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


    //将二进制字节转换成音频
    private bool audio_iat(byte[] AudioData)
    {
        var aud_stat = AudioStatus.MSP_AUDIO_SAMPLE_CONTINUE;//音频状态
        var ep_stat = EpStatus.MSP_EP_LOOKING_FOR_SPEECH;//端点状态
        var rec_stat = RecogStatus.MSP_REC_STATUS_SUCCESS;//识别状态
        byte[] audio_content = AudioData;

        //写入音频
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
            Debug.Log("写入音频结束失败！" + res);
            Debug.Log("错误编号: " + res);
            return false;
        }

        int errcode = (int)Errors.MSP_SUCCESS;
        StringBuilder result = new StringBuilder();//存储最终识别的结果
        int totalLength = 0;//用来记录总的识别后的结果的长度，判断是否超过缓存最大值

        while (RecogStatus.MSP_REC_STATUS_COMPLETE != rec_stat)
        {//如果没有完成就一直继续获取结果
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
        return true;
    }

    //将音频转换成二进制的字节
    private byte[] convertClipToBytes(AudioClip clip)
    {
        //clip.length;
        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        byte[] bytesData = new byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        return bytesData;
    }


    /*private void OnMouseDown()
    {
        audios.clip = Microphone.Start(null, false, 5, frequency);//开始录制音频,5秒
        Thread.Sleep(5000);//5秒后停止录音
        byte[] audioData = convertClipToBytes(audios.clip);//将录音转换成字节
        sessionBegin(session_begin_params);//建立会话
        audio_iat(audioData);//将字节转换成音频
        audios.Play();//播放音频
        sessionEnd();//结束对话
    }*/
    //识别玩家录音并输出播放录制玩家语音
    public void SoundRecord()
    {
        audios.clip = Microphone.Start(null, false, 5, frequency);//开始录制音频,5秒
        //Thread.Sleep(5000);//5秒后停止录音
        StartCoroutine("latetimego");
    }
    //延时执行
    IEnumerator latetimego()
    {
        yield return new WaitForSeconds(5.0f);
        byte[] audioData = convertClipToBytes(audios.clip);//将录音转换成字节
        sessionBegin(session_begin_params);//建立会话
        audio_iat(audioData);//将字节转换成音频
        audios.Play();//播放音频
        sessionEnd();//结束对话
    }

    
    
}

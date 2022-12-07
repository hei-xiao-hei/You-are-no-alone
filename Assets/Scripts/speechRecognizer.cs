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

    public AudioSource audios; //�洢¼�Ƶ���Ƶ
    private int frequency = 16000;//������

    // Start is called before the first frame update
    void Start()
    {
        login(app_id);//��¼
    }

    private void OnDestroy()
    {
        logOut();//�˳���¼
    }
    
    //��¼
    private bool login(string my_appid)
    {
        //�û��������룬��½��Ϣ��ǰ������Ϊ��
        int res = MSCDLL.MSPLogin(null, null, my_appid);
        if (res != (int)Errors.MSP_SUCCESS) //˵����½ʧ��
        {
            Debug.Log("��½ʧ�ܣ�");
            Debug.Log(my_appid);
            Debug.Log("������: " + res);
            return false;
        }

        Debug.Log("��½�ɹ���");
        return true;
    }


    //�����Ự
    private void sessionBegin(string session_begin_params)
    {
        int errcode = (int)Errors.MSP_SUCCESS;

        session_id = MSCDLL.QISRSessionBegin(null, session_begin_params, ref errcode);
        if (errcode != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("�����Ựʧ�ܣ�");
            Debug.Log("������: " + errcode);
        }
    }

    //�����Ի�
    private bool sessionEnd()
    {
        string hints = "hiahiahia";
        int res;

        res = MSCDLL.QISRSessionEnd(session_id, hints);
        if (res != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("�Ự����ʧ�ܣ�");
            Debug.Log("������:" + res);
            return false;
        }
        return true;
    }


    //�˳���¼
    private bool logOut()
    {
        int res;

        res = MSCDLL.MSPLogout();
        if (res != (int)Errors.MSP_SUCCESS)
        {//˵����½ʧ��
            Debug.Log("�˳���¼ʧ�ܣ�");
            Debug.Log("������:" + res);
            return false;
        }
        Debug.Log("�˳���¼�ɹ���");
        return true;
    }


    //���������ֽ�ת������Ƶ
    private bool audio_iat(byte[] AudioData)
    {
        var aud_stat = AudioStatus.MSP_AUDIO_SAMPLE_CONTINUE;//��Ƶ״̬
        var ep_stat = EpStatus.MSP_EP_LOOKING_FOR_SPEECH;//�˵�״̬
        var rec_stat = RecogStatus.MSP_REC_STATUS_SUCCESS;//ʶ��״̬
        byte[] audio_content = AudioData;

        //д����Ƶ
        int res = MSCDLL.QISRAudioWrite(session_id, audio_content, (uint)audio_content.Length, aud_stat, ref ep_stat, ref rec_stat);
        if (res != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("д����Ƶʧ�ܣ�");
            Debug.Log("������: " + res);
            return false;
        }
        //��֪ʶ�����
        res = MSCDLL.QISRAudioWrite(session_id, null, 0, AudioStatus.MSP_AUDIO_SAMPLE_LAST, ref ep_stat, ref rec_stat);
        if (res != (int)Errors.MSP_SUCCESS)
        {
            Debug.Log("д����Ƶ����ʧ�ܣ�" + res);
            Debug.Log("������: " + res);
            return false;
        }

        int errcode = (int)Errors.MSP_SUCCESS;
        StringBuilder result = new StringBuilder();//�洢����ʶ��Ľ��
        int totalLength = 0;//������¼�ܵ�ʶ���Ľ���ĳ��ȣ��ж��Ƿ񳬹��������ֵ

        while (RecogStatus.MSP_REC_STATUS_COMPLETE != rec_stat)
        {//���û����ɾ�һֱ������ȡ���
            IntPtr now_result = MSCDLL.QISRGetResult(session_id, ref rec_stat, 0, ref errcode);
            if (errcode != (int)Errors.MSP_SUCCESS)
            {
                Debug.Log("��ȡ���ʧ�ܣ�");
                Debug.Log("������: " + errcode);
                return false;
            }
            if (now_result != null)
            {
                int length = now_result.ToString().Length;
                totalLength += length;
                if (totalLength > 4096)
                {
                    Debug.Log("����ռ䲻��" + totalLength);
                    return false;
                }
                result.Append(Marshal.PtrToStringAnsi(now_result));
            }
            Thread.Sleep(150);//��ֹƵ��ռ��cpu
        }
        Debug.Log("������д���������Ϊ�� \n ");
        Debug.Log(result.ToString());
        return true;
    }

    //����Ƶת���ɶ����Ƶ��ֽ�
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
        audios.clip = Microphone.Start(null, false, 5, frequency);//��ʼ¼����Ƶ,5��
        Thread.Sleep(5000);//5���ֹͣ¼��
        byte[] audioData = convertClipToBytes(audios.clip);//��¼��ת�����ֽ�
        sessionBegin(session_begin_params);//�����Ự
        audio_iat(audioData);//���ֽ�ת������Ƶ
        audios.Play();//������Ƶ
        sessionEnd();//�����Ի�
    }*/
    //ʶ�����¼�����������¼���������
    public void SoundRecord()
    {
        audios.clip = Microphone.Start(null, false, 5, frequency);//��ʼ¼����Ƶ,5��
        //Thread.Sleep(5000);//5���ֹͣ¼��
        StartCoroutine("latetimego");
    }
    //��ʱִ��
    IEnumerator latetimego()
    {
        yield return new WaitForSeconds(5.0f);
        byte[] audioData = convertClipToBytes(audios.clip);//��¼��ת�����ֽ�
        sessionBegin(session_begin_params);//�����Ự
        audio_iat(audioData);//���ֽ�ת������Ƶ
        audios.Play();//������Ƶ
        sessionEnd();//�����Ի�
    }

    
    
}

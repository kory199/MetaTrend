using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioMGR : MonoBehaviour
{
    // Clip ī�װ� �з��� ���� Enum
    public enum Type { Background, Unit, UI };

    // Type �� Audio Clip �з�
    [SerializeField] AudioClip[] BackGroundClip = null;
    [SerializeField] AudioClip[] UnitSFXClip = null;
    [SerializeField] AudioClip[] UISFXClip = null;

    AudioClip audioClip = null;

    // AudioClip Name, AudioClip���� Dictionary ����
    Dictionary<string, AudioClip> BackgroundDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> UnitSFXDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> UISFXDic = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Init();
    }

    //  AudioClip Name�� Ű, AudioClip�� ������ Dictionary�� �߰� 
    private void Init()
    {
        if (BackGroundClip != null)
        {
            for (int i = 0; i < BackGroundClip.Length; i++)
            {
                BackgroundDic.Add(BackGroundClip[i].name, BackGroundClip[i]);
            }
        }

        if (UnitSFXClip != null)
        {
            for (int i = 0; i < UnitSFXClip.Length; i++)
            {
                UnitSFXDic.Add(UnitSFXClip[i].name, UnitSFXClip[i]);
            }
        }

        if (UISFXClip != null)
        {
            for (int i = 0; i < UISFXClip.Length; i++)
            {
                UISFXDic.Add(UISFXClip[i].name, UISFXClip[i]);
            }
        }
    }

    // Ÿ Ŭ�������� �Լ� ȣ�� �� Type, ClipName�� �´� AudioClip ��ȯ
    public AudioClip ReturnAudioClip(Type AudioType, string clipName)
    {
        switch (AudioType.ToString())
        {
            case "Background":
                audioClip = BackgroundDic[clipName];
                break;
            case "Unit":
                audioClip = UnitSFXDic[clipName];
                break;
            case "UI":
                audioClip = UISFXDic[clipName];
                break;
        }
        return audioClip;
    }

    // ����ڴ� �ش� �Լ��� �����Ŭ���� �����ϰ� �÷��̱���
    public void PlaySound(bool loop, AudioClip clip)
    {

    }
}

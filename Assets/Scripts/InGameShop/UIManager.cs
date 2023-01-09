using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    [SerializeField] Text goldTXT = null;
    [SerializeField] Text notGoldTXT = null;

    [SerializeField] public GameObject sell = null;
    [SerializeField] Text sellTxt = null;

    public int goldCount = 10;

    // Public ������Ƽ�� �����ؼ� �ܺο��� private ��������� ���ٸ� �����ϰ� ����
    public static UIManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        sell.gameObject.SetActive(false);
        sellTxt.gameObject.SetActive(false);

        if (null == instance)
        {
            // �� ���۵ɶ� �ν��Ͻ� �ʱ�ȭ, ���� �Ѿ���� �����Ǳ����� ó��
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // instance��, GameManager�� �����Ѵٸ� GameObject ���� 
            Destroy(this.gameObject);
        }
        notGoldTXT.gameObject.SetActive(false);
    }

    private void Update()
    {
        goldTXT.text = "Gold : " + goldCount.ToString();
        if(sell.activeSelf == false)
        {
            sellTxt.gameObject.SetActive(false);
        }
        else
        {
            sellTxt.gameObject.SetActive(true);
        }
    }

    public IEnumerator COR_NotGold()
    {
        notGoldTXT.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        notGoldTXT.gameObject.SetActive(false);
    }
}

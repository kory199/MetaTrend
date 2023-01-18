using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{

    [Header("�����̴�")]
    [SerializeField] Slider timerSlider = null;
    [Header("��ư")]
    public Button reFreshButton = null;
    public Button levelUpButton = null;
    [SerializeField] Button infoButton = null;
    [SerializeField] Button optionButton = null;
    [Header("�ؽ�Ʈ")]
    [SerializeField] Text NowShopLevelTXT = null;
    [SerializeField] Text goldTXT = null;
    public Text shopLevelTXT = null;
    [SerializeField] Text sellTXT = null;
    [SerializeField] Text timerTXT = null;
    [SerializeField] public GameObject sell = null;

    public int shopMoney = 0;
    public int goldCount = 10;
    public int shopLevel = 1;
    public float timer = 60f;
    private bool isScene;


    private void Update()
    {
        if (!isScene) return;
        // �� ���϶��� 

        // �ð��� ������ ��ŭ slider Value ������ �մϴ�.
        timer -= Time.deltaTime;
        timerSlider.value = timer;
        timerTXT.text = string.Format("Timer : {0:N0}", timer);

        goldTXT.text = "Gold : " + goldCount.ToString();
        shopLevelTXT.text = "Shop Gold :" + shopMoney.ToString();
        NowShopLevelTXT.text = "Shop Level :" + shopLevel.ToString();

        if (sell.activeSelf == false)
        {
            sellTXT.gameObject.SetActive(false);
        }
        else
        {
            sellTXT.gameObject.SetActive(true);
        }
    }

    public void Init_Scene2()
    {
        timerSlider.maxValue = timer;
        sell.gameObject.SetActive(false);
        sellTXT.gameObject.SetActive(false);
    }
}

//41 23 56 70
//4 23 5 70
//42 31 47 60
//40 27 35
//0
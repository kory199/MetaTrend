using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{

    [Header("�����̴�")]
    [SerializeField] public Slider timerSlider = null;
    [Header("��ư")]
    [SerializeField] public Button reFreshButton = null;
    [SerializeField]public Button levelUpButton = null;
    [SerializeField]public Button infoButton = null;
    [SerializeField]public Button optionButton = null;
    [Header("�ؽ�Ʈ")]
    [SerializeField]public Text NowShopLevelTXT = null;
    [SerializeField]public Text goldTXT = null;
    [SerializeField]public Text shopLevelTXT = null;
    [SerializeField]public Text sellTXT = null;
    [SerializeField]public Text timerTXT = null;
    [SerializeField]public GameObject sell = null;

    public int shopMoney = 0;
    public int goldCount = 10;
    public int shopLevel = 1;
    public float timer = 60f;
    private bool isScene;

    public void Init_Scene2()
    {
        timerSlider = GameObject.Find("TimerSlider").GetComponent<Slider>();
        reFreshButton = GameObject.Find("ReFreshButton").GetComponent<Button>();
        levelUpButton = GameObject.Find("LevelUPButton").GetComponent<Button>();
        infoButton = GameObject.Find("InfoButton").GetComponent<Button>();
        optionButton = GameObject.Find("OptionButton").GetComponent<Button>();
        NowShopLevelTXT = GameObject.Find("NowShopLevelTXT").GetComponent<Text>();
        goldTXT = GameObject.Find("GoldTXT").GetComponent<Text>();
        shopLevelTXT = GameObject.Find("ShopLevelUpTXT").GetComponent<Text>();
        sellTXT = GameObject.Find("SellTXT").GetComponent<Text>();
        timerTXT = GameObject.Find("TimerTXT").GetComponent<Text>();
        sell = GameObject.Find("Sell");

        timerSlider.maxValue = timer;
        sell.gameObject.SetActive(false);
        sellTXT.gameObject.SetActive(false);
    }



    private void Update()
    {
        if (!isScene) return;

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
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    [Header("버튼")]
    public Button ReFreshButton = null;
    public Button LevelUpButton = null;
    [Header("텍스트")]
    [SerializeField] Text NowShopLevelTXT = null;
    [SerializeField] Text goldTXT = null;
    public Text shopLevelTXT = null;
    [SerializeField] Text sellTxt = null;

    [SerializeField] public GameObject sell = null;

    public int shopMoney = 0;
    public int goldCount = 10;
    public int shopLevel = 1;

    // Public 프로퍼티로 선언해서 외부에서 private 멤버변수에 접근만 가능하게 구현
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
        // 처음 시작시 초기화
        FirstReset();

        if (null == instance)
        {
            // 씬 시작될때 인스턴스 초기화, 씬을 넘어갈때도 유지되기위한 처리
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // instance가, GameManager가 존재한다면 GameObject 제거 
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        goldTXT.text = "Gold : " + goldCount.ToString();
        shopLevelTXT.text = "Shop Level :" + shopMoney.ToString();
        NowShopLevelTXT.text = "Shop Level :" + shopLevel.ToString();

        if(sell.activeSelf == false)
        {
            sellTxt.gameObject.SetActive(false);
        }
        else
        {
            sellTxt.gameObject.SetActive(true);
        }
    }



    private void FirstReset()
    {
        sell.gameObject.SetActive(false);
        sellTxt.gameObject.SetActive(false);
    }
}

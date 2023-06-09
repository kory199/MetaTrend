using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum faidType
{
    Out,
    In,
}
public partial class UIManager : MonoBehaviour
{
    [Header("Pannel")]
    public GameObject cardPannel;
    private GameObject lobbyPannel;
    private GameObject myDeckPannel;
    private GameObject packChoicePannel;
    private GameObject customPannel;
    private GameObject menuPannel;
    private GameObject nameMakeUI;
    private GameObject deleteWarringUI;
    public GameObject loginSystemUI;
    public GameObject blackUI;
    private GameObject logoPannel;

    public TextMeshProUGUI tournamentText;
    public bool isLobby = true;

    [Header("PackList")]
    [SerializeField] private MyDeck packButton;
    private GameObject myPackList;
    private GameObject packAddButton;
    private ToggleGroup toggleGroup;
    [Header("UserProfile")]
    public TextMeshProUGUI userName;
    public Image userImage;



    [Header("CardInfo")]
    private Image cardImage;
    private TextMeshProUGUI cardName;
    private TextMeshProUGUI attackValue;
    private TextMeshProUGUI hpValue;
    private TextMeshProUGUI myDeckName;
    private TextMeshProUGUI[] skillExplantion;
    private GameObject[] star;

    [Header("Tier")]
    public Transform myContent;
    public Transform[] tier1;
    public TextMeshProUGUI[] tierCountText;
    public void Init_Scene1()
    {
        userName = GameObject.Find("UserName").GetComponent<TextMeshProUGUI>();
        userImage= GameObject.Find("UserImage").GetComponent<Image>();
        loginSystemUI = GameObject.Find("LoginSystem");
        blackUI = GameObject.Find("BlackUI");
        logoPannel = GameObject.Find("LogoPannel");
        lobbyPannel = GameObject.Find("LobbyPannel");
        myDeckPannel = GameObject.Find("MyDeckPannel");
        customPannel = GameObject.Find("CustomPannel");
        cardPannel = GameObject.Find("CardPannel");
        menuPannel = GameObject.Find("MenuPannel");
        deleteWarringUI = GameObject.Find("DeleteWarringUI");
        packAddButton = GameObject.Find("PackAddButton");
        packChoicePannel = GameObject.Find("PackChoicePannel");
        myPackList = GameObject.Find("PackList");
        nameMakeUI = GameObject.Find("NameMakeUI");
        cardImage = GameObject.Find("CardImage").GetComponent<Image>();
        cardName = GameObject.Find("UNITNAME").GetComponent<TextMeshProUGUI>();
        attackValue = GameObject.Find("UNITATKValue").GetComponent<TextMeshProUGUI>();
        myDeckName = GameObject.Find("MyDeckName").GetComponent<TextMeshProUGUI>();
        hpValue = GameObject.Find("UNITHPValue").GetComponent<TextMeshProUGUI>();
        skillExplantion = new TextMeshProUGUI[3];
        tier1 = new Transform[6];
        star = new GameObject[6];
        tierCountText = new TextMeshProUGUI[6];
        skillExplantion[0] = GameObject.Find("Skillexplanation1").GetComponent<TextMeshProUGUI>();
        skillExplantion[1] = GameObject.Find("Skillexplanation2").GetComponent<TextMeshProUGUI>();
        skillExplantion[2] = GameObject.Find("Skillexplanation3").GetComponent<TextMeshProUGUI>();
        myContent = GameObject.Find("MyContent").transform;
        tier1[0] = GameObject.Find("ContentTier1").transform;
        tier1[1] = GameObject.Find("ContentTier2").transform;
        tier1[2] = GameObject.Find("ContentTier3").transform;
        tier1[3] = GameObject.Find("ContentTier4").transform;
        tier1[4] = GameObject.Find("ContentTier5").transform;
        tier1[5] = GameObject.Find("ContentTier6").transform;
        star[0] = GameObject.Find("oneStar");
        star[1] = GameObject.Find("twoStar");
        star[2] = GameObject.Find("threeStar");
        star[3] = GameObject.Find("fourStar");
        star[4] = GameObject.Find("fiveStar");
        star[5] = GameObject.Find("sixStar");
        tierCountText[0] = GameObject.Find("1").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tierCountText[1] = GameObject.Find("2").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tierCountText[2] = GameObject.Find("3").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tierCountText[3] = GameObject.Find("4").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tierCountText[4] = GameObject.Find("5").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tierCountText[5] = GameObject.Find("6").transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        tournamentText = GameObject.Find("MonthlyTime").GetComponent<TextMeshProUGUI>();
        toggleGroup = FindObjectOfType<ToggleGroup>();
        deleteWarringUI.SetActive(false);
        customPannel.SetActive(false);
        loginSystemUI.SetActive(false);
        packChoicePannel.SetActive(false);
        lobbyPannel.SetActive(false);
        cardPannel.SetActive(false);
        myDeckPannel.SetActive(false);
        nameMakeUI.SetActive(false);
        menuPannel.SetActive(false);
        blackUI.SetActive(false);
        SetFalseStar(0);
        StartCoroutine(COR_FaidDelay());

        isLobby = true;
        StartCoroutine(GameMGR.Instance.metaTrendAPI.processRequestGetDummy());
    }
    IEnumerator COR_FaidDelay()
    {
        Faid(logoPannel, faidType.Out, 0.02f);
        yield return new WaitForSeconds(2f);
        Faid(lobbyPannel, faidType.In, 0.03f);

    }
    public void OnClick_Move_Matching()
    {
        if (GameMGR.Instance.myCustomDeck != null)
        {
            GameMGR.Instance.photonLauncher.OnClick_Join_Room();
            // SceneManager.LoadScene("StoreScene");
        }
        else Debug.Log("������");
    }
    public void SetFalseStar(int set)
    {
        for (int i = 0; i < star.Length; i++)
        {
            if (set == i) star[i].SetActive(true);
            else star[i].SetActive(false);
        }
    }
    public void SetNameMakeUI(bool set)
    {
        nameMakeUI.SetActive(set);

    }
    public void SetMyDeckName(string name)
    {
        myDeckName.text = name;
    }
    public void OnCilck_Join_PackChoice()
    {
        lobbyPannel.SetActive(false);
        packChoicePannel.SetActive(true);
    }
    public void CreateMyPackButton(CustomDeck customDeck)
    {
        MyDeck obj = GameObject.Instantiate<MyDeck>(packButton);
        obj.transform.GetChild(3).GetComponent<Toggle>().group = toggleGroup;
        if(customDeck.DeckName=="Free Pack")obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"FreePack");
        obj.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = customDeck.DeckName;
        obj.SetMyDeck(customDeck);
        obj.transform.SetParent(myPackList.transform);
        obj.transform.localScale = Vector3.one;
        GameMGR.Instance.uiManager.SetParentPackAddButton();
    }

    public void SetParentPackAddButton()
    {
        packAddButton.transform.SetParent(null);
        packAddButton.transform.SetParent(myPackList.transform);
        packAddButton.transform.localScale = Vector3.one;
    }
    public void OnClick_Join_Custom()
    {
        GameMGR.Instance.customDeckShop.OnClick_Join_CustomDeckShop();
        packChoicePannel.SetActive(false);
        customPannel.SetActive(true);
    }
    public void OnClick_Join_MyDeckInfo()
    {
        myDecks = FindObjectsOfType<MyDeck>();
        packChoicePannel.SetActive(false);
        myDeckPannel.SetActive(true);
    }
    public void OnClick_Move_Home()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        packChoicePannel.SetActive(false);
        myDeckPannel.SetActive(false);
        lobbyPannel.SetActive(true);
        cardPannel.SetActive(false);
        menuPannel.SetActive(false);
    }
    public MyDeck[] myDecks;
    public void OnClick_Move_Back()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        cardPannel.SetActive(false);
        nameMakeUI.SetActive(false);
        if (myDeckPannel.activeSelf)
        {
            for (int i = 0; i < myDecks.Length; i++) myDecks[i].DelateMyDeckList();
            myDeckPannel.SetActive(false);
        }
        packChoicePannel.SetActive(true);
        if (customPannel.activeSelf)
        {
            GameMGR.Instance.customDeckShop.ClearCustomDeckList();
            customPannel.SetActive(false);
        }
    }
    public void OnPointerEnter_CardInfo(CardInfo cardInfo)
    {
        cardPannel.SetActive(true);
        SetFalseStar(cardInfo.tier - 1);
        attackValue.text = $"{cardInfo.atk}";
        hpValue.text = $"{cardInfo.hp}";
        cardName.text = $"{cardInfo.objName}";
        cardImage.sprite = Resources.Load<Sprite>($"Sprites/Nomal/{cardInfo.objName.Replace(" ", "")}");
        skillExplantion[0].text = cardInfo.GetSkillExplantion(1);
        skillExplantion[1].text = cardInfo.GetSkillExplantion(2);
        skillExplantion[2].text = cardInfo.GetSkillExplantion(3);

    }
    public void OnClick_Join_Menu()
    {
        lobbyPannel.SetActive(false);
        menuPannel.SetActive(true);
    }
    GameObject myDeck;
    int myDeckNum;
    public void OnClick_Set_DeleteUI(GameObject deck,int deckNum)
    {
        myDeck = deck;
        myDeckNum= deckNum;
        deleteWarringUI.SetActive(true);
    }
    public void OnClick_Set_DeleteUIFalse()
    {
        deleteWarringUI.SetActive(false);

    }
    public void OnClick_Delete_MyDeck()
    {
        deleteWarringUI.SetActive(false);
        GameMGR.Instance.dataBase.inventoryData.DeleteCustomDeck(myDeckNum);
        Destroy(myDeck);
    }
    
    public void Faid(GameObject obj, faidType type,float time)
    {
        obj.SetActive(true);
        faidTime = new WaitForSeconds(time);
        if (type == faidType.In) StartCoroutine(COR_FaidIn(obj));
        else if(type == faidType.Out) StartCoroutine(COR_FaidOut(obj));
    }
    WaitForSeconds faidTime = new WaitForSeconds(0.02f);
    IEnumerator COR_FaidIn(GameObject obj)
    {
        obj.TryGetComponent(out CanvasGroup canvasGroup);
        if (canvasGroup == null) canvasGroup = obj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        while(canvasGroup.alpha<1)
        {
            canvasGroup.alpha += 0.03f;
            yield return faidTime;
        }

    }
    IEnumerator COR_FaidOut(GameObject obj)
    {
        obj.TryGetComponent(out CanvasGroup canvasGroup);
        if (canvasGroup == null) canvasGroup = obj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 0.03f;
            yield return faidTime;
        }
        obj.SetActive(false);

    }



}

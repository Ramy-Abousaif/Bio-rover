using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance { get; private set; }

    public bool inStore = false;
    public GameObject store;

    public TMP_Text creditsUI;
    public StoreItem[] storeItems;
    public GameObject[] storePanelsGO;
    public StoreTemplate[] storePanels;
    public Button[] purchaseBtns;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StoreReset();

        for (int i = 0; i < storePanels.Length; i++)
            storePanelsGO[i].SetActive(true);

        LoadPanels();
        CheckPurchasable();
        store.SetActive(false);
    }

    public void LoadPanels()
    {
        for (int i = 0; i < storeItems.Length; i++)
        {
            storePanels[i].titleTxt.text = storeItems[i].title;
            storePanels[i].descriptionTxt.text = storeItems[i].description;
            storePanels[i].costTxt.text = storeItems[i].baseCost.ToString() + " Credits";
            storePanels[i].capacityTxt.text = storeItems[i].currentProgress + "/" + storeItems[i].capacity.ToString();
        }
    }

    public void AddCredit(int _credits)
    {
        ShowAddedCreditsText(_credits);
        GameManager.instance.credits += _credits;
        creditsUI.text = "Credits: " + GameManager.instance.credits.ToString();
        UIManager.instance.creditsText.text = GameManager.instance.credits.ToString() + " Credits";
        CheckPurchasable();
    }

    public void ShowAddedCreditsText(int _credits)
    {
        GameObject text = Instantiate(UIManager.instance.creditAddTextPrefab, UIManager.instance.creditAddTextPos.position, Quaternion.identity, UIManager.instance.inGame.transform);
        text.layer = LayerMask.NameToLayer("UI");
        text.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("UI");
        text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "+" + _credits;
    }

    public void CheckPurchasable()
    {
        creditsUI.text = "Credits: " + GameManager.instance.credits.ToString();

        for (int i = 0; i < storeItems.Length; i++)
        {
            storePanels[i].UpdateProgress(i);

            if (GameManager.instance.credits >= storeItems[i].baseCost && storeItems[i].currentProgress < storeItems[i].capacity)
                purchaseBtns[i].interactable = true;
            else
                purchaseBtns[i].interactable = false;
        }

        LoadPanels();
    }

    public void PurchaseItem(int btnNo)
    {
        if(GameManager.instance.credits >= storeItems[btnNo].baseCost)
        {
            GameManager.instance.credits -= storeItems[btnNo].baseCost;
            creditsUI.text = "Credits: " + GameManager.instance.credits.ToString();
            UIManager.instance.creditsText.text = GameManager.instance.credits.ToString() + " Credits";
            storePanels[btnNo].Effect(btnNo);
            CheckPurchasable();
            storePanels[btnNo].capacityTxt.text = storeItems[btnNo].currentProgress + "/" + storeItems[btnNo].capacity.ToString();
        }
    }

    public void CheckStore()
    {
        inStore = true;
        PlayerManager.instance.bc.enabled = false;
        store.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CheckPurchasable();
    }

    public void ExitStore()
    {
        inStore = false;
        PlayerManager.instance.bc.enabled = true;
        store.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnApplicationQuit()
    {
        StoreReset();
    }

    public void StoreReset()
    {
        foreach (var item in storeItems)
        {
            item.currentProgress = 0;
        }
    }
}

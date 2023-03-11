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
            storePanels[i].capacityTxt.text = storeItems[i].capacity.ToString();
        }
    }

    public void AddCredit(int _credits)
    {
        GameManager.instance.credits += _credits;
        creditsUI.text = "Credits: " + GameManager.instance.credits.ToString();
        UIManager.instance.creditsText.text = GameManager.instance.credits.ToString() + " Credits";
        CheckPurchasable();
    }

    public void CheckPurchasable()
    {
        for (int i = 0; i < storeItems.Length; i++)
        {
            if (GameManager.instance.credits >= storeItems[i].baseCost)
                purchaseBtns[i].interactable = true;
            else
                purchaseBtns[i].interactable = false;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if(GameManager.instance.credits >= storeItems[btnNo].baseCost)
        {
            GameManager.instance.credits -= storeItems[btnNo].baseCost;
            creditsUI.text = "Credits: " + GameManager.instance.credits.ToString();
            UIManager.instance.creditsText.text = GameManager.instance.credits.ToString() + " Credits";
            CheckPurchasable();
            // Trigger unique function here
        }
    }

    public void CheckStore()
    {
        inStore = true;
        store.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CheckPurchasable();
    }

    public void ExitStore()
    {
        inStore = false;
        store.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
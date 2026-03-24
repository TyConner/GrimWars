using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]

    [SerializeField] private Vector3 cursorOffset = new Vector3(0f, 20f, 0f);
    [SerializeField] TMP_Text potionText;
    [SerializeField] TMP_Text hiPotionText;
    [SerializeField] TMP_Text etherText;
    [SerializeField] TMP_Text hiEtherText;
    [SerializeField] TMP_Text elixirText;

    [SerializeField] RectTransform cursor;
    [SerializeField] RectTransform[] itemSlots;
    [SerializeField] InputActionReference nextItem;
    [SerializeField] InputActionReference previousItem;
    [SerializeField] InputActionReference useItem;

    PlayerController player;
    int currentItemIndex = 0;

    Dictionary<Items, int> inventory = new Dictionary<Items, int>();

    Dictionary<Items, int> maxInventory = new Dictionary<Items, int>()
{
    {Items.Potion, 3},
    {Items.HiPotion, 1},
    {Items.Ether, 3},
    {Items.HiEther, 1},
    {Items.Elixir, 1}
};

    List<Items> itemOrder = new List<Items>()
{
    Items.Potion,
    Items.HiPotion,
    Items.Ether,
    Items.HiEther,
    Items.Elixir
};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();

        foreach (Items item in itemOrder)
        {
            inventory[item] = 0;
        }
        UpdateCursor();
        InitItemsText();
    }
    void InitItemsText()
    {
        potionText.text = "0";
        hiPotionText.text = "0";
        etherText.text = "0";
        hiEtherText.text = "0";
        elixirText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if (nextItem.action.WasPressedThisFrame())
        {
            currentItemIndex++;

            if (currentItemIndex >= itemSlots.Length)
                currentItemIndex = 0;

            UpdateCursor();
        }

        if (previousItem.action.WasPressedThisFrame())
        {
            currentItemIndex--;

            if (currentItemIndex < 0)
                currentItemIndex = itemSlots.Length - 1;

            UpdateCursor();
        }

        if (useItem.action.WasPressedThisFrame())
        {
            UseItem();
        }
    }

    public void AddItem(Items item)
    {
        if (inventory[item] >= maxInventory[item])
            return;

        inventory[item]++;
        UpdateInventoryUI();
    }

    void UseItem()
    {
        Items item = itemOrder[currentItemIndex];

        if (inventory[item] <= 0)
            return;

        inventory[item]--;

        switch (item)
        {
            case Items.Potion:
                player.Heal(25);
                break;

            case Items.HiPotion:
                player.Heal(50);
                break;

            case Items.Ether:
                player.RestoreMana(25);
                break;

            case Items.HiEther:
                player.RestoreMana(50);
                break;

            case Items.Elixir:
                player.Heal(50);
                player.RestoreMana(50);
                break;
        }

        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        potionText.text = inventory[Items.Potion].ToString();
        hiPotionText.text = inventory[Items.HiPotion].ToString();
        etherText.text = inventory[Items.Ether].ToString();
        hiEtherText.text = inventory[Items.HiEther].ToString();
        elixirText.text = inventory[Items.Elixir].ToString();

        UpdateCursor();
    }

    void UpdateCursor()
    {
        if (cursor == null || itemSlots.Length == 0)
            return;

        cursor.position = itemSlots[currentItemIndex].position + cursorOffset;
    }

    void OnEnable()
    {
        nextItem.action.Enable();
        previousItem.action.Enable();
        useItem.action.Enable();
    }

    void OnDisable()
    {
        nextItem.action.Disable();
        previousItem.action.Disable();
        useItem.action.Disable();
    }
}

public enum Items
{
    Potion,
    HiPotion,
    Ether,
    HiEther,
    Elixir
}
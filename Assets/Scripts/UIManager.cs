using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    private void Awake() { i = this; }

    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _shieldSlider;
    [SerializeField] private Slider _bossSlider;
    [SerializeField] private Image _hpHaze;

    [SerializeField] private Color _parryColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _reloadingColor;

    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _healthValueText;

    [Header("Inventory")]
    [SerializeField] private List<ItemSlot> _itemSlots = new List<ItemSlot>();
    [SerializeField] private GameObject _newItemParent;
    [SerializeField] private GameObject _keepButton;
    [SerializeField] private TextMeshProUGUI _newItemName;
    [SerializeField] private Image _newItemSprite;
    [SerializeField] private TextMeshProUGUI _newItemDescription;
    [SerializeField] private TextMeshProUGUI _newItemQuote;
    [HideInInspector] public int MaxItems => _itemSlots.Count;

    [SerializeField, ReadOnly] private ItemData _newItem;

    private void Start()
    {
        GameManager.i.Player.OnHealthChange.AddListener(UpdatePlayerHealth);
    }

    public void UpdateInventory(List<ItemData> items)
    {
        for (int i = 0; i < MaxItems; i++) {
            var sprite = items.Count <= i ? null : items[i].Sprite;
            _itemSlots[i].ItemImage.sprite = sprite;
            _itemSlots[i].ItemImage.enabled = sprite != null;
        }
    }

    private void UpdatePlayerHealth(float value)
    {
        _hpSlider.value = value;
        var col = _hpHaze.color;
        col.a = 0.6f * (1 - value);
        _hpHaze.color = col;
        _healthValueText.text = Mathf.RoundToInt(GameManager.i.Player.Health) + " | " + GameManager.i.Player.MaxHealth;

    }

    public void DisplayBossBar(IHealth boss)
    {
        _bossSlider.transform.parent.gameObject.SetActive(true);
        boss.GetOnHealthChange().AddListener((value) => _bossSlider.value = value);
    }

    public void HideBossBar()
    {
        _bossSlider.transform.parent.gameObject.SetActive(false);
    }

    public void DisplayShieldCooldown(float value)
    {
        _shieldSlider.value = value;
        _shieldSlider.fillRect.GetComponent<Image>().color = value > 0.95f ? _parryColor : _reloadingColor;
    }

    public void StartParryColor()
    {
        _shieldSlider.fillRect.GetComponent<Image>().color = _parryColor;
    }

    public void StopParryColor()
    {
        _shieldSlider.fillRect.GetComponent<Image>().color = _normalColor;
    }

    private void Update()
    {
        var seconds = Time.timeSinceLevelLoad;
        seconds = Mathf.FloorToInt(seconds);
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds); 
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        _timerText.text = timeText;
    }

    public void DisplayNewItemScreen(ItemData newItem, List<ItemData> items)
    {
        if (newItem == null) return;

        _newItem = newItem;
        _newItem.Used = false;

        _newItemParent.SetActive(true);
        _newItemName.text = newItem.Name;
        _newItemSprite.sprite = newItem.Sprite;
        _newItemDescription.text = newItem.Description;
        _newItemQuote.text = '"' + newItem.Quote + '"';

        _keepButton.SetActive(items.Count < MaxItems);
    }

    public void KeepButtonPress()
    {
        InventoryManager.i.ActuallyAddItem(_newItem);
        _newItemParent.SetActive(false);
    }

    public void UseItemButton()
    {
        _newItemParent.SetActive(false);
        _newItem.Use();
    }

    public void TossItemButton()
    {
        _newItemParent.SetActive(false);
    }

}

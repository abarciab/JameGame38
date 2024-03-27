using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        GameManager.i.Player.OnHealthChange.AddListener(UpdatePlayerHealth);
    }

    private void UpdatePlayerHealth(float value)
    {
        _hpSlider.value = value;
        var col = _hpHaze.color;
        col.a = 0.6f * (1 - value);
        _hpHaze.color = col;

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

    public void HideShieldbar()
    {
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
}

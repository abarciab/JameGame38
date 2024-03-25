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

    private void Start()
    {
        GameManager.i.Player.OnHealthChange.AddListener(UpdatePlayerHealthBar);
    }

    private void UpdatePlayerHealthBar(float value)
    {
        _hpSlider.value = value;
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
        _shieldSlider.gameObject.SetActive(false);
    }

    public void DisplayShieldCooldown(float value)
    {
        _shieldSlider.gameObject.SetActive(true);
        _shieldSlider.value = value;
    }
}

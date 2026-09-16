using System;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    public static CoinUI Instance { get; private set; } //TODO: for the pickup coin
    [SerializeField] private TextMeshProUGUI coinText;

    private int coinCount = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateDisplay();
    }

    public void AddCoin(int amount = 1) 
    {
        coinCount += amount;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (coinText == null) return;
        coinText.text = coinCount.ToString();
    }

    public int GetCoinCount() => coinCount; //TODO: for the main menu recordsboard (maybe

    public void ResetCoins() //TODO: for new round 
    {
        coinCount = 0;
        UpdateDisplay();
    }

}

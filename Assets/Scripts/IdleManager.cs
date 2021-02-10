using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour
{
    [HideInInspector] public int length;
    [HideInInspector] public int strength;
    [HideInInspector] public int offlineEarnings;
    [HideInInspector] public int lengthCost;
    [HideInInspector] public int strengthCost;
    [HideInInspector] public int offlineEarningsCost;
    [HideInInspector] public int wallet;
    [HideInInspector] public int totalGain;

    public int[] marketCosts = new int[10];

    public static IdleManager instance;

    private void Awake()
    {
        if (IdleManager.instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
           IdleManager.instance = this;
        }

        length = -PlayerPrefs.GetInt("Length", 30);
        strength = PlayerPrefs.GetInt("Strength", 3);
        offlineEarnings = PlayerPrefs.GetInt("OfflineEarnings", 3);
        lengthCost = marketCosts[-length / 10 - 3];
        strengthCost = marketCosts[strength - 3];
        offlineEarningsCost = marketCosts[offlineEarnings - 3];
        wallet = PlayerPrefs.GetInt("Wallet", 0);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            DateTime now = DateTime.Now;
            PlayerPrefs.SetString("Date", now.ToString());
        }
        else
        {
            string @string = PlayerPrefs.GetString("Date", string.Empty);
            if (@string != string.Empty)
            {
                DateTime d = DateTime.Parse(@string);
                totalGain = (int)((DateTime.Now - d).TotalMinutes * offlineEarnings + 1.0);
                ScreenManager.instance.ChangeScreen(Screens.Return);
            }
        }
    }

    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }

    public void BuyLength()
    {
        length -= 10;
        wallet -= lengthCost;
        lengthCost = marketCosts[-length / 10 - 3];
        PlayerPrefs.SetInt("Length", -length);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreen(Screens.Main);
    }

    public void BuyStrength()
    {
        strength++;
        wallet -= strengthCost;
        strengthCost = marketCosts[-length / 10 - 3];
        PlayerPrefs.SetInt("Strength", strength);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreen(Screens.Main);
    }

    public void OfflineEarnings()
    {
        offlineEarnings++;
        wallet -= offlineEarningsCost;
        offlineEarningsCost = marketCosts[offlineEarnings - 3];
        PlayerPrefs.SetInt("OfflineEarnings", offlineEarnings);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreen(Screens.Main);
    }

    public void CollectMoney()
    {
        wallet += totalGain;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreen(Screens.Main);
    }

    public void CollectDoubleMoney()
    {
        wallet += totalGain * 2;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreen(Screens.Main);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class GameController : MonoBehaviour
{
    public double money;
    public double dpc;
    public double dps;
    public double health;
    public double healthCap
    {
        get 
        {
            return  10 * System.Math.Pow(2, stage - 1) * isBoss;
        }
 
    }
   
    public float timer;

    public int stage;
    public int stageMax;
    public int kills;
    public int killsMax;
    public int isBoss;
    public int timerCap;

    public Text moneyText;
    public Text dPCText;
    public Text dPSText;
    public Text stageText;
    public Text killsText;
    public Text healthText;
    public Text timerText;

    public GameObject back;
    public GameObject forward;

    public Image healthBar;
    public Image timerBar;

    public Animator coinExplode;
    public GameObject coinExplodeGameObject;

    // Offline
    public int OfflineProgressCheck;
    public float idleTime;
    public Text offlineTimeText;
    public float saveTime;
    public GameObject offlineBox;

    // Username
    public TMP_InputField usernameInput;
    public string username;
    public Text usernameText;
    public GameObject usernameBox;  

    // Upgrades
    public Text pCostText;
    public Text pLevelText;
    public Text pPowerText;
    public double pCost
    {
        get
        {
            return 10 * Math.Pow(1.07, pLevel);
        }
    }
    public int pLevel;
    public double pPower
    {
        get
        {
            return 5 * pLevel;
        }
    }
    public Text cCostText;
    public Text cLevelText;
    public Text cPowerText;
    public double cCost
    {
        get
        {
            return 10 * Math.Pow(1.07, cLevel);
        }
    }    
    public int cLevel;
    public double cPower
      {
        get
        {
            return 2 * cLevel;
        }
    }


    public void Start()
    {
        offlineBox.gameObject.SetActive(false);
        Load();
        if (username == "<Username>")
            usernameBox.gameObject.SetActive(true);
        else
            usernameBox.gameObject.SetActive(false);
        IsBossChecker();
        health = healthCap;
        timerCap = 30;
    }

    public void Update()
    {
        moneyText.text = "$" + money.ToString("F2");
        dPCText.text = dpc + " DPC";
        dPSText.text = dps + " DPS";
        stageText.text = " Stage - "+ stage;
        killsText.text = kills + "/" + killsMax + "Kills";
        healthText.text = health + "/" + healthCap + "HP";
        
        healthBar.fillAmount = (float)(health / healthCap);

        if(stage > 1) back.gameObject.SetActive(true);
        else
            back.gameObject.SetActive(false);
        
        if(stage != stageMax) forward.gameObject.SetActive(true);
        else
            forward.gameObject.SetActive(false);

        IsBossChecker();
        usernameText.text = username;
        Upgrades();

        saveTime += Time.deltaTime;
        if(saveTime >= 5)
        {
        saveTime = 0;
        Save();
        }
    
    }

    public void Upgrades()
    {
        cCostText.text = "Cost: $" + cCost.ToString("F2");
        cLevelText.text = "Level : " + cLevel;
        cPowerText.text = "+2 DPC";

        pCostText.text = "Cost: $" + pCost.ToString("F2");
        pLevelText.text = "Level : " + pLevel;
        pPowerText.text = "+5 DPS";
        dps = pPower;
        dpc = 1 + cPower;
    }
    public void UsernameChange()
    {
        username = usernameInput.text;

    }

    public void CloseUsernameBox()
    {
        usernameBox.gameObject.SetActive(false);
    }

    public void IsBossChecker()
    {
       if (stage % 5 == 0) 
       {
            isBoss = 10;
            stageText.text = "BOSS! Stage - " + stage;
            timer -= Time.deltaTime;
            if (timer <= 0) Back();
            
            timerText.text = timer + "/" + timerCap;
            timerBar.gameObject.SetActive(true);
            timerBar.fillAmount = timer / timerCap;
            killsMax = 1;
        }
        else
        {    isBoss = 1;
             stageText.text = "Stage - " + stage;
             timerText.text = "";
             timerBar.gameObject.SetActive(false);
             timer = 30;
             killsMax = 10;
        }  
    }
    public void Hit()
    {
        health -= dpc;
        if (health <= 0)
        {
            money += System.Math.Ceiling(healthCap / 14);
            if (stage == stageMax)
            {
                kills += 1;
                coinExplode.Play("CoinExplode", 0, 0);
                if (kills >= killsMax)
                {
                    kills = 0;
                    stage += 1;
                    stageMax += 1;
                }
            }
           IsBossChecker();
           health = healthCap;
           if (isBoss > 1) timer = timerCap;
           killsMax = 10;
        }
    }

    public void Back()
    {
        stage -= 1;
        IsBossChecker();
        health = healthCap;
    }
    public void Forward()
    {
        stage += 1;
        IsBossChecker();
        health = healthCap;
    }

    
    public void BuyUpgrade(string id)
    {
        switch (id)
        {
            case "p1":
                if(money >= pCost) UpgradeDefaults(ref pLevel, pCost);
                break;
            case "c1":
                if(money >= cCost) UpgradeDefaults(ref cLevel, cCost);
                break;
                
        }

    }  

    public void UpgradeDefaults(ref int level, double cost)
    {
        money -= cost;
        level++;
    }
    
    public void Save()
    {
        OfflineProgressCheck = 1;
        PlayerPrefs.SetString("money", money.ToString());
        PlayerPrefs.SetString("dpc", dpc.ToString());
        PlayerPrefs.SetString("dps", dps.ToString());
        PlayerPrefs.SetString(" username",  username);
        PlayerPrefs.SetInt("stage", stage);
        PlayerPrefs.SetInt("stageMax", stageMax);
        PlayerPrefs.SetInt("kills", kills);
        PlayerPrefs.SetInt("killsMax", killsMax);
        PlayerPrefs.SetInt("isBoss", isBoss);
        PlayerPrefs.SetInt("pLevel", pLevel);
        PlayerPrefs.SetInt("cLevel", cLevel);
        PlayerPrefs.SetInt("OfflineProgressCheck", OfflineProgressCheck);
    }

    public void Load()
    {
        money = double.Parse(PlayerPrefs.GetString("money", "0"));
        dpc = double.Parse(PlayerPrefs.GetString("dpc", "1"));
        dps = double.Parse(PlayerPrefs.GetString("dps", "0"));
        stage = PlayerPrefs.GetInt("stage", 1);
        stageMax = PlayerPrefs.GetInt("stageMax", 1);
        kills = PlayerPrefs.GetInt("kills", 0);
        killsMax = PlayerPrefs.GetInt("killsMax", 10);
        isBoss = PlayerPrefs.GetInt("isBoss", 1);
        pLevel = PlayerPrefs.GetInt("pLevel", 0);
        cLevel = PlayerPrefs.GetInt("cLevel", 0);
        OfflineProgressCheck = PlayerPrefs.GetInt("OfflineProgressCheck", 0);
        username = (PlayerPrefs.GetString("username", "<Username>"));
    }
}
  

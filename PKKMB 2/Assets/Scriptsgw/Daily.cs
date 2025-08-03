using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daily : MonoBehaviour
{
    public int LastDay;

    public int Day_1;
    public int Day_2;
    public int Day_3;
    public int Day_4;
    public int Day_5;
    public int Day_6;
    public int Day_7;



    public GameObject NotClaimed_1;
    public GameObject Claim_1;
    public GameObject Claimed_1;

    public GameObject NotClaimed_2;
    public GameObject Claim_2;
    public GameObject Claimed_2;

    public GameObject NotClaimed_3;
    public GameObject Claim_3;
    public GameObject Claimed_3;

    public GameObject NotClaimed_4;
    public GameObject Claim_4;
    public GameObject Claimed_4;

    public GameObject NotClaimed_5;
    public GameObject Claim_5;
    public GameObject Claimed_5;

    public GameObject NotClaimed_6;
    public GameObject Claim_6;
    public GameObject Claimed_6;

    public GameObject NotClaimed_7;
    public GameObject Claim_7;
    public GameObject Claimed_7;


    void Start()
    {
        Day_1 = PlayerPrefs.GetInt("Day_1");
        Day_2 = PlayerPrefs.GetInt("Day_2");
        Day_3 = PlayerPrefs.GetInt("Day_3");
        Day_4 = PlayerPrefs.GetInt("Day_4");
        Day_5 = PlayerPrefs.GetInt("Day_5");
        Day_6 = PlayerPrefs.GetInt("Day_6");
        Day_7 = PlayerPrefs.GetInt("Day_7");
        LastDay = PlayerPrefs.GetInt("LastDay");

        Reward();

        if (LastDay != System.DateTime.Now.Day)
        {

            if (Day_1 == 0)
            {
                Day_1 = 1;
            }
            else if (Day_2 == 0)
            {
                Day_2 = 1;
            }
            else if (Day_3 == 0)
            {
                Day_3 = 1;
            }
            else if (Day_4 == 0)
            {
                Day_4 = 1;
            }
            else if (Day_5 == 0)
            {
                Day_5 = 1;
            }
            else if (Day_6 == 0)
            {
                Day_6 = 1;
            }
            else if (Day_7 == 0)
            {
                Day_7 = 1;
            }

            Reward();

        }
    }

    public void Reward()
    {
        if (Day_1 == 0)
        {
            NotClaimed_1.SetActive(true);
            Claim_1.SetActive(true);
            Claimed_1.SetActive(false);
        }
        if (Day_1 == 1)
        {
            NotClaimed_1.SetActive(false);
            Claim_1.SetActive(true);
            Claimed_1.SetActive(false);;
        }
        if (Day_1 == 2)
        {
            NotClaimed_1.SetActive(false);
            Claim_1.SetActive(false);
            Claimed_1.SetActive(true);
        }

        if (Day_2 == 0)
        {
            NotClaimed_2.SetActive(true);
            Claim_2.SetActive(true);
            Claimed_2.SetActive(false);
        }
        if (Day_2 == 1)
        {
            NotClaimed_2.SetActive(false);
            Claim_2.SetActive(true);
            Claimed_2.SetActive(false);;
        }
        if (Day_2 == 2)
        {
            NotClaimed_2.SetActive(false);
            Claim_2.SetActive(false);
            Claimed_2.SetActive(true);
        }

        if (Day_3 == 0)
        {
            NotClaimed_3.SetActive(true);
            Claim_3.SetActive(true);
            Claimed_3.SetActive(false);
        }
        if (Day_3 == 1)
        {
            NotClaimed_3.SetActive(false);
            Claim_3.SetActive(true);
            Claimed_3.SetActive(false);;
        }
        if (Day_3 == 2)
        {
            NotClaimed_3.SetActive(false);
            Claim_3.SetActive(false);
            Claimed_3.SetActive(true);
        }

        if (Day_4 == 0)
        {
            NotClaimed_4.SetActive(true);
            Claim_4.SetActive(true);
            Claimed_4.SetActive(false);
        }
        if (Day_4 == 1)
        {
            NotClaimed_4.SetActive(false);
            Claim_4.SetActive(true);
            Claimed_4.SetActive(false);;
        }
        if (Day_4 == 2)
        {
            NotClaimed_4.SetActive(false);
            Claim_4.SetActive(false);
            Claimed_4.SetActive(true);
        }

        if (Day_5 == 0)
        {
            NotClaimed_5.SetActive(true);
            Claim_5.SetActive(true);
            Claimed_5.SetActive(false);
        }
        if (Day_5 == 1)
        {
            NotClaimed_5.SetActive(false);
            Claim_5.SetActive(true);
            Claimed_5.SetActive(false);;
        }
        if (Day_5 == 2)
        {
            NotClaimed_5.SetActive(false);
            Claim_5.SetActive(false);
            Claimed_5.SetActive(true);
        }

        if (Day_6 == 0)
        {
            NotClaimed_6.SetActive(true);
            Claim_6.SetActive(true);
            Claimed_6.SetActive(false);
        }
        if (Day_6 == 1)
        {
            NotClaimed_6.SetActive(false);
            Claim_6.SetActive(true);
            Claimed_6.SetActive(false);;
        }
        if (Day_6 == 2)
        {
            NotClaimed_6.SetActive(false);
            Claim_6.SetActive(false);
            Claimed_6.SetActive(true);
        }

        if (Day_7 == 0)
        {
            NotClaimed_7.SetActive(true);
            Claim_7.SetActive(true);
            Claimed_7.SetActive(false);
        }
        if (Day_7 == 1)
        {
            NotClaimed_7.SetActive(false);
            Claim_7.SetActive(true);
            Claimed_7.SetActive(false);;
        }
        if (Day_7 == 2)
        {
            NotClaimed_7.SetActive(false);
            Claim_7.SetActive(false);
            Claimed_7.SetActive(true);
        }
    }

    public void GetReward_1()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 1");

        Day_1 = 2;
        PlayerPrefs.SetInt("Day_1", 2);

        Reward();
    }

    public void GetReward_2()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 2");

        Day_2 = 2;
        PlayerPrefs.SetInt("Day_2", 2);
 
        Reward();
    }

    public void GetReward_3()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 3");

        Day_3 = 2;
        PlayerPrefs.SetInt("Day_3", 2);

        Reward();
    }

    public void GetReward_4()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 4");

        Day_4 = 2;
        PlayerPrefs.SetInt("Day_4", 2);

        Reward();
    }

    public void GetReward_5()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 5");

        Day_5 = 2;
        PlayerPrefs.SetInt("Day_5", 2);

        Reward();
    }

    public void GetReward_6()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 6");

        Day_6 = 2;
        PlayerPrefs.SetInt("Day_6", 2);

        Reward();
    }

    public void GetReward_7()
    {
        LastDay = System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastDay", LastDay);

        print("Reward 7");

        Day_7 = 2;
        PlayerPrefs.SetInt("Day_7", 2);

        Reward();
    }

}

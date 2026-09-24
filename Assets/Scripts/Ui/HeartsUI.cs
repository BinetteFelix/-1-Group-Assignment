using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    public static HeartsUI Instance;
    public GameObject heartImagePrefab;
    public Transform heartsUIParent;

    public int maxHP = 3;

    private GameObject[] hearts;
    private int currentHP;
    private bool isDead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hearts = new GameObject[maxHP];
        currentHP = maxHP;


        for (int i = 0; i < maxHP; i++)
        {
            hearts[i] = Instantiate(heartImagePrefab, heartsUIParent);
        }
    }

    //TEST
    private void Update()
    {
        if (Keyboard.current[Key.O].wasPressedThisFrame)
        {
            RemoveHeart(1);
        }

    }


    public void RemoveHeart(int amount)
    {
        if (isDead) return;

        for (int i = 0; i < amount; i++)
        {
            if (currentHP <= 0) break;
            currentHP--;
            hearts[currentHP].SetActive(false);
        }

        if (currentHP <= 0) Die();
    }

    public void AddHeart(int amount)
    {
        if (isDead) return;

        for (int i = 0; i < amount; i++)
        {
            if (currentHP >= maxHP) break;
            hearts[currentHP].SetActive(true);
            currentHP++;
        }
    }

    //Game Over

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (hearts == null || currentHP == 0)
        {
            GameOver.Instance.Fail();
        }
    }

}


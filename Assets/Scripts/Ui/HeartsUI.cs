using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    public static HeartsUI Instance;
    public GameObject heartSlotPrefab;
    public RawImage heartUIPrefab;
    public Transform heartsRigParent;
    public Transform heartsUIParent;
    public int maxHP = 3;

    private HeartController[] hearts;
    private int currentHP;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hearts = new HeartController[maxHP];
        currentHP = maxHP;


        for (int i = 0; i < maxHP; i++)
        {
            GameObject rig = Instantiate(heartSlotPrefab, heartsRigParent);
            rig.transform.localPosition = new Vector3(i * 5f, 0, 0);  //TODO: NEED TO TEST SINCE THERE IS ANIMATION

            RenderTexture rt = new RenderTexture(256, 256, 16);
            Camera cam = rig.GetComponentInChildren<Camera>();
            cam.targetTexture = rt;

            RawImage ui = Instantiate(heartUIPrefab, heartsUIParent);
            ui.texture = rt;

            HeartController controller = rig.AddComponent<HeartController>();
            controller.Init(cam, ui.gameObject);
            hearts[i] = controller;
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

    private void FixedUpdate()
    {
        Die();
    }



    public void RemoveHeart(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentHP <= 0) break;
            currentHP--;
            hearts[currentHP].Hide();
        }
    }

    public void AddHeart(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentHP >= maxHP) break;
            hearts[currentHP].Show();
            currentHP++;
        }
    }

    //Game Over

    public void Die()
    {
        if (hearts == null || currentHP == 0)
        {
            GameOver.Instance.GameOverSceen();
        }
    }

}


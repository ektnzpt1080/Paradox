using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryController : MonoBehaviour
{
    public Sprite battery0;
    public Sprite battery1;
    public Sprite battery2;
    public Sprite battery3;
    public Sprite battery4;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.resetLimit >= 4)
        {
            sr.sprite = battery4;
        }
        else if (GameManager.instance.resetLimit == 3)
        {
            sr.sprite = battery3;
        }
        else if (GameManager.instance.resetLimit == 2)
        {
            sr.sprite = battery2;
        }
        else if (GameManager.instance.resetLimit == 1)
        {
            sr.sprite = battery1;
        }
        else if (GameManager.instance.resetLimit == 0)
        {
            sr.sprite = battery0;
        }
    }
}

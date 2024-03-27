using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MessageController : MonoBehaviour
{
    BoxCollider2D cd2d;
    Sprite sprite;// 원래 스프라이트
    public Sprite destroyed;//메세지가 부숴졌을 때 스프라이트
    public int timeLimit;//메시지의 수명
    int time;//시간
    public GameObject message;//표시할 메시지
    public TextMeshPro display;//메시지 수명 나타내는 거
    public TextMeshPro countdown;//메시지의 카운트 다운

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        cd2d = GetComponent<BoxCollider2D>();
        countdown = message.GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        time = GameManager.instance.GetTime();
        if (cd2d.IsTouchingLayers(LayerMask.GetMask("Player")) && timeLimit - time > 0)
        {
            message.SetActive(true);
            countdown.text = "<이 메시지는 " + (timeLimit - time) + "타임 뒤에 제거됩니다>";
        }//플레이어를 만나면 + 메세지가 안 부숴졌으면 = 메세지를 띄움
        else
        {
            message.SetActive(false);
        }

        if(timeLimit - time > 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            display.enabled = true;
            display.text = "" + (timeLimit - time);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = destroyed;
            display.enabled = false;
        }
    }
}

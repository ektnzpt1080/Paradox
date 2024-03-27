using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ArrayList alterCharacter;//플레이어 분신 목록
    public GameObject playerAlter;//플레이어 분신 프리팹
    public PlayerController player;//플레이어
    public SwitchController[] switches;//스테이지의 모든 스위치를 넣을 것
    public BlockFallController[] blocks;//스테이지의 모든 떨어지는 블록을 넣을 것
    public FloorController[] floors;//스테이지의 모든 바닥 스위치를 넣을 것
    public int resetLimit;//스테이지 당 돌릴수 있는 횟수
    public TextMeshPro timeText;//시간 표시하는 UI
    public GameObject rewindEffect;//시간돌리는 이펙트
    public GameObject clockEffect;//STAGE 넘어갈때 이펙트
    public Sprite wink;
    bool rewindComplete;
    private bool[] switches_status;//스테이지의 모든 스위치의 첫 상태
    int time;//시간
    bool playerMoved;
    
    

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        time = 0;
        alterCharacter = new ArrayList();
        playerMoved = false;
        switches_status = new bool[switches.Length];
        for(int i = 0; i < switches.Length; i++)
        {
            switches_status[i] = switches[i].isActive;
        }
        timeText.text = "TIME : 0 0 0";  
        if(floors == null)
        {
            floors = new FloorController[0];
        }
        rewindComplete = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerMoved)
        {
            if (AlterMoveComplete() && BlockMoveComplete())
            {
                playerMoved = false;
                player.Check();
                for (int i = switches.Length - 1; i >= 0; i--)
                {
                    switches[i].Check();
                }
                for (int i = floors.Length - 1; i >= 0; i--)
                {
                    floors[i].Check();
                }

            }
        }
        timeText.text = "TIME : " + (int)(time / 100) + " " + (int)((time % 100) / 10) + " " + (int)(time % 10);
    }

    public void TimeChange()
    {

        for (int i = blocks.Length - 1; i >= 0; i--)
        {
            blocks[i].Check();
        }
        for (int i = alterCharacter.Count - 1; i >= 0; i--)
        {
            ((GameObject)alterCharacter[i]).GetComponent<AltPlayer>().NextMove();
        }
        time++;
    }

    public void PlayerMoveComplete()
    {
        playerMoved = true;
    }

    IEnumerator NewTimeReset()
    {
        resetLimit--;
        for (int i = floors.Length - 1; i >= 0; i--)
        {
            floors[i].SetEnabled(false);
        }
        for (int i = switches.Length - 1; i >= 0; i--)
        {
            switches[i].SetEnabled(false);
        }
        for (int i = alterCharacter.Count - 1; i >= 0; i--)
        {
            ((GameObject)alterCharacter[i]).GetComponent<AltPlayer>().SetEnabled(false);
        }
        float timeShard;

        if (time <= 10)
        {
            timeShard = 0.1f;
        }
        else
        {
            timeShard = Mathf.Log(time, 10f) / time;
        }

        StartCoroutine(Rewinding());

        while (time > 0)
        {
            time--;
            for (int i = alterCharacter.Count - 1; i >= 0; i--)
            {
                ((GameObject)alterCharacter[i]).GetComponent<AltPlayer>().UndoMove();
            }
            for (int i = blocks.Length - 1; i >= 0; i--)
            {
                blocks[i].UndoMove();
            }
            for (int i = floors.Length - 1; i >= 0; i--)
            {
                floors[i].Undo();
            }
            for (int i = switches.Length - 1; i >= 0; i--)
            {
                switches[i].Undo();
            }
            yield return new WaitForSeconds(timeShard);
        }
        rewindComplete = true;
        for (int i = alterCharacter.Count - 1; i >= 0; i--)
        {
            ((GameObject)alterCharacter[i]).GetComponent<AltPlayer>().SetEnabled(true);
        }
        for (int i = blocks.Length - 1; i >= 0; i--)
        {
            blocks[i].MakeNewList();
        }
        for (int i = floors.Length - 1; i >= 0; i--)
        {
            floors[i].SetEnabled(true);
            floors[i].MakeNewList();
        }
        for (int i = switches.Length - 1; i >= 0; i--)
        {
            switches[i].SetEnabled(true);
            switches[i].isActive = switches_status[i];
            switches[i].MakeNewList();
        }
        yield return new WaitForSeconds(0.05f);
        player.Check();
    }

    IEnumerator Rewinding()
    {
        GameObject re3;
        int i = 0;
        re3 = Instantiate(rewindEffect, new Vector3(15.36f, -1.57f, 0), new Quaternion(0, 0, 0, 0));
        while (!rewindComplete)
        {
            i++;
            if(i == 25)
            {
                re3.SetActive(!re3.activeInHierarchy);
                i = 0;
            }
            yield return new WaitForSeconds(0.01f);
        }
        rewindComplete = false;
        
        Destroy(re3);
    }

    /*
    public void TimeReset()//시간 되돌리기. 시간을 0으로 만들고, 분신을 생성하고, 모든 스위치를 원래 상태로 만듬.
    {
        time = 0;
        for (int i = alterCharacter.Count - 1; i >= 0; i--)
        {
            ((GameObject)alterCharacter[i]).GetComponent<AltPlayer>().TimeReset();
        }
        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].isActive = switches_status[i];
        }
        resetLimit--;
        timeText.text = "Time : " + time + "\nResetLimit : " + resetLimit;
    }
    */

    public void MakeAlter(ArrayList list, Vector3 startingPoint)
    {
        alterCharacter.Add(Instantiate(playerAlter, player.transform.position, new Quaternion(0, 0, 0, 0)));
        ((GameObject)alterCharacter[alterCharacter.Count - 1]).GetComponent<AltPlayer>().SetList(list);
        ((GameObject)alterCharacter[alterCharacter.Count - 1]).GetComponent<AltPlayer>().SetStartingPoint(startingPoint);
        StartCoroutine(NewTimeReset());
    }

    public int GetTime()
    {
        return time;
    }

    public bool AlterMoveComplete()
    {
        bool ret = true;
        for (int i = alterCharacter.Count - 1; i >= 0; i--)
        {
            ret = ret&&((GameObject)alterCharacter[i]).GetComponent<AltPlayer>().IsMoveComplete();
        }
        return ret;
    }

    public bool BlockMoveComplete()
    {
        bool ret = true;
        for (int i = blocks.Length - 1; i >= 0; i--)
        {
            ret = ret && blocks[i].IsMoveComplete();
        }
        return ret;
    }
    
    public void GameOver()
    {
        player.PlayerGameOver();
    }

    public void StageClear()
    {
        StartCoroutine(ToNextStage());
    }

    IEnumerator ToNextStage()
    {
        GameObject clk = Instantiate(clockEffect, player.transform.position, new Quaternion(0, 0, 0, 0));
        GameObject mask = clk.transform.Find("Mask").gameObject;
        GameObject mask2 = clk.transform.Find("Mask2").gameObject;
        for (int i = 0; i < 10; i++)
        {
            mask.transform.localScale = mask.transform.localScale + new Vector3(1, 1) / 10;
            mask2.transform.localScale = mask2.transform.localScale + new Vector3(1, 1) / 10;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        if (SceneManager.GetActiveScene().buildIndex == 14)
        {
            player.GetComponent<SpriteRenderer>().sprite = wink;
            yield return new WaitForSeconds(1.5f);
        }
        player.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        mask.transform.localScale = new Vector3(1, 1, 1);
        for (int i = 0; i < 10; i++)
        {
            mask.transform.localScale = mask.transform.localScale - new Vector3(1, 1) / 10;
            mask2.transform.localScale = mask2.transform.localScale - new Vector3(1, 1) / 10;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(clk);
        yield return new WaitForSeconds(1f);
        if (SceneManager.GetActiveScene().buildIndex == 11)
        {
            SceneManager.LoadScene(0);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 14)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

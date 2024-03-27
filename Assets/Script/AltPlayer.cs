using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltPlayer : MonoBehaviour
{
    ArrayList record;
    public float moveTime = 0.1f;
    BoxCollider2D down;
    BoxCollider2D cd2d;
    Vector3 startingPoint;
    int time;//시간
    bool destroyed;//시간이 다 되서 사라졌을 시 true
    bool moveComplete;//움직임이 끝날시 true
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite rightSprite;
    public Sprite leftSprite;
    public Sprite zSprite;
    public Sprite xSprite;
    public Sprite cSprite;
    public Sprite defaultSprite;
    public Sprite deathSprite;
    SpriteRenderer sr;
    bool isEnabled;
    bool gameover;

    // Start is called before the first frame update
    void Start()
    {
        destroyed = false;
        cd2d = GetComponent<BoxCollider2D>();
        down = transform.Find("Down").gameObject.GetComponent<BoxCollider2D>();
        moveComplete = true;
        sr = GetComponent<SpriteRenderer>();
        isEnabled = false;
        gameover = false;
    }

    // Update is called once per frame
    void Update()
    {
        time = GameManager.instance.GetTime();
        if (time == 0)
        {
            transform.position = startingPoint;
        }
        if (!destroyed)
        {
            cd2d.enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        if (gameover)
        {
            sr.sprite = deathSprite;
        }
        else if (isEnabled)
        {
            if (record.Count <= time)
            {
                sr.sprite = xSprite;
            }
            else
            {
                switch ((int)record[time])
                {
                    case 2:
                        sr.sprite = upSprite;
                        return;
                    case 3:
                        sr.sprite = leftSprite;
                        return;
                    case 4:
                        sr.sprite = rightSprite;
                        return;
                    case 1:
                    case 5:
                        sr.sprite = downSprite;
                        return;
                    case 6:
                        sr.sprite = zSprite;
                        return;
                    case 7:
                        sr.sprite = cSprite;
                        return;
                }
            }
        }
        else
        {
            sr.sprite = defaultSprite;
        }
        

        //생각 바뀜, 움직임이 끝나고 겹쳐 있을때만 죽는걸로 (교차가능)
    }

    public void UndoMove()
    {
        time = GameManager.instance.GetTime();
        if (record.Count < time)
        {
            destroyed = true;
            cd2d.enabled = false;//판정만 없애야 됨
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else if(record.Count == time)
        {
            destroyed = false;
            cd2d.enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            Vector2 movloc = ((Vector2)transform.position);
            switch ((int)record[time])
            {
                case 2:
                    transform.position = ((Vector2)transform.position) + Vector2.down;
                    return;
                case 3:
                    transform.position = ((Vector2)transform.position) + Vector2.right;
                    return;
                case 4:
                    transform.position = ((Vector2)transform.position) + Vector2.left;
                    return;
                case 1:
                case 5:
                    transform.position = ((Vector2)transform.position) + Vector2.up;
                    return;
                case 6:
                case 7:
                    return;
            }
        }
    }

    public void NextMove()
    {
        if (record.Count <= time)
        {
            destroyed = true;
            cd2d.enabled = false;//판정만 없애야 됨
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            moveComplete = false;
            int i = (int)record[time];
            switch (i)
            {
                case 2:
                    StartCoroutine(Move(Vector2.up));
                    return;
                case 3:
                    StartCoroutine(Move(Vector2.left));
                    return;
                case 4:
                    StartCoroutine(Move(Vector2.right));
                    return;
                case 1:
                case 5:
                    StartCoroutine(Move(Vector2.down));
                    return;
                case 6:
                    StartCoroutine(PushSwitch());
                    return;
                case 7:
                    StartCoroutine(JustStop());
                    return;
            }
        }
    }

    IEnumerator Move(Vector2 dest)
    {
        Vector2 movloc = ((Vector2)transform.position) + dest;
        for (int i = 0; i < moveTime * 100 - 1; i++)
        {
            transform.position = ((Vector2)transform.position) + dest / (moveTime * 100);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = movloc;
        yield return new WaitForSeconds(0.05f);
        Check();
    }

    IEnumerator PushSwitch()
    {
        if (cd2d.IsTouchingLayers(LayerMask.GetMask("Switch")))
        {
            ContactFilter2D cf = new ContactFilter2D();
            cf.SetLayerMask(LayerMask.GetMask("Switch"));
            Collider2D[] ds = new Collider2D[5];
            cd2d.OverlapCollider(cf, ds);
            ds[0].gameObject.GetComponent<SwitchController>().pushSwitch();
            yield return new WaitForSeconds(0.1f);
            Check();
        }
    }

    IEnumerator JustStop()
    {
        yield return new WaitForSeconds(0.1f);
        Check();
    }

    public void Check()
    {
        if(BlockCheck())
        {
            moveComplete = true;
        }
        else {
            AltPlayerDestroyed();
        }
    }//FallCheck()뺌. 필요하면 다시 넣을 것.

    bool BlockCheck()
    {
        return (cd2d.IsTouchingLayers(LayerMask.GetMask("Ladder"))||!cd2d.IsTouchingLayers(LayerMask.GetMask("Block")))&&!cd2d.IsTouchingLayers(LayerMask.GetMask("AltPlayer"))&&!cd2d.IsTouchingLayers(LayerMask.GetMask("Player"));
    }//끼였는지 체크

    bool FallCheck()
    {
        if(record.Count <= time || (int)record[time] == 5)
        {
            return true;
        }
        else
        {
            return down.IsTouchingLayers(LayerMask.GetMask("Block"))||down.IsTouchingLayers(LayerMask.GetMask("Ladder"))|| down.IsTouchingLayers(LayerMask.GetMask("AltPlayer"));
        }
    }// 좌우로 가야 하는 상황에 옆으로 못 가는 지 체크

    public void SetList(ArrayList list)
    {
        record = (ArrayList)list.Clone();
    }
    /*
    public void TimeReset()
    {
        destroyed = false;
        Check();
    }
    */

    public bool IsMoveComplete()
    {
        return moveComplete;
    }

    public void AltPlayerDestroyed()
    {
        gameover = true;
        GameManager.instance.GameOver();
    }

    public void SetStartingPoint(Vector2 point)
    {
        startingPoint = point;
    }

    public void SetEnabled(bool TF)
    {
        isEnabled = TF;
    }
}

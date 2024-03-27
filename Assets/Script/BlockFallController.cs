using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFallController : MonoBehaviour
{
    public BoxCollider2D down;
    bool moveComplete;
    public float moveTime = 0.1f;
    Vector3 startingPoint;
    int time;
    public ArrayList record;

    // Start is called before the first frame update
    void Start()
    {
        down = transform.Find("Down").gameObject.GetComponent<BoxCollider2D>();
        startingPoint = transform.position;
        moveComplete = true;
        record = new ArrayList();

    }

    void Update()
    {
        time = GameManager.instance.GetTime();
        if(time == 0)
        {
            transform.position = startingPoint;
        }
    }
    public void Check()
    {
        if (!down.IsTouchingLayers(LayerMask.GetMask("Block")))
        {
            moveComplete = false;
            StartCoroutine(Fall());
            record.Add(1);
        }
        else
        {
            record.Add(0);
        }
    }

    IEnumerator Fall()
    {
        Vector2 movloc = ((Vector2)transform.position) + Vector2.down;
        for (int i = 0; i < moveTime * 100 - 1; i++)
        {
            transform.position = ((Vector2)transform.position) + Vector2.down / (moveTime * 100);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = movloc;
        yield return new WaitForSeconds(0.02f);
        moveComplete = true;
    }

    public void UndoMove()
    {
        time = GameManager.instance.GetTime();
        if ((int)record[time] == 0)
        {
            return;
        }
        else
        {
            transform.position = (Vector2)transform.position + Vector2.up;
        }
    }

    public void MakeNewList()
    {
        record = new ArrayList();
    }

    public bool IsMoveComplete()
    {
        return moveComplete;
    }
}

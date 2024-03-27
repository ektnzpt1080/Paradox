using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveTime;
    public float fallTime;
    public bool canMove;
    public bool isLadder;
    private bool gameover;
    BoxCollider2D down;
    BoxCollider2D up;
    BoxCollider2D left;
    BoxCollider2D right;
    BoxCollider2D cd2d;
    Vector3 startingPoint;
    ArrayList recorder;
    int action;//1: down 2: up 3: left 4: right 5: fall 6: interact
    public GameObject restartMessage;
    public Sprite deathSprite;

    // Start is called before the first frame update
    void Awake()
    {
        cd2d = GetComponent<BoxCollider2D>();
        down = transform.Find("Down").gameObject.GetComponent<BoxCollider2D>();
        up = transform.Find("Up").gameObject.GetComponent<BoxCollider2D>();
        left = transform.Find("Left").gameObject.GetComponent<BoxCollider2D>();
        right = transform.Find("Right").gameObject.GetComponent<BoxCollider2D>();
        recorder = new ArrayList();

    }

    private void Start()
    {
        moveTime = 0.1f;
        fallTime = 0.1f;
        canMove = true;
        isLadder = false;
        startingPoint = transform.position;
        gameover = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (canMove && !gameover)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (down.IsTouchingLayers(LayerMask.GetMask("Ladder")) || (cd2d.IsTouchingLayers(LayerMask.GetMask("Ladder"))&&!down.IsTouchingLayers(LayerMask.GetMask("Block"))))
                {
                    canMove = false;
                    isLadder = true;
                    recorder.Add(1);
                    StartCoroutine(Move(Vector2. down));
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (cd2d.IsTouchingLayers(LayerMask.GetMask("Ladder")) && (!up.IsTouchingLayers(LayerMask.GetMask("Block")) || up.IsTouchingLayers(LayerMask.GetMask("Ladder"))))
                {                
                    canMove = false;
                    isLadder = true;
                    recorder.Add(2);
                    StartCoroutine(Move(Vector2.up));
                }

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!isLadder&&!left.IsTouchingLayers(LayerMask.GetMask("Block")))
                {
                    canMove = false;
                    recorder.Add(3);
                    StartCoroutine(Move(Vector2.left));
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!isLadder&&!right.IsTouchingLayers(LayerMask.GetMask("Block")))
                {
                    canMove = false;
                    recorder.Add(4);
                    StartCoroutine(Move(Vector2.right));
                }
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                PushSwitch();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                TimeResetByX();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                canMove = false;
                recorder.Add(7);
                GameManager.instance.TimeChange();
                GameManager.instance.PlayerMoveComplete();
            }
        }

        if (gameover)
        {
            GetComponent<SpriteRenderer>().sprite = deathSprite;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(0);
        }
    }

    void PushSwitch()
    { 
        if (cd2d.IsTouchingLayers(LayerMask.GetMask("Switch")))
        {
            canMove = false;
            ContactFilter2D cf = new ContactFilter2D();
            cf.SetLayerMask(LayerMask.GetMask("Switch"));
            Collider2D[] ds = new Collider2D[5];
            cd2d.OverlapCollider(cf, ds);
            ds[0].gameObject.GetComponent<SwitchController>().pushSwitch();
            recorder.Add(6);
            GameManager.instance.TimeChange();
            GameManager.instance.PlayerMoveComplete();
            
        }
    }

    IEnumerator Fall()
    {
        GameManager.instance.TimeChange();
        recorder.Add(5);
        for (int i = 0; i < fallTime * 100 - 1; i++)
        {
            transform.position = ((Vector2)transform.position) + Vector2.down / (fallTime * 100);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.05f);
        GameManager.instance.PlayerMoveComplete();
    }

    IEnumerator Move(Vector2 dest)
    {
        GameManager.instance.TimeChange();
        Vector2 movloc = ((Vector2)transform.position) + dest;
        for(int i = 0;i < moveTime * 100 - 1;i++)
        {
            transform.position = ((Vector2)transform.position) + dest / (moveTime * 100);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = movloc;
        //yield return new WaitForSeconds(0.05f);
        GameManager.instance.PlayerMoveComplete();
    }

    void TimeResetByX()
    {
        if(GameManager.instance.resetLimit > 0 && GameManager.instance.GetTime() > 0)
        {
            canMove = false;
            GameManager.instance.MakeAlter(recorder, startingPoint);
            recorder = new ArrayList();
            startingPoint = transform.position;
        }
    }

    public void Check()
    {
        if (cd2d.IsTouchingLayers(LayerMask.GetMask("Spike")))
        {
            PlayerGameOver();
        }
        else if (gameover || cd2d.IsTouchingLayers(LayerMask.GetMask("AltPlayer")) || (cd2d.IsTouchingLayers(LayerMask.GetMask("Block")) && !cd2d.IsTouchingLayers(LayerMask.GetMask("Ladder"))))
        {
            PlayerGameOver();
        }
        else if (cd2d.IsTouchingLayers(LayerMask.GetMask("Goal")))
        {
            GameManager.instance.StageClear();
        }
        else if (down.IsTouchingLayers(LayerMask.GetMask("Block")) || down.IsTouchingLayers(LayerMask.GetMask("AltPlayer")) || (down.IsTouchingLayers(LayerMask.GetMask("Ladder"))&&!cd2d.IsTouchingLayers(LayerMask.GetMask("Ladder"))))
        {
            isLadder = false;
            canMove = true;
        }
        else if (isLadder)
        {
            canMove = true;
        }
        else
        {
            StartCoroutine(Fall());
        }
    }

    public void PlayerGameOver()
    {
        gameover = true;
        Instantiate(restartMessage, new Vector3(0, -7.7f ,0), new Quaternion(0, 0, 0, 0));
        Debug.Log("죽었어!");
    }
}

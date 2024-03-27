using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorController : MonoBehaviour
{
    public GameObject switchBlockT;
    public GameObject switchBlockF;
    TilemapRenderer TR;
    TilemapCollider2D TC2D;
    TilemapRenderer TRF;
    TilemapCollider2D TC2DF;
    TilemapCollider2D tc2d;
    bool isEnabled;
    ArrayList record;

    // Start is called before the first frame update
    void Start()
    {
        isEnabled = true;
        if(switchBlockT != null)
        {
            TR = switchBlockT.GetComponent<TilemapRenderer>();
            TC2D = switchBlockT.GetComponent<TilemapCollider2D>();

        }
        if (switchBlockF != null)
        {
            TRF = switchBlockF.GetComponent<TilemapRenderer>();
            TC2DF = switchBlockF.GetComponent<TilemapCollider2D>();
        }
        tc2d = GetComponent<TilemapCollider2D>();
        record = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            if (tc2d.IsTouchingLayers(LayerMask.GetMask("Player")) || tc2d.IsTouchingLayers(LayerMask.GetMask("AltPlayer")) || tc2d.IsTouchingLayers(LayerMask.GetMask("Spike")))
            {
                if (switchBlockT != null)
                {
                    TR.enabled = true;
                    TC2D.enabled = true;
                }
                if (switchBlockF != null)
                {
                    TRF.enabled = false;
                    TC2DF.enabled = false;
                }
            }
            else
            {
                if (switchBlockT != null)
                {
                    TR.enabled = false;
                    TC2D.enabled = false;
                }
                if (switchBlockF != null)
                {
                    TRF.enabled = true;
                    TC2DF.enabled = true;
                }
            }

        }
    }

    public void SetEnabled(bool TF)
    {
        isEnabled = TF;
    }

    public void Check()
    {
        if (tc2d.IsTouchingLayers(LayerMask.GetMask("Player")) || tc2d.IsTouchingLayers(LayerMask.GetMask("AltPlayer")) || tc2d.IsTouchingLayers(LayerMask.GetMask("Spike")))
        {
            record.Add(1);
        }
        else
        {
            record.Add(0);
        }
    }

    public void Undo()
    {
        int time = GameManager.instance.GetTime();
        if (time - 1 < 0)
        {
            return;
        }
        if ((int)record[time - 1] == 1)
        {
            if (switchBlockT != null)
            {
                TR.enabled = true;
                TC2D.enabled = true;
            }
            if (switchBlockF != null)
            {
                TRF.enabled = false;
                TC2DF.enabled = false;
            }
        }
        else
        {
            if (switchBlockT != null)
            {
                TR.enabled = false;
                TC2D.enabled = false;
            }
            if (switchBlockF != null)
            {
                TRF.enabled = true;
                TC2DF.enabled = true;
            }
        }
    }

    public void MakeNewList()
    {
        record = new ArrayList();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    bool isControllable;//움직일수 있는지 아닌지
    bool isStart;//start위에 있는지 아닌지
    public GameObject arrow;
    bool e = false;
    bool x = false;
    bool t = false;
    bool r = false;
    bool a = false;
    public GameObject ex;

    // Start is called before the first frame update
    void Start()
    {
        isControllable = true;
        isStart = true;
        e = false;
        x = false;
        t = false;
        r = false;
        a = false;
        ex.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isControllable)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                isStart = !isStart;
            }
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                if (isStart && !a)
                {
                    isControllable = false;
                    SceneManager.LoadScene(1);
                }
                else if(isStart && a)
                {
                    isControllable = false;
                    SceneManager.LoadScene(12);
                }
                else
                {
                    Application.Quit();
                }
            }
        }

        if (isStart)
        {
            arrow.transform.position = new Vector3(0.47f, -0.4f);//화살표가 start에 위치
        }
        else
        {
            arrow.transform.position = new Vector3(0.47f, -2.56f);//화살표가 start에 위치//화살표가 quit에 위치
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            e = true;
        }
        if (Input.GetKeyDown(KeyCode.X) && e)
        {
            x = true;
        }
        if (Input.GetKeyDown(KeyCode.T) && x)
        {
            t = true;
        }
        if (Input.GetKeyDown(KeyCode.R) && t)
        {
            r = true;
        }
        if (Input.GetKeyDown(KeyCode.A) && r)
        {
            a = true;
            ex.SetActive(true);
        }



    }

}

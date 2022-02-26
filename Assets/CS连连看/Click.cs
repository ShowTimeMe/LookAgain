using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Click : MonoBehaviour
{
    public Vector2 id;//
    public float point;//分数
    public float time;//时间增加
    Manager m;
    // Start is called before the first frame update
    void Start()
    {
        m = Manager.instance;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }


    void OnClick()
    {
        if (m.gameState !=Manager.GameState.Play)
        {
            return;
        }
        if (m.isClick)
        {
            return;
        }
        if (!m.firestBtn)
        {
            m.firestBtn = GetComponent<Button>();
            m.firestBtn.GetComponent<Outline>().effectColor = Color.green;
        }
        else
        {
            if (!m.secondBtn && !m.firestBtn.Equals(GetComponent<Button>()))//
            {
                m.secondBtn = GetComponent<Button>();
                m.secondBtn.GetComponent<Outline>().effectColor = Color.green;
                //判断是否可销毁
                m.TestBlockDestory();
            }
        }
    }
}

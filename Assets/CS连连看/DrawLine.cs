using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    bool isDraw;//是否划线
    float lineWidth=5;//线的宽度
    float time = 0.4f;//停顿时间
    List<Vector3> vList = new List<Vector3>();
    public GameObject linePrefad;
    LineRenderer lineRenderer;//组件



   
    void Start()
    {
        
    }

   
    void Update()
    {
        if (isDraw)
        {
            for (int i = 0; i < vList.Count; i++)
            {
                lineRenderer.SetPosition(i, vList[i]);
            }
        }
    }

    //public void SetLine(Vector2 v1,Vector2 v2)
    //{
    //    GameObject lr = Instantiate(linePrefad, transform);
    //    lineRenderer = lr.GetComponent<LineRenderer>();
    //    lineRenderer.startWidth = lineWidth;
    //    lineRenderer.endWidth = lineWidth;
    //    lineRenderer.positionCount = 2;
    //    lineRenderer.sortingOrder = 100;
    //    vList.Clear();
    //    vList.Add(new Vector3(v1.x,v1.y,-10));
    //    vList.Add(new Vector3(v2.x, v2.y, -10));
    //    StartCoroutine(Draw());
    //}
    //public void SetLine(Vector2 v1, Vector2 v2,Vector3 v3)
    //{
    //    GameObject lr = Instantiate(linePrefad, transform);
    //    lineRenderer = lr.GetComponent<LineRenderer>();
    //    lineRenderer.startWidth = lineWidth;
    //    lineRenderer.endWidth = lineWidth;
    //    lineRenderer.positionCount = 3;
    //    lineRenderer.sortingOrder = 100;
    //    vList.Clear();
    //    vList.Add(new Vector3(v1.x, v1.y, -10));
    //    vList.Add(new Vector3(v2.x, v2.y, -10));
    //    vList.Add(new Vector3(v3.x, v3.y, -10));
    //    StartCoroutine(Draw());
    //}
    //public void SetLine(Vector2 v1, Vector2 v2, Vector3 v3 ,Vector3 v4)
    //{
    //    GameObject lr = Instantiate(linePrefad, transform);
    //    lineRenderer = lr.GetComponent<LineRenderer>();
    //    lineRenderer.startWidth = lineWidth;
    //    lineRenderer.endWidth = lineWidth;
    //    lineRenderer.positionCount = 4;
    //    lineRenderer.sortingOrder = 100;
    //    vList.Clear();
    //    vList.Add(new Vector3(v1.x, v1.y, -10));
    //    vList.Add(new Vector3(v2.x, v2.y, -10));
    //    vList.Add(new Vector3(v3.x, v3.y, -10));
    //    vList.Add(new Vector3(v4.x, v4.y, -10));
    //    StartCoroutine(Draw());
    //}



    public void SetLine(params Vector2[] vPos)
    {
        GameObject lr = Instantiate(linePrefad, transform);
        lineRenderer = lr.GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = vPos.Length;
        lineRenderer.sortingOrder = 100;
        vList.Clear();
        for (int i = 0; i < vPos.Length; i++)
        {
            vList.Add(new Vector3(vPos[i].x, vPos[i].y, -10));
        }
        StartCoroutine(Draw());
    }
    public IEnumerator Draw()
    {
        isDraw = true;
        yield return new WaitForSeconds(time);
        isDraw = false;
        yield return new WaitForSeconds(0.1f);
        DelLine();
    }

    void DelLine()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }
}

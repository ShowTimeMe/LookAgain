using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    public DrawLine drawLine;
    public Transform Panel;//连连看的小按钮
    public RectTransform rectTransform;//图片的位置
    float picWidth = 100;
    public GameObject[] picArray;//预制体数组
    public GameObject[,] blocks;//矩阵


    int xNum = 10;
    int yNum = 10;
    int lv = 1;//等级
    int blockNum = 8;//块数
    float time = 45;//通关时间

    Vector3 screenV3;//屏幕中心坐标
    float xStartPos;///
    float yStartPos;//


    float  totalPoint;
    public Text pointText;//分数面板
    public Transform cancas;
    public Transform infoBar;//信息面板
    Text infoText;//信息文本
    public Slider timeSlider;//时间条

    public enum GameState { Play,Menu};
    public GameState gameState = GameState.Play;
    /// <summary>
    /// 点击变量
    /// </summary>
    public bool isClick;
    [HideInInspector]
    public Button firestBtn;//第一个按钮
    [HideInInspector]
    public Button secondBtn;//第二个按钮

    Vector2 point2;
    Vector2 point3;


    public void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetLv();
        SetScreenPos();
        SpawnAllBlock();
    }
    void SetLv()//设置等级
    {
        xNum = 4 + 2 * lv;
        yNum = 2 + 2 * lv;
        time--;//每升一级 时间减一秒
        //设置时间条
        timeSlider.maxValue = time;
        timeSlider.value = time;
        picWidth = picWidth - 8 * Mathf.Clamp(lv - 3, 0, 6);//根据等级变化缩小
        if (blockNum<picArray.Length)
        {
            blockNum = lv + 6;
        }
    }

    void SetScreenPos()
    {
        screenV3 = new Vector3(Screen.width / 2, Screen.height / 2, 0);//中心点
        xStartPos =  - picWidth * (xNum + 2) / 2;//
        yStartPos = - picWidth * (yNum + 2) / 2;//

        infoText = infoBar.GetComponentInChildren<Text>();
    }


    void SpawnBlock(int num, int x, int y)//生成
    {
        GameObject obj = Instantiate(picArray[num], Panel);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(picWidth, picWidth);//方块大小
        //设置按钮点击id
        obj.GetComponent<Click>().id = new Vector2(x, y);
        blocks[x, y] = obj;
        blocks[x, y].GetComponent<RectTransform>().position =IdToPosition(new Vector2(x, y));
    }

    void SpawnAllBlock()//生成所有块
    {
        blocks = new GameObject[xNum + 2, yNum + 2];//
        int v = 0;
        for (int i = 1; i < yNum + 1; i++)
        {
            for(int j=1;j<=xNum/2;j++)
            {
                if (v == blockNum)
                {
                    v = 0;
                }
                SpawnBlock(v, j, i);
                SpawnBlock(v, j + xNum / 2, i);
                v++;
            }
        }
        //循环
        for (int i = 0; i < xNum * yNum; i++)
        {
            RandomBlock();
        }
    }
    void RandomBlock()//方块位置随机交换
    {
        Vector2 v2 = new Vector2();//块要交换的id
        Vector2 id1 = new Vector2();
        Vector2 id2 = new Vector2();
        //随机点
        int rxNum1 = Random.Range(1, xNum + 1);
        int ryNum1 = Random.Range(1, yNum + 1);
        int rxNum2 = Random.Range(1, xNum + 1);
        int ryNum2 = Random.Range(1, yNum + 1);
        //位置转换
        v2 = blocks[rxNum1, ryNum1].GetComponent<RectTransform>().position;//获取坐标
        //获取id1  id2  id
        id1 = blocks[rxNum1, ryNum1].GetComponent<Click>().id;
        id2 = blocks[rxNum2, ryNum2].GetComponent<Click>().id;
        GameObject temp = blocks[rxNum1, ryNum1];
        //换位置
        blocks[rxNum1, ryNum1].GetComponent<RectTransform>().position =
            blocks[rxNum2, ryNum2].GetComponent<RectTransform>().position;
        //换数组
        blocks[rxNum1, ryNum1] = blocks[rxNum2, ryNum2];
        blocks[rxNum1, ryNum1].GetComponent<Click>().id=id1;//更改block[rx1,ry1]的id

        blocks[rxNum2, ryNum2].GetComponent<RectTransform>().position = v2;
        blocks[rxNum2, ryNum2] = temp;
        blocks[rxNum2, ryNum2].GetComponent<Click>().id=id2;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameState != GameState.Play)
        {
            return;
        }
        timeSlider.value -= Time.deltaTime;
        if (timeSlider.value == 0)
        {
            gameState = GameState.Menu;
            infoBar.gameObject.SetActive(true);
            infoText.text = "游戏结束";
        }
    }
    public void GameOver()
    {
        if (Panel.childCount == 0)
        {
            if (blockNum < picArray.Length)
            {
                totalPoint += lv * 1000;
                pointText.text = "分数" + totalPoint;
                StartCoroutine(SetMessage("开始下一局"));
            }
            else
            {
                StartCoroutine(SetMessage("恭喜通关"));
            }
        }
    }
    IEnumerator SetMessage(string str)
    {
        gameState = GameState.Menu;
        //程序暂停
        infoBar.gameObject.SetActive(true);
        //加分
        AddTimePoint(0, Mathf.Round(timeSlider.value * 10));
        infoText.text = str;
        lv++;//等级增加
        yield return new WaitForSeconds(2);
        //开始下一局
        gameState = GameState.Play;
        Start();
        infoBar.gameObject.SetActive(false);
    }

    bool TestBlockIsPass(Vector2 ft, Vector2 st)
    {
        int startX = (int)Mathf.Min(ft.x, st.x);//第一个和第二个 那个小
        int endX = (int)Mathf.Max(ft.x, st.x);//那个大
        int startY = (int)Mathf.Min(ft.y, st.y);//第一个和第二个 那个小
        int endY = (int)Mathf.Max(ft.y, st.y);//那个大

        if (startX == endX)//若果在一列上
        {
            for (int j = startY + 1; j < endY; j++)//遍历 
            {
                if (blocks[endX, j])
                {
                    return false;
                }
            }
        }
        else
        {
            if (startY == endY)
            {
                for (int i = startX + 1; i < endX; i++)//遍历 
                {
                    if (blocks[i, endY])
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    Vector3 IdToPosition(Vector2 v2)
    {
        Vector3 v = new Vector3(xStartPos + picWidth * v2.x + picWidth / 2, yStartPos + picWidth * v2.y + picWidth / 2);
        return v;
    }

    bool Linel(Vector2 btn1Id, Vector2 btn2Id)
    {
        return TestBlockIsPass(btn1Id, btn2Id);
    }
    bool Line2(Vector2 id1, Vector2 id2, out Vector2 point2)
    {
        Vector2 fId = id1;
        Vector2 sId = id2;
        Vector2 newId1 = new Vector2(fId.x, sId.y);
        Vector2 newId2 = new Vector2(sId.x, fId.y);
        if (!blocks[(int)newId1.x,(int)newId1.y])
        {
            if (Linel(newId1, fId) && Linel(newId1, sId))
            {
                // 返回中间点坐标point2
                point2 = IdToPosition(new Vector2((int)newId1.x, (int)newId1.y));
                return true;
            }
        }
        if (!blocks[(int)newId2.x, (int)newId2.y])
        {
            if (Linel(newId2, fId) && Linel(newId2, sId))
            {
                // 返回中间点坐标point2
                point2 = IdToPosition(new Vector2((int)newId2.x, (int)newId2.y));
                return true;
            }
        }
        point2 = new Vector2();
        return false;
    }

    bool Line3(Vector2 id,out Vector2 point3)
    {
        for (int i=(int )id.x-1;i>=0;i--)//左
        {
            if (blocks[i, (int)id.y])
            {
                break;
            }
            else
            {
                if (Line2(new Vector2(i,(int )id.y),secondBtn.GetComponent<Click>().id,out point2))
                {
                    point3 = IdToPosition(new Vector2(i, (int)id.y));
                    return true;
                }
            }
        }

        for (int i = (int)id.x + 1; i <xNum+2; i++)//右
        {
            if (blocks[i, (int)id.y])
            {
                break;
            }
            else
            {
                if (Line2(new Vector2(i, (int)id.y), secondBtn.GetComponent<Click>().id, out point2))
                {
                    point3 = IdToPosition(new Vector2(i, (int)id.y));
                    return true;
                }
            }
        }

        for (int i = (int)id.y - 1; i >= 0; i--)//下
        {
            if (blocks[ (int)id.x,i])
            {
                break;
            }
            else
            {
                if (Line2(new Vector2( (int)id.x,i), secondBtn.GetComponent<Click>().id, out point2))
                {
                    point3 = IdToPosition(new Vector2((int)id.x,i));
                    return true;
                }
            }
        }

        for (int i = (int)id.y + 1; i < yNum + 2; i++)//上
        {
            if (blocks[(int)id.x, i])
            {
                break;
            }
            else
            {
                if (Line2(new Vector2((int)id.x, i), secondBtn.GetComponent<Click>().id, out point2))
                {
                    point3 = IdToPosition(new Vector2((int)id.x, i));
                    return true;
                }
            }
        }
        point3 = Vector2.zero;
        return false;
    }

    public void TestBlockDestory()
    {
        if (firestBtn.name == secondBtn.name)//名字相同
        {
            Vector2 fId = firestBtn.GetComponent<Click>().id;
            Vector2 sId = secondBtn.GetComponent<Click>().id;

            Vector3 tempV1;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cancas.GetComponent<RectTransform>(), firestBtn.transform.position, Camera.main, out tempV1);
            Vector3 tempV2;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cancas.GetComponent<RectTransform>(), secondBtn.transform.position, Camera.main, out tempV2);
            if (Linel(fId, sId))
            {
                isClick = true;
               
                //销毁物体按钮
                StartCoroutine(DestoryObj());
                //Invoke("DestoryObj", 1);
                //画线
                drawLine.SetLine(tempV1+screenV3, tempV2+screenV3);
                print("11");
                return;
            }
            if (Line2(fId, sId,out point2))
            {
                isClick = true;
                Vector3 tempV3;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cancas.GetComponent<RectTransform>(),new Vector2(point2.x,point2.y), Camera.main, out tempV3);
                StartCoroutine(DestoryObj());
                //画线
                drawLine.SetLine(tempV1 + screenV3,tempV3+screenV3, tempV2 + screenV3);
                print("22");
                return;
            }
            if (Line3(fId, out point3))
            {
                isClick = true;
                Vector3 tempV3;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cancas.GetComponent<RectTransform>(), new Vector2(point2.x, point2.y), Camera.main, out tempV3);

                Vector3 tempV4;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cancas.GetComponent<RectTransform>(), new Vector2(point3.x, point3.y), Camera.main, out tempV4);

                StartCoroutine(DestoryObj());
                //画线
                drawLine.SetLine(tempV1 + screenV3,tempV4+screenV3, tempV3 + screenV3, tempV2 + screenV3);
                print("33");
                return;
            }
        }
        firestBtn.GetComponent<Outline>().effectColor = Color.white;//点击两次恢复颜色
        secondBtn.GetComponent<Outline>().effectColor = Color.white;
        firestBtn = null;//清空
        secondBtn = null;
    }

    IEnumerator DestoryObj()
    {
        yield return new WaitForSeconds(0.5f);
        //加分
        AddTimePoint(secondBtn.GetComponent<Click>().time,secondBtn.GetComponent <Click>().point);
        Destroy(firestBtn.gameObject);
        Destroy(secondBtn.gameObject);
        isClick = false;
        yield return new WaitForSeconds(0.1f);
        //检测是否过关
       
        GameOver();
    }

    //void DestoryObj()
    //{
    //    Destroy(firestBtn.gameObject);
    //    Destroy(secondBtn.gameObject);
    //    //加分
    //    isClick = false;
    //    //检测是否过关
    //}


    void AddTimePoint(float time, float   point)//加分
    {
        timeSlider.value += time;
        totalPoint += point;
        pointText.text = "分数:" + totalPoint;
    }
}

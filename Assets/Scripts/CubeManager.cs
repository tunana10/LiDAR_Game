using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeManager : MonoBehaviour
{
    [Header("UI 容器設定")]
    public RectTransform gridParent;    // 放置所有 Cube 的 UI 物件 (Grid Layout Group)
    public GameObject cubePrefab;       // Cube 預製物（必須包含 Image 與 CubeBehavior 腳本）

    [Header("格子設定")]
    public int rows = 4;
    public int cols = 4;
    public float spacing = 15f;          // UI間距

    [Header("顏色設定")]
    public Color greenColor = Color.green;
    public Color redColor = Color.red;
    public Color blueColor = Color.blue;
    public Color blackColor = Color.black;



    private List<CubeBehavior> cubes = new List<CubeBehavior>();
    private bool isPlaying = false;

    void Start()
    {
        SetupGrid();
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isPlaying)
            {
                isPlaying = true;
                StartCoroutine(GameLoop());
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Cursor.visible = false;
        }
    }

    void SetupGrid()
    {
        // 先清空舊物件
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        cubes.Clear();

        // 建立新的方塊
        GridLayoutGroup grid = gridParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = cols;
            grid.spacing = new Vector2(spacing, spacing);
        }

        for (int i = 0; i < rows * cols; i++)
        {
            GameObject cube = Instantiate(cubePrefab, gridParent);
            CubeBehavior cb = cube.GetComponent<CubeBehavior>();
            cb.manager = this;
            cubes.Add(cb);
        }
    }

    IEnumerator GameLoop()
    {
        while (isPlaying)
        {
            // 分配顏色
            AssignRandomColors();

            // 等待直到所有藍色消失
            yield return new WaitUntil(() => AllBlueGone());

            // 全部變黑
            foreach (var cube in cubes)
                cube.SetColor(blackColor);

            yield return new WaitForSeconds(3f);
        }
    }

    void AssignRandomColors()
    {
        foreach (var cube in cubes)
        {
            int r = Random.Range(0, 3); // 0,1,2
            switch (r)
            {
                case 0:
                    cube.SetColor(greenColor);
                    cube.colorType = CubeBehavior.ColorType.Green;
                    break;
                case 1:
                    cube.SetColor(redColor);
                    cube.colorType = CubeBehavior.ColorType.Red;
                    break;
                case 2:
                    cube.SetColor(blueColor);
                    cube.colorType = CubeBehavior.ColorType.Blue;
                    break;
            }
        }
    }

    bool AllBlueGone()
    {
        foreach (var cube in cubes)
        {
            if (cube.colorType == CubeBehavior.ColorType.Blue)
                return false;
        }
        return true;
    }
}

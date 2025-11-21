using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [Header("Target 設定")]
    public GameObject targetPrefab;
    public int minTargetCount = 3;
    public int maxTargetCount = 5;
    public Vector2 spawnArea = new Vector2(8f, 4.5f);
    public float triggerDis = 1f;

    [Header("禁區設定")]
    public bool enableNoSpawnZone = true;
    public Vector2 noSpawnZoneCenter = Vector2.zero;
    public Vector2 noSpawnZoneSize = new Vector2(2f, 2f);

    [Header("滑鼠修正設定")]
    public Vector2 mouseOffset = Vector2.zero;       // 手動修正滑鼠座標（像素）
    public Vector2 mouseScale = new Vector2(1f, 1f); // 若畫面被縮放，可調整比例
    public bool showMouseGizmo = true;               // 是否在 Scene 顯示滑鼠點

    private List<GameObject> targets = new List<GameObject>();
    private bool isRespawning = false;
    public Camera mainCam;

    private Vector3 debugMouseWorld; // 用於 Gizmo 顯示滑鼠位置

    void Start()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        SpawnTargets();
    }

    void Update()
    {
        CheckMouseCollision();
    }

    void SpawnTargets()
    {
        int count = Random.Range(minTargetCount, maxTargetCount + 1);
        targets.Clear();

        for (int i = 0; i < count; i++)
        {
            Vector3 pos;
            int attempts = 0;

            do
            {
                pos = new Vector3(Random.Range(-spawnArea.x, spawnArea.x),
                                  Random.Range(-spawnArea.y, spawnArea.y),
                                  0);
                attempts++;
                if (attempts > 100) break;
            }
            while (enableNoSpawnZone && IsInsideNoSpawnZone(pos));

            GameObject newTarget = Instantiate(targetPrefab, pos, Quaternion.identity);
            targets.Add(newTarget);
        }

        isRespawning = false;
    }

    bool IsInsideNoSpawnZone(Vector3 position)
    {
        Vector2 halfSize = noSpawnZoneSize / 2f;
        return (position.x > noSpawnZoneCenter.x - halfSize.x &&
                position.x < noSpawnZoneCenter.x + halfSize.x &&
                position.y > noSpawnZoneCenter.y - halfSize.y &&
                position.y < noSpawnZoneCenter.y + halfSize.y);
    }

    void CheckMouseCollision()
    {
        // 調整滑鼠位置（可修正畫面對位誤差）
        Vector3 mousePos = Input.mousePosition;
        mousePos.x = (mousePos.x + mouseOffset.x) * mouseScale.x;
        mousePos.y = (mousePos.y + mouseOffset.y) * mouseScale.y;

        // 轉為世界座標
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;
        debugMouseWorld = worldPos;

        // 判斷是否接觸 target
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            GameObject target = targets[i];
            if (target == null) continue;

            float dist = Vector2.Distance(worldPos, target.transform.position);
            if (dist < triggerDis)
            {
                Destroy(target);
                targets.RemoveAt(i);
            }
        }

        if (targets.Count == 0 && !isRespawning)
        {
            isRespawning = true;
            StartCoroutine(RespawnAfterDelay());
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        SpawnTargets();
    }

    private void OnDrawGizmosSelected()
    {
        // 畫禁區
        if (enableNoSpawnZone)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(noSpawnZoneCenter, new Vector3(noSpawnZoneSize.x, noSpawnZoneSize.y, 0.1f));
        }

        // 畫生成區域
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnArea.x * 2f, spawnArea.y * 2f, 0.1f));

        // 畫滑鼠位置
        if (showMouseGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(debugMouseWorld, 0.2f);
        }
    }
}

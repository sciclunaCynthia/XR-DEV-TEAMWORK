using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveManager : MonoBehaviour
{
    // Turret (and anything else) can listen to this to start when waves begin
    public static event System.Action OnWavesStarted;

    [Header("Setup")]
    public GameObject enemyPrefab;
    public List<WaypointPath> lanes = new();

    [Header("Wave Settings")]
    public int enemiesPerWave = 6;
    public float spawnInterval = 0.8f;
    public float timeBetweenWaves = 6f;

    [Header("Spawn Spacing")]
    public float spacing = 0.6f; // distance between spawned enemies

    private bool _running;
    private int _wave = 0;

    public void StartWaves()
    {
        Debug.Log("StartWaves() CALLED");

        if (_running) return;
        _running = true;

        // Notify listeners (e.g., Turret) that waves have started
        OnWavesStarted?.Invoke();

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            _wave++;
            Debug.Log($"Wave {_wave} Incoming!");
            UIManager.Instance.UpdateWaveText();

            yield return SpawnWave(enemiesPerWave);

            Debug.Log($"Wave {_wave} Active");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnWave(int count)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("WaveManager: Enemy Prefab is NOT assigned!");
            yield break;
        }

        if (lanes == null || lanes.Count == 0)
        {
            Debug.LogError("WaveManager: No lanes assigned!");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            WaypointPath lane = lanes[Random.Range(0, lanes.Count)];

            if (lane == null || lane.waypoints == null || lane.waypoints.Count < 2)
            {
                Debug.LogError("WaveManager: Lane needs at least 2 waypoints (start + next).");
                yield break;
            }

            Transform wp0 = lane.waypoints[0];
            Transform wp1 = lane.waypoints[1];

            // Direction from waypoint0 -> waypoint1
            Vector3 dir = (wp1.position - wp0.position).normalized;

            // Spawn each enemy slightly behind the previous one to avoid stacking
            Vector3 spawnPos = wp0.position - dir * spacing * i;

            // Optional ground snap
            if (Physics.Raycast(spawnPos + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
            {
                spawnPos.y = hit.point.y;
            }

            Quaternion spawnRot = wp0.rotation;
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, spawnRot);

            var mover = enemy.GetComponent<EnemyMover>();
            if (mover == null)
            {
                Debug.LogError("Spawned enemy has NO EnemyMover component! Add EnemyMover to the prefab.");
            }
            else
            {
                mover.Init(lane);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

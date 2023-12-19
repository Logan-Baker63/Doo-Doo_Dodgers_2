using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Spawnable
{
    [SerializeField] GameObject m_prefab;
    [SerializeField] int m_spawnWeight;

    public GameObject prefab { get { return m_prefab; } }
    public int weight { get { return m_spawnWeight; } }
}

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] List<Spawnable> m_spawnables;

    [SerializeField] bool m_isTeleporter = false;
    [SerializeField] [ConditionalHide("m_isTeleporter")] float m_teleportPerSec = 1;
    [SerializeField] [ConditionalHide("m_isTeleporter")] Vector3 m_teleportOffset;
    Vector3 m_originalPos;

    GameObject m_obj;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }

    public void Spawn()
    {
        m_obj = Instantiate(GetRandomSpawnable(), transform.position, transform.rotation);
        m_obj.transform.SetParent(transform.parent);

        m_originalPos = m_obj.transform.localPosition;
    }

    GameObject GetRandomSpawnable()
    {
        int totalWeight = 0;
        foreach (Spawnable spawnable in m_spawnables) totalWeight += spawnable.weight;

        int randomWeight = Random.Range(0, totalWeight);
        for (int i = 0; i < m_spawnables.Count; ++i)
        {
            randomWeight -= m_spawnables[i].weight;
            if (randomWeight < 0)
            {
                return m_spawnables[i].prefab;
            }
        }

        Debug.LogWarning("Weight find failed");
        return m_spawnables[0].prefab;
    }

    float m_counter = 0;
    private void Update()
    {
        if (m_isTeleporter)
        {
            m_counter += Time.deltaTime;

            if (m_counter >= 1 / m_teleportPerSec)
            {
                if (m_obj.transform.localPosition != m_originalPos) m_obj.transform.localPosition = m_originalPos;
                else m_obj.transform.localPosition += m_teleportOffset;

                m_counter = 0;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public GameObject _spawnPointContainer;
    public GameObject _tankPrefab;

    public delegate void OnTankDeathManager();    // This will be called when no tank left in the scene
    public OnTankDeathManager dOnTankDeath = null;

    public delegate void OnTankDamageManager(Tank tank);    // This will be called when no tank left in the scene
    public OnTankDamageManager dOnTankDamage = null;

    protected Color[] mPlayerColors =
    {
        Color.green,
        Color.blue,
    };

    protected int mPlayerCount;
    protected List<Tank> mTanks = new List<Tank>();
    protected List<Transform> mSpawnPoints = new List<Transform>();

    private void Awake()
    {
        // Setup the spawn points from spawn parent
        Transform spawnTrans = _spawnPointContainer.transform;
        for (int i = 0; i < spawnTrans.childCount; i++)
            mSpawnPoints.Add(spawnTrans.GetChild(i));

        SpawnTanks();
    }

    public void OnTankDeath()
    {
        // Reduce the player count and put the dead tank to the back of the list
        mPlayerCount--;
        if(mPlayerCount == 0)
        {
            dOnTankDeath.Invoke();
        }
    }

    public void OnTankDamage(Tank tank)
    {
        dOnTankDamage.Invoke(tank);
    }

    public void Restart()
    {
        foreach (Tank tank in mTanks)
        {
            int num = tank._playerNum;
            tank.Restart(mSpawnPoints[num].position, mSpawnPoints[num].rotation);
            tank.mHealth = tank._maxHealth;
        }
        mPlayerCount = mTanks.Count;
    }

    // Spawn and setup their color
    public void SpawnTanks()
    {
        mPlayerCount = mSpawnPoints.Count;

        for (int i = 0; i < mPlayerCount; i++)
        {
            // Spawn Tank and store it
            GameObject tank = Instantiate(_tankPrefab, mSpawnPoints[i].position, mSpawnPoints[i].rotation);
            mTanks.Add(tank.GetComponent<Tank>());
            mTanks[i]._playerNum = i;
            mTanks[i].dTankDestroyed = OnTankDeath;
            mTanks[i].dTankDamaged = OnTankDamage;

            // Color Setup
            MeshRenderer[] renderers = mTanks[i].GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
                rend.material.color = mPlayerColors[i];
        }
    }

    public Transform[] GetTanksTransform()
    {
        int count = mTanks.Count;
        Transform[] tanksTrans = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            tanksTrans[i] = mTanks[i].transform;
        }

        return tanksTrans;
    }

    public int NumberOfPlayers
    {
        get { return mSpawnPoints.Count; }
    }
}

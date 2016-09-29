using UnityEngine;
using System.Collections.Generic;
using PicaVoxel;

public class SmokeSystem : MonoBehaviour {
    [Header("The Volume")]
    [SerializeField]
    Volume volume;
    [Header("Every So Often Spawn")]
    [SerializeField]
    float spawnCD = 0.2f;
    [Header("Smoke Config")]
    [SerializeField]
    float smokeUpwardsSpeed = 0.1f;
    [SerializeField]
    float smokeRandomCloud = 0.1f;
    [SerializeField]
    Vector3 smokeWindTilt = Vector3.zero;

    [Header("SpawnFrom")]
    [SerializeField]
    List<Transform> LocationsToSpawn;

    float currentTime;


    void Update()
    {
        if (currentTime <= 0f)
        {
            //FailSafe
            if (LocationsToSpawn == null || LocationsToSpawn.Count == 0) return;
            //ResetTimer
            currentTime = spawnCD;

            //Create Batch
            Batch batch = new Batch(volume);            
            //Voxel to Spawn
            Voxel v = new Voxel();

            v.Color = Color.gray;
            int x = 0;
            foreach (Transform trans in LocationsToSpawn)
            {
                batch.Add(v, x, 0, 0, trans.position);
                x++;
            }

            VoxelParticleSystem.Instance.SpawnBatch(batch, pos => (Vector3.up *smokeUpwardsSpeed) + (Random.insideUnitSphere * smokeRandomCloud )+ (smokeWindTilt));

        }else
        {
            currentTime -= Time.deltaTime;
        }

    }



}
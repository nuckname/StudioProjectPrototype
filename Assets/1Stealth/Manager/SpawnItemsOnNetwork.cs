using Unity.Netcode;
using UnityEngine;

public class SpawnItemsOnNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject ItemToSpawn;
    
    //this could be a list that it random picks from -> so how got to change it so that it cant spawn in the same place twice.
    [SerializeField] private Transform spawnPoint;
    
    void Start()
    {
        SpawnObject(ItemToSpawn);
    }

    private void SpawnObject(GameObject objectToSpawn)
    {
        GameObject spawnedItem = Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);
        
        NetworkObject networkObject = spawnedItem.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
    }
}

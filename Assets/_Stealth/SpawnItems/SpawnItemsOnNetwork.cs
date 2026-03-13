using Unity.Netcode;
using UnityEngine;

public class SpawnItemsOnNetwork : NetworkBehaviour
{
    //this could be a list that it random picks from -> so how got to change it so that it cant spawn in the same place twice.
    [SerializeField] private Transform spawnPoint;
    
    
    [SerializeField] private GameObjectsSpawnSO gameObjectsSpawnSO;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        
        SpawnObjectServerRpc(0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(int objectIndexToSpawn)
    {
        GameObject spawnedItem = Instantiate(gameObjectsSpawnSO.gameObjects[objectIndexToSpawn], spawnPoint.position, Quaternion.identity);
        
        NetworkObject networkObject = spawnedItem.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
    }
 
}

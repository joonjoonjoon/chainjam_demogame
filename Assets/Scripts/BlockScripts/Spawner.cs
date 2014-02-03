using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public List<GameObject> prefabs;
	public float randomOffset;
	public float firepower;
	public float cooldownMax = 0.2f;
	public Transform spawnPoint;
	public bool enabled;
	private float cooldown;

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		if(enabled)
		{
			if(cooldown <= 0)
			{
				cooldown = cooldownMax;
				SpawnCube();
			}

			cooldown-= Time.deltaTime;
		}
	}

	void SpawnCube(){
		GameObject cube = Instantiate(prefabs[(int)(Random.value * prefabs.Count)],spawnPoint.position,spawnPoint.rotation) as GameObject;
		cube.rigidbody.AddForce((float)(spawnPoint.rotation.eulerAngles.z + (Random.value - 0.5) * randomOffset) * firepower,0,0);
		//cube.rigidbody.AddForce((float)(spawnPoint.rotation.eulerAngles.z ) * firepower,0,0);
	}
}

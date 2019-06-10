using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public float spawnTime = 2;

    public GameObject target;
    public bool spawning = false;
    public Attribute.eTeam team;

    public Color spawnColor;
    public float width;
    public float height;

    public void SetTarget(GameObject itarget)
    {
        target = itarget;
    }

    public void Spawn()
    {
        StartCoroutine(SpawnAfter(spawnTime));
        spawning = true;
    }

    public void CharacterSpawn()
    {
        StartCoroutine(SpawnCharacterAfter(spawnTime));
        spawning = true;
    }

    //Dung de Spawn ngay lap tuc
    public bool DirectSpawn(GameObject itarget)
    {
        float x = Random.Range(this.transform.position.x - width / 2, this.transform.position.x + width / 2);
        float y = Random.Range(this.transform.position.y - height / 2, this.transform.position.y + height / 2);

        Vector3 pos = new Vector3(x, y, 0);

        itarget.transform.position = pos;

        if (itarget.GetComponent<NPCController>().ReSpawn())
            return true;
        return false;
    }

    //Dung de Spawn Character ngay lap tuc
    public void DirectSpawnCharacter()
    {
        float x = Random.Range(this.transform.position.x - width / 2, this.transform.position.x + width / 2);
        float y = Random.Range(this.transform.position.y - height / 2, this.transform.position.y + height / 2);

        Vector3 pos = new Vector3(x, y, 0);

        target.transform.position = pos;

        target.GetComponent<Attribute>().ReSpawn();
        spawning = false;
    }

    IEnumerator SpawnAfter(float _time)
    {
        yield return new WaitForSeconds(_time);
    
        //target.transform.position = this.transform.position;

        float x = Random.Range(this.transform.position.x - width / 2, this.transform.position.x + width / 2);
        float y = Random.Range(this.transform.position.y - height / 2, this.transform.position.y + height / 2);

        Vector3 pos = new Vector3(x, y, 0);

        target.transform.position = pos;

        target.GetComponent<NPCController>().ReSpawn();
        spawning = false;
    }

    IEnumerator SpawnCharacterAfter(float _time)
    {
        yield return new WaitForSeconds(_time);

        //target.transform.position = this.transform.position;

        float x = Random.Range(this.transform.position.x - width / 2, this.transform.position.x + width / 2);
        float y = Random.Range(this.transform.position.y - height / 2, this.transform.position.y + height / 2);

        Vector3 pos = new Vector3(x, y, 0);

        target.transform.position = pos;

        target.GetComponent<Attribute>().ReSpawn();
        spawning = false;
    }
}

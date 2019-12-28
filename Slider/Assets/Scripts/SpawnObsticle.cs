using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObsticle : MonoBehaviour
{
    [SerializeField]
    GameObject Obsticle;
    [SerializeField]
    GameObject Obsticle2;
    [SerializeField]
    GameObject Player;

    bool Which = true;
   
    float time = 0;
    float posZ = 0;
    
    void Update()
    {
        posZ = Player.transform.position.z;
        time += Time.deltaTime;
        if (time >= 2-posZ/5000 && posZ < 9500 && posZ > 1000)
        {
            time = 0;
            Which = !Which;
            if (Which)
            {
                Instantiate(Obsticle, Player.transform.position + new Vector3(0, 0, 250), Quaternion.identity);
            }
            else
            {
                Instantiate(Obsticle2, Player.transform.position + new Vector3(0, 0, 250), Quaternion.identity);
            }
        }
    }
}

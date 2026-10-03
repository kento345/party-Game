using UnityEngine;

public class EfectDestory : MonoBehaviour
{
    float time = 0f;

    float t = 2;

    void Start()
    {
        time = t;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(time >= 0)
        {
            time -= Time.deltaTime;
            if(time < 0)
            {
                Destroy(gameObject);
            }
        }

    }
}

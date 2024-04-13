using UnityEngine;
using EbMasterData;

public class SampleScript : MonoBehaviour
{
    private readonly Core core = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        core.Log();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

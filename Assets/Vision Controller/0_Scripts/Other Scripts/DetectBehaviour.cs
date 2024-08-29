using UnityEngine;

public class DetectBehaviour : MonoBehaviour
{
    public void SomethingDetected(Transform obj)
    {
        obj.GetComponent<ChangeColor>().StartChangeColor();
    }
    
    public void SomethingExit(Transform obj)
    {
        obj.GetComponent<ChangeColor>().StopChangeColor();
    }
}

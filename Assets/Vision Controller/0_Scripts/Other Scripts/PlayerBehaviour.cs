using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject sensedSign;


    private void Start()
    {
        sensedSign.SetActive(false);
    }


    public void SomethingDetected(Transform obj)
    {
        obj.GetComponent<ChangeColor>().StartChangeColor();
        Debug.Log($"{obj.name} detected!");
    }
    
    public void SomethingExit(Transform obj)
    {
        obj.GetComponent<ChangeColor>().StopChangeColor();
        Debug.Log($"{obj.name} went outside the vision area!");
    }

    public void SomethingSensed(Transform obj)
    {
        sensedSign.SetActive(true);
        Debug.Log($"{obj.name} sensed!");
    }
    
    public void SomeSensedThingExit(Transform obj)
    {
        sensedSign.SetActive(false);
        Debug.Log($"{obj.name} went outside the sense area!");
    }
}

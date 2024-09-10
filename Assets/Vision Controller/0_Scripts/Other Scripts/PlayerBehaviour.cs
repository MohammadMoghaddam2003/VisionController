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
        SomeSensedThingExit(obj);
    }
    
    public void SomethingExit(Transform obj)
    {
        obj.GetComponent<ChangeColor>().StopChangeColor();
    }

    public void SomethingSensed(Transform obj)
    {
        sensedSign.SetActive(true);        
    }
    
    public void SomeSensedThingExit(Transform obj)
    {
        sensedSign.SetActive(false);        
    }
}

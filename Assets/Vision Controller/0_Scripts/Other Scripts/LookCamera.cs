using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private Transform _cameraTran;


    private void Awake() => _cameraTran = Camera.main.transform;


    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(_cameraTran.position - transform.position);
    }
}

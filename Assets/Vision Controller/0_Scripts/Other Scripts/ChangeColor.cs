using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private List<Gradient> gradients = new List<Gradient>();
    [SerializeField] private float speed = .1f;

        
    private Material[] _materials;
    private WaitForSeconds _wait;
    private Coroutine _coroutine;
    
    
    
    
    
    private void Awake()
    {
        _materials = GetComponent<MeshRenderer>().materials;
        _wait = new(speed);
    }

    public void StartChangeColor() => _coroutine ??= StartCoroutine(Change_Color());
    

    public void StopChangeColor()
    {
        for (int i = 0; i < gradients.Count; i++)
        {
            _materials[i].color = gradients[i].Evaluate(1);
        }
        StopCoroutine(_coroutine);
        _coroutine = null;
    }


    private IEnumerator Change_Color()
    {
        float evaluate = 0;
        float additionalValue = .1f;
        
        while (enabled)
        {
            for (int i = 0; i < gradients.Count; i++)
            {
                _materials[i].color = gradients[i].Evaluate(evaluate);
            }

            evaluate += additionalValue;

            if (evaluate is >= 1 or <= 0) additionalValue = -additionalValue;
            
            yield return _wait;
        }
            
        StopCoroutine(_coroutine);
    }
}

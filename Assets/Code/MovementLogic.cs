using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementLogic : MonoBehaviour
{
    [field: SerializeField]
    public Person Person = new Person();

    public int Id;

    public List<int> opinion;

    [field: SerializeField]
    public Opinion OpinionObject = new Opinion(new List<float>());

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}


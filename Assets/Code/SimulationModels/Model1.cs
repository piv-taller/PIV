using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Model1 : MonoBehaviour
{
    public List<Person> Population = new List<Person>();
    
    void Start()
    {
        Population = new List<Person>()
        {
            new Person(1, new Opinion(new List<float> { RndDouble(-1, 1) }), RndDouble(0, 1), RndDouble(0, 1)),
            new Person(2, new Opinion(new List<float> { RndDouble(-1, 1) }), RndDouble(0, 1), RndDouble(0, 1)),
            new Person(3, new Opinion(new List<float> { RndDouble(-1, 1) }), RndDouble(0, 1), RndDouble(0, 1)),
            new Person(4, new Opinion(new List<float> { RndDouble(-1, 1) }), RndDouble(0, 1), RndDouble(0, 1)),
            new Person(5, new Opinion(new List<float> { RndDouble(-1, 1) }), RndDouble(0, 1), RndDouble(0, 1))
        };

        for (int i = 0; i < 5; i++)
        {
            Person person1 = Population[Random.Range(0, Population.Count)];
            Person person2 = Population[Random.Range(0, Population.Count)];

            Interaction(person1, person2);

            foreach (Person person in Population)
            {
                Debug.Log($"Person {person.Id}:\n\tStubbornness: {person.Stubbornness}\t\tPrestige: {person.Prestige}");
                Debug.Log($"\tOPINION:");

                for (int j = 0; j < person.Opinion.OpinionVector.Count; j++)
                {
                    Debug.Log($"\t\t{j:##}: {person.Opinion.OpinionVector[j]}");
                }
            }
        }
    }

    public float RndDouble(float start, float end)
    {
        return Random.Range(start, end);
    }

    public (Person, Person) Interaction(Person p1, Person p2)
    {
        for (int j = 0; j < p1.Opinion.OpinionVector.Count; j++)
        {
            float baricenter = Baricenter(p1, p2, j);
            p1.Opinion.OpinionVector[j] += (baricenter - p1.Opinion.OpinionVector[j]) * (1 - p1.Stubbornness);
            p2.Opinion.OpinionVector[j] += (baricenter - p2.Opinion.OpinionVector[j]) * (1 - p2.Stubbornness);
        }

        return (p1, p2);
    }

    public float Baricenter(Person p1, Person p2, int j)
    {
        return ((p1.Opinion.OpinionVector[j]*(p1.Prestige + Mathf.Abs(p1.Opinion.OpinionVector[j])) + p2.Opinion.OpinionVector[j]*(p2.Prestige + Mathf.Abs(p2.Opinion.OpinionVector[j]))) / (p1.Prestige + Mathf.Abs(p1.Opinion.OpinionVector[j]) + p2.Prestige + Mathf.Abs(p2.Opinion.OpinionVector[j])));
    }
}

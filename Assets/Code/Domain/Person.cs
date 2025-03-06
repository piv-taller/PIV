using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Represents a person.
/// </summary>
[Serializable, StructLayout(LayoutKind.Sequential)]
public class Person
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the opinion.
    /// </summary>
    public Opinion Opinion { get; set; }

    /// <summary>
    /// Gets or sets the prestige.
    /// </summary>
    public float Prestige { get; set; }

    /// <summary>
    /// Gets or sets the stubbornness.
    /// </summary>
    public float Stubbornness { get; set; }

    public Person()
    {
        Id = 0;
        Opinion = new Opinion(new List<float>());
        Prestige = 0;
        Stubbornness = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="opinion"></param>
    /// <param name="prestige"></param>
    /// <param name="stubbornness"></param>
    public Person(int id, Opinion opinion, float prestige, float stubbornness)
    {
        Id = id;
        Opinion = opinion;
        Prestige = prestige;
        Stubbornness = stubbornness;
    }
}

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
    /// Gets or sets the influence.
    /// </summary>
    public double Influence { get; set; }

    /// <summary>
    /// Gets or sets the stubbornness.
    /// </summary>
    public double Stubbornness { get; set; }

    public Person()
    {
        Id = 0;
        Opinion = new Opinion(new List<int>());
        Influence = 0;
        Stubbornness = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="coords"></param>
    /// <param name="influence"></param>
    /// <param name="stubbornness"></param>
    public Person(int id, Opinion opinion, double influence, double stubbornness)
    {
        Id = id;
        Opinion = opinion;
        Influence = influence;
        Stubbornness = stubbornness;
    }
}

using System.Collections.Generic;

/// <summary>
/// Represents a pair of coordinates.
/// </summary>
public class Opinion
{
    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public List<int> OpinionVector { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Opinion"/> class.
    /// </summary>
    /// <param name="opinionVector">Represents the opinion vector.</param>
    public Opinion(List<int> opinionVector)
    {
        OpinionVector = opinionVector;
    }
}

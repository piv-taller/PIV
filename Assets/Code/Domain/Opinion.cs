using System.Collections.Generic;

/// <summary>
/// Represents a pair of coordinates.
/// </summary>
public class Opinion
{
    /// <summary>
    /// Gets the X coordinate. It represents a value from -1 to 1 for each choice.
    /// </summary>
    public List<float> OpinionVector { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Opinion"/> class.
    /// </summary>
    /// <param name="opinionVector">Represents the opinion vector.</param>
    public Opinion(List<float> opinionVector)
    {
        OpinionVector = opinionVector;
    }
}

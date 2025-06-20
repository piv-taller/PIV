using UnityEngine;
using System.Collections.Generic;

public class Grafico : MonoBehaviour
{
    [Header("Configuración del gráfico")]
    int opciones;
    public Color[] colores;
    public float lineWidth = 0.1f;
    // Listas para almacenar puntos y LineRenderer de cada línea
    private List<Vector3>[] points;
    private LineRenderer[] lines;
    public GameObject modelo;

    void Start()
    {
        opciones = modelo.GetComponent<Modelo>().opciones;
        colores = new Color[opciones];

        for (int i = 0; i < opciones; i++) colores[i] = Color.HSVToRGB(i / (float)opciones, 0.8f, 1.0f);;
        
        points = new List<Vector3>[opciones];
        lines = new LineRenderer[opciones];

        // Crear cada línea
        for (int i = 0; i < opciones; i++)
        {
            points[i] = new List<Vector3>();
            lines[i] = CreateLine(i);
        }
    }

    // Crea un GameObject con LineRenderer y lo configura
    LineRenderer CreateLine(int index)
    {
        GameObject lineObj = new GameObject("Line_" + index);
        lineObj.transform.parent = transform;

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 0;
        line.widthMultiplier = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));

        // Asignar el color definido en el array
        line.startColor = colores[index];
        line.endColor = colores[index];

        // Si deseas usar coordenadas locales
        line.useWorldSpace = false;
        
        return line;
    }

    public void ResetLine(int index) 
    {
        if (index >= 0 && index < lines.Length && lines[index] != null)
        {
            Destroy(lines[index].gameObject);
            lines[index] = CreateLine(index);
            points[index] = new List<Vector3>();
        }
    }

    // Método para agregar un nuevo punto a una línea específica y actualizarla
    public void AddPoint(int lineIndex, Vector3 newPoint)
    {
        if (lineIndex < 0 || lineIndex >= opciones)
        {
            Debug.LogWarning("Índice de línea inválido");
            return;
        }

        points[lineIndex].Add(newPoint);
        UpdateGraph(lineIndex);
    }

    // Actualiza el LineRenderer de la línea especificada
    void UpdateGraph(int lineIndex)
    {
        LineRenderer line = lines[lineIndex];
        line.positionCount = points[lineIndex].Count;
        line.SetPositions(points[lineIndex].ToArray());
    }
}

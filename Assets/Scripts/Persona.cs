using UnityEngine;

public class Persona : MonoBehaviour
{
    private Modelo modelo;
    public float[] opiniones;
    public float prestigio;
    public float cabezoneria;
    public Transform cabeza, torso, ojoD, ojoI;

    private int index;
    void Start()
    {
        modelo = GetComponentInParent<Modelo>();
        opiniones = new float[modelo.opciones];
        for(int i = 0; i < modelo.opciones; i++) {
            opiniones[i] = ((float)Random.Range(1,2000))/1000-1;
        }
        prestigio = ((float)Random.Range(1,1000))/1000;
        cabezoneria = ((float)Random.Range(1,1000))/1000;
    }

    void Update()
    {

    }

    public void SetIndex(int i) {
        index = i;
    }

    public void ModificarOpinion(int i, float o) {
        opiniones[i] = o;
    }
}

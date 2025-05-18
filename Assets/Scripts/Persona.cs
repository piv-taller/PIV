using UnityEngine;

public class Persona : MonoBehaviour
{
    private Modelo modelo;
    public float[] opiniones;
    public float prestigio;
    public float cabezoneria;
    public Transform cabeza, torso, ojoD, ojoI;
    private float ruido;


    private int index;
    void Start()
    {
        modelo = GetComponentInParent<Modelo>();
        opiniones = new float[modelo.opciones];
        for(int i = 0; i < modelo.opciones; i++) {
            opiniones[i] = ((float)Random.Range(1,2000))/1000-1;
        }
        do {
            ruido = Random.Range(-0.5f, 0.5f);
            prestigio = 0.5f + ruido;
        } while (prestigio <= 0f || prestigio >= 1f);
        
        do {
            ruido = Random.Range(-0.5f, 0.5f);
            cabezoneria = 0.5f + ruido;
        } while (cabezoneria <= 0f || cabezoneria >= 1f);

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
    public void ModificarPrestigio(float p) {
        prestigio = p;
    }
    public void ModificarCabezoneria(float c) {
        cabezoneria = c;
    }
}

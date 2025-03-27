using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class Modelo : MonoBehaviour
{
    public GameObject personaPrefab;
    public Camera camara;
    public int opciones, p_num, iteraciones, grado_inicial;
    public float sens_c, sens_p;
    private List<Persona> personas = new List<Persona>();
    private float dx = 2f;
    private float dy = 3f;
    bool esperar = false;
    public Dictionary<int, HashSet<int>> relaciones;
    public Dictionary<int, List<int>> redes_sociales;
    private int it = 0;
    int[] contador, n_conv;
    public float t = 0;
    List<Color> colores;
    public TMP_Text textvot;
    void Start()
    {
        n_conv = new int[p_num];

        colores = new List<Color>();

        for (int i = 0; i < opciones; i++) {
            float hue = i / (float)opciones; // Espaciamos el Hue equitativamente
            Color color = Color.HSVToRGB(hue, 0.8f, 1.0f); // Alta saturación y brillo
            colores.Add(color);
        }
        
        float proporcion = 2.0f; 

        int filas = Mathf.CeilToInt(Mathf.Sqrt(p_num / proporcion)); 
        int columnas = Mathf.CeilToInt((float)p_num / filas); 

        float offsetX = (columnas - 1) * dx / 2f;
        float offsetY = (filas - 1) * dy / 2f; // Ajuste basado en filas con objetos

        for (int i = 0; i < p_num; i++) {
            int fila = i / columnas;
            int columna = i % columnas;

            Vector3 posicion = new Vector3(columna * dx - offsetX, fila * dy - offsetY, 0); 

            GameObject child = Instantiate(personaPrefab, posicion, Quaternion.identity, transform);
            Persona cs = child.GetComponent<Persona>();
            if (cs != null) {
                cs.SetIndex(i);
                personas.Add(cs);
            }
        }


        relaciones = new Dictionary<int, HashSet<int>>();

        for (int i = 0; i < p_num; i++) relaciones[i] = new HashSet<int>();

        for (int i = 0; i < p_num; i++) {
            for (int j = 1; j <= grado_inicial/2; j++) {
                Amigo(i,(i+j+p_num)%p_num);
                Amigo(i,(i-j+p_num)%p_num);
            }
        }

        // p es la probabilidad de reorientar un enlace
        float prob = 0.5f; 

        // Recorrer cada nodo para reorientar enlaces
        for (int i = 0; i < p_num; i++) {
            // Hacemos una copia de la lista de vecinos para evitar modificar la colección mientras iteramos.
            List<int> vecinos = new List<int>(relaciones[i]);
    
            // Para evitar procesar dos veces el mismo enlace, solo consideramos aquellos vecinos con índice mayor que i.
            foreach (int vecino in vecinos) {
                if (vecino > i) {
                    // Con probabilidad p, reorientamos el enlace actual
                    if (Random.value < prob) {
                        // Desconectamos el enlace actual
                        Desamigo(i, vecino);

                        // Buscamos un nuevo nodo que no sea i y que aún no esté conectado a i
                        int nuevoVecino;
                        do {
                            nuevoVecino = Random.Range(0, p_num);
                        } while (nuevoVecino == i || relaciones[i].Contains(nuevoVecino));
                
                        // Conectamos i con el nuevo vecino
                        Amigo(i, nuevoVecino);
                    }
                }
            }
        }

        for (int i = 0; i < p_num; i++) Debug.Log($"Tenemos que los amigos de {i} son {string.Join(", ", relaciones[i])}");

        
        redes_sociales = new Dictionary<int, List<int>>();

        for (int i = 0; i < p_num; i++) redes_sociales[i] = new List<int>();


        for (int i = 0; i < p_num; i++) {
            int[] p = Permutacion(p_num);
            int[] q = Permutacion(p_num);
            for (int j = 0; j < p_num; j++) {
                if ((float)Random.Range(0,101)/100 > 0.75f && (i > Mathf.Floor(p_num/50) || (float)Random.Range(0,101)/100 < 0.25f)) break;
                if (p[j] == p[i]) j++;
                redes_sociales[p[i]].Add(p[j]);
            }
        }

        for (int i = 0; i < p_num; i++) Debug.Log($"La persona {i} tiene de seguidores {string.Join(", ", redes_sociales[i])}");

        AjustarCamara(columnas, filas, dx, dy);
    }

    void Update()
    {
        if (!esperar && it < iteraciones) {
            StartCoroutine(Iteración());
            esperar = true;
        }
        if (!esperar && it == iteraciones) {
            Debug.Log("Se ha terminado el experimento! Resultados:");
            Debug.Log("Cada persona ha tenido esta cantidad de conversaciones: " + string.Join(", ",n_conv));
            esperar = true;
        }
    }

    void AjustarCamara(int columnas, int filas, float dx, float dy) {
        if (camara == null) camara = Camera.main;

        float wT = (columnas - 1) * dx;
        float hT = (filas - 1) * dy;

        camara.transform.position = new Vector3(0, 1, -10);

        float tX = wT / (2f * camara.aspect);
        float tY = hT / 2f;

        camara.orthographicSize = Mathf.Max(5, Mathf.Max(tX, tY) + 5);
    }



    int[] Permutacion(int n) {
        int[] p = new int[n];

        for (int i = 0; i < n; i++) p[i] = i;

        for(int i = 0; i < n; i++) {
            int temp = p[i];
            int r = Random.Range(i,n);
            p[i] = p[r];
            p[r] = temp;
        }
        return p;
    }

    float Baricentro(int n, float[] p, float[] o) {
        float num = 0;
        float den = 0;
        for (int i = 0; i < n; i++) {
            num += o[i] * ( p[i] + Mathf.Abs(o[i]));
            den += p[i] + Mathf.Abs(o[i]);
        }
        return num/den;
    }

    public void Amigo(int i, int j) {
        if(relaciones[i].Add(j)) {
            relaciones[j].Add(i);
        }
    }

    public void Desamigo(int i, int j) {
        if(relaciones[i].Remove(j)) {
            relaciones[j].Remove(i);
        }
    }

    int IndiceMaximo(float[] lista) {
        int indiceMax = 0;
        float valorMax = lista[0];

        for (int i = 1; i < lista.Length; i++) {
            if (lista[i] > valorMax) {
                valorMax = lista[i];
                indiceMax = i;
            }
        }

        return indiceMax;
    }

    void ActualizarTextoVotos(int[] contador) {
        string texto = "Votacions:\n";
        for (int i = 0; i < opciones; i++) {
            texto += $"Opció {i+1}: {System.Math.Truncate(((float)contador[i])/p_num * 100000) / 1000}%\n";
        }
        textvot.text = texto; // Actualizar el texto en pantalla
    }

    void Conversación(int conv_num, int[] conv, float[] o_temp, float[] p_temp) {
        for (int i = 0; i < opciones; i++) {

            float baricentro = Baricentro(conv_num, p_temp, o_temp);
            
            //Debug.Log("Tenemos el baricentro de la opinión " + i + " que es " + baricentro);
            
            for (int j = 0; j < conv_num; j++) {
                //Debug.Log("Persona " + conv[j] + ", opinion " + i + ": " + personas[conv[j]].opiniones[i] + " --> " + (personas[conv[j]].opiniones[i] + (baricentro - personas[conv[j]].opiniones[i]) * personas[conv[j]].cabezoneria * 0.3f));
                personas[conv[j]].ModificarOpinion(i,personas[conv[j]].opiniones[i] + (baricentro - personas[conv[j]].opiniones[i]) * personas[conv[j]].cabezoneria * sens_c);
            }
        }
    }
    
    void Publicación(int publ_num, int[] publ, int emisor) {
        for (int i = 0; i < opciones; i++) {

            float[] baricentro;
            List<float> o,p;
            baricentro = new float[publ_num];
            for (int j = 0; j < publ_num; j++) {
                o = new List<float> {personas[emisor].opiniones[i], personas[publ[j]].opiniones[i]};
                p = new List<float> {personas[emisor].prestigio, personas[publ[j]].prestigio};
                //Debug.Log($"Las opiniones son {string.Join(", ", o)} y los prestigios son {string.Join(", ", p)}");
                baricentro[j] = Baricentro(2,o.ToArray(),p.ToArray());
            }
            //Debug.Log($"La persona {emisor} va a hacer una publicación que van a ver {string.Join(", ", publ)}");
            //Debug.Log("Tenemos el baricentro de la opinión " + i + " que es " + string.Join(", ", baricentro));
            
            for (int j = 0; j < publ_num; j++) {
                //Debug.Log("Persona " + publ[j] + ", opinion " + i + ": " + personas[publ[j]].opiniones[i] + " --> " + (personas[publ[j]].opiniones[i] + (baricentro[j] - personas[publ[j]].opiniones[i]) * personas[publ[j]].cabezoneria * 0.3f));
                personas[publ[j]].ModificarOpinion(i,personas[publ[j]].opiniones[i] + (baricentro[j] - personas[publ[j]].opiniones[i]) * personas[publ[j]].cabezoneria * sens_p);
            }
        }
    }

    int[] Elegir_conversación (int conv_num) {

        int[] conv, or;
        conv = new int[conv_num];

        for (int i = 0; i < conv_num; i++) conv[i] = -1;

        conv[0] = Random.Range(0, p_num);
        
        for (int i = 1; i < conv_num; i++) {
            if(Random.Range(0,10)==0) {
                //Debug.Log("Persona aleatoria (1)");
                int num = Random.Range(0,p_num);
                while(conv.Contains(num)) num = Random.Range(0,p_num);
                conv[i]=num;
            } else {
                //Debug.Log("Persona conocida (2)");
                List<int> total = new List<int>();
                for (int j = 0; j < i; j++) total.AddRange(relaciones[conv[j]]);
                if(total.Count > 0) {
                    or = Permutacion(total.Count);
                    //Debug.Log("La permutación es: " + string.Join(",",or));
                    for(int j = 0; j < total.Count; j++) {
                        if(!conv.Contains(total[or[j]])) {
                            //Debug.Log("Vamos a meter el vecino " + total[or[j]]);
                            conv[i] = total[or[j]];
                            break;
                        } else {
                            if (j == total.Count - 1) {
                                //Debug.Log("Ultimo amigo. Elegimos un random");
                                int num = Random.Range(0,p_num);
                                while(conv.Contains(num)) num = Random.Range(0,p_num);
                                conv[i]=num;
                            }
                        }
                    }
                } else {
                    //Debug.Log("Esta persona no tiene amigos");
                    int num = Random.Range(0,p_num);
                    while(conv.Contains(num)) num = Random.Range(0,p_num);
                    conv[i]=num;
                }
                //Debug.Log("Tenemos que los amigos de " + string.Join(", ", conv) + " son " + string.Join(", ", total));
            }
        }

        return conv;
    }

    int[] Elegir_publicación (int emisor) {
        int publ_mg = 0;
        int publ_num = redes_sociales[emisor].Count;
        for (int i = 0; i < publ_num - 1; i++) {
            if (Random.Range(0,2) == 0) break;
            publ_mg++;
        }
        publ_num += (Random.Range(0,2) == 0 ? -1 : 1) * publ_mg;
        Debug.Log($"La persona {emisor} va a hacer una publicación con un alcance de {publ_num} personas");
        int[] publ = new int[publ_num];
        int[] p = Permutacion(redes_sociales[emisor].Count);
        int j = 0;
        for (int i = 0; i < publ_num; i++) {
            if (Random.Range(0,10) == 0 || redes_sociales[emisor].Count <= j) {
                int t = Random.Range(0,p_num);
                while (publ.Contains(t)) t = Random.Range(0,p_num);
                publ[i] = t;
            } else {
                publ[i] = redes_sociales[emisor][p[j]];
                j++;
            }
        }

        return publ;
    }
    


    public IEnumerator Iteración () {

        int conv_num = 1;
        int[] conv;
        float[] o_temp, p_temp;
        bool seguir = true;

        while(seguir) {
            if (Random.Range(0,2) == 1 || conv_num == p_num-1) seguir = false;
            conv_num++;
        }

        if(it%100 == 0) Debug.Log("Iteración número " + it);

        //HACEMOS UNA CONVERSACIÓN

        conv = Elegir_conversación(conv_num);
        
        for (int i = 0; i < conv_num; i++) n_conv[conv[i]]++;

        o_temp = new float[conv_num];
        p_temp = new float[conv_num];

        for (int i = 0; i < opciones; i++) {
            for (int j = 0; j < conv_num; j++) {
                o_temp[j] = personas[conv[j]].opiniones[i];
                p_temp[j] = personas[conv[j]].prestigio;
                //Debug.Log("Persona " + j + ", opinion " + i + ": " + personas[j].opiniones[i] + ", prestigio: " + personas[j].prestigio + ", cabezoneria: " + personas[j].cabezoneria);
            }
        }

        
        for (int i = 0; i < conv_num; i++) {
            personas[conv[i]].torso.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
            personas[conv[i]].cabeza.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
        }
        

        Conversación(conv_num, conv, o_temp, p_temp);

        //ALGUIEN HACE UNA PUBLICACIÓN
        /*
        int[] publ;
        int emisor = Random.Range(0,p_num);
        while(redes_sociales[emisor].Count == 0) emisor = Random.Range(0,p_num);

        publ = Elegir_publicación(emisor);

        Publicación(publ.Length, publ, emisor);

        */

        yield return new WaitForSeconds(t);

        contador = new int[opciones];
        
        for (int i = 0; i < p_num; i++) {
            personas[i].torso.gameObject.GetComponent<Renderer>().material.color = colores[IndiceMaximo(personas[i].opiniones)];
            personas[i].cabeza.gameObject.GetComponent<Renderer>().material.color = colores[IndiceMaximo(personas[i].opiniones)];
            contador[IndiceMaximo(personas[i].opiniones)]++;
        }

        ActualizarTextoVotos(contador);
        
        it++;
        esperar = false;
    }
}

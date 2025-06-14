using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Modelo_Rumania : MonoBehaviour
{
    [Header("Condiciones iniciales")]

    public int opciones;
    public int p_num;
    public int iteraciones;
    public float t = 0;

    [Header("Ajustes del grafo")]

    public int grado_inicial;
    public float sens_amigo;
    public float sens_desamigo;
    public float sens_seguidor;
    public float sens_deseguidor;

    [Header("Ajustes de la conversación")]

    public float sens_c;
    public float sens_p;
    public float exp_conv;
    public float exp_publ;

    [Header("Cámaras")]
    public Camera camPersonas;
    public Camera camGrafico1;
    public Camera camGrafico2;

    [Header("Prefabs y asignación de objetos")]  

    public GameObject personaPrefab;
    private List<Persona> personas = new List<Persona>();
    private float dx = 2f;
    private float dy = 3f;
    bool esperar = false;
    public Dictionary<int, HashSet<int>> relaciones;
    public Dictionary<int, List<int>> redes_sociales;
    private int it = 0;
    private int muy_bien = 0;
    private int bien = 0;
    private int mal = 0;
    private int p_muy_bien = 0;
    private int p_bien = 0;
    private int p_mal = 0;
    int[] contador, n_conv;
    float[] resultados_i, resultados_f;
    public List<Color> colores;
    public TMP_Text textvot;
    public GameObject grafico, grafico2;
    Grafico graph, graph2;
    public int particion;
    public int precision;
    private int[] cambios;
    private int[] prev_ind;
    public float epsilon;
    private float[] distribución;
    private int[] conversaciones;
    public int grupo;
    public int g_seguidores;

    void Start()
    {
        n_conv = new int[p_num];
        conversaciones = new int[p_num-1];

        graph = grafico.GetComponent<Grafico>();
        graph2 = grafico2.GetComponent<Grafico>();

        resultados_i = new float[opciones];
        resultados_f = new float[opciones];
        cambios = new int[p_num];
        prev_ind = new int[p_num];

        colores = new List<Color>();

        for (int i = 0; i < opciones; i++) {
            float hue = i / (float)opciones; // Espaciamos el Hue equitativamente
            Color color = Color.HSVToRGB(hue, 0.8f, 1.0f); // Alta saturación y brillo
            colores.Add(color);
        }
        

        //graph.colores = colores;


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
            for (int j = 0; j < p_num; j++) {
                if (Random.Range(0,2) == 0 && (i > grupo || j > g_seguidores)) break;
                else redes_sociales[i].Add(p[j]);
            }
        }

        for (int i = 0; i < p_num; i++) Debug.Log($"La persona {i} tiene de seguidores {string.Join(", ", redes_sociales[i])}");

        float total = 0;
        distribución = new float [p_num+1];

        for (int i = 0; i < p_num; i++){
            distribución[i] = total;
            total += Mathf.Exp(-(i+2)/10)/(float)Mathf.Pow(i+2,2.5f);
        }
        distribución[p_num]=total;

        Debug.Log($"La distribución sin dividir es ( {string.Join(" ; ", distribución)} ) y se tiene que dividir por {total}");

        for (int i = 0; i <= p_num; i++) {
            distribución[i] /= total;
        }

        Debug.Log($"La distribución es ( {string.Join(" ; ", distribución)} )");

        


        
        /*
        for (int i = 0; i < p_num; i++) {
            personas[i].torso.gameObject.GetComponent<Renderer>().enabled = false;
            personas[i].cabeza.gameObject.GetComponent<Renderer>().enabled = false;
            personas[i].ojoI.gameObject.GetComponent<Renderer>().enabled = false;
            personas[i].ojoD.gameObject.GetComponent<Renderer>().enabled = false;
        }
        */

        //camPersonas.rect = new Rect(0f, 0f, 2f/3f, 1f);
        //camPersonas.depth = 0;

        camGrafico1.rect = new Rect(0f, 0f, 0.5f, 1f);
        camGrafico1.depth = 1;

        camGrafico2.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        camGrafico2.depth = 1;
        
    }

    void Update()
    {

        if (it == 0) {
            for (int i = 0; i < p_num; i++) {
                if (i < grupo)
                {
                    personas[i].ModificarCabezoneria((float)Random.Range(0.8f, 0.99f));
                    personas[i].ModificarPrestigio((float)Random.Range(0.8f, 0.99f));
                }
                else
                {
                    personas[i].ModificarCabezoneria((float)Random.Range(0.01f, 0.3f));
                }
                for (int opcion = 0; opcion < opciones; opcion++) {
                    if (i < grupo)
                    {
                        personas[i].ModificarOpinion(opcion, (opcion == opciones - 1 ? Random.Range(-0.50f, 0.99f) : Random.Range(-0.99f, 0.5f)));
                    }
                    else
                    {
                        if (i - grupo < 172)
                        {
                            personas[i].ModificarOpinion(opcion, opcion == 0 ? Random.Range(0f, 0.99f) : Random.Range(-0.99f, 0.25f));
                        }
                        else if (i - grupo - 172 < 234)
                        {
                            personas[i].ModificarOpinion(opcion, opcion == 1 ? Random.Range(0f, 0.99f) : Random.Range(-0.99f, 0.25f));
                        }
                        else if (i - grupo - 406 < 198)
                        {
                            personas[i].ModificarOpinion(opcion, opcion == 2 ? Random.Range(0f, 0.99f) : Random.Range(-0.99f, 0.25f));
                        }
                        else if (i - grupo - 604 < 62)
                        {
                            personas[i].ModificarOpinion(opcion, opcion == 3 ? Random.Range(0f, 0.99f) : Random.Range(-0.99f, 0.25f));
                        }
                        else if (i - grupo - 666 < 171)
                        {
                            personas[i].ModificarOpinion(opcion, opcion == 4 ? Random.Range(0f, 0.99f) : Random.Range(-0.99f, 0.25f));
                        }
                        else if (i - grupo - 837 < 38)
                        {
                            personas[i].ModificarOpinion(opcion, opcion == 5 ? Random.Range(0f, 0.99f) : Random.Range(-0.99f, 0.25f));
                        }
                        else
                        {
                            personas[i].ModificarOpinion(opcion, Random.Range(-0.99f, 0f));
                        }
                        if (opcion == opciones-1) personas[i].ModificarOpinion(opcion, Random.Range(-0.99f,0f));

                    }
                }
            }
        }

        if (!esperar && it < iteraciones) {
            StartCoroutine(Iteración());
            esperar = true;
        }
        if (!esperar && it == iteraciones) {
            for (int i = 0; i < opciones; i++) {
                resultados_f[i] = (float)System.Math.Truncate(((float)contador[i])/p_num * 100000) / 1000;
            }
            
            for (int i = 0; i < p_num; i++) Debug.Log($"Tenemos que los amigos de {i} son {string.Join(", ", relaciones[i])}");
            for (int i = 0; i < p_num; i++) Debug.Log($"La persona {i} tiene de seguidores {string.Join(", ", redes_sociales[i])}");

            Debug.Log("Se ha terminado el experimento! Resultados:");
            Debug.Log("Cada persona ha tenido esta cantidad de conversaciones: " + string.Join(", ",n_conv));
            for (int i = 0; i < opciones; i++) {
                Debug.Log($"La opción {i} ha pasado de {resultados_i[i]} -> {resultados_f[i]}");
            } 
            Debug.Log($"Si particionamos cada opción en {particion} trozos, queda...");
            
            
            Debug.Log($"Han salido muy bien {muy_bien} conversaciones, han salido bien {bien} conversaciones y han salido mal {mal} conversaciones");
            Debug.Log($"Han salido muy bien {p_muy_bien} publicaciones, han salido bien {p_bien} publicaciones y han salido mal {p_mal} publicaciones");
            
            System.Array.Sort(cambios);

            int[] cambios_num = new int[cambios[p_num-1]+1];

            Debug.Log("La persona que más cambios de opinión ha tenido es de " + cambios[p_num-1]);

            for(int i = 0; i < p_num; i++) cambios_num[cambios[i]]++;

            Debug.Log($"El número de cambios de opinión ha sido ({string.Join(", ",cambios_num)})");

            Debug.Log($"La distribución del número de personas por conversación ha sido ({string.Join(", ",conversaciones)})");

            esperar = true;
        }

        if (Input.GetKeyDown("r")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

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

    float Expn (float a, int n) {
        float b = 1;
        for (int i = 0; i < n; i++) b*=a;
        return b;
    }

    void ActualizarTextoVotos(int[] contador) {
        string texto = "Votacions:\n";
        float[] opm = new float[opciones];
        for (int i = 0; i < opciones; i++) {
            for (int j = 0; j < p_num; j++) opm[i] += personas[j].opiniones[i];
            opm[i] /= p_num;
        }
        for (int i = 0; i < opciones; i++) {
            texto += $"Opció {i+1}: {System.Math.Truncate(((float)contador[i])/p_num * 1000) / 10}% \n OPM: {System.Math.Truncate(opm[i] * 1000) / 1000}\n\n";
        }
        textvot.text = texto; // Actualizar el texto en pantalla
    }

    void Conversación(int conv_num, int[] conv) {
        for (int i = 0; i < p_num; i++) prev_ind[i] = IndiceMaximo(personas[i].opiniones);
        for (int opcion = 0; opcion < opciones - 1; opcion++) {
            if (Random.Range(0,2) == 0) {
                float[] o_temp = new float[conv_num];
                float[] p_temp = new float[conv_num];

                for (int j = 0; j < conv_num; j++) {
                    o_temp[j] = personas[conv[j]].opiniones[opcion];
                    p_temp[j] = personas[conv[j]].prestigio;
                }

                float baricentro = Baricentro(conv_num, p_temp, o_temp);

                for (int j = 0; j < conv_num; j++) {
                    float opinionActual = o_temp[j];
                    float distancia = baricentro - opinionActual;
                    float nuevaOpinion;
                    float num = (float)Random.Range(0,1000)/1000;
                    //Debug.Log($"El baricentro está en {baricentro} y la opinión de la persona {conv[j]} es de {opinionActual}");
                    //Debug.Log($"La partición del intervalo de la conversación es [0 ; {Mathf.Exp(-exp_conv*(1/(Expn(personas[conv[j]].cabezoneria,3)))*distancia*distancia)} ; {Mathf.Exp(-exp_conv*(personas[conv[j]].cabezoneria)*distancia*distancia)} ; 1]");
                    if (Mathf.Exp(-exp_conv*(1/Expn(personas[conv[j]].cabezoneria,3))*distancia*distancia) > num) {
                        nuevaOpinion = opinionActual + ((personas[conv[j]].opiniones[opcion] > 0 ? 1 : -1) - opinionActual) * (1 - personas[conv[j]].cabezoneria) * sens_c * (1-Mathf.Exp(-exp_conv*(1-personas[conv[j]].cabezoneria)*distancia*distancia));
                        //Debug.Log("La conversación ha salido super bien!!");
                        muy_bien++;
                    }
                    else if (Mathf.Exp(-exp_conv*(personas[conv[j]].cabezoneria)*distancia*distancia) < num) {
                        nuevaOpinion = opinionActual + ((personas[conv[j]].opiniones[opcion] - baricentro > 0 ? 1 : -1) - opinionActual) * (1 - personas[conv[j]].cabezoneria) * sens_c * (1-Mathf.Exp(-exp_conv*(1-personas[conv[j]].cabezoneria)*distancia*distancia));
                        //Debug.Log("La conversación ha salido mal...");
                        mal++;
                    } else {
                        nuevaOpinion = opinionActual + distancia * (1 - personas[conv[j]].cabezoneria) * sens_c;
                        //Debug.Log("La conversación ha salido bien!");
                        bien++;
                    }
                    //Debug.Log($"Se modifica la opinión {opinionActual} -> {nuevaOpinion}");
                    personas[conv[j]].ModificarOpinion(opcion, nuevaOpinion);
                }
            }
        }
        float sum_op;
        float prob;

        for (int i = 0; i < conv_num; i++) {
            for (int j = i+1; j < conv_num; j++) {
                sum_op = 0;
                for (int opcion = 0; opcion < opciones-1; opcion++) {
                    sum_op += Mathf.Abs(personas[conv[i]].opiniones[opcion] - personas[conv[j]].opiniones[opcion]);
                }

                prob = (1-sum_op/(2*(opciones-1)))*sens_desamigo/(1+relaciones[conv[i]].Count+relaciones[conv[j]].Count);

                if (Mathf.Exp(-prob*prob) > (float)Random.Range(0,1000)/1000) {
                    Desamigo(conv[i],conv[j]);
                    //Debug.Log($"Las personas {conv[i]} y {conv[j]} han dejado de ser amigos! D = {sum_op/(2*opciones)}, a = {(relaciones[conv[i]].Count+relaciones[conv[j]].Count)}");
                    //Debug.Log($"Las opiniones de la persona {conv[i]} y {conv[j]} son las siguientes");
                    //yfor (int op = 0; op < opciones; op++) Debug.Log($"Opcion {op}:{personas[conv[i]].opiniones[op]} y {personas[conv[j]].opiniones[op]}");
                }

                prob = (sum_op/(2*(opciones-1)))*sens_amigo*(relaciones[conv[i]].Count+relaciones[conv[j]].Count);

                if (Mathf.Exp(-prob*prob) > (float)Random.Range(0,1000)/1000) {
                    Amigo(conv[i],conv[j]);
                    //Debug.Log($"Las personas {conv[i]} y {conv[j]} Ahora son amigos! D = {sum_op/(2*opciones)}, a = {(relaciones[conv[i]].Count+relaciones[conv[j]].Count)}");
                    //Debug.Log($"Las opiniones de la persona {conv[i]} y {conv[j]} son las siguientes");
                    //for (int op = 0; op < opciones; op++) Debug.Log($"Opcion {op}:{personas[conv[i]].opiniones[op]} y {personas[conv[j]].opiniones[op]}");
                }

                

            }
        }

        for (int i = 0; i < p_num; i++) if(prev_ind[i] != IndiceMaximo(personas[i].opiniones)) cambios[i]++;
    }
    
    void Publicación(int publ_num, int[] publ, int emisor) {

        for (int i = 0; i < p_num; i++) prev_ind[i] = IndiceMaximo(personas[i].opiniones);

        for (int opcion = 0; opcion < opciones; opcion++) {

            float baricentro;
            List<float> o_temp,p_temp;
            for (int j = 0; j < publ_num; j++) {
                o_temp = new List<float> {personas[emisor].opiniones[opcion], personas[publ[j]].opiniones[opcion]};
                p_temp = new List<float> {personas[emisor].prestigio, personas[publ[j]].prestigio};
                //Debug.Log($"Las opiniones son {string.Join(", ", o)} y los prestigios son {string.Join(", ", p)}");
                baricentro = Baricentro(2,p_temp.ToArray(),o_temp.ToArray());
            
                float opinionActual = personas[publ[j]].opiniones[opcion];
                float distancia = baricentro - opinionActual;
                float nuevaOpinion;
                float num = (float)Random.Range(0,1000)/1000;
                //Debug.Log($"El baricentro está en {baricentro} y la opinión de la persona {publ[j]} es de {opinionActual}");
                //Debug.Log($"La partición del intervalo de la publicación es [0 ; {Mathf.Exp(-exp_publ*(1/Mathf.Pow(personas[publ[j]].cabezoneria,5))*distancia*distancia)} ; {Mathf.Exp(-exp_publ*(personas[publ[j]].cabezoneria)*distancia*distancia)} ; 1]");
                if (Mathf.Exp(-exp_publ*(1/Expn(personas[publ[j]].cabezoneria,3))*distancia*distancia) > num) {
                    nuevaOpinion = opinionActual + ((personas[publ[j]].opiniones[opcion] > 0 ? 1 : -1) - opinionActual) * (1 - personas[publ[j]].cabezoneria) * sens_p * (1-Mathf.Exp(-exp_publ*(1-personas[publ[j]].cabezoneria)*distancia*distancia));
                    //Debug.Log("La publicación ha salido super bien!!");
                    p_muy_bien++;
                }
                else if (Mathf.Exp(-exp_publ*(personas[publ[j]].cabezoneria)*distancia*distancia) < num) {
                    nuevaOpinion = opinionActual + ((personas[publ[j]].opiniones[opcion] - baricentro > 0 ? 1 : -1) - opinionActual) * (1 - personas[publ[j]].cabezoneria) * sens_p * (1-Mathf.Exp(-exp_publ*(1-personas[publ[j]].cabezoneria)*distancia*distancia));
                    //Debug.Log("La publicación ha salido mal...");
                    p_mal++;
                } else {
                    nuevaOpinion = opinionActual + distancia * (1 - personas[publ[j]].cabezoneria) * sens_p;
                    //Debug.Log("La publicación ha salido bien!");
                    p_bien++;
                }
                personas[publ[j]].ModificarOpinion(opcion, nuevaOpinion);
            }
        }
        float sum_op;
        float prob;

        for (int i = 0; i < publ_num; i++) {
            sum_op = 0;
            for (int opcion = 0; opcion < opciones-1; opcion++) {
                sum_op += Mathf.Abs(personas[publ[i]].opiniones[opcion] - personas[emisor].opiniones[opcion]);
            }

            prob = (1-sum_op/(2*(opciones-1)))*sens_deseguidor;

            if (Mathf.Exp(-prob*prob) > (float)Random.Range(0,1000)/1000 && !redes_sociales[emisor].Contains(i)) {
                redes_sociales[emisor].Add(i);
                //Debug.Log($"La persona {publ[i]} ha dejado de seguir a {emisor}! D = {sum_op/(2*opciones)}");
                //Debug.Log($"Las opiniones de la persona {publ[i]} y {emisor} son las siguientes");
                //for (int op = 0; op < opciones; op++) Debug.Log($"Opcion {op}:{personas[publ[i]].opiniones[op]} y {personas[emisor].opiniones[op]}");
            }

            prob = sum_op/(2*(opciones-1))*sens_seguidor;

            if (Mathf.Exp(-prob*prob) > (float)Random.Range(0,1000)/1000 && redes_sociales[emisor].Contains(i)) {
                redes_sociales[emisor].Remove(i);
                //Debug.Log($"La persona {publ[i]} ahora sigue a {emisor}! D = {sum_op/(2*opciones)}");
                //Debug.Log($"Las opiniones de la persona {publ[i]} y {emisor} son las siguientes");
                //for (int op = 0; op < opciones; op++) Debug.Log($"Opcion {op}:{personas[publ[i]].opiniones[op]} y {personas[emisor].opiniones[op]}");
            }
        }

        for (int i = 0; i < p_num; i++) if(prev_ind[i] != IndiceMaximo(personas[i].opiniones)) cambios[i]++;

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
        //Debug.Log($"La persona {emisor} va a hacer una publicación con un alcance de {publ_num} personas");
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

        float temp = Random.value;

        for (int i = 0; temp > distribución[i]; i++) {
            conv_num++;
        }

        //Debug.Log($"Se va a hacer una conversación de {conv_num} personas");

        conversaciones[conv_num-2]++;

        if(it%100 == 0) Debug.Log("Iteración número " + it);

        //HACEMOS UNA CONVERSACIÓN

        conv = Elegir_conversación(conv_num);
        
        for (int i = 0; i < conv_num; i++) n_conv[conv[i]]++;

        
        for (int i = 0; i < conv_num; i++) {
            personas[conv[i]].torso.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
            personas[conv[i]].cabeza.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
        }
        

        if (it%5 == 0)Conversación(conv_num, conv);

        //ALGUIEN HACE UNA PUBLICACIÓN
        
        int[] publ;
        int emisor = (Random.Range(0,3) == 0 ? Random.Range(0,p_num) : Random.Range(0,grupo));
        if (emisor < 10) {
            Debug.Log($"La persona {emisor} ha hecho una publicación");
        }
        while(redes_sociales[emisor].Count == 0) emisor = Random.Range(0,p_num);

        publ = Elegir_publicación(emisor);

        Publicación(publ.Length, publ, emisor);

        

        yield return new WaitForSeconds(t);

        contador = new int[opciones];
        
        for (int i = 0; i < p_num; i++) {
            personas[i].torso.gameObject.GetComponent<Renderer>().material.color = colores[IndiceMaximo(personas[i].opiniones)];
            personas[i].cabeza.gameObject.GetComponent<Renderer>().material.color = colores[IndiceMaximo(personas[i].opiniones)];
            contador[IndiceMaximo(personas[i].opiniones)]++;
        }

        if (it == 0) for (int i = 0; i < opciones; i++) resultados_i[i] = (float)System.Math.Truncate(((float)contador[i])/p_num * 100000) / 1000;

        ActualizarTextoVotos(contador);

        for(int i = 0; i < opciones; i++) {
            graph.AddPoint(i, new Vector3(it / (float)iteraciones * (camGrafico1.orthographicSize * 2 * camGrafico1.aspect) - camGrafico1.orthographicSize * camGrafico1.aspect+314.2f, (((float)contador[i]) / p_num * 100) * (camGrafico1.orthographicSize * 2) / 100 - camGrafico1.orthographicSize+138.9684f, 0));
        }

        if (it%10 == 0) {
            for (int i = 0; i < opciones; i++) graph2.ResetLine(i);
            for (int i = 0; i < opciones; i++) {
                // 1) Capturo y ordeno las opiniones de la opción i
                float[] opiniones_ordenadas = new float[p_num];
                for (int k = 0; k < p_num; k++)
                    opiniones_ordenadas[k] = personas[k].opiniones[i];
                System.Array.Sort(opiniones_ordenadas);

                // 2) Recorro los pasos de la ecdf
                for (int j = 0; j <= precision; j++) {
                    // umbral x_j desde -1 hasta +1
                    float xj = -1f + 2f * j / precision;

                    // 3) cuento cuántos valores <= xj
                    int cuenta = 0;
                    // como está ordenado, puedo hacer break al llegar al primero > xj
                    for (int k = 0; k < p_num; k++) {
                        if (opiniones_ordenadas[k] <= xj + epsilon && opiniones_ordenadas[k] >= xj - epsilon)
                            cuenta++;;
                    }

                    //Debug.Log($"A l'interval ( {xj-epsilon} ; {xj+epsilon} ) tenim que la densitat és de {(float)cuenta / p_num}");

                    // 4) calculo la probabilidad acumulada
                    float F = (float)cuenta / p_num; 

                    // 5) dibujo el punto en graph2
                    float xpos = (float)j / precision 
                                 * (camGrafico2.orthographicSize * 2 * camGrafico2.aspect)
                                 - camGrafico2.orthographicSize * camGrafico2.aspect
                                 + 414.2f;

                    float ypos = F * (camGrafico2.orthographicSize * 2) 
                                 - camGrafico2.orthographicSize
                                 + 138.9684f;

                    graph2.AddPoint(i, new Vector3(xpos, ypos, 0));
                }
            }
        }
        
        it++;
        esperar = false;
    }
}
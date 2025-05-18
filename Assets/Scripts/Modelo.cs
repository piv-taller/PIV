using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Modelo : MonoBehaviour
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

    private float[] opinion1 = {-0.044f, 0.259f, 0.788f, -0.758f, -0.992f, -0.551f, 0.226f, 0.073f, 0.719f, 0.916f, 0.977f, -0.343f, -0.497f, 0.58f, -0.646f, 0.539f, -0.885f, 0.486f, 0.977f, -0.122f, 0.626f, -0.113f, -0.412f, -0.742f, -0.427f, 0.187f, -0.387f, -0.067f, 0.931f, 0.333f, 0.13f, 0.008f, 0.884f, 0.243f, 0.613f, -0.747f, 0.024f, 0.671f, 0.158f, -0.801f, -0.45f, 0.035f, -0.211f, 0.236f, -0.605f, 0.016f, -0.428f, 0.914f, -0.151f, 0.617f, -0.059f, 0.83f, 0.723f, 0.199f, 0.801f, 0.71f, -0.555f, -0.108f, -0.742f, 0.259f, 0.292f, 0.182f, 0.605f, 0.656f, 0.257f, 0.086f, 0.145f, 0.716f, 0.123f, -0.681f, 0.848f, 0.939f, -0.868f, -0.619f, -0.8f, -0.134f, -0.096f, -0.35f, 0.307f, -0.15f, 0.096f, -0.837f, -0.091f, 0.774f, 0.109f, -0.502f, -0.163f, -0.375f, 0.025f, -0.821f, 0.535f, 0.494f, -0.492f, 0.061f, -0.452f, -0.333f, -0.757f, -0.556f, -0.834f, -0.252f, 0.569f, 0.547f, 0.52f, 0.24f, -0.574f, -0.466f, 0.125f, -0.771f, -0.383f, 0.059f, -0.173f, -0.299f, -0.9f, -0.062f, 0.685f, -0.591f, 0.067f, -0.069f, -0.366f, -0.046f, -0.231f, -0.897f, -0.707f, 0.475f, 0.795f, -0.586f, -0.895f, -0.498f, -0.721f, -0.495f, -0.783f, -0.185f, -0.998f, 0.136f, -0.431f, -0.3f, 0.715f, -0.093f, -0.889f, 0.572f, -0.272f, 0.228f, -0.273f, -0.612f, -0.125f, 0.83f, -0.984f, 0.375f, 0.128f, -0.83f, 0.895f, -0.593f, 0.661f, 0.104f, -0.696f, 0.22f, -0.82f, 0.583f, 0.572f, -0.068f, -0.928f, -0.969f, -0.728f, 0.902f, 0.368f, 0.275f, -0.374f, -0.545f, 0.163f, -0.096f, 0.029f, -0.796f, -0.764f, -0.843f, 0.741f, 0.525f, 0.332f, 0.527f, 0.605f, -0.175f, 0.705f, 0.884f, 0.272f, 0.005f, -0.975f, 0.951f, -0.527f, 0.643f, 0.767f, -0.832f, -0.773f, -0.24f, -0.872f, -0.825f, 0.608f, 0.57f, -0.35f, -0.909f, 0.069f, 0.566f, 0.736f, 0.398f, 0.697f, -0.303f, 0.628f, -0.021f, -0.126f, 0.674f, -0.227f, -0.22f, -0.71f, -0.347f, -0.183f, -0.253f, 0.384f, -0.48f, -0.536f, -0.233f, 0.309f, -0.184f, 0.646f, -0.494f, -0.666f, -0.628f, 0.996f, -0.025f, -0.246f, -0.422f, 0.046f, 0.715f, 0.925f, 0.6f, -0.493f, 0.931f, -0.312f, -0.442f, 0.96f, -0.904f, 0.355f, -0.101f, -0.65f, 0.161f, 0.918f, -0.689f, 0.003f, -0.617f, 0.958f, -0.587f, -0.225f, 0.548f, -0.001f, 0.945f, -0.687f, 0.409f, -0.096f, 0.854f, 0.152f, -0.607f, 0.668f, -0.612f, -0.401f, 0.749f, 0.809f, 0.068f, -0.564f, 0.722f, 0.934f, 0.407f, -0.889f, 0.521f, 0.878f, 0.871f, 0.46f, 0.352f, 0.679f, -0.558f, -0.569f, 0.434f, -0.992f, -0.652f, -0.378f, -0.45f, -0.221f, 0.65f, 0.843f, 0.664f, 0.293f, 0.863f, -0.049f, 0.835f, -0.774f, 0.573f, -0.663f, -0.014f, 0.912f, 0.344f, 0.384f, -0.952f, 0.103f, 0.546f};
    private float[] opinion2 = {0.583f, -0.81f, 0.133f, -0.139f, -0.622f, 0.877f, -0.453f, -0.801f, 0.66f, -0.736f, -0.362f, -0.24f, 0.186f, 0.135f, 0.055f, -0.472f, -0.462f, -0.373f, 0.842f, 0.124f, -0.207f, 0.78f, 0.984f, 0.71f, -0.847f, 0.547f, -0.492f, -0.213f, -0.88f, 0.661f, 0.348f, 0.992f, 0.742f, 0.57f, -0.711f, 0.591f, -0.649f, -0.089f, 0.439f, 0.995f, 0.987f, 0.099f, -0.252f, 0.056f, -0.833f, 0.699f, 0.154f, -0.41f, -0.354f, -0.74f, -0.892f, 0.256f, 0.255f, -0.095f, 0.674f, -0.343f, 0.541f, 0.137f, 0.504f, 0.1428597f, 0.9893339f, -0.640156f, 0.59f, 0.594f, -0.144f, 0.318f, 0.01f, -0.887f, -0.52f, -0.725f, 0.545f, 0.906f, 0.351f, 0.309f, -0.532f, 0.488f, -0.119f, -0.531f, -0.446f, -0.996f, 0.575f, 0.668f, 0.048f, -0.265f, 0.05f, 0.207f, 0.81f, 0.652f, -0.953f, 0.874f, 0.949f, 0.338f, 0.986f, 0.93f, 0.163f, 0.6f, -0.22f, -0.844f, -0.985f, 0.336f, 0.132f, -0.852f, 0.661f, 0.891f, 0.493f, 0.544f, 0.174f, -0.913f, -0.65f, -0.482f, -0.728f, -0.454f, -0.792f, 0.015f, -0.714f, 0.231f, -0.405f, 0.38f, -0.494f, 0.846f, -0.852f, -0.941f, -0.014f, -0.939f, 0.741f, 0.874f, 0.146f, 0.705f, 0.491f, 0.533f, 0.839f, 0.655f, 0.304f, 0.005f, 0.044f, -0.459f, 0.829f, 0.115f, 0.008f, 0.041f, -0.822f, 0.073f, 0.219f, -0.79f, 0.861f, 0.056f, 0.176f, -0.632f, 0.112f, -0.198f, 0.767f, -0.756f, -0.066f, 0.211f, 0.183f, -0.007f, -0.87f, -0.584f, 0.731f, 0.303f, 0.203f, -0.625f, 0.508f, 0.058f, -0.297f, -0.797f, -0.565f, -0.535f, 0.526f, 0.067f, -0.658f, -0.718f, 0.683f, -0.825f, 0.883f, 0.021f, -0.424f, 0.265f, 0.36f, 0.578f, -0.014f, 0.671f, -0.884f, -0.724f, -0.473f, 0.15f, -0.357f, -0.351f, 0.7f, 0.445f, -0.525f, 0.706f, 0.391f, 0.068f, -0.183f, -0.914f, 0.834f, 0.478f, 0.755f, -0.941f, 0.461f, -0.876f, -0.01f, -0.358f, -0.91f, -0.44f, 0.712f, 0.989f, 0.217f, 0.483f, -0.243f, 0.377f, 0.882f, -0.917f, -0.325f, 0.865f, 0.117f, 0.225f, -0.46f, 0.421f, -0.905f, 0.166f, 0.5857351f, 0.199f, 0.462f, -0.913f, 0.831f, 0.662f, -0.539f, 0.148f, 0.725f, -0.834f, -0.617f, 0.827f, -0.452f, -0.507f, 0.009f, 0.604f, 0.436f, -0.834f, -0.99f, -0.381f, 0.693f, 0.323f, -0.1f, 0.763f, 0.832f, -0.962f, -0.801f, 0.338f, 0.642f, -0.644f, 0.347f, -0.02f, -0.67f, -0.774f, 0.452f, 0.548f, 0.381f, -0.767f, 0.557f, -0.666f, 0.257f, 0.335f, -0.409f, 0.375f, -0.547f, 0.678f, -0.57f, -0.202f, 0.755f, -0.687f, 0.548f, 0.556f, -0.616f, -0.393f, -0.511f, 0.052f, 0.241f, -0.075f, -0.736f, -0.358f, 0.158f, 0.879f, 0.321f, -0.877f, 0.081f, -0.437f, 0.374f, 0.799f, -0.527f, 0.631f, -0.538f, 0.089f, 0.307f, 0.061f, 0.506f, -0.264f, -0.719f, -0.978f};
    private float[] opinion3 = {0.341f, -0.8f, -0.126f, -0.725f, -0.085f, 0.42f, 0.004f, 0.566f, -0.051f, -0.129f, 0.864f, 0.476f, -0.329f, -0.984f, 0.857f, 0.421f, -0.594f, 0.307f, 0.992f, 0.741f, 0.835f, -0.538f, -0.117f, 0.679f, -0.929f, 0.532f, -0.891f, 0.083f, 0.739f, -0.184f, 0.559f, -0.648f, -0.996f, -0.783f, 0.511f, 0.801f, -0.515f, 0.146f, 0.08f, -0.353f, 0.862f, -0.997f, 0.295f, -0.466f, 0.349f, 0.639f, 0.888f, -0.008f, 0.335f, -0.596f, -0.725f, 0.583f, 0.597f, -0.963f, 0.1f, -0.972f, -0.619f, -0.307f, -0.282f, -0.8612387f, -0.7160289f, 0.6901596f, 0.375f, 0.482f, -0.91f, -0.528f, -0.399f, 0.194f, -0.975f, 0.106f, -0.531f, -0.436f, -0.59f, -0.348f, -0.846f, -0.846f, 0.087f, 0.301f, 0.934f, -0.939f, -0.206f, 0.752f, 0.6f, -0.274f, -0.157f, 0.054f, 0.203f, -0.831f, -0.498f, -0.798f, -0.015f, -0.204f, -0.05f, 0.623f, 0.198f, -0.739f, 0.002f, -0.923f, -0.419f, -0.052f, -0.755f, 0.179f, 0.335f, -0.062f, 0.106f, -0.988f, -0.672f, 0.313f, -0.182f, 0.408f, 0.084f, 0.638f, 0.618f, -0.843f, -0.684f, 0.476f, 0.214f, -0.53f, -0.661f, 0.898f, 0.838f, 0.449f, -0.919f, 0.427f, 0.384f, -0.445f, 0.266f, -0.459f, 0.017f, -0.21f, -0.714f, 0.063f, 0.67f, -0.714f, 0.619f, -0.692f, 0.541f, -0.154f, -0.263f, -0.457f, 0.915f, 0.783f, -0.172f, 0.984f, -0.698f, 0.811f, -0.407f, -0.028f, 0.51f, -0.314f, 0.748f, -0.916f, 0.697f, 0.849f, -0.689f, -0.545f, -0.807f, -0.479f, 0.151f, -0.074f, 0.347f, -0.903f, 0.02f, 0.964f, 0.134f, 0.332f, 0.478f, 0.508f, -0.933f, -0.763f, 0.268f, -0.574f, 0.35f, 0.817f, -0.673f, 0.895f, -0.095f, -0.837f, -0.193f, -0.852f, 0.197f, 0.092f, 0.046f, 0.03f, -0.427f, 0.147f, 0.703f, -0.427f, 0.251f, -0.924f, 0.815f, 0.342f, -0.817f, 0.742f, -0.378f, -0.941f, -0.032f, 0.296f, -0.488f, -0.937f, -0.298f, 0.532f, -0.174f, 0.798f, -0.254f, 0.978f, 0.094f, -0.267f, 0.679f, -0.938f, -0.506f, -0.74f, -0.249f, 0.546f, -0.85f, 0.657f, -0.758f, -0.184f, -0.762f, 0.18f, -0.07f, 0.066f, 0.0392957f, -0.976f, 0.02f, -0.118f, 0.138f, 0.853f, -0.549f, 0.353f, 0.017f, -0.256f, 0.966f, 0.6f, -0.505f, 0.508f, 0.882f, -0.383f, -0.14f, 0.337f, -0.828f, 0.573f, -0.724f, -0.956f, 0.464f, -0.727f, -0.111f, -0.984f, -0.933f, 0.234f, 0.425f, -0.522f, 0.664f, -0.665f, 0.579f, -0.353f, -0.193f, -0.108f, 0.593f, 0.899f, 0.476f, 0.937f, -0.593f, 0.962f, 0.449f, -0.815f, 0.2f, -0.896f, -0.941f, 0.268f, 0.741f, 0.367f, 0.479f, -0.12f, -0.368f, 0.08f, -0.326f, -0.416f, 0.593f, 0.264f, 0.734f, 0.6f, 0.508f, 0.773f, 0.567f, -0.015f, 0.52f, 0.437f, 0.984f, 0.903f, 0.86f, -0.346f, 0.107f, -0.638f, 0.916f, 0.722f, -0.145f, 0.211f, 0.102f, 0.367f};
    private float[] opinion4 = {-0.274f, -0.917f, 0.199f, -0.759f, 0.408f, 0.169f, -0.935f, -0.664f, -0.182f, -0.269f, 0.361f, -0.162f, -0.132f, -0.303f, 0.443f, -0.114f, -0.261f, 0.759f, -0.576f, -0.305f, -0.276f, 0.173f, 0.125f, -0.08f, -0.655f, 0.724f, -0.178f, -0.979f, -0.732f, 0.591f, -0.819f, -0.109f, 0.968f, -0.275f, 0.015f, -0.207f, 0.008f, -0.423f, -0.767f, -0.512f, 0.562f, -0.966f, 0.503f, -0.416f, 0.553f, -0.35f, 0.458f, -0.019f, -0.018f, -0.291f, -0.949f, 0.578f, -0.54f, -0.338f, -0.508f, -0.588f, 0.144f, 0.48f, 0.326f, 0.474f, -0.48f, 0.368f, -0.92f, -0.791f, 0.592f, 0.34f, -0.86f, -0.913f, 0.624f, -0.669f, -0.034f, 0.213f, -0.419f, 0.321f, -0.023f, 0.666f, 0.493f, -0.157f, -0.744f, 0.858f, 0.526f, -0.089f, -0.875f, -0.48f, 0.744f, -0.075f, -0.863f, -0.08f, -0.479f, -0.47f, -0.648f, -0.06f, 0.962f, -0.54f, -0.287f, -0.465f, -0.911f, 0.471f, -0.8f, 0.948f, -0.369f, 0.756f, 0.022f, 0.69f, -0.535f, -0.624f, 0.574f, 0.483f, 0f, 0.403f, 0.082f, 0.436f, -0.708f, -0.03f, 0.118f, -0.35f, 0.34f, -0.39f, 0.988f, -0.451f, -0.827f, -0.827f, 0.451f, 0.526f, 0.34f, 0.419f, 0.963f, 0.665f, 0.09f, -0.084f, -0.899f, 0.304f, 0.826f, 0.192f, -0.229f, 0.451f, -0.66f, -0.9f, -0.94f, -0.878f, -0.321f, 0.506f, -0.087f, 0.173f, 0.352f, 0.123f, -0.709f, -0.009f, -0.403f, 0.96f, 0.432f, -0.263f, 0.407f, -0.828f, -0.565f, -0.582f, 0.936f, -0.065f, 0.621f, -0.741f, -0.025f, 0.854f, 0.243f, 0.412f, -0.582f, 0.278f, 0.529f, 0.237f, 0.911f, -0.918f, -0.158f, -0.221f, 0.724f, 0.061f, -0.096f, -0.954f, 0.68f, -0.557f, 0.034f, 0.097f, 0.996f, 0.451f, 0.914f, 0.03f, -0.354f, 0.324f, 0.943f, 0.083f, -0.519f, -0.416f, 0.882f, 0.983f, 0.721f, -0.376f, -0.666f, -0.607f, -0.058f, 0.511f, 0.614f, -0.905f, 0.962f, 0.537f, -0.35f, -0.263f, 0.999f, 0.602f, -0.784f, -0.245f, -0.848f, -0.874f, 0.537f, 0.374f, 0.878f, -0.746f, -0.668f, 0.034f, -0.65f, 0.74f, 0.472f, 0.291f, 0.446f, 0.813f, 0.922f, -0.218f, 0.012f, -0.847f, -0.405f, -0.632f, 0.699f, 0.232f, 0.588f, 0.833f, -0.746f, -0.646f, -0.708f, -0.155f, 0.402f, 0.49f, 0.989f, 0.819f, -0.053f, 0.486f, 0.224f, 0.347f, -0.904f, 0.517f, 0.226f, -0.082f, 0.553f, 0.118f, 0.523f, 0.44f, 0.743f, 0.755f, -0.128f, 0.326f, -0.855f, -0.881f, 0.218f, 0.701f, -0.936f, 0.89f, -0.188f, -0.251f, 0.885f, 0.329f, -0.904f, -0.129f, -0.093f, 0.948f, -0.345f, -0.728f, 0.153f, 0.098f, -0.899f, 0.441f, 0.756f, 0.535f, -0.328f, -0.175f, -0.096f, 0.247f, -0.951f, 0.979f, -0.512f, -0.176f, 0.755f, 0.039f, -0.983f, -0.702f, 0.558f, -0.623f, 0.391f, -0.346f, -0.093f, 0.165f, 0.007f, 0.584f, -0.317f, 0.908f};
    private float[] prestigio = {0.693929f, 0.8693424f, 0.6508691f, 0.860738f, 0.3228809f, 0.6593453f, 0.2383099f, 0.3756673f, 0.2332028f, 0.4443638f, 0.1007725f, 0.4794452f, 0.749691f, 0.5654066f, 0.2775168f, 0.5366061f, 0.5581002f, 0.170838f, 0.5293669f, 0.7076371f, 0.4736292f, 0.7720933f, 0.03870058f, 0.4366903f, 0.9143989f, 0.1718664f, 0.8162119f, 0.3891843f, 0.9321281f, 0.6222418f, 0.7978708f, 0.4423313f, 0.1347033f, 0.8951396f, 0.4609663f, 0.2050487f, 0.9418007f, 0.8662341f, 0.8050983f, 0.7688905f, 0.1584215f, 0.141485f, 0.7306253f, 0.5659927f, 0.1059306f, 0.5147234f, 0.6970618f, 0.9663231f, 0.04881334f, 0.462203f, 0.8481201f, 0.2725989f, 0.0482384f, 0.7659851f, 0.8464949f, 0.2527633f, 0.162266f, 0.9587916f, 0.3996007f, 0.8911961f, 0.5945616f, 0.3674368f, 0.02072334f, 0.001606226f, 0.08731294f, 0.5830746f, 0.2516499f, 0.1833991f, 0.693175f, 0.3000409f, 0.5948336f, 0.9301212f, 0.608834f, 0.8656219f, 0.9893395f, 0.9248183f, 0.6612544f, 0.1219392f, 0.8607892f, 0.502561f, 0.4506992f, 0.2001289f, 0.4213982f, 0.7744625f, 0.4880071f, 0.6399809f, 0.1421169f, 0.8512794f, 0.2055436f, 0.7821401f, 0.1772259f, 0.1472781f, 0.6100008f, 0.660758f, 0.9545568f, 0.178098f, 0.09073091f, 0.3095297f, 0.04874313f, 0.1221652f, 0.2737841f, 0.2991628f, 0.2229632f, 0.8466084f, 0.1526127f, 0.4420612f, 0.4987985f, 0.104037f, 0.8950934f, 0.294754f, 0.3339394f, 0.7884967f, 0.9907203f, 0.8578025f, 0.1428787f, 0.6806486f, 0.2395096f, 0.4666327f, 0.6099963f, 0.4997965f, 0.9033791f, 0.557205f, 0.294355f, 0.9062923f, 0.5010304f, 0.8046783f, 0.7113812f, 0.3036742f, 0.9338398f, 0.8667566f, 0.517069f, 0.2731189f, 0.1573933f, 0.8593003f, 0.2349118f, 0.6367329f, 0.5430822f, 0.8998483f, 0.9689258f, 0.6729549f, 0.1448827f, 0.03481162f, 0.3014223f, 0.4596326f, 0.05681872f, 0.03823268f, 0.8304969f, 0.6284884f, 0.3020357f, 0.1928935f, 0.4026935f, 0.4890253f, 0.4438429f, 0.9217854f, 0.1345239f, 0.9528304f, 0.873932f, 0.9162831f, 0.5899591f, 0.9549704f, 0.02310669f, 0.1070887f, 0.1264949f, 0.1414279f, 0.3655444f, 0.6560112f, 0.2290404f, 0.3897732f, 0.4304199f, 0.3853977f, 0.2949228f, 0.7430681f, 0.2882801f, 0.9222907f, 0.08941352f, 0.3075646f, 0.4622063f, 0.6091718f, 0.2974733f, 0.2702271f, 0.3656308f, 0.6687869f, 0.6127595f, 0.5449613f, 0.6665148f, 0.2172953f, 0.6046341f, 0.9395469f, 0.08160865f, 0.5316175f, 0.1180915f, 0.7151594f, 0.1462319f, 0.9181345f, 0.8783022f, 0.43728f, 0.4938535f, 0.4772275f, 0.3690482f, 0.371235f, 0.1085714f, 0.6606057f, 0.1658806f, 0.6373595f, 0.6507508f, 0.5199378f, 0.7732346f, 0.3050397f, 0.3461357f, 0.9192933f, 0.91651f, 0.9055058f, 0.8321401f, 0.5257302f, 0.802267f, 0.8867569f, 0.9089081f, 0.06166327f, 0.3226939f, 0.08168328f, 0.8114401f, 0.05897975f, 0.09610713f, 0.7511294f, 0.5645294f, 0.7554303f, 0.7560601f, 0.9527053f, 0.1512442f, 0.259916f, 0.8897635f, 0.9858198f, 0.3871434f, 0.4777594f, 0.3504463f, 0.5963338f, 0.1307079f, 0.7684577f, 0.1295311f, 0.3535596f, 0.9873755f, 0.23396f, 0.7402898f, 0.1138253f, 0.1266853f, 0.7889234f, 0.9460346f, 0.6927781f, 0.8510442f, 0.6603787f, 0.2450908f, 0.3141159f, 0.6830288f, 0.6454868f, 0.7233018f, 0.7780361f, 0.6347156f, 0.1411157f, 0.4721915f, 0.7042438f, 0.2771674f, 0.9419932f, 0.9068046f, 0.4651163f, 0.6237157f, 0.02672994f, 0.3897037f, 0.2293419f, 0.8060561f, 0.3258589f, 0.3430597f, 0.009986043f, 0.7530367f, 0.06291163f, 0.3330277f, 0.7590234f, 0.9749383f, 0.3125073f, 0.3905663f, 0.9387645f, 0.5161526f, 0.3257026f, 0.3314579f, 0.6608801f, 0.2345911f, 0.6823913f, 0.902192f, 0.974486f, 0.7342949f, 0.08729219f, 0.6885288f, 0.2080011f, 0.1232957f, 0.1551403f, 0.8620931f, 0.01560915f, 0.07275891f, 0.1107161f, 0.8639895f, 0.2399637f};
    private float[] cabezoneria = {0.955089f, 0.6640795f, 0.8153068f, 0.2332165f, 0.8871709f, 0.6497562f, 0.8490267f, 0.8679937f, 0.9778821f, 0.8570149f, 0.703114f, 0.9255764f, 0.7045416f, 0.5379254f, 0.3418069f, 0.398348f, 0.5486049f, 0.06694257f, 0.6597261f, 0.8451608f, 0.4002618f, 0.8652469f, 0.3114155f, 0.6380911f, 0.7611967f, 0.9987706f, 0.002494216f, 0.9183875f, 0.7643365f, 0.1905224f, 0.06593788f, 0.9393365f, 0.1774166f, 0.6679242f, 0.7635512f, 0.6658553f, 0.8049117f, 0.4941044f, 0.1194308f, 0.5061031f, 0.4398506f, 0.1224477f, 0.8307856f, 0.1280662f, 0.7507205f, 0.4207328f, 0.3755208f, 0.2995946f, 0.4545924f, 0.7341362f, 0.3582471f, 0.7493228f, 0.2436105f, 0.38305f, 0.8876543f, 0.9185396f, 0.5671536f, 0.406867f, 0.5914224f, 0.4613138f, 0.1862969f, 0.1173379f, 0.5792723f, 0.7699451f, 0.9182262f, 0.7839397f, 0.3252583f, 0.2621205f, 0.5516834f, 0.5134568f, 0.8603417f, 0.3775304f, 0.7968004f, 0.6038181f, 0.9720664f, 0.06478751f, 0.0357126f, 0.6465888f, 0.2869646f, 0.2295884f, 0.6989657f, 0.149603f, 0.3159645f, 0.5777156f, 0.5627533f, 0.5922545f, 0.3323213f, 0.792417f, 0.8395087f, 0.6949315f, 0.8805084f, 0.1077361f, 0.09886324f, 0.7967727f, 0.1705755f, 0.2966831f, 0.3899485f, 0.7273782f, 0.4662144f, 0.3397396f, 0.3005342f, 0.7516944f, 0.7995396f, 0.3450502f, 0.1515046f, 0.6507547f, 0.5012539f, 0.799705f, 0.01843774f, 0.07823265f, 0.7561308f, 0.5240424f, 0.1099797f, 0.8045609f, 0.5707951f, 0.6776825f, 0.3724049f, 0.05027449f, 0.1269217f, 0.1639515f, 0.8674943f, 0.3165329f, 0.7260683f, 0.6396557f, 0.6812356f, 0.8175842f, 0.002295136f, 0.9031434f, 0.4928995f, 0.6707507f, 0.9734639f, 0.4319176f, 0.9201014f, 0.7739761f, 0.4877432f, 0.8621292f, 0.8384882f, 0.4706053f, 0.6283126f, 0.9391375f, 0.5610672f, 0.7906424f, 0.3661477f, 0.7840475f, 0.811349f, 0.23399f, 0.005769968f, 0.8596302f, 0.8620507f, 0.2877693f, 0.4962842f, 0.5694764f, 0.6805189f, 0.4977598f, 0.2312933f, 0.2732872f, 0.7655005f, 0.5855765f, 0.3021132f, 0.4691666f, 0.4230034f, 0.9765128f, 0.2134674f, 0.8279065f, 0.8571178f, 0.3344561f, 0.8766525f, 0.7795609f, 0.4897286f, 0.4744037f, 0.2551411f, 0.4631788f, 0.1594888f, 0.2191068f, 0.2021723f, 0.7310678f, 0.1482067f, 0.3515903f, 0.7064862f, 0.5523744f, 0.2811777f, 0.8867198f, 0.5321998f, 0.1721799f, 0.5384375f, 0.7597345f, 0.05117285f, 0.5210977f, 0.7684529f, 0.3865288f, 0.5696713f, 0.8839966f, 0.1042888f, 0.1415581f, 0.8654914f, 0.6120399f, 0.3123181f, 0.120581f, 0.5572578f, 0.7717845f, 0.4368083f, 0.1452538f, 0.8342373f, 0.5691673f, 0.9788131f, 0.7768469f, 0.6167234f, 0.5191122f, 0.8043855f, 0.8616617f, 0.9984922f, 0.1971672f, 0.2901447f, 0.9218793f, 0.08565378f, 0.8941032f, 0.008978963f, 0.09031892f, 0.4649073f, 0.6331884f, 0.502968f, 0.7645359f, 0.3027768f, 0.2676347f, 0.5546543f, 0.263753f, 0.1446317f, 0.664363f, 0.9955279f, 0.07916415f, 0.74861f, 0.275342f, 0.4388431f, 0.6799415f, 0.677758f, 0.9374286f, 0.886752f, 0.2857775f, 0.1454593f, 0.4228964f, 0.794264f, 0.6253427f, 0.4347523f, 0.4226976f, 0.06426764f, 0.8696071f, 0.4983435f, 0.3494394f, 0.5460766f, 0.2248189f, 0.4397207f, 0.2977797f, 0.2683355f, 0.6891499f, 0.05355167f, 0.1737416f, 0.8775455f, 0.7393743f, 0.9509121f, 0.2998109f, 0.9153184f, 0.9088784f, 0.3692355f, 0.5863493f, 0.9885068f, 0.5834796f, 0.3372703f, 0.239861f, 0.294939f, 0.4514083f, 0.001043558f, 0.1590842f, 0.4884216f, 0.9709595f, 0.1803387f, 0.7500849f, 0.5873773f, 0.8083584f, 0.9827933f, 0.6511055f, 0.9068254f, 0.952805f, 0.03031898f, 0.6063736f, 0.7430531f, 0.8774858f, 0.6561912f, 0.7247708f, 0.9222763f, 0.9217877f, 0.6748147f, 0.3140202f, 0.317982f, 0.8463814f, 0.03287613f, 0.1671596f, 0.3544068f, 0.842098f, 0.609163f, 0.956638f};


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
                if (Random.Range(0,2) == 0) break;
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
        if (it == 1) {
            for (int i = 0; i < p_num; i++) {
                personas[i].ModificarOpinion(0,opinion1[i]);
                personas[i].ModificarOpinion(1,opinion2[i]);
                personas[i].ModificarOpinion(2,opinion3[i]);
                personas[i].ModificarOpinion(3,opinion4[i]);
                personas[i].ModificarCabezoneria(cabezoneria[i]);
                personas[i].ModificarPrestigio(prestigio[i]);
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
        for (int opcion = 0; opcion < opciones; opcion++) {
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
                for (int opcion = 0; opcion < opciones; opcion++) {
                    sum_op += Mathf.Abs(personas[conv[i]].opiniones[opcion] - personas[conv[j]].opiniones[opcion]);
                }

                prob = (1-sum_op/(2*opciones))*sens_desamigo/(1+relaciones[conv[i]].Count+relaciones[conv[j]].Count);

                if (Mathf.Exp(-prob*prob) > (float)Random.Range(0,1000)/1000) {
                    Desamigo(conv[i],conv[j]);
                    //Debug.Log($"Las personas {conv[i]} y {conv[j]} han dejado de ser amigos! D = {sum_op/(2*opciones)}, a = {(relaciones[conv[i]].Count+relaciones[conv[j]].Count)}");
                    //Debug.Log($"Las opiniones de la persona {conv[i]} y {conv[j]} son las siguientes");
                    //yfor (int op = 0; op < opciones; op++) Debug.Log($"Opcion {op}:{personas[conv[i]].opiniones[op]} y {personas[conv[j]].opiniones[op]}");
                }

                prob = (sum_op/(2*opciones))*sens_amigo*(relaciones[conv[i]].Count+relaciones[conv[j]].Count);

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
            for (int opcion = 0; opcion < opciones; opcion++) {
                sum_op += Mathf.Abs(personas[publ[i]].opiniones[opcion] - personas[emisor].opiniones[opcion]);
            }

            prob = (1-sum_op/(2*opciones))*sens_deseguidor;

            if (Mathf.Exp(-prob*prob) > (float)Random.Range(0,1000)/1000 && !redes_sociales[emisor].Contains(i)) {
                redes_sociales[emisor].Add(i);
                //Debug.Log($"La persona {publ[i]} ha dejado de seguir a {emisor}! D = {sum_op/(2*opciones)}");
                //Debug.Log($"Las opiniones de la persona {publ[i]} y {emisor} son las siguientes");
                //for (int op = 0; op < opciones; op++) Debug.Log($"Opcion {op}:{personas[publ[i]].opiniones[op]} y {personas[emisor].opiniones[op]}");
            }

            prob = sum_op/(2*opciones)*sens_seguidor;

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
        

        Conversación(conv_num, conv);

        //ALGUIEN HACE UNA PUBLICACIÓN
        
        int[] publ;
        int emisor = Random.Range(0,p_num);
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

        if (it%200 == 0) {
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

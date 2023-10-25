using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TitleGradient : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private Color midColor;


    // Start is called before the first frame update
    void Start()
    {
        ApplyGradient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ApplyGradient() {
        titleText.ForceMeshUpdate();
        TMPro.TMP_TextInfo textInfo = titleText.textInfo;
        int count = textInfo.characterCount;
        Color[] steps = GetGradients(titleText.colorGradient.topLeft, midColor, titleText.colorGradient.topRight, count + 1);
        VertexGradient[] gradients = new VertexGradient[steps.Length];
        for (int i = 0; i < steps.Length - 1; i++) {
            gradients[i] = new VertexGradient (steps[i], steps[i + 1], steps[i], steps[i + 1]);
        }
        Color32[] colors;
        int index = 0;
        while (index < count) {
            int materialIndex = textInfo.characterInfo[index].materialReferenceIndex;
            colors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[index].vertexIndex;
            if (textInfo.characterInfo[index].isVisible) {
                colors[vertexIndex + 0] = gradients[index].bottomLeft;
                colors[vertexIndex + 1] = gradients[index].topLeft;
                colors[vertexIndex + 2] = gradients[index].bottomRight;
                colors[vertexIndex + 3] = gradients[index].topRight;
                titleText.UpdateVertexData (TMP_VertexDataUpdateFlags.Colors32);
            }
            index++;
        }
    }

    public static Color[] GetGradients(Color start, Color mid, Color end, int steps)
{
    Color[] result = new Color[steps];
    
    int midIndex = steps / 2;

    float r1 = ((mid.r - start.r) / (midIndex - 1));
    float g1 = ((mid.g - start.g) / (midIndex - 1));
    float b1 = ((mid.b - start.b) / (midIndex - 1));
    float a1 = ((mid.a - start.a) / (midIndex - 1));

    float r2 = ((end.r - mid.r) / (steps - midIndex - 1));
    float g2 = ((end.g - mid.g) / (steps - midIndex - 1));
    float b2 = ((end.b - mid.b) / (steps - midIndex - 1));
    float a2 = ((end.a - mid.a) / (steps - midIndex - 1));

    for (int i = 0; i < midIndex; i++)
    {
        result[i] = new Color(start.r + (r1 * i), start.g + (g1 * i), start.b + (b1 * i), start.a + (a1 * i));
    }

    for (int i = midIndex; i < steps; i++)
    {
        result[i] = new Color(mid.r + (r2 * (i - midIndex)), mid.g + (g2 * (i - midIndex)), mid.b + (b2 * (i - midIndex)), mid.a + (a2 * (i - midIndex)));
    }

    return result;
}
}

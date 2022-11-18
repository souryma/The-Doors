using UnityEngine;
using System.Collections.Generic;

using Face = VDT.FaceRecognition.SDK;

public class FaceController : MonoBehaviour
{
    [Header ("Main")]
    [SerializeField] List<Vector3> points = new List<Vector3>();
    [SerializeField] List<int> triangles = new List<int>();
    [SerializeField] List<Vector2> uvs = new List<Vector2>();

    [Header("Visual")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;

    float opacity;
    Mesh mesh;
    List<Vector3> lerpPoints = new List<Vector3>();

    [Header ("Advanced")]
    [SerializeField] EmotionsController emotionsController;

    // For the FDA Tracker config, you need to adjust the chin height. 
    // This will be fixed in future updates to the Face SDK.

    [Header("Chin corrections")]
    [SerializeField] AnimationCurve correctionCurve;

    List<List<int>> pointIndexesChinCorrection = new List<List<int>>
    {
        new List<int> {135, 136},

        new List<int> {43, 202, 210, 169, 150 },
        new List<int> {106, 204, 211, 170, 149 },
        new List<int> {182, 194, 32, 140, 176 },
        new List<int> {83, 201, 208, 171, 148 },

        new List<int> {18, 200, 199, 175, 152 },

        new List<int> {313, 421, 428, 396, 377 },
        new List<int> {406, 418, 262, 369, 400 },
        new List<int> {335, 424, 431, 395, 378 },
        new List<int> {273, 422, 430, 394, 379 },

        new List<int> {364, 365},
    };

    public bool EmotionVisible
    {
        get
        {
            return emotionsController != null;
        }
    }

    public float Opacity
    {
        get
        {
            return opacity;
        }
        set
        {
            opacity = value;
            meshRenderer.material.SetFloat("_Transparency", value);
        }
    }

    public float LerpFactor
    {
        get;
        set;
    }

    public List<Face.EmotionsEstimator.EmotionConfidence> Emotions
    {
        get;
        private set;
    }

    public List<Vector3> Points
    {
        get
        {
            return new List<Vector3>(points);
        }
    }

    void Initialize(List<Vector3> _points, List<Face.EmotionsEstimator.EmotionConfidence> _emotions)
    {
        points = _points;
        Emotions = _emotions;
        lerpPoints = new List<Vector3>(points);

        mesh = new Mesh();
        meshFilter.mesh = mesh;

        mesh.SetVertices(points);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
    }

    public void UpdateFace(List<Vector3> _points, bool useCorrection, List<Face.EmotionsEstimator.EmotionConfidence> _emotions = null)
    {
        if (mesh == null)
            Initialize(_points, _emotions);
        else
        {
            points = _points;
            Emotions = _emotions;

            if (emotionsController != null)
                emotionsController.UpdateEmotion(_emotions);

            if(useCorrection)
                ChinCorrection();
        }
    }

    void ChinCorrection()
    {
        Dictionary<int, Vector3> newPositions = new Dictionary<int, Vector3>();

        for(int i = 0; i < pointIndexesChinCorrection.Count; i++)
        {
            List<int> cl = pointIndexesChinCorrection[i];

            float multCorrection = correctionCurve.Evaluate((float)i / (pointIndexesChinCorrection.Count - 1));

            for (int p = 1; p < cl.Count; p++)
            {
                Vector3 delta = points[cl[p]] - points[cl[p - 1]];
                Vector3 startPoint = p == 1 ? points[cl[p]] : newPositions[cl[p - 1]];

                Vector3 newPosition = startPoint + delta * (1f + multCorrection);

                newPositions.Add(cl[p], newPosition);
            }
        }

        foreach (KeyValuePair<int, Vector3> newPosition in newPositions)
            points[newPosition.Key] = newPosition.Value;
    }

    void Update()
    {
        for (int i = 0; i < points.Count; i++)
            lerpPoints[i] = Vector3.Lerp(lerpPoints[i], points[i], Time.deltaTime * LerpFactor);

        mesh.SetVertices(lerpPoints);
    }
}

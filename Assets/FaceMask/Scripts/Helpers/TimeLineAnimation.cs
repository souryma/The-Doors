using UnityEngine;

[System.Serializable]
public class TimeLineAnimation
{
    [SerializeField] float speed = 4f;
    [SerializeField] AnimationCurve animationCurve;

    float t = 0;

    public bool IsRuning
    {
        get;
        private set;
    }

    public float Value
    {
        get;
        private set;
    }

    public void Start()
    {
        IsRuning = true;
        t = 0;
    }

    public void Stop()
    {
        IsRuning = false;
    }

    public bool Update(float deltaTime)
    {
        if (!IsRuning)
            return false;

        if (t < 1)
        {
            t += deltaTime * speed;
            Value = animationCurve.Evaluate(t);
            return true;
        }
        else
        {
            IsRuning = false;
            return false;
        }
    }
}

[System.Serializable]
public class JumpingFloor
{
    public string id;
    public JumpFloor[] floor;
}

[System.Serializable]
public class JumpFloor
{
    public float[][] xMax;
    public float[][] yMax;
    public float[][] xMin;
    public float[][] yMin;
    public string[] direction;
}
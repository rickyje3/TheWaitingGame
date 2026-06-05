using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public Sprite s_hat;
    public Sprite s_shirt;
    public Sprite s_pants;
    public Sprite s_shoes;
    public Sprite s_accessories;
    public Sprite s_chair;
    public Sprite s_shelf;
    public Sprite s_table;
    public Sprite s_decorations;
    public Sprite s_lighting;


    public static GameAssets Instance
    {
        get
        {
            if (_instance == null) _instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _instance;
        }
    }
}

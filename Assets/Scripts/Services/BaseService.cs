using UnityEngine;

public abstract class BaseService : MonoBehaviour
{

    public void Init()
    {
       Services.Register(this);
    }
}

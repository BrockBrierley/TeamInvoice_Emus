using UnityEngine;

[ExecuteInEditMode()]
public class FlexibleUI : MonoBehaviour
{
    public UIData themeData;

    protected virtual void OnThemeUI()
    {

    }

    public virtual void Awake()
    {
        OnThemeUI();
    }

    public virtual void Update()
    {

        //Not optimal, can remove later or make an editor tool
        if (Application.isEditor)
        {
            OnThemeUI();
        }
    }
}

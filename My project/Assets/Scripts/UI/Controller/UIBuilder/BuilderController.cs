using UnityEngine.UIElements;

public abstract class BuilderController : UIBase
{
    #region Default
    public virtual bool Initialize(VisualElement _Layer)
    {
       
        return true;
    }
    #endregion
}

using UI.Enum;
using UnityEngine;

public class RegisterCanvas : MonoBehaviour
{
    [SerializeField] private EUICanvas layer;

    private void OnEnable()
    {
        var UIMgr = UIManager.instance;
        UIMgr.RegisteCanvas(layer, gameObject.GetComponent<Canvas>());
    }

    private void OnDisable()
    {
        var UIMgr = UIManager.instance;
        UIMgr.UnRegisteCanvas(layer);
    }
}
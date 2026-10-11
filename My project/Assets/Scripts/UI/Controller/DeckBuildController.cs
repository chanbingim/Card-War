using UnityEngine;
using UnityEngine.Events;

public class DeckBuildController : MonoBehaviour
{
    [SerializeField] private  UnityEvent<uint>     OnChangeSelectItem;
    private uint              SelectItem;

    public void RequestSelectItem(uint Index)
    {
        SelectItem = Index;

        // 여기서 보여주기 Refesh
        OnChangeSelectItem?.Invoke(SelectItem);
    }





}

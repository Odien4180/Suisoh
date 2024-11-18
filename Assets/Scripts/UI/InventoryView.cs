using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoView
{
    [SerializeField]
    private ScrollRect _invenScroll;
    public ScrollRect InvenScroll => _invenScroll;

    [SerializeField]
    private ItemIconView _itemIconOrigin;
    public ItemIconView ItemIconOrigin => _itemIconOrigin;
}

public class InventoryPreseneter : Presenter<InventoryView>
{
    private InventoryModel _inventoryModel;
    public void Initialize(InventoryModel inventoryModel)
    {
        _inventoryModel = inventoryModel;

        foreach (var item in _inventoryModel.Items)
        {
            InstantiateItemIcon(item.Value);
        }

        _inventoryModel.Items.ObserveAdd().Subscribe(addEvent =>
        {
            InstantiateItemIcon(addEvent.Value);
        }).AddTo(this);
    }

    private void InstantiateItemIcon(ItemModel itemModel)
    {
        var itemIcon = GameObject.Instantiate(View.ItemIconOrigin, View.InvenScroll.content);

        itemIcon.Binding<ItemIconView, ItemIconPresneter>()
            .Initialize(itemModel);
    }
}

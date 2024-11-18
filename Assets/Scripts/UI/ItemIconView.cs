using UniRx;

public class ItemIconView : MonoView
{
    
}

public class ItemIconPresneter : Presenter<ItemIconView>
{
    private ItemModel _itemModel;
    public void Initialize(ItemModel itemModel)
    {
        _itemModel = itemModel;

        _itemModel.Amount
            .Subscribe(OnChangeItemAmount).AddTo(this);
    }

    private void OnChangeItemAmount(int amount)
    {
        if (amount <= 0)
        {
            View.gameObject.SetActive(false);
        }
    }
}

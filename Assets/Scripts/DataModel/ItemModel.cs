using UniRx;

public class ItemModel
{
    private IntReactiveProperty _amount;
    public IReadOnlyReactiveProperty<int> Amount => _amount;
}

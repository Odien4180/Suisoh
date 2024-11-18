using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class InventoryModel
{
    private ReactiveDictionary<int, ItemModel> _items;
    public IReadOnlyReactiveDictionary<int, ItemModel> Items => _items;
}

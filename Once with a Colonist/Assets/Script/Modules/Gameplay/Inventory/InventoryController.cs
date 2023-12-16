using System.Linq;
using TendedTarsier;
using UniRx;
using UnityEngine;
using Zenject;

public class InventoryController : MonoBehaviour
{
    private readonly CompositeDisposable _compositeDisposable = new();

    [SerializeField]
    private Transform _gridContainer;

    private InventoryProfile _inventoryProfile;
    private InventoryConfig _inventoryConfig;

    private InventoryCellView[][] _grid;

    [Inject]
    private void Construct(
        InventoryProfile inventoryProfile,
        InventoryConfig inventoryConfig)
    {
        _inventoryProfile = inventoryProfile;
        _inventoryConfig = inventoryConfig;

        _inventoryProfile.InventoryItems.ObserveAdd().Subscribe(Put).AddTo(_compositeDisposable);
    }

    private void Start()
    {
        var counter = 0;
        _grid = new InventoryCellView[_inventoryConfig.InventoryGrid.x][];
        for (var x = 0; x < _inventoryConfig.InventoryGrid.x; x++)
        {
            _grid[x] = new InventoryCellView[_inventoryConfig.InventoryGrid.y];
            for (var y = 0; y < _inventoryConfig.InventoryGrid.y; y++)
            {
                _grid[x][y] = Instantiate(_inventoryConfig.InventoryCellView, _gridContainer);
                _grid[x][y].OnButtonClicked.Subscribe(onCellClicked).AddTo(_compositeDisposable);
                if (_inventoryProfile.InventoryItems.Count > counter)
                {
                    var item = _inventoryProfile.InventoryItems.ElementAt(counter);
                    SetItem(_grid[x][y], item.Key, item.Value);
                }
                counter++;
            }
        }

        void onCellClicked(string id)
        {
            _inventoryProfile.SelectedItem.Value = id;
            _inventoryProfile.Save();
        }
    }

    private void Put(DictionaryAddEvent<string, ReactiveProperty<int>> item)
    {
        foreach (var row in _grid)
        {
            var cell = row.FirstOrDefault(t => t.IsEmpty());
            if (cell != null)
            {
                SetItem(cell, item.Key, item.Value);
                break;
            }
        }
    }

    private void SetItem(InventoryCellView cell, string id, ReactiveProperty<int> value)
    {
        cell.SetItem(_inventoryConfig[id], value);
    }

    private void OnDestroy()
    {
        _compositeDisposable.Dispose();
    }
}
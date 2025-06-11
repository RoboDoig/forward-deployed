using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

public class WorldItemContainer
{
    private ObservableCollection<WorldItem> Items = new ObservableCollection<WorldItem>();

    public int ItemCount => Items.Count;

    public void AddOperatorItem(ItemType type)
    {
        Items.Add(
            new WorldItem { OperatorType = type }
        );
    }
}

public class WorldItem
{
    public ItemType OperatorType;

    //public IOperator CreateOperatorFromItem()
    //{
    //    return (IOperator)Activator.CreateInstance(OperatorType.ComponentType);
    //}
}

public class ItemType
{
    public Type ComponentType { get; private set; }
    
    private ItemType(Type componentType)
    {
        ComponentType = componentType;
    }

    //public static ItemType Create<T>() where T : IOperator
    //{
    //    return new ItemType(typeof(T));
    //}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

public class WorldItemContainer
{
    public ObservableCollection<WorldItem> Items;
}

public class WorldItem
{
    public Type OperatorType;

    public IOperator Operator => (IOperator)Activator.CreateInstance(OperatorType);
}

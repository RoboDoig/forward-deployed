using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OperatorGraph
{
    public AdjacencyGraph<IOperator, Edge<IOperator>> Graph { get; private set; }
    private Dictionary<Edge<IOperator>, IDisposable> ObservableEdges;

    public OperatorGraph()
    {
        Graph = new AdjacencyGraph<IOperator, Edge<IOperator>>();
        ObservableEdges = new Dictionary<Edge<IOperator>, IDisposable>();

        Graph.EdgeAdded += (v) =>
        {
            // Create the disposable connection and add to observable edges
            //ObservableEdges.Add(v.Source);
        };

        Graph.EdgeRemoved += (v) =>
        {
            // Find the disposable connection and dispose it
        };
    }

    public void AddOperator(IOperator op)
    {
        Graph.AddVertex(op);
    }

    public void RemoveOperator(IOperator op)
    {
        Graph.RemoveVertex(op);
    }

    public void ConnectOperators(IOperator from, IOperator to)
    {
        Graph.AddEdge(new Edge<IOperator>(from, to));
    }

    public void DisconnectOperators(IOperator from, IOperator to)
    {
    }
}

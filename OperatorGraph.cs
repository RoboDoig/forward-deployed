using Godot;
using QuikGraph;
using R3;
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

        Graph.EdgeAdded += (e) =>
        {
            // Create the disposable connection and add to observable edges
            var fromOperator = e.Source;
            var fromDataSubject = e.Source.GetType().GetProperty("DataSubject");
            var toInputSubject = e.Target.GetType().GetProperty("InputSubject");

            var connectMethod = GetType().GetMethod("ConnectSubjects").MakeGenericMethod(fromDataSubject.PropertyType.GetGenericArguments());
            var connect = connectMethod.Invoke(null, [fromDataSubject.GetValue(e.Source), toInputSubject.GetValue(e.Target)]) as IDisposable;

            ObservableEdges.Add(e, connect);
        };

        Graph.EdgeRemoved += (e) =>
        {
            // Find the disposable connection and dispose it
            ObservableEdges[e].Dispose();
            ObservableEdges.Remove(e);
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
        Edge<IOperator> edgeToRemove;
        bool edgeExists = Graph.TryGetEdge(from, to, out edgeToRemove);

        if (edgeExists)
            Graph.RemoveEdge(edgeToRemove);
    }

    public static IDisposable ConnectSubjects<T>(ConnectableObservable<T> from, Subject<T> to)
    {
        return from.Multicast(to).Connect();
    }
}

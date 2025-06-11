using Godot;
using QuikGraph;
using QuikGraph.Algorithms;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OperatorGraph
{
    public AdjacencyGraph<GraphNodeMetadata, Edge<GraphNodeMetadata>> Graph { get; private set; }
    private Dictionary<Edge<GraphNodeMetadata>, IDisposable> ObservableEdges; // Map of graph edges to disposable connections between operators

    public OperatorGraph()
    {
        Graph = new AdjacencyGraph<GraphNodeMetadata, Edge<GraphNodeMetadata>>();
        ObservableEdges = new Dictionary<Edge<GraphNodeMetadata>, IDisposable>();

        Graph.EdgeAdded += (e) =>
        {
            // Create the disposable connection and add to observable edges
            var fromOperator = e.Source.Operator;
            var fromDataSubject = e.Source.Operator.GetType().GetProperty("DataSubject");
            var toInputSubject = e.Target.Operator.GetType().GetProperty("InputSubject");

            var connectMethod = GetType().GetMethod("ConnectSubjects").MakeGenericMethod(fromDataSubject.PropertyType.GetGenericArguments());
            var connect = connectMethod.Invoke(null, [fromDataSubject.GetValue(e.Source.Operator), toInputSubject.GetValue(e.Target.Operator)]) as IDisposable;

            ObservableEdges.Add(e, connect);
        };

        Graph.EdgeRemoved += (e) =>
        {
            // Find the disposable connection and dispose it
            ObservableEdges[e].Dispose();
            ObservableEdges.Remove(e);
        };

        Graph.VertexRemoved += (v) =>
        {

        };
    }

    public GraphNodeMetadata AddOperator(OperatorResource op)
    {
        var node = new GraphNodeMetadata { Operator = op };
        Graph.AddVertex(new GraphNodeMetadata { Operator = op });
        return node;
    }

    public GraphNodeMetadata AddOperator(GraphNodeMetadata node)
    {
        Graph.AddVertex(node);
        return node;
    }

    public void RemoveOperator(OperatorResource op)
    {
        Graph.RemoveVertex(Graph.Vertices.Where(x => x.Operator == op).First());
    }

    public void ConnectOperators(GraphNodeMetadata from, GraphNodeMetadata to)
    {
        Graph.AddEdge(new Edge<GraphNodeMetadata>(from, to));
    }

    public void DisconnectOperators(GraphNodeMetadata from, GraphNodeMetadata to)
    {
        Edge<GraphNodeMetadata> edgeToRemove;
        bool edgeExists = Graph.TryGetEdge(from, to, out edgeToRemove);

        if (edgeExists)
            Graph.RemoveEdge(edgeToRemove);
    }

    public static IDisposable ConnectSubjects<T>(ConnectableObservable<T> from, Subject<T> to)
    {
        return from.Multicast(to).Connect();
    }

    public class GraphNodeMetadata
    {
        public OperatorResource Operator;
        public Vector2 LayoutOffset;
        public bool Permanent;
    }
}

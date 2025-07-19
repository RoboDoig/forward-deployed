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
    public AdjacencyGraph<GraphNodeMetadata, STaggedEdge<GraphNodeMetadata, Vector2I>> Graph { get; private set; }
    private Dictionary<STaggedEdge<GraphNodeMetadata, Vector2I>, IDisposable> ObservableEdges; // Map of graph edges to disposable connections between operators

    public OperatorGraph()
    {
        Graph = new AdjacencyGraph<GraphNodeMetadata, STaggedEdge<GraphNodeMetadata, Vector2I>>();
        ObservableEdges = new Dictionary<STaggedEdge<GraphNodeMetadata, Vector2I>, IDisposable>();

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

    public GraphNodeMetadata RemoveOperator(OperatorResource op)
    {
        GraphNodeMetadata vertexToRemove = Graph.Vertices.Where(x => x.Operator == op).First();
        Graph.RemoveVertex(vertexToRemove);

        return vertexToRemove;
    }

    public void ConnectOperators(GraphNodeMetadata from, int fromSlotIndex, GraphNodeMetadata to, int toSlotIndex)
    {
        Graph.AddEdge(new STaggedEdge<GraphNodeMetadata, Vector2I>(from, to, new Vector2I(fromSlotIndex, toSlotIndex)));
    }

    public void DisconnectOperators(GraphNodeMetadata from, int fromSlotIndex, GraphNodeMetadata to, int toSlotIndex)
    {
        IEnumerable<STaggedEdge<GraphNodeMetadata, Vector2I>> validEdges = new List<STaggedEdge<GraphNodeMetadata, Vector2I>>();
        Vector2I edgeComparer = new Vector2I(fromSlotIndex, toSlotIndex);
        bool edgesExist = Graph.TryGetEdges(from, to, out validEdges);


        if (edgesExist)
        {
            var edgeToRemove = validEdges.Where(x => x.Tag == edgeComparer).ToList();
            if (edgeToRemove != null)
            {
                Graph.RemoveEdge(edgeToRemove[0]); // TODO - hard coded 0, should be a better way
            }
        }
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

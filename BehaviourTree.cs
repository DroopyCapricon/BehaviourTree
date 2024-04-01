using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{ 
    RUNNING, 
    SUCCESS, 
    FAILURE 
}

public class Node
{
    public NodeState state;
    public Node parent;
    public List<Node> children;


    private Dictionary<string, object> data = new();
    public Node()
    {
        parent = null;
    }

    public Node(List<Node> children)
    {
        foreach(Node child in children)
            Attach(child);
    }

    public void Attach(Node node)
    {
        node.parent = this;
        children.Add(node);
    }

    public virtual NodeState Evaluate() => NodeState.FAILURE;

    public void SetData(string key, object value)
    {
        data[key] = value;
    }

    public object GetData(string key)
    {
        object value = null;
        if(data.TryGetValue(key, out value))
            return value;

        Node node = parent;
        while(node != null)
        {
            value = node.GetData(key);
            if (value != null) return value;
            node = node.parent;
        }
        return null;
    }

    public bool ClearData(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
            return true;
        }

        Node node = parent;
        while (node != null)
        {
            bool cleared = node.ClearData(key);
            if (cleared) return true;
            node = node.parent;
        }
        return false;
    }
}

public class Sequence : Node
{
    public Sequence() : base() { }
    public Sequence(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        bool isRunning = false;

        foreach(Node child in children)
            switch (child.Evaluate())
            {
                case NodeState.FAILURE: state = NodeState.FAILURE; return state;
                case NodeState.RUNNING: isRunning = true; continue;
                case NodeState.SUCCESS: continue;
                default : state = NodeState.SUCCESS; return state;
            }
        state = isRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}

public class Selector : Node
{
    public Selector() : base() { }
    public Selector(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        foreach (Node child in children)
            switch (child.Evaluate())
            {
                case NodeState.FAILURE: continue;
                case NodeState.RUNNING: state = NodeState.RUNNING; return state;
                case NodeState.SUCCESS: state = NodeState.SUCCESS; return state;
                default: continue;
            }
        state = NodeState.FAILURE;
        return state;
    }
}

public abstract class Tree : MonoBehaviour
{
    private Node root = null;

    private void Start()
    {
        root = SetupTree();
    }

    private void Update()
    {
        if(root != null)
            root.Evaluate();
    }

    public abstract Node SetupTree();
}

public class BehaviourTree : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

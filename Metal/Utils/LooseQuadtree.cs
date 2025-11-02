namespace Metal.Utils;

public struct AoiEntity
{
    public int Id;
    public float X, Y;
    public float Radius;
    public int ContainerNode; // 索引到 QuadNode 池
    public int Next; // 链表 next
}

public struct QuadNode
{
    public float X, Y, W, H; // bounds
    public float LooseFactor; // 松散倍数
    public int Head; // 链表头实体索引
    public int[] Children; // 子节点索引，-1 表示无子
    public bool IsLeaf;
}

public class LooseQuadtree
{
    private const int MaxObjects = 8;
    private const int MaxDepth = 6;
    private const float LooseFactor = 2.0f;
    private readonly int _rootIndex;

    private AoiEntity[] _entities;
    private int _entityCount;
    private int[] _mark;
    private int _nodeCount;

    private QuadNode[] _nodePool;
    private int _queryStamp;

    public LooseQuadtree(float x, float y, float w, float h, int maxNodes, int maxEntities)
    {
        _nodePool = new QuadNode[maxNodes];
        _entities = new AoiEntity[maxEntities];
        _mark = new int[maxEntities];

        _rootIndex = AllocNode();
        ref var root = ref _nodePool[_rootIndex];
        root.X = x;
        root.Y = y;
        root.W = w;
        root.H = h;
        root.LooseFactor = LooseFactor;
        root.Head = -1;
        root.IsLeaf = true;
        root.Children = [-1, -1, -1, -1];

        _queryStamp = 0;
        _nodeCount = 1;
        _entityCount = 0;
    }

    private int AllocNode()
    {
        if (_nodeCount >= _nodePool.Length)
        {
            throw new Exception("QuadNode pool exhausted");
        }

        return _nodeCount++;
    }

    public int AddEntity(float x, float y, float radius)
    {
        if (_entityCount >= _entities.Length)
        {
            throw new Exception("Entity pool full");
        }

        var id = _entityCount++;
        var e = new AoiEntity
        {
            Id = id,
            X = x,
            Y = y,
            Radius = radius,
            ContainerNode = -1,
            Next = -1
        };
        _entities[id] = e;
        Insert(_rootIndex, id, 0);
        return id;
    }

    private void Insert(int nodeIdx, int entityId, int depth)
    {
        ref var node = ref _nodePool[nodeIdx];
        ref var e = ref _entities[entityId];

        // 如果不是叶子且有子节点，递归尝试插入
        if (!node.IsLeaf)
        {
            var childIdx = GetFittingChild(nodeIdx, entityId);
            if (childIdx != -1)
            {
                Insert(childIdx, entityId, depth + 1);
                return;
            }
        }

        // 否则插入当前节点链表
        e.Next = node.Head;
        node.Head = entityId;
        e.ContainerNode = nodeIdx;

        // 判断是否需要分裂
        if (node.IsLeaf && CountNodeEntities(nodeIdx) > MaxObjects && depth < MaxDepth)
        {
            Split(nodeIdx);
        }
    }

    private int CountNodeEntities(int nodeIdx)
    {
        var count = 0;
        var cur = _nodePool[nodeIdx].Head;
        while (cur != -1)
        {
            count++;
            cur = _entities[cur].Next;
        }

        return count;
    }

    private void Split(int nodeIdx)
    {
        ref var node = ref _nodePool[nodeIdx];
        var hw = node.W / 2;
        var hh = node.H / 2;
        node.IsLeaf = false;
        node.Children = new int[4];

        for (var i = 0; i < 4; i++)
        {
            var child = AllocNode();
            node.Children[i] = child;
            ref var c = ref _nodePool[child];
            c.W = hw;
            c.H = hh;
            c.LooseFactor = LooseFactor;
            c.Head = -1;
            c.IsLeaf = true;
            c.Children = [-1, -1, -1, -1];
        }

        _nodePool[node.Children[0]].X = node.X;
        _nodePool[node.Children[0]].Y = node.Y; // TL
        _nodePool[node.Children[1]].X = node.X + hw;
        _nodePool[node.Children[1]].Y = node.Y; // TR
        _nodePool[node.Children[2]].X = node.X;
        _nodePool[node.Children[2]].Y = node.Y + hh; // BL
        _nodePool[node.Children[3]].X = node.X + hw;
        _nodePool[node.Children[3]].Y = node.Y + hh; // BR

        // 重分布实体
        var cur = node.Head;
        node.Head = -1;
        while (cur != -1)
        {
            var next = _entities[cur].Next;
            Insert(nodeIdx, cur, 0); // 递归插入到新子节点
            cur = next;
        }
    }

    private int GetFittingChild(int nodeIdx, int entityId)
    {
        ref var node = ref _nodePool[nodeIdx];
        ref var e = ref _entities[entityId];
        for (var i = 0; i < 4; i++)
        {
            ref var child = ref _nodePool[node.Children[i]];
            var looseW = child.W * child.LooseFactor;
            var looseH = child.H * child.LooseFactor;
            if (e.X >= child.X && e.X <= child.X + looseW &&
                e.Y >= child.Y && e.Y <= child.Y + looseH)
            {
                return node.Children[i];
            }
        }

        return -1;
    }

    // 删除实体
    public void RemoveEntity(int entityId)
    {
        ref var e = ref _entities[entityId];
        var nodeIdx = e.ContainerNode;
        var cur = _nodePool[nodeIdx].Head;
        var prev = -1;
        while (cur != -1)
        {
            if (cur == entityId)
            {
                if (prev == -1)
                {
                    _nodePool[nodeIdx].Head = e.Next;
                }
                else
                {
                    _entities[prev].Next = e.Next;
                }

                e.ContainerNode = -1;
                e.Next = -1;
                break;
            }

            prev = cur;
            cur = _entities[cur].Next;
        }

        TryMerge(nodeIdx);
    }

    private void TryMerge(int nodeIdx)
    {
        ref var node = ref _nodePool[nodeIdx];
        if (node.IsLeaf)
        {
            return;
        }

        var totalEntities = 0;
        for (var i = 0; i < 4; i++)
        {
            totalEntities += CountNodeEntities(node.Children[i]);
        }

        if (totalEntities <= MaxObjects)
        {
            // 合并子节点实体到父节点
            node.Head = -1;
            for (var i = 0; i < 4; i++)
            {
                var childHead = _nodePool[node.Children[i]].Head;
                var cur = childHead;
                while (cur != -1)
                {
                    var next = _entities[cur].Next;
                    _entities[cur].Next = node.Head;
                    node.Head = cur;
                    _entities[cur].ContainerNode = nodeIdx;
                    cur = next;
                }

                _nodePool[node.Children[i]].Head = -1;
            }

            node.IsLeaf = true;
            node.Children = [-1, -1, -1, -1];
        }
    }

    // 查询 AOI 圆
    public void QueryAOI(float x, float y, float radius, Action<AoiEntity> callback)
    {
        _queryStamp++;
        QueryNode(_rootIndex, x, y, radius, callback);
    }

    private void QueryNode(int nodeIdx, float x, float y, float radius, Action<AoiEntity> callback)
    {
        ref var node = ref _nodePool[nodeIdx];
        var looseW = node.W * node.LooseFactor;
        var looseH = node.H * node.LooseFactor;
        if (!RectCircleIntersect(node.X, node.Y, looseW, looseH, x, y, radius))
        {
            return;
        }

        var cur = node.Head;
        var r2 = radius * radius;
        while (cur != -1)
        {
            ref var e = ref _entities[cur];
            var dx = e.X - x;
            var dy = e.Y - y;
            if (dx * dx + dy * dy <= r2)
            {
                callback(e);
            }

            cur = e.Next;
        }

        if (!node.IsLeaf)
        {
            for (var i = 0; i < 4; i++)
            {
                QueryNode(node.Children[i], x, y, radius, callback);
            }
        }
    }

    private bool RectCircleIntersect(float rx, float ry, float rw, float rh, float cx, float cy, float r)
    {
        var closestX = Math.Clamp(cx, rx, rx + rw);
        var closestY = Math.Clamp(cy, ry, ry + rh);
        var dx = cx - closestX;
        var dy = cy - closestY;
        return dx * dx + dy * dy <= r * r;
    }

    // 更新实体位置
    public void UpdateEntity(int id, float newX, float newY)
    {
        RemoveEntity(id);
        _entities[id].X = newX;
        _entities[id].Y = newY;
        Insert(_rootIndex, id, 0);
    }
}

using Amethyst.Players;

namespace Teleportation;

internal sealed class Request
{
    private const long _timeoutSecs = 30;

    private static readonly List<Request> _globalRequests = [];
    private static readonly Lock _lock = new();

    private readonly WeakReference<NetPlayer> _from;
    private readonly WeakReference<NetPlayer> _to;
    private readonly CancellationTokenSource _timeoutCts; // Timeout control

    public Request(NetPlayer from, NetPlayer to)
    {
        _from = new WeakReference<NetPlayer>(from);
        _to = new WeakReference<NetPlayer>(to);

        lock (_lock)
        {
            foreach (Request existingRequest in _globalRequests)
            {
                if (existingRequest.IsActive && existingRequest.From == from || from == to)
                {
                    throw new InvalidOperationException("teleportation.tprequest.unavailable");
                }
            }

            _globalRequests.Add(this);
        }

        // Setup timeout
        _timeoutCts = new();

        _timeoutCts.CancelAfter(TimeSpan.FromSeconds(_timeoutSecs));

        _timeoutCts.Token.Register(OnTimeOut);
    }

    public static IReadOnlyList<Request> GlobalRequests
    {
        get
        {
            lock (_lock)
            {
                _globalRequests.RemoveAll(r => !r.IsActive);
                return [.. _globalRequests];
            }
        }
    }

    public NetPlayer? From => _from.TryGetTarget(out NetPlayer? f) ? f : null;
    public NetPlayer? To => _to.TryGetTarget(out NetPlayer? t) ? t : null;
    public bool IsValid => From != null && To != null;

    public bool IsActive
    {
        get
        {
            NetPlayer? from = From;
            NetPlayer? to = To;

            return from != null
                && to != null
                && from.IsActive
                && to.IsActive;
        }
    }

    public void Efectuate()
    {
        Remove(); // Will cancel timeout when called manually

        if (IsActive)
        {
            From!.Utils.Teleport(To!.Utils.PosX, To.Utils.PosY);
        }
    }

    public void Remove()
    {
        lock (_lock)
        {
            _globalRequests.Remove(this);
        }

        // Cancel and dispose the CTS to prevent further timeout triggers
        try
        {
            if (!_timeoutCts.IsCancellationRequested)
            {
                _timeoutCts.Cancel(); // Prevents any pending callbacks if not already triggered
            }
            _timeoutCts.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed, ignore
        }
    }

    private void OnTimeOut()
    {
        bool isActiveInGlobalList;

        lock (_lock)
        {
            isActiveInGlobalList = _globalRequests.Contains(this);
        }

        // If the request is no longer in the list, do nothing
        if (!isActiveInGlobalList)
        {
            return;
        }

        if (IsValid)
        {
            From!.ReplyError("teleportation.timeout.from");
            To!.ReplyError("teleportation.timeout.to", From!.Name);
        }

        Remove(); // This will handle CTS disposal
    }
}

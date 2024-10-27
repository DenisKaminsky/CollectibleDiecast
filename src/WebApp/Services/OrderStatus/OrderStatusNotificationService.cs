namespace CollectibleDiecast.WebApp.Services;

public class OrderStatusNotificationService
{
    // Locking manually because we need multiple values per key, and only need to lock very briefly
    private readonly object _subscriptionsLock = new();
    private readonly Dictionary<string, HashSet<Subscription>> _subscriptionsByBuyerId = new();

    public IDisposable SubscribeToOrderStatusNotifications(string buyerId, Func<Task> callback)
    {
        var subscription = new Subscription(this, buyerId, callback);

        lock (_subscriptionsLock)
        {
            if (!_subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions))
            {
                subscriptions = new HashSet<Subscription>();
                _subscriptionsByBuyerId.Add(buyerId, subscriptions);
            }

            subscriptions.Add(subscription);
        }

        return subscription;
    }

    public Task NotifyOrderStatusChangedAsync(string buyerId)
    {
        lock (_subscriptionsLock)
        {
            return _subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions)
                ? Task.WhenAll(subscriptions.Select(s => s.NotifyAsync()))
                : Task.CompletedTask;
        }
    }

    private void Unsubscribe(string buyerId, Subscription subscription)
    {
        lock (_subscriptionsLock)
        {
            if (_subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions))
            {
                subscriptions.Remove(subscription);
                if (subscriptions.Count == 0)
                {
                    _subscriptionsByBuyerId.Remove(buyerId);
                }
            }
        }
    }

    private class Subscription : IDisposable
    {
        private readonly OrderStatusNotificationService _owner;
        private readonly string _buyerId;
        private readonly Func<Task> _callback;

        public Subscription(OrderStatusNotificationService owner, string buyerId, Func<Task> callback)
        {
            _owner = owner;
            _buyerId = buyerId;
            _callback = callback;
        }

        public Task NotifyAsync()
        {
            return _callback();
        }

        public void Dispose()
            => _owner.Unsubscribe(_buyerId, this);
    }
}

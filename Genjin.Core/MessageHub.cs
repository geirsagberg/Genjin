using System.Collections.Concurrent;
using BunnyLand.DesktopGL.Utils;
using Dynamitey;

namespace Genjin.Example;

/// Inspired by https://github.com/upta/pubsub/blob/master/PubSub/Core/Hub.cs and MediatR
public class MessageHub {
    private readonly ConcurrentDictionary<Type, List<Delegate>> notificationHandlers = new();
    private readonly ConcurrentDictionary<Type, Delegate> requestHandlers = new();

    private IDictionary<Type, List<Delegate>> NotificationHandlers => notificationHandlers;
    private IDictionary<Type, Delegate> RequestHandlers => requestHandlers;

    public void Publish<T>(T notification) where T : INotification {
        if (NotificationHandlers.TryGetValue(typeof(T), out var handlers)) {
            foreach (var handler in handlers) {
                switch (handler) {
                    case Action<T> action:
                        action(notification);
                        break;
                    case Func<T, Task> func:
                        func(notification).GetAwaiter().GetResult();
                        break;
                }
            }
        }
    }
    
    public TResponse Send<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse> {
        if (RequestHandlers.TryGetValue(typeof(TRequest), out var handler)) {
            return handler switch {
                Func<TRequest, TResponse> func => func(request),
                Func<TRequest, Task<TResponse>> funcAsync => funcAsync(request).GetAwaiter().GetResult(),
                _ => throw new Exception("Unknown handler type")
            };
        }
        throw new Exception($"No handler found for {typeof(TRequest)}");
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request) {
        if (RequestHandlers.TryGetValue(request.GetType(), out var handler)) {
            var result = handler.FastDynamicInvoke(request);
            return result switch {
                Task<TResponse> task => await task,
                TResponse response => response,
                _ => throw new Exception("Unknown result " + result)
            };
        } else {
            throw new Exception("No handler for request " + request.GetType());
        }
    }
    
    public void Send<T>(T request) where T : class, IRequest {
        if (RequestHandlers.TryGetValue(typeof(T), out var handler)) {
            switch (handler) {
                case Action<T> action:
                    action(request);
                    break;
                case Func<T, Task>:
                    throw new Exception("Non-async request not supported");
                default:
                    throw new Exception("Unknown handler type");
            }
        } else {
            throw new Exception("No handler for request " + typeof(T));
        }
    }

    public async Task SendAsync<T>(T request) where T : class, IRequest {
        if (RequestHandlers.TryGetValue(typeof(T), out var handler)) {
            switch (handler) {
                case Action<T> action:
                    action(request);
                    break;
                case Func<T, Task> func:
                    await func(request);
                    break;
                default:
                    throw new Exception("Unknown handler type");
            }
        } else {
            throw new Exception("No handler for request " + request.GetType());
        }
    }

    public void SubscribeMany(Action<INotification> action, params Type[] messageTypes) {
        foreach (var messageType in messageTypes) {
            notificationHandlers.GetOrAdd(messageType, new List<Delegate>());
            notificationHandlers[messageType].Add(action);
        }
    }

    public void Subscribe<T>(Action<T> action) where T : INotification {
        notificationHandlers.GetOrAdd(typeof(T), new List<Delegate>());
        notificationHandlers[typeof(T)].Add(action);
    }
    
    public void Subscribe<T>(Func<T, Task> action) where T : INotification {
        notificationHandlers.GetOrAdd(typeof(T), new List<Delegate>());
        notificationHandlers[typeof(T)].Add(action);
    }

    public void Handle<TRequest, TResponse>(Func<TRequest, TResponse> handler) where TRequest : IRequest<TResponse> {
        if (!requestHandlers.TryAdd(typeof(TRequest), handler)) {
            throw new Exception("Handler already registered for " + typeof(TRequest));
        }
    }

    public void Handle<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        where TRequest : IRequest<TResponse> {
        if (!requestHandlers.TryAdd(typeof(TRequest), handler)) {
            throw new Exception("Handler already registered for " + typeof(TRequest));
        }
    }

    public void Handle<TRequest>(Action<TRequest> handler) where TRequest : IRequest {
        if (!requestHandlers.TryAdd(typeof(TRequest), handler)) {
            throw new Exception("Handler already registered for " + typeof(TRequest));
        }
    }

    public void Handle<TRequest>(Func<TRequest, Task> handler) where TRequest : IRequest {
        if (!requestHandlers.TryAdd(typeof(TRequest), handler)) {
            throw new Exception("Handler already registered for " + typeof(TRequest));
        }
    }

    public void SubscribeMany(Action<INotification> action, Dictionary<Type, BoundMethod> handlers) {
        SubscribeMany(action, handlers.Keys.ToArray());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PushPractice.Controllers
{
    [AsyncTimeout(60000)]
    public abstract class CometController : AsyncController
    {
        private static readonly object _syncLock = new object();
        private static readonly Dictionary<object, WeakReference> _clients = new Dictionary<object, WeakReference>();

        protected void AddObserver(object key, Action callback)
        {
            lock (_syncLock)
            {
                var reference = new WeakReference(callback);

                if (_clients.ContainsKey(key))
                {
                    _clients[key] = reference;
                }
                else
                {
                    _clients.Add(key, reference);
                }
            }
        }

        protected void Notify()
        {
            lock (_clients)
            {
                foreach(var client in _clients){
                    var reference = client.Value;

                    if (reference.IsAlive)
                    {
                        ((Action)reference.Target)?.Invoke();
                    }
                }
            }
        }

        protected void RemoveObserver(object key)
        {
            lock (_syncLock)
            {
                _clients.Remove(key);
            }
        }
    }
}
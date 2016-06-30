using Knightware.Diagnostics;
using Knightware.Net;
using Spyder.Client.Net.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spyder.Client.IO;
using Spyder.Client.Common;
using Spyder.Client.Net;
using Knightware.Threading.Tasks;

namespace Spyder.Client
{
    /// <summary>
    /// Listens for Spyder UDP messages and maintains a list of BindableSpyderClient instances for all discovered servers
    /// </summary>
    public class SpyderClientManagerBase : INotifyPropertyChanged
    {
        private readonly SynchronizationContext context;
        private readonly Func<Task<SpyderServerEventListenerBase>> getServerEventListener;
        private readonly Func<string, Task<ISpyderClientExtended>> getSpyderClient;
        private readonly AsyncLock spyderServersLock = new AsyncLock();
        private readonly AsyncLock spyderServersInitializingLock = new AsyncLock();
        private List<BindableSpyderClient> spyderServers = new List<BindableSpyderClient>();
        private SpyderServerEventListenerBase serverEventListener;

        public event EventHandler ServerListChanged;
        protected void OnServerListChanged(EventArgs e)
        {
            if (ServerListChanged != null)
                ServerListChanged(this, e);
        }

        public event DrawingDataReceivedHandler DrawingDataReceived;
        protected void OnDrawingDataReceived(DrawingDataReceivedEventArgs e)
        {
            if(DrawingDataReceived != null)
                DrawingDataReceived(this, e);
        }

        /// <summary>
        /// Determines if the DrawingDataChanged event should be fired from this class.  Setting to true will cause this
        /// </summary>
        public bool RaiseDrawingDataChanged
        {
            get { return raiseDrawingDataChanged; }
            set
            {
                if (raiseDrawingDataChanged != value)
                {
                    if (IsRunning)
                    {
                        throw new InvalidOperationException("Cannot change RaiseDrawingDataChanged while SpyderClientManager is running");
                    }
                    raiseDrawingDataChanged = value;
                }
            }
        }
        private bool raiseDrawingDataChanged;

        public bool IsRunning { get; private set; }
        
        protected SpyderClientManagerBase(Func<Task<SpyderServerEventListenerBase>> getServerEventListener, Func<string, Task<ISpyderClientExtended>> getSpyderClient)
        {
            this.getSpyderClient = getSpyderClient;
            this.getServerEventListener = getServerEventListener;
                        
            //Grab current synchronization context
            this.context = SynchronizationContext.Current;                
        }

        public virtual async Task<bool> StartupAsync()
        {
            await ShutdownAsync();
            IsRunning = true;

            serverEventListener = await getServerEventListener();
            if (serverEventListener != null)
            {
                serverEventListener.ServerAnnounceMessageReceived += serverEventListener_ServerAnnounceMessageReceived;
            }
            return true;
        }

        public virtual async Task ShutdownAsync()
        {
            IsRunning = false;

            if (serverEventListener != null)
            {
                serverEventListener.ServerAnnounceMessageReceived -= serverEventListener_ServerAnnounceMessageReceived;
                serverEventListener = null;
            }

            using(await spyderServersLock.LockAsync())
            {
                var tasks = new List<Task>();
                foreach (var bindableClient in spyderServers)
                {
                    bindableClient.DrawingDataReceived -= BindableClient_DrawingDataReceived;
                    tasks.Add(bindableClient.ShutdownAsync());
                }
                await Task.WhenAll(tasks);

                spyderServers.Clear();
                spyderServersInitializing.Clear();
            }
        }

        public async Task<bool> AddDemoServer(SystemData serverData)
        {
            var client = new SpyderDemoClient(serverData);
            var bindableClient = new BindableSpyderClient(client);
            if (!await bindableClient.StartupAsync())
                return false;

            using(await spyderServersLock.LockAsync())
            {
                spyderServers.Add(bindableClient);
            }
            return true;
        }

        public async Task<BindableSpyderClient> GetServerAsync(string serverIP)
        {
            if (string.IsNullOrEmpty(serverIP))
                return null;

            using(await spyderServersLock.LockAsync())
            {
                return spyderServers.FirstOrDefault(s => s.ServerIP == serverIP);
            }
        }

        public async Task<List<BindableSpyderClient>> GetServers()
        {
            using(await spyderServersLock.LockAsync())
            {
                return new List<BindableSpyderClient>(spyderServers);
            }
        }

        /// <summary>
        /// Contains client instances that are being initialized in the background, and are not yet ready to be displayed
        /// </summary>
        private List<string> spyderServersInitializing = new List<string>();

        private async void serverEventListener_ServerAnnounceMessageReceived(object sender, SpyderServerAnnounceInformation serverInfo)
        {
            if (!IsRunning)
                return;
            
            if (await GetServerAsync(serverInfo.Address) == null)
            {
                bool isInitializing;
                using(await spyderServersInitializingLock.LockAsync())
                {
                    isInitializing = spyderServersInitializing.Contains(serverInfo.Address);
                }

                if (!isInitializing)
                {
                    spyderServersInitializing.Add(serverInfo.Address);

                    //Add a new server to our list
                    ISpyderClientExtended client = await getSpyderClient(serverInfo.Address);
                    var spyderClient = client as SpyderClient;
                    if (spyderClient != null)
                    {
                        spyderClient.Version = serverInfo.Version;
                        spyderClient.ServerName = serverInfo.ServerName;
                    }

                    //Startup our client asynchronously
                    var bindableClient = new BindableSpyderClient(client);
                    if (await bindableClient.StartupAsync())
                    {
                        using(await spyderServersLock.LockAsync())
                        {
                            spyderServers.Add(bindableClient);
                        }
                        using(await spyderServersInitializingLock.LockAsync())
                        {
                            spyderServersInitializing.Remove(serverInfo.Address);
                        }

                        if (raiseDrawingDataChanged)
                        {
                            bindableClient.DrawingDataReceived += BindableClient_DrawingDataReceived;
                        }
                        OnServerListChanged(EventArgs.Empty);
                    }
                    else
                    {
                        TraceQueue.Trace(this, TracingLevel.Warning, "Failed to startup BindableSpyderClient for {0}.", serverInfo.Address);
                    }
                }
            }
        }

        private void BindableClient_DrawingDataReceived(object sender, DrawingDataReceivedEventArgs e)
        {
            //Bubble up DrawingData received event
            OnDrawingDataReceived(e);
        }

        private Task<T> InvokeAsync<T>(Func<T> func)
        {
            if (context == null)
                return Task.FromResult(func());

            var tcs = new TaskCompletionSource<T>();
            context.Post((state) =>
                {
                    T result = func();
                    ((TaskCompletionSource<T>)state).TrySetResult(result);
                }, tcs);

            return tcs.Task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

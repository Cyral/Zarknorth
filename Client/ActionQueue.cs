using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    public class QueueItem
    {
        public Action Action { get; set; }
        public TimeSpan Delay { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool UseThread { get; private set; }
        public bool Dispose { get; set; }

        /// <summary>
        /// Creates a new Queueable item
        /// </summary>
        /// <param name="action">The code to be run</param>
        /// <param name="delay">The delay before running</param>
        /// <param name="threaded">If the code should be executed in a new thread</param>
        public QueueItem(Action action, float delay = 0, bool threaded = false, bool dispose = true)
        {
            Action = action;
            Delay = TimeSpan.FromMilliseconds(delay);
            UseThread = threaded;
            Timestamp = DateTime.Now;
            Dispose = true;
        }
        public void Reload()
        {
            Timestamp = DateTime.Now;
        }
    }
    public class ActionQueue
    {
        //Properties
        public bool Enabled { get; set; }

        //Fields
        private List<QueueItem> queue;
        private Thread monitor;

        /// <summary>
        /// Creates a new ActionQueue object for processing actions after time periods
        /// </summary>
        public ActionQueue()
        {
            queue = new List<QueueItem>();
            monitor = new Thread(delegate()
            {
                while (true)
                {
                    if (Enabled)
                        Process();
                    Thread.Sleep(16);
                }
            });
            monitor.IsBackground = true;
            monitor.Start();
            Enabled = true;
        }
        public void Enqueue(QueueItem action)
        {
            queue.Add(action);
        }
        public void Process()
        {
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                QueueItem item = queue[i];
                if (DateTime.Now > item.Timestamp + item.Delay)
                {
                    if (item.UseThread)
                        new Thread(delegate() { item.Action.Invoke(); }).Start();
                    else
                    item.Action.Invoke();
                    if (item.Dispose)
                        queue.RemoveAt(i);
                    else
                        item.Reload();
                }
            }
        }
    }
}
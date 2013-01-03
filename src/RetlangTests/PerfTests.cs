﻿using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Core;
using Retlang.Fibers;

namespace RetlangTests
{
    public class PerfExecutor : IExecutor
    {
        public void Execute(List<Action> toExecute)
        {
            foreach (var action in toExecute)
            {
                action();
            }
            if (toExecute.Count < 10000)
            {
                Thread.Sleep(1);
            }
        }

        public void Execute(Action toExecute)
        {
            toExecute();
        }
    }

    public struct MessageStruct
    {
        public int count;
    }

    [TestFixture]
    public class PerfTests
    {
        [Test, Explicit]
        public void PointToPointPerfTestWithStruct()
        {
            RunBoundedQueue();
        }

        [Test, Explicit]
        public void BusyWaitQueuePointToPointPerfTestWithStruct()
        {
            RunBusyWaitQueue();
        }
      
        private static void RunBoundedQueue()
        {
            var executor = new BoundedQueue(new PerfExecutor()) { MaxDepth = 10000, MaxEnqueueWaitTimeInMs = 1000 };
            using (var fiber = new ThreadFiber(executor))
            {
                fiber.Start();
                var channel = new Channel<MessageStruct>();
                const int max = 5000000;
                var reset = new AutoResetEvent(false);
                Action<MessageStruct> onMessage = delegate(MessageStruct count)
                {
                    if (count.count == max)
                    {
                        reset.Set();
                    }
                };
                channel.Subscribe(fiber, onMessage);
                using (new PerfTimer(max))
                {
                    for (var i = 0; i <= max; i++)
                    {
                        channel.Publish(new MessageStruct { count = i });
                    }
                    Assert.IsTrue(reset.WaitOne(30000, false));
                }
            }
        }

        private static void RunBusyWaitQueue()
        {
            var executor = new BusyWaitQueue(new PerfExecutor(), 100000, 30000);
            using (var fiber = new ThreadFiber(executor))
            {
                fiber.Start();
                var channel = new Channel<MessageStruct>();
                const int max = 5000000;
                var reset = new AutoResetEvent(false);
                Action<MessageStruct> onMessage = delegate(MessageStruct count)
                                              {
                                                  if (count.count == max)
                                                  {
                                                      reset.Set();
                                                  }
                                              };
                channel.Subscribe(fiber, onMessage);
                using (new PerfTimer(max))
                {
                    for (var i = 0; i <= max; i++)
                    {
                        channel.Publish(new MessageStruct { count = i });
                    }
                    Assert.IsTrue(reset.WaitOne(30000, false));
                }
            }
        }

        [Test, Explicit]
        public void PointToPointPerfTestWithInt()
        {
            var executor = new BoundedQueue(new PerfExecutor()) { MaxDepth = 10000, MaxEnqueueWaitTimeInMs = 1000 };
            using (var fiber = new ThreadFiber(executor))
            {
                fiber.Start();
                var channel = new Channel<int>();
                const int max = 5000000;
                var reset = new AutoResetEvent(false);
                Action<int> onMessage = delegate(int count)
                                        {
                                            if (count == max)
                                            {
                                                reset.Set();
                                            }
                                        };
                channel.Subscribe(fiber, onMessage);
                using (new PerfTimer(max))
                {
                    for (var i = 0; i <= max; i++)
                    {
                        channel.Publish(i);
                    }
                    Assert.IsTrue(reset.WaitOne(30000, false));
                }
            }
        }

        [Test, Explicit]
        public void PointToPointPerfTestWithObject()
        {
            var executor = new BoundedQueue(new PerfExecutor()) { MaxDepth = 100000, MaxEnqueueWaitTimeInMs = 1000 };
            using (var fiber = new ThreadFiber(executor))
            {
                fiber.Start();
                var channel = new Channel<object>();
                const int max = 5000000;
                var reset = new AutoResetEvent(false);
                var end = new object();
                Action<object> onMessage = delegate(object message)
                                           {
                                               if (message == end)
                                               {
                                                   reset.Set();
                                               }
                                           };
                channel.Subscribe(fiber, onMessage);
                using (new PerfTimer(max))
                {
                    var message = new object();
                    for (var i = 0; i <= max; i++)
                    {
                        channel.Publish(message);
                    }
                    channel.Publish(end);
                    Assert.IsTrue(reset.WaitOne(30000, false));
                }
            }
        }
    }
}
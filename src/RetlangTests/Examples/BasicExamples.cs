using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;

namespace RetlangTests.Examples
{
    [TestFixture]
    public class BasicExamples
    {
        [Test]
        public void PubSubWithPool()
        {
            //PoolFiber uses the .NET thread pool by default
            using (var fiber = new PoolFiber())
            {
                fiber.Start();
                var channel = new Channel<string>();

                var reset = new AutoResetEvent(false);
                channel.Subscribe(fiber, delegate { reset.Set(); });
                channel.Publish("hello");

                Assert.IsTrue(reset.WaitOne(5000, false));
            }
        }

        [Test]
        public void PubSubWithDedicatedThread()
        {
            using (var fiber = new ThreadFiber())
            {
                fiber.Start();
                var channel = new Channel<string>();

                var reset = new AutoResetEvent(false);
                channel.Subscribe(fiber, delegate { reset.Set(); });
                channel.Publish("hello");

                Assert.IsTrue(reset.WaitOne(5000, false));
            }
        }

        [Test]
        public void PubSubWithDedicatedThreadWithFilter()
        {
            using (var fiber = new ThreadFiber())
            {
                fiber.Start();
                var channel = new Channel<int>();

                var reset = new AutoResetEvent(false);
                Action<int> onMsg = x =>
                {
                    Assert.IsTrue(x % 2 == 0);
                    if (x == 4)
                    {
                        reset.Set();
                    }
                };
                var sub = new Receiver<int>(fiber, onMsg);
                sub.Filter = x => x % 2 == 0;
                channel.Subscribe(sub);
                channel.Publish(1);
                channel.Publish(2);
                channel.Publish(3);
                channel.Publish(4);

                Assert.IsTrue(reset.WaitOne(5000, false));
            }
        }

        [Test]
        public void Batching()
        {
            using (var fiber = new ThreadFiber())
            {
                fiber.Start();
                var counter = new Channel<int>();
                var reset = new ManualResetEvent(false);
                var total = 0;
                Action<IList<int>> cb = delegate(IList<int> batch)
                                            {
                                                total += batch.Count;
                                                if (total == 10)
                                                {
                                                    reset.Set();
                                                }
                                            };

                counter.SubscribeToBatch(fiber, cb, 1);

                for (var i = 0; i < 10; i++)
                {
                    counter.Publish(i);
                }

                Assert.IsTrue(reset.WaitOne(10000, false));
            }
        }

        [Test]
        public void BatchingWithKey()
        {
            using (var fiber = new ThreadFiber())
            {
                fiber.Start();
                var counter = new Channel<int>();
                var reset = new ManualResetEvent(false);
                Action<IDictionary<String, int>> cb = delegate(IDictionary<String, int> batch)
                {
                    if (batch.ContainsKey("9"))
                    {
                        reset.Set();
                    }
                };

                Converter<int, String> keyResolver = x => x.ToString();
                counter.SubscribeToKeyedBatch<int, String>(fiber, cb, keyResolver, 0);

                for (var i = 0; i < 10; i++)
                {
                    counter.Publish(i);
                }

                Assert.IsTrue(reset.WaitOne(10000, false));
            }
        }

        [Test]
        public void RequestReply()
        {
            using (var fiber = new PoolFiber())
            {
                fiber.Start();
                var channel = new RequestReplyChannel<string, string>();
                channel.Subscribe(fiber, req => req.SendReply("bye"));
                var reply = channel.SendRequest("hello");

                string result;
                Assert.IsTrue(reply.Receive(10000, out result));
                Assert.AreEqual("bye", result);
            }
        }

        [Test]
        public void ShouldIncreasePoolFiberSubscriberCountByOne()
        {
            var fiber = new PoolFiber();
            fiber.Start();
            var channel = new Channel<int>();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
            channel.Subscribe(fiber, x => { });

            Assert.AreEqual(1, fiber.SubscriptionsCount);
            Assert.AreEqual(1, channel.SubscribersCount);
            fiber.Dispose();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
        }

        [Test]
        public void ShouldIncreaseThreadFiberSubscriberCountByOne()
        {
            var fiber = new ThreadFiber();
            fiber.Start();
            var channel = new Channel<int>();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
            channel.Subscribe(fiber, x => { });

            Assert.AreEqual(1, fiber.SubscriptionsCount);
            Assert.AreEqual(1, channel.SubscribersCount);
            fiber.Dispose();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
        }

        [Test]
        public void ShouldIncreaseStubFiberSubscriberCountByOne()
        {
            var fiber = new StubFiber();
            fiber.Start();
            var channel = new Channel<int>();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
            channel.Subscribe(fiber, x => { });

            Assert.AreEqual(1, fiber.SubscriptionsCount);
            Assert.AreEqual(1, channel.SubscribersCount);
            fiber.Dispose();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
        }

        [Test]
        public void UnsubscriptionShouldRemoveSubscriber()
        {
            var fiber = new PoolFiber();
            fiber.Start();
            var channel = new Channel<int>();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
            var unsubscriber = channel.Subscribe(fiber, x => { });

            Assert.AreEqual(1, fiber.SubscriptionsCount);
            Assert.AreEqual(1, channel.SubscribersCount);
            unsubscriber.Dispose();

            Assert.AreEqual(0, fiber.SubscriptionsCount);
            Assert.AreEqual(0, channel.SubscribersCount);
        }
    }
}

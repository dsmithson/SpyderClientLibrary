using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Diagnostics
{
    [TestClass]
    public class TraceQueueTests
    {
        private static List<TraceMessage> messagesLogged;
        private static bool traceLevelChanged;

        [ClassInitialize]
        public static void TestClassInitialize(TestContext state)
        {
            TraceQueue.TraceMessageRaised += OnTrace;
            TraceQueue.TracingLevelChanged += OnLevelChanged;
        }

        [ClassCleanup]
        public static void TestClassCleanup()
        {
            TraceQueue.TraceMessageRaised -= OnTrace;
            TraceQueue.TracingLevelChanged -= OnLevelChanged;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            messagesLogged = new List<TraceMessage>();
            traceLevelChanged = false;
        }

        static void OnTrace(TraceMessage message)
        {
            messagesLogged?.Add(message);
        }

        static void OnLevelChanged(TracingLevel tracingLevel)
        {
            traceLevelChanged = true;
        }

        [TestMethod]
        public async Task TraceTest()
        {
            //Log all messages
            TraceQueue.TracingLevel = TracingLevel.Success;

            //Iterate all levels
            const string testMessage = "Test Message";
            foreach (TracingLevel level in Enum.GetValues(typeof(TracingLevel)))
            {
                messagesLogged.Clear();

                TraceQueue.Trace(level, testMessage);

                //Long, and possibly unnecessary wait for our messaging thread to process this message
                await Task.Delay(100);

                Assert.IsTrue(messagesLogged.Count > 0, "No message logged");
                Assert.AreEqual(messagesLogged[0].Message, testMessage, "Message incorrect");
                Assert.AreEqual(messagesLogged[0].Level, level, "Level incorrect");
            }
        }

        [TestMethod]
        public async Task TraceFilterTest()
        {
            //Log all messages
            TraceQueue.TracingLevel = TracingLevel.Success;

            //Send a success message
            TraceQueue.Trace(TracingLevel.Success, "Success!");
            await Task.Delay(100);
            Assert.AreEqual(1, messagesLogged.Count, "No message was logged");

            //Set tracing level to warnings and try to re-send our success message
            messagesLogged.Clear();
            TraceQueue.TracingLevel = TracingLevel.Warning;

            TraceQueue.Trace(TracingLevel.Success, "Success!");
            await Task.Delay(100);
            Assert.AreEqual(0, messagesLogged.Count, "No message should have been propagated");
        }

        [TestMethod]
        public void TraceLevelChangeTest()
        {
            TraceQueue.TracingLevel = TracingLevel.Warning;
            traceLevelChanged = false;

            //Change the level
            TraceQueue.TracingLevel = TracingLevel.Success;
            Assert.IsTrue(traceLevelChanged, "TracingLevelChanged event failed to fire");
        }
    }
}

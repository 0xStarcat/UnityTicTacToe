using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ComputerPlayerTests
    {
        ComputerPlayer computerPlayer;
        // A Test behaves as an ordinary method
        [SetUp]
        public void Setup()
        {
            computerPlayer = new ComputerPlayer();
        }

        [Test]
        public void ComputerPlayerTestsSimplePasses()
        {
            Assert.True(computerPlayer.Test());
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ComputerPlayerTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}

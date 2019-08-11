using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ComputerPlayerTests
    {
        private ComputerPlayer computerPlayer;
        private GameController gameController;
        private GameObject gameGameObject;
        // A Test behaves as an ordinary method
        
        [SetUp]
        public void Setup()
        {
            gameGameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
            computerPlayer = gameGameObject.GetComponentInChildren<ComputerPlayer>();
            gameController = gameGameObject.GetComponentInChildren<GameController>();
            
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(gameGameObject);
        }

        private void SetupHumanVsComputerGame()
        {
            gameController.SetHumanSide("X");
        }

        [Test]
        public void ComputerPlayerTestsSimplePasses()
        {
            Assert.True(computerPlayer.Test());
            // Use the Assert class to test conditions
        }

        [Test]
        public void CalculateMoveScore1()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |
            Assert.AreEqual(3, computerPlayer.CalculateMoveScore(gameController.buttonList[0].GetComponent<GridSpace>()));
        }

        [Test]
        public void CalculateMoveScore2()
        {
            SetupHumanVsComputerGame();
            // X | |
            //   | |
            //   | |

            gameController.buttonList[0].GetComponent<GridSpace>().SetSpace();

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

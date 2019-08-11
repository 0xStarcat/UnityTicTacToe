using System.Linq;
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
        public void EvaluateMove1a()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //   | |
            //   |X|
            //   | |
            var space = gameController.buttonList[4].GetComponent<GridSpace>();
            Assert.AreEqual(4, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space), space, "X").Sum(el => el.Score));

        }

        [Test]
        public void EvaluateMove1b()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //  X| |
            //   | |
            //   | |
            var space = gameController.buttonList[0].GetComponent<GridSpace>();
            Assert.AreEqual(3, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space), space, "X").Sum(el => el.Score));
        }


        [Test]
        public void EvaluateMove1c()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //   | |
            //  X| |
            //   | |
            var space = gameController.buttonList[3].GetComponent<GridSpace>();
            Assert.AreEqual(2, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space), space, "X").Sum(el => el.Score));

        }

        [Test]
        public void EvaluateMove2a()
        {
            SetupHumanVsComputerGame();
            gameController.SetSpaceAtIndex(0, "X");
            gameController.SetSpaceAtIndex(4, "O");
            gameController.SetSpaceAtIndex(8, "X");
            gameController.SetSpaceAtIndex(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            //  X| |O
            //  X|O|
            //   | |X

            // Check move
            var space = gameController.buttonList[3].GetComponent<GridSpace>();
            Assert.AreEqual(18, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space), space, "X").Sum(el => el.Score));

        }

        [Test]
        public void EvaluateMove2b()
        {
            SetupHumanVsComputerGame();
            gameController.SetSpaceAtIndex(0, "X");
            gameController.SetSpaceAtIndex(4, "O");
            gameController.SetSpaceAtIndex(8, "X");
            gameController.SetSpaceAtIndex(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            //  X| |O
            //   |O|
            //  X| |X

            // 63 = 18 * 2 (2 checks) + 27 (1 prevent loss)
            var space = gameController.buttonList[6].GetComponent<GridSpace>();
            var moveEval = computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space), space, "X");
            Assert.AreEqual(63, moveEval.Sum(el => el.Score));

        }

        [Test]
        public void CalculateDecisionScore1a()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //   | |
            //   |X|
            //   | |
            var space = gameController.buttonList[4].GetComponent<GridSpace>();
            Assert.AreEqual(-216, computerPlayer.CalculateDecisionScore(space, "X"));

        }

        [Test]
        public void CalculateDecisionScore1b()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //  X| |
            //   | |
            //   | |
            var space = gameController.buttonList[0].GetComponent<GridSpace>();
            Assert.AreEqual(-218, computerPlayer.CalculateDecisionScore(space, "X"));
        }


        [Test]
        public void CalculateDecisionScore1c()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //   | |
            //  X| |
            //   | |
            var space = gameController.buttonList[3].GetComponent<GridSpace>();
            Assert.AreEqual(-220, computerPlayer.CalculateDecisionScore(space, "X"));

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

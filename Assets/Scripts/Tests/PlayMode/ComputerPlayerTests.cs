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
            Assert.AreEqual(4, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space, "X"), space, "X"));

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
            Assert.AreEqual(3, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space, "X"), space, "X"));
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
            Assert.AreEqual(2, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space, "X"), space, "X"));

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

            // Loss move
            var space = gameController.buttonList[3].GetComponent<GridSpace>();
            Assert.AreEqual(0, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space, "X"), space, "X"));

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
            var moveEval = computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(space, "X"), space, "X");
            Assert.AreEqual(63, moveEval);

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
            Assert.AreEqual(-314, computerPlayer.CalculateDecisionScore(space, "X"));

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
            Assert.AreEqual(-398, computerPlayer.CalculateDecisionScore(space, "X"));
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
            Assert.AreEqual(-434, computerPlayer.CalculateDecisionScore(space, "X"));

        }

        [Test]
        public void CalculateDecisionScore1Alt()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |
            
            // 4
            //   | |
            //   |X|
            //   | |
            
            // 0
            //  X| |
            //   | |
            //   | |

            // 3
            //   | |
            //  X| |
            //   | |

            var space1 = gameController.buttonList[4].GetComponent<GridSpace>();
            var space2 = gameController.buttonList[0].GetComponent<GridSpace>();
            var space3 = gameController.buttonList[3].GetComponent<GridSpace>();
            Assert.Greater(computerPlayer.CalculateDecisionScore(space1, "X"), computerPlayer.CalculateDecisionScore(space2, "X"));
            Assert.Greater(computerPlayer.CalculateDecisionScore(space2, "X"), computerPlayer.CalculateDecisionScore(space3, "X"));

        }

        [Test]
        public void CalculateDecisionScore2a()
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
            Assert.AreEqual(-189, computerPlayer.CalculateDecisionScore(space, "X"));

        }

        [Test]
        public void CalculateDecisionScore2b()
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

            var space = gameController.buttonList[6].GetComponent<GridSpace>();
            Assert.AreEqual(-126, computerPlayer.CalculateDecisionScore(space, "X"));

        }

        [Test]
        public void CalculateDecisionScore2Alt()
        {
            SetupHumanVsComputerGame();
            gameController.SetSpaceAtIndex(0, "X");
            gameController.SetSpaceAtIndex(4, "O");
            gameController.SetSpaceAtIndex(8, "X");
            gameController.SetSpaceAtIndex(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            // 6
            //  X| |O
            //   |O|
            //  X| |X

            // vs

            // 3
            //  X| |O
            //  X|O|
            //   | |X

            var space = gameController.buttonList[6].GetComponent<GridSpace>();
            var cornerMove = computerPlayer.CalculateDecisionScore(space, "X");
            var space2 = gameController.buttonList[3].GetComponent<GridSpace>();
            var sideMove = computerPlayer.CalculateDecisionScore(space2, "X");

            Assert.That(cornerMove, Is.GreaterThan(sideMove));

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

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
        public void EvaluateMove1a()
        {
            SetupHumanVsComputerGame();
            //   | |
            //   | |
            //   | |

            //   | |
            //   |X|
            //   | |
            Assert.AreEqual(4, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(4, "X", gameController.gameBoard.Spaces), 4, "X"));

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
            Assert.AreEqual(3, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(0, "X", gameController.gameBoard.Spaces), 0, "X"));
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
            Assert.AreEqual(2, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(3, "X", gameController.gameBoard.Spaces), 3, "X"));

        }

        [Test]
        public void EvaluateMove2a()
        {
            SetupHumanVsComputerGame();
            gameController.MakeMove(0, "X");
            gameController.MakeMove(4, "O");
            gameController.MakeMove(8, "X");
            gameController.MakeMove(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            //  X| |O
            //  X|O|
            //   | |X

            // Loss move
            Assert.AreEqual(0, computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(3, "X", gameController.gameBoard.Spaces), 3, "X"));

        }

        [Test]
        public void EvaluateMove2b()
        {
            SetupHumanVsComputerGame();
            gameController.MakeMove(0, "X");
            gameController.MakeMove(4, "O");
            gameController.MakeMove(8, "X");
            gameController.MakeMove(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            //  X| |O
            //   |O|
            //  X| |X

            // 63 = 18 * 2 (2 checks) + 27 (1 prevent loss)
            var moveEval = computerPlayer.EvaluateMove(computerPlayer.CreateMoveModel(6, "X", gameController.gameBoard.Spaces), 6, "X");
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
            Assert.AreEqual(-8, computerPlayer.CalculateDecisionScore(4, "X"));

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
            Assert.AreEqual(-12, computerPlayer.CalculateDecisionScore(0, "X"));
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
            Assert.AreEqual(-16, computerPlayer.CalculateDecisionScore(3, "X"));

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
            var center = computerPlayer.CalculateDecisionScore(4, "X");
            var corner = computerPlayer.CalculateDecisionScore(0, "X");
            var side = computerPlayer.CalculateDecisionScore(3, "X");

            Assert.Greater(center, corner);
            Assert.Greater(corner, side);

        }

        [Test]
        public void CalculateDecisionScore2a()
        {
            SetupHumanVsComputerGame();
            gameController.MakeMove(0, "X");
            gameController.MakeMove(4, "O");
            gameController.MakeMove(8, "X");
            gameController.MakeMove(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            //  X| |O
            //  X|O|
            //   | |X

            // Loss
            Assert.AreEqual(-108, computerPlayer.CalculateDecisionScore(3, "X"));

        }

        [Test]
        public void CalculateDecisionScore2b()
        {
            SetupHumanVsComputerGame();
            gameController.MakeMove(0, "X");
            gameController.MakeMove(4, "O");
            gameController.MakeMove(8, "X");
            gameController.MakeMove(2, "O");

            //  X| |O
            //   |O|
            //   | |X

            //  X| |O
            //   |O|
            //  X| |X

            Assert.AreEqual(63, computerPlayer.CalculateDecisionScore(6, "X"));

        }

        [Test]
        public void CalculateDecisionScore2Alt()
        {
            SetupHumanVsComputerGame();
            gameController.MakeMove(0, "X");
            gameController.MakeMove(4, "O");
            gameController.MakeMove(8, "X");
            gameController.MakeMove(2, "O");

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

            var cornerMove = computerPlayer.CalculateDecisionScore(6, "X");
            var sideMove = computerPlayer.CalculateDecisionScore(3, "X");

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

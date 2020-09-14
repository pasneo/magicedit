using System;
using System.Collections.Generic;
using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void TestSquareAttributeRestrictions()
        {

            Map map = new Map(3, 2);

            map.AddSpawner(new Position(0, 0));
            map.AddSpawner(new Position(1, 0));
            map.AddSpawner(new Position(2, 0));

            SquareType fireland = new SquareType();
            fireland.AllowedAttributes.Add("firey");
            fireland.ForbiddenAttributes.Add("holy");
            
            map.AddSquare(new Square { Type = fireland, Position = new Position(0, 1) });
            map.AddSquare(new Square { Type = fireland, Position = new Position(1, 1) });
            map.AddSquare(new Square { Type = fireland, Position = new Position(2, 1) });

            Character normal_beast = new Character("normal_beast", "normal_beast");
            Character firey_beast = new Character("firey_beast", "firey_beast");
            Character holy_firey_beast = new Character("holy_firey_beast", "holy_firey_beast");

            firey_beast.SetAttribute("firey");
            holy_firey_beast.SetAttribute("firey");
            holy_firey_beast.SetAttribute("holy");

            Config config = new Config();
            config.Map = map;

            Game game = new Game(config);

            List<Character> characters = new List<Character>(){ normal_beast, firey_beast, holy_firey_beast};
            game.SetupPlayers(characters);

            game.Start();

            try
            {
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.South);
                Assert.Fail();
            } catch (GameException ge)
            {
                Assert.AreEqual("This character is not allowed to step on this square", ge.Message);
            }

            game.DoAction(Game.BasicActions.EndTurn);

            try
            {
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.South);
            }
            catch (GameException)
            {
                Assert.Fail();
            }

            game.DoAction(Game.BasicActions.EndTurn);

            try
            {
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.South);
                Assert.Fail();
            }
            catch (GameException ge)
            {
                Assert.AreEqual("This character is not allowed to step on this square", ge.Message);
            }

        }

        [TestMethod]
        public void TestSquareActions()
        {

            Scheme mapScheme = new Scheme("map");
            mapScheme.Code = @"scheme map {
                                    action Step ( 0 ) {
                                        rick_rolled of actor = true
                                    }
                               }";

            Map map = new Map(2, 1);
            map.Scheme = mapScheme;

            map.AddSpawner(new Position(0, 0));

            SquareType squareType = new SquareType();
            squareType.ActionName = "Step";
            map.AddSquare(new Square { Type = squareType, Position = new Position(1, 0) });

            Config config = new Config();
            config.Map = map;

            Game game = new Game(config);

            Character ch = new Character("ch", "ch");
            ch.Scheme = new Scheme("ch_scheme");
            ch.Variables.Add(new ObjectVariable(VariableTypes.Logical, "rick_rolled", false));

            List<Character> characters = new List<Character>() { ch };
            game.SetupPlayers(characters);

            game.Start();

            game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.East);

            Assert.AreEqual(true, ch.GetVariableByName("rick_rolled", config).Value);

        }

    }
}

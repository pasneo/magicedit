﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Object
    {
        
        public string Id { get; set; }      //A unique identifier
        public string Name { get; set; }    //The name the creator gave in the editor

        public Scheme Scheme { get; set; }
        public Visual Visual { get; set; }

        public Text Description { get; set; }   //Current description (can be changed with scheme execution)

        public List<ObjectVariable> Variables = new List<ObjectVariable>();
        public List<ObjectAttribute> Attributes = new List<ObjectAttribute>();
        public List<ObjectVariable> Parameters = new List<ObjectVariable>();

        /* *** */

        public void Create(Game game)
        {
            //Compile scheme and call its constructor
            Scheme.Construct(this, Parameters);
            Scheme.Init(this, game);
        }

        public int ExecuteAction(string actionName, Object actor, Game game)
        {
            SchemeFunction action = Scheme.GetFunctionByName(actionName);

            if (action == null) throw new GameException("Given action does not exist.");

            return action.Execute(this, actor, game);
        }

        public ObjectVariable GetVariableByName(string name)
        {
            foreach(var variable in Variables)
            {
                if (variable.Name == name) return variable;
            }
            return null;
        }

    }
}

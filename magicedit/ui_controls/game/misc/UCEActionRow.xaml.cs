using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace magicedit
{

    public abstract class ActionPlan
    {
        protected Game Game { get; set; }
        protected Character Actor { get; set; }

        public ActionPlan(Game game, Character actor)
        {
            Game = game;
            Actor = actor;
        }

        protected abstract void ExecutePlan();
        
        public void Execute()
        {
            try
            {
                ExecutePlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ObjectActionPlan : ActionPlan
    {
        protected string TargetId { get; set; }
        protected string Action { get; set; }

        public ObjectActionPlan(Game game, Character actor, Object target, string action) : base(game, actor)
        {
            TargetId = target.Id;
            Action = action;
        }

        protected override void ExecutePlan()
        {
            Game.SelectObject(TargetId);
            Game.DoAction(Action);
        }
    }

    public class MoveActionPlan : ActionPlan
    {
        string Param { get; set; }

        public MoveActionPlan(Game game, Character actor, Position position) : base(game, actor)
        {
            Position diff = (actor.Position - position);
            if (diff.Equals(new Position(0, 1))) Param = Game.MovementParameters.Norht;
            else if (diff.Equals(new Position(0, -1))) Param = Game.MovementParameters.South;
            else if (diff.Equals(new Position(1, 0))) Param = Game.MovementParameters.West;
            else if (diff.Equals(new Position(-1, 0))) Param = Game.MovementParameters.East;
            else throw new GameException("Given position is not a valid target");
        }

        protected override void ExecutePlan()
        {
            Game.SelectObject(null);
            Game.DoAction(Game.BasicActions.Movement, Param);
        }
    }

    public class EndTurnActionPlan : ActionPlan
    {
        public EndTurnActionPlan(Game game, Character actor) : base(game, actor) { }
        protected override void ExecutePlan()
        {
            Game.SelectObject(null);
            Game.DoAction(Game.BasicActions.EndTurn);
        }
    }

    /// <summary>
    /// Interaction logic for UCEActionRow.xaml
    /// </summary>
    public partial class UCEActionRow : UserControl
    {
        ActionPlan ActionPlan { get; set; }

        public UCEActionRow()
        {
            InitializeComponent();
        }

        public UCEActionRow(string action, int actionPoint, bool available, ActionPlan actionPlan)
        {
            InitializeComponent();

            tbAction.Text = action;
            tbActionPoint.Text = actionPoint.ToString();

            if (!available)
            {
                tbAction.Foreground = tbActionPoint.Foreground = Brushes.LightGray;
            }

            ActionPlan = actionPlan;
        }

        public void Execute()
        {
            ActionPlan?.Execute();
        }

    }
}

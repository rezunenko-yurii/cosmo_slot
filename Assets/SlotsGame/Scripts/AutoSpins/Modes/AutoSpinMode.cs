using Zenject;

namespace SlotsGame.Scripts.AutoSpins.Modes
{
    public abstract class AutoSpinMode
    {
        protected AutoSpin Context;
        public abstract AutoSpinType Type { get; protected set; }
        public virtual bool CanChangeState()
        {
            return true;
        }

        public virtual void Merge(int amount) { }
        
        public virtual bool CanMerge()
        {
            return false;
        }

        public virtual void SetContext(AutoSpin context)
        {
            Context = context;
        }

        public virtual void OnContextChanged() { }
    }

    public class Off : AutoSpinMode
    {
        public override AutoSpinType Type { get; protected set; } = AutoSpinType.Off;
    }

    public class Infinity : AutoSpinMode
    {
        public override AutoSpinType Type { get; protected set; } = AutoSpinType.Infinity;
    }
    
    public class ForciblyAmount : AutoSpinMode
    {
        [Inject] private SignalBus _signalBus;
        
        private int _count;
        public override AutoSpinType Type { get; protected set; } = AutoSpinType.ForcedAmount;

        ~ForciblyAmount()
        {
            _signalBus.TryUnsubscribe<Game.Signal.EffectsEnded>(DecreaseCount);
        }
        
        public override void SetContext(AutoSpin context)
        {
            base.SetContext(context);
            _signalBus.Subscribe<Game.Signal.EffectsEnded>(DecreaseCount);
        }

        public override void OnContextChanged()
        {
            base.OnContextChanged();
            _signalBus.TryUnsubscribe<Game.Signal.EffectsEnded>(DecreaseCount);
        }

        public override bool CanChangeState()
        {
            return _count == 0;
        }
        
        public override bool CanMerge()
        {
            return true;
        }
        
        public override void Merge(int autoSpinsCount)
        {
            _count += autoSpinsCount;
        }
        
        private void DecreaseCount()
        {
            if (_count == 0)
            {
                _signalBus.Unsubscribe<Game.Signal.EffectsEnded>(DecreaseCount);
                Context.TransitionTo(AutoSpinType.Off);
            }
            
            _count--;
        }
    }
    
}
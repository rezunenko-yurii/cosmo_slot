using Core.Buttons;
using Core.Timers;
using UnityEngine;
using Zenject;

namespace WheelLib
{
    public class BaseWheelButton : NormalButton
    {
        [Inject(Id = ModuleType.Wheel)] private MemoryTimer timer;
        [SerializeField] protected Wheels wheels;
        [SerializeField] private bool hideOnExpire = true;
        
        protected override void CheckAvailability()
        {
            ChangeInteractableState(timer.IsExpired);

            if (hideOnExpire)
            {
                Button.gameObject.SetActive(timer.IsExpired);
            }
        }
        
        protected override void AddListeners()
        {
            base.AddListeners();
            
            timer.Over += CheckAvailability;
            timer.Started += SetNoClickable;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            
            timer.Over -= CheckAvailability;
            timer.Started -= SetNoClickable;
        }
    }
}
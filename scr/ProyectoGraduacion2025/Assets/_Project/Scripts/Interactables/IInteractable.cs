using UnityEngine;

namespace Project.Interactables
{
    
    public interface IInteractable
    {
        
        void Activate(GameObject activator = null);

        
        void Deactivate();

        
        bool IsActive { get; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void ProcesarInteraccion();
    
    void MostrarMensaje(string texto, bool temporal = false);
    
    void ActivarInteraccion();
    
    void DesactivarInteraccion();
    
    bool PuedeInteractuar();
    
    event InteractableObject.InteraccionEventHandler OnInteraccionDisponible;
    
    event InteractableObject.InteraccionEventHandler OnInteraccionCompletada;
}

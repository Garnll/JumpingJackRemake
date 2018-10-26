using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable {

    void ReceiveInput(float horizontal);
    void ReceiveInput(bool input);
}

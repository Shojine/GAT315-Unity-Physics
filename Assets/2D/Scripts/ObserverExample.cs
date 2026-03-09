using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/**
* ObserverExample - Demonstrates how observers work in Unity/C#
*/
public class ObserverExample : MonoBehaviour
{
	// Define two delegate types with different signatures
	//public delegate void DelegateFunction();          // Delegate type that takes no parameters and returns void
	//public delegate void IntDelegateFunction(int value);  // Delegate type that takes an int parameter and returns void

	// Create delegate variables (these will hold references to methods)
	public UnityAction onFunctionStart;       // Will hold references to methods with no parameters
	public UnityAction<int> onFunctionStop;     // Will hold references to methods that accept an int parameter


	void Start()
	{
		// Subscribe methods to delegates using += operator (multicast delegates)
		onFunctionStart += FunctionStart1;   // Add FunctionStart1 to onFunctionStart delegate chain
		onFunctionStart += FunctionStart2;   // Add FunctionStart2 to onFunctionStart delegate chain
		onFunctionStop = FunctionStop;      // Add FunctionStop to onFunctionStop delegate
	}

	void Update()
	{
		// When space key is pressed down, invoke the onFunctionStart delegate
		if (Keyboard.current.zKey.wasPressedThisFrame)
		{
			// This will call ALL methods subscribed to onFunctionStart (FunctionStart1 and FunctionStart2)
			onFunctionStart.Invoke();
		}

		// When space key is released, invoke the onFunctionStop delegate with parameter 5
		if (Keyboard.current.zKey.wasReleasedThisFrame)
		{
			// This will call FunctionStop with the value 5
			onFunctionStop.Invoke(5);
		}
	}

	// Methods that match the delegate signatures
	void FunctionStart1()
	{
		print("start1");  // Will be called when onFunctionStart is invoked
	}

	void FunctionStart2()
	{
		print("start2");  // Will also be called when onFunctionStart is invoked (multicast)
	}

	void FunctionStop(int value)
	{
		print("stop " + value);  // Will be called when onFunctionStop is invoked, receiving the passed value
	}
}
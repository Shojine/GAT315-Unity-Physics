using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
	public void OnAnimEvent()
	{
		print("animation event");
	}

	public void OnAnimEventString(string str)
	{
		print("animation event: " + str);
	}

	public void OnAnimEventInt(int v)
	{
		print("animation event: " + v);
	}

	public void OnAnimEventFloat(float f)
	{
		print("animation event: " + f);
	}

	public void OnAnimEventGameObect(GameObject go)
	{
		print("animation event: " + go.name);
	}

	public void OnAnimationEvent(AnimationEvent animationEvent)
	{
		print("animation event: " + animationEvent);
	}
}

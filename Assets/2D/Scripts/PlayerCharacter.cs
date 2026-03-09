using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	[Header("Player")]
	[SerializeField] CharacterController2D characterController;
	[SerializeField] AnimationEventRouter animationEventRouter;
	[SerializeField] GameObject meleeWeaponL;
	[SerializeField] GameObject meleeWeaponR;

	private void Awake()
	{
		animationEventRouter.AddListener("MeleeAttack", OnMeleeAttack);
	}

	void OnMeleeAttack(AnimationEvent animationEvent)
	{
		if (characterController.Facing == CharacterController2D.FACE_LEFT) meleeWeaponL.SetActive((animationEvent.intParameter == 1));
		else if (characterController.Facing == CharacterController2D.FACE_RIGHT) meleeWeaponR.SetActive((animationEvent.intParameter == 1));
	}
}
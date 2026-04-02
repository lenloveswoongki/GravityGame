using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour, IInteractible
{
    public InteractionType interactionType => InteractionType.CharacterCollision;

#if UNITY_EDITOR
    [SerializeField] private SceneAsset TargetLevel;
#endif
    private string targetLevelName;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(TargetLevel != null)
        {
            targetLevelName = TargetLevel.name;
        }
    }
#endif
    public void Interact(Character triggerReference)
    {
        if(triggerReference.IsPlayer())
        {
            StartCoroutine(FinishLevel());
        }
    }

    private IEnumerator FinishLevel()
    {
        yield return null;
        GameManager.Instance.LoadLevel(targetLevelName);
    }
}

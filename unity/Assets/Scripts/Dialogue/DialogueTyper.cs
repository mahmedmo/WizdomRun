using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class DialogueTyper : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    // Time between each character appearing
    public float typingSpeed = 0.05f;

    public void DisplayDialogue(string dialogue, Action onFinished)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(dialogue, onFinished));
    }

    private IEnumerator TypeText(string dialogue, Action onFinished)
    {
        dialogueText.text = "";
        int index = 0;

        // Use a loop that adds one character at a time
        while (index < dialogue.Length)
        {
            // Check for rich text tags: if the current character starts a tag,
            // skip waiting for the tag content.
            if (dialogue[index] == '<')
            {
                // Find the end of the tag
                int closingIndex = dialogue.IndexOf('>', index);
                if (closingIndex != -1)
                {
                    // Append the entire tag at once
                    dialogueText.text += dialogue.Substring(index, closingIndex - index + 1);
                    index = closingIndex + 1;
                    continue;
                }
            }

            // Append one character and then wait.
            dialogueText.text += dialogue[index];
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }
        if (onFinished != null)
        {
            onFinished.Invoke();
        }
    }
}
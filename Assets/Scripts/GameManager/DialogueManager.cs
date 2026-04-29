using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;


public class DialogueManager : MonoBehaviour
{
    private class Dialogue
    {
        public Dialogue(string speaker, string content, float delay, bool appendDialogue)
        {
            this.speaker = speaker;
            this.content = content;
            this.delay = delay;
            this.appendDialogue = appendDialogue;
        }

        public string speaker = "";
        public string content = "";
        public float delay = 0f;
        public bool appendDialogue = false; 

        public string GetCleanContent()
        {
            string result = "";
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (c == '^' && !(i - 1 >= 0 && content[i - 1] == '\\'))
                {
                    int j = i + 1;
                    while (j < content.Length && char.IsDigit(content[j]))
                    {
                        j++;
                    }
                    i = j - 1;
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }
    }

    private PlayerUIController _playerUI => PlayerUIController.Instance;

    [SerializeField] private float _writerSpeed = 1f;
    private Queue<Dialogue> _nextDialogueList = new();
    private Coroutine _currentWriter;
    private Coroutine currentWriter
    {
        get => _currentWriter;
        set
        {
            _currentWriter = value;
            if (_currentWriter == null) { NextDialogue(); }
        }
    }
    private string _writedText;

    void Start()
    {
        CreatePlaceholder();
        CreatePlaceholder();
    }

    void Update()
    {
        if (InputSystem.actions.FindAction("Interact").WasPressedThisFrame())
        {
            SkipCurrentDialogue();
        }
    }

    public void CreateDialogue(string speaker, string content, float delay = 2f)
    {
        _nextDialogueList.Enqueue(new(speaker, content, delay, false));
        NextDialogue();
    }
    
    public void AppendDialogue(string speaker, string content, float delay = 2f)
    {
        _nextDialogueList.Enqueue(new(speaker, content, delay, true));
        NextDialogue();
    }

    private void NextDialogue()
    {
        if (currentWriter != null) return;
        StopAllCoroutines();

        if (_nextDialogueList.Count > 0)
        {
            _playerUI.EnableTextbox();
            currentWriter = StartCoroutine(WriteDialogue(_nextDialogueList.Dequeue()));
        }
        else
        {
            _playerUI.DisableTextbox();
        }
    }

    private IEnumerator WriteDialogue(Dialogue dialogue)
    {
        // TODO: Menyesuaikan logic untuk append dialog
        Coroutine _revealCoroutine = StartCoroutine(RevealCurrentDialogue(dialogue));
        _writedText = "<b>" + dialogue.speaker + ":</b> ";
        
        foreach (char c in dialogue.content)
        {
            _writedText += c;
            _playerUI.SetTextboxContent(_writedText);
            yield return new WaitForSeconds(0.2f / _writerSpeed);
        }
        StopCoroutine(_revealCoroutine);
        StartCoroutine(SkipDelay());

        yield return new WaitForSeconds(dialogue.delay);
        currentWriter = null;
    }

    IEnumerator RevealCurrentDialogue(Dialogue dialogue)
    {
        if (currentWriter == null)
        {
            yield return null;
            yield return new WaitUntil(() => InputSystem.actions.FindAction("Exit").WasPressedThisFrame());
            StopCoroutine(currentWriter);
            yield return null;

            string _writedText = "<b>" + dialogue.speaker + ":</b> " + dialogue.GetCleanContent();
            _playerUI.SetTextboxContent(_writedText);

            StartCoroutine(SkipDelay());
            yield return new WaitForSeconds(dialogue.delay);
            currentWriter = null;
        }
    }

    IEnumerator SkipDelay()
    {
        yield return null;
        yield return new WaitUntil(() => InputSystem.actions.FindAction("Exit").WasPressedThisFrame());
        currentWriter = null;
        yield return null;
    }

    void SkipCurrentDialogue()
    {
        if (currentWriter == null) return;

        StopCoroutine(currentWriter);
        currentWriter = null;
    }

    void SkipAllDialogues()
    {
        _nextDialogueList.Clear();
        SkipCurrentDialogue();
    }

    private void CreatePlaceholder()
    {
        CreateDialogue
        (
            "Lorem Ipsum",
            "\"Dolor sit amet, consectetur adipiscing elit. Donec euismod commodo risus et ullamcorper. In sit amet justo eget eros imperdiet blandit eu a ipsum. Vivamus nunc tellus, auctor vel nibh et, efficitur molestie felis.\""
        );
    }

    private static DialogueManager s_instance;
    public static DialogueManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<DialogueManager>();
            }
            return s_instance;
        }
    }
}

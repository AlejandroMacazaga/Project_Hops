using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.EventBus;
using Utils.Singletons;

namespace Utils.DialogueSystem
{
    public class DialogueController : RegulatorSingleton<DialogueController>
    {
        [SerializeField] private GameObject textBox;
        private DialogueData _dialogues;
        [SerializeField] private GameObject character1;
        [SerializeField] private GameObject character2;
        [SerializeField] private GameObject canvas;
        
        private string _text = "";
        private string _textId = "";
        // The text ids work like x.x.x.x.x.n with the n in order, this variable need to change depending on the conversation
        
        private char[] _characters;
        
        private int _index;
        private int _currentDialogue = 1;
        public int lang = 5;
        
        private TextMeshProUGUI _textMeshProUGUI;
        private Image _imageChar1;
        private Image _imageChar2;
        
        private bool _conversationEnd; // Controls when the conversation ends
        private bool _textFound; // Controls when new text is found
        private bool _finished = true; // Controls when the text finish to appear in the screen
        private bool _currentPos;
        
        private void Start()
        {
            _textMeshProUGUI = textBox.GetComponent<TextMeshProUGUI>();
            _imageChar1 = character1.GetComponent<Image>();
            _imageChar2 = character2.GetComponent<Image>();
            EventBus<InitiateConversationEvent>.Register(new EventBinding<InitiateConversationEvent>(InitiateConversation));
        }

        private void InitiateConversation(InitiateConversationEvent e)
        {
            canvas.SetActive(true);
            _dialogues = e.dialogueData;
            _textId = e.textId;
            Next();
        }

        private void ConvertTextToCharacters()
        {
            _characters = _text.ToCharArray();
        }
        
        public void Next()
        {
            if (_conversationEnd)
            {
                End();
            }
            else
            {
                if (_finished)
                {
                    StartCoroutine(nameof(DisplayNewText));
                }
                else
                {
                    StopCoroutine(nameof(DisplayNewText));
                    StartCoroutine(nameof(DisplayTextFast));
                }
            }
        }

        private void GetNewText()
        {
            _textFound = false;
            foreach (var item in _dialogues.items)
            {
                if (!item.idText.Equals(_textId + "." + _currentDialogue)) continue;
                _textFound = true;
                _text = lang switch
                {
                    1 => item.langEs,
                    2 => item.langEus,
                    3 => item.langCat,
                    4 => item.langEn,
                    5 => item.langChi,
                    _ => _text
                };

                _currentPos = item.position.Equals("left");
            }

            if (_textFound) return;
            _conversationEnd = true;
            End();
        }

        private void End()
        {
            StopCoroutine(nameof(DisplayNewText));
            StopCoroutine(nameof(DisplayTextFast));
            canvas.SetActive(false);
        }

        private void ChangePos() // TODO: HACER QUE EL COLOR LERPEE
        {
            if (_currentPos)
            {
                _imageChar1.color = new Color32(255,255,255,255);
                _imageChar2.color = new Color32(128,128,128,255);
            }
            else
            {
                _imageChar1.color = new Color32(128,128,128,255);
                _imageChar2.color = new Color32(255,255,255,255);
            }
        }
        
        private IEnumerator DisplayNewText()
        {
            GetNewText();
            yield return new WaitUntil(() => _textFound);
            ChangePos();
            _finished = false;
            _textMeshProUGUI.text = "";
            ConvertTextToCharacters();
            InvokeRepeating(nameof(PutLetter), 0f, 0.03f);
            yield return new WaitUntil(() => _finished);
            CancelInvoke(nameof(PutLetter));
            _index = 0;
            _currentDialogue++;
        }

        private IEnumerator DisplayTextFast()
        {
            InvokeRepeating(nameof(PutLetter), 0f, 0.001f);
            yield return new WaitUntil(() => _finished);
            CancelInvoke(nameof(PutLetter));
            _index = 0;
            _currentDialogue++;
        }

        private void PutLetter()
        {
            switch (_index)
            {
                case var i when i < _characters.Length: 
                    _textMeshProUGUI.text = _textMeshProUGUI.text += _characters[_index];
                    _index++;
                    break;
                case var i when i >= _characters.Length:
                    _finished = true;
                    break;
            }
        }
    }
}

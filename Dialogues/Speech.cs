using Suburbs.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suburbs.Dialogues;

public static class Speech 
{
    // RANDOM NAMES!!!
    public const string BOB_TAG = "B:";
    public const string AMELIA_TAG = "A:";

    public const string BOB_NAME = "BOB";
    public const string AMELIA_NAME = "AMELIA";
    public static readonly Dictionary<string, int> SpeakerVoiceID = new()
    {
        { BOB_NAME, Voice.BAD_TIME },
        { AMELIA_NAME, Voice.BAD_TIME },
    };

    public static Dialogue CurrentDialogue { get; private set; }
    public static Voice CurrentVoice { get; private set; }
    public static string SpeakerName { get; private set; }

    public static readonly List<int> SeenDialogues = [];

    public static int ActiveLine { get; private set; } = 0;
    public static event Action<int?> OnDialogueFired;

    private static readonly Dictionary<int, (string speaker, List<DynamicText> line)> _dialogueLines = new();

    private static float _timePerCharacter; // Typewriter speed
    private static int _visibleCharacters = 0;  // Global visible character count
    private static float _characterTimer = 0f;
    private static float _pendingDelay = 0f; // Stores delay to be applied after a word
    private static bool _typingComplete = false;

    public static void FireDialogue(int dialogueId, float timePerCharacter = 0.033f, float startPosX = 55, float startPoxY = 575)
    {
        CurrentDialogue = Dialogue.Get(dialogueId);

        _dialogueLines.Clear();
        _timePerCharacter = timePerCharacter;
        _visibleCharacters = 0;
        _typingComplete = false;

        for (int i = 0; i < CurrentDialogue.Text.Length; i++)
        {
            string line = CurrentDialogue.Text[i];
            string speaker = null;

            switch (line[0].ToString() + line[1])
            {
                // Random names, to change later lol
                case BOB_TAG:
                    speaker = BOB_NAME;
                    line = line.Replace(BOB_TAG, null);
                    break;

                case AMELIA_TAG:
                    speaker = AMELIA_NAME;
                    line = line.Replace(AMELIA_TAG, null);
                    break;
            }

            List<DynamicText> dynamicLine = [];
            _dialogueLines.Add(i, (speaker, dynamicLine));

            string[] wordsInLine = line.Split(' ');
            Vector2 word_position = new(startPosX, startPoxY);
            foreach (string word in wordsInLine)
            {
                string processedWord = word; 
                TextProperties word_info = TextProperties.ParseTags(ref processedWord);

                DynamicText prev = dynamicLine.LastOrDefault();
                if (prev is not null)
                {
                    word_position = new(prev.Position.X + prev.Size.X * DynamicText.Scale + DynamicText.Kerning, prev.Position.Y);
                    if ((word_position.X + GameRoot.StandardRegularFont.MeasureString(processedWord).X) > DynamicText.MAX_WIDTH - 55f || prev.Text.Contains('\n'))
                    {
                        word_position.X = startPosX;
                        word_position.Y += GameRoot.StandardRegularFont.LineSpacing * DynamicText.Scale;
                    }
                }

                DynamicText dText = new(processedWord, word_position, word_info);
                dynamicLine.Add(dText);
            }
        }

        SeenDialogues.Add(dialogueId);
        OnDialogueFired?.Invoke(dialogueId);
    }

    public static void Update(GameTime gameTime)
    {
        if (CurrentDialogue is null || _typingComplete) return;

        _characterTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_pendingDelay > 0f) // If there's a delay from the last word, apply it first
        {
            if (_characterTimer >= _pendingDelay)
            {
                _characterTimer -= _pendingDelay;
                _pendingDelay = 0f; // Reset after applying
            }
            else return; // Wait for delay to finish
        }

        while (_characterTimer >= _timePerCharacter && _visibleCharacters < GetTotalCharacters())
        {
            var (nextChar, delay, isLastInWord) = GetNextCharacterWithDelay();

            if (char.IsLetter(nextChar))
            {
                CurrentVoice?.Play();
            }

            _visibleCharacters++;
            _characterTimer -= _timePerCharacter;

            if (isLastInWord) 
            {
                _pendingDelay = delay;
                break; 
            }
        }

        if (_visibleCharacters >= GetTotalCharacters())
        {
            _typingComplete = true;
        }
    }

    public static void Draw()
    {
        if (CurrentDialogue is null)
            return;

        int drawnCharacters = 0;
        foreach (var text in _dialogueLines[ActiveLine].line)
        {
            SpeakerName = _dialogueLines[ActiveLine].speaker;
            if (SpeakerName is not null) CurrentVoice = Voice.Get(SpeakerVoiceID[SpeakerName]);
            else CurrentVoice = Voice.Get(Voice.GENERIC_VOICE_1);
            drawnCharacters += text.DrawSpeech(_visibleCharacters - drawnCharacters);
        }
    }

    public static void SkipAnimation()
    {
        if (CurrentDialogue is null) return;

        _visibleCharacters = GetTotalCharacters();
        _typingComplete = true;
    }

    public static void NextLine()
    {
        if (CurrentDialogue is null) return;
        if (_visibleCharacters != GetTotalCharacters()) return;

        ActiveLine++;
        if (ActiveLine > CurrentDialogue.Text.Length - 1)
        {
            CurrentDialogue = null;
            CurrentVoice = null;
            ActiveLine = 0;
            _dialogueLines.Clear();
            OnDialogueFired?.Invoke(null);
        }
        else
        {
            _visibleCharacters = 0;
            _typingComplete = false;
        }
    }

    private static int GetTotalCharacters()
    {
        return _dialogueLines[ActiveLine].line.Sum(text => text.Text.Length);
    }

    private static (char character, float delay, bool isLastInWord) GetNextCharacterWithDelay()
    {
        int count = 0;
        foreach (var text in _dialogueLines[ActiveLine].line)
        {
            if (_visibleCharacters - count < text.Text.Length)
            {
                bool isLast = (_visibleCharacters - count == text.Text.Length - 1); 
                return (text.Text[_visibleCharacters - count], text.Properties.Delay, isLast);
            }
            count += text.Text.Length;
        }

        return (' ', 0f, false); // Default fallback (shouldn't happen)
    }
}

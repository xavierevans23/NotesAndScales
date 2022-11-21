using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesAndScales
{
    public class KeySignature
    {
        public readonly string Name;

        public readonly Accidental Accidental;
        public readonly int AccidentalCount;

        public readonly PitchClass First;
        public readonly IReadOnlyList<PitchClass> Notes;
        public readonly IReadOnlyDictionary<Letter, Accidental> LetterAccidentals;

        private KeySignature(PitchClass[] notes)
        {
            Name = notes[0].ToString();
            Notes = notes.ToArray();
            First = notes[0];

            Accidental = Accidental.Natural;

            Dictionary<Letter, Accidental> accidentals = new();
            AccidentalCount = 0;
            foreach (PitchClass note in notes)
            {
                if (note.Accidental != Accidental.Natural)
                {
                    AccidentalCount++;
                    Accidental = note.Accidental;
                }
                accidentals[note.Letter] = note.Accidental;
            }

            LetterAccidentals = accidentals;
        }

        public static IEnumerable<Note> GetMajorScaleNotes(Note start, bool useThirteenNotes = false)
        {
            yield return start;
            start++;
            start++;
            yield return start;
            start++;
            start++;
            yield return start;
            start++;
            yield return start;
            start++;
            start++;
            yield return start;
            start++;
            start++;
            yield return start;
            start++;
            start++;
            yield return start;
            start++;
            if (useThirteenNotes)
            {
                yield return start;
            }
        }

        public static KeySignature? GetKeySignature(PitchClass start)
        {
            Letter letter = start.Letter;
            Note[] notes = GetMajorScaleNotes(new(new NoteLabel(start, 4))).ToArray();
            List<PitchClass> pitches = new();

            for (int i = 0; i < 7; i++)
            {
                if (notes[i].Name.PitchClass.Letter == letter)
                {
                    pitches.Add(notes[i].Name.PitchClass);
                }
                else if (notes[i].AlternativeName?.PitchClass.Letter == letter)
                {
                    pitches.Add(notes[i].AlternativeName!.Value.PitchClass);
                }
                else
                {
                    return null;
                }

                letter = letter.StepBy(1);
            }

            return new(pitches.ToArray());
        }

        public IEnumerable<PitchClass> Accidentals()
        {
            foreach (PitchClass note in Notes)
            {
                if (note.Accidental != Accidental.Natural)
                {
                    yield return note;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Key
    {
        public readonly KeySignature KeySignature;
        public readonly KeySignature? AlternativeKeySignature;

        public readonly string Name;

        public readonly bool Flat;
        public readonly bool Natural;
        public readonly bool Sharp;       

        public Key(PitchClass start)
        {
            KeySignature? first = KeySignature.GetKeySignature(start);
            KeySignature? second = null;

            Note note = new Note(new NoteLabel(start, 4));
            PitchClass altPitchClass = note.Name.PitchClass;

            if (altPitchClass == start && note.AlternativeName is NoteLabel label)
            {
                altPitchClass = label.PitchClass;
            }

            if (altPitchClass != start)
            {
                second = KeySignature.GetKeySignature(altPitchClass);
            }

            if (first == null)
            {
                if (second != null)
                {
                    first = second;
                    second = null;
                }
            }

            KeySignature = first!;
            AlternativeKeySignature = second;

            Name = KeySignature.Name;

            Flat = (KeySignature.Accidental == Accidental.Flat || AlternativeKeySignature?.Accidental == Accidental.Flat);
            Natural = (KeySignature.Accidental == Accidental.Natural || AlternativeKeySignature?.Accidental == Accidental.Natural);
            Sharp = (KeySignature.Accidental == Accidental.Sharp || AlternativeKeySignature?.Accidental == Accidental.Sharp);            
        }

        public override string ToString()
        {
            return AlternativeKeySignature is not null ? $"{KeySignature.Name} ({AlternativeKeySignature.Name})" : KeySignature.Name;
        }

        public IEnumerable<Note> GetNotes(int octave)
        {
            return KeySignature.GetMajorScaleNotes(new(new NoteLabel(KeySignature.First, octave)));
        }
    }
}

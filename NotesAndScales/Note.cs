using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesAndScales
{
    public class Note : IEquatable<Note>
    {
        public readonly int Number;
        public readonly int MidiNumber;
        public readonly int PianoNumber;
        
        public readonly int Octave;

        public readonly NoteLabel Name;
        public readonly NoteLabel? AlternativeName;

        public readonly double Frequency;        

        public Note(int number)
        {
            Number = number;
            PianoNumber = number - 8;
            MidiNumber = number + 12;

            int pitchClassNumber = number % 12;
            if (pitchClassNumber < 0)
            {
                pitchClassNumber += 12;
            }

            Octave = (number - pitchClassNumber) / 12;

            Name = new(names[pitchClassNumber], Octave);

            if (alternativeNames[pitchClassNumber] is not null)
            {
                AlternativeName = new(alternativeNames[pitchClassNumber]!.Value, Octave + AlternativeNameOctaveOffset(pitchClassNumber));
            }
            else
            {
                AlternativeName = null;
            }

            Frequency = FixedNoteFrequency * Math.Pow(FrequencyMultipler, (number - FixedNoteNumber));
        }

        public Note(NoteLabel label) : this(NoteNumberFromLabel(label)) { }
        public Note(string text) : this(NoteNumberFromLabel(NoteLabel.TryParse(text, out NoteLabel noteLabel) ? 
            noteLabel : throw new ArgumentException("Please provide a valid string."))) { }

        public override int GetHashCode()
        {
            return Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Note other && Equals(other);
        }

        public bool Equals(Note? other)
        {
            return other?.Number == Number;
        }

        public static bool operator ==(Note left, Note right)
        {
            return left.Number == right.Number;
        }

        public static bool operator !=(Note left, Note right)
        {
            return left.Number != right.Number;
        }

        public static Note operator +(int left, Note right)
        {
            return new(left + right.Number );
        }

        public static Note operator +(Note right, int left)
        {
            return new(right.Number + left);
        }

        public static Note operator ++(Note right)
        {
            return new(right.Number + 1);
        }

        public static Note operator --(Note right)
        {
            return new(right.Number - 1);
        }

        public static Note operator -(Note right, int left)
        {
            return new(right.Number - left);
        }

        public override string ToString()
        {
            return AlternativeName != null ? $"{Name} ({AlternativeName})" : Name.ToString();
        }

        public const double FixedNoteFrequency = 440f;
        public const int FixedNoteNumber = 45;
        public const double FrequencyMultipler = 1.059463f;

        private static readonly IReadOnlyList<PitchClass> names = new PitchClass[]
        {
            new(Letter.C, Accidental.Natural),
            new(Letter.C, Accidental.Sharp),
            new(Letter.D, Accidental.Natural),
            new(Letter.E, Accidental.Flat),
            new(Letter.E, Accidental.Natural),
            new(Letter.F, Accidental.Natural),
            new(Letter.F, Accidental.Sharp),
            new(Letter.G, Accidental.Natural),
            new(Letter.A, Accidental.Flat),
            new(Letter.A, Accidental.Natural),
            new(Letter.B, Accidental.Flat),
            new(Letter.B, Accidental.Natural),
        };

        private static readonly IReadOnlyList<PitchClass?> alternativeNames = new PitchClass?[]
        {
            new(Letter.B, Accidental.Sharp),
            new(Letter.D, Accidental.Flat),
            null,
            new(Letter.D, Accidental.Sharp),
            new(Letter.F, Accidental.Flat),
            new(Letter.E, Accidental.Sharp),
            new(Letter.G, Accidental.Flat),
            null,
            new(Letter.G, Accidental.Sharp),
            null,
            new(Letter.A, Accidental.Sharp),
            new(Letter.C, Accidental.Flat),
        };

        private static int AlternativeNameOctaveOffset(int pitchClassNumber)
        {
            if (pitchClassNumber == 11)
            {
                return 1;
            }
            if (pitchClassNumber == 0)
            {
                return -1;
            }
            
            return 0;
        }

        private static readonly IReadOnlyDictionary<Letter, int> letterOffset = new Dictionary<Letter, int>()
        {
            { Letter.C, 0},
            { Letter.D, 2},
            { Letter.E, 4},
            { Letter.F, 5},
            { Letter.G, 7},
            { Letter.A, 9},
            { Letter.B, 11},            
        };

        private static readonly IReadOnlyDictionary<Accidental, int> accidentalOffset = new Dictionary<Accidental, int>()
        {
            { Accidental.Flat, -1},
            { Accidental.Natural, 0},
            { Accidental.Sharp, 1}
        };

        private static int NoteNumberFromLabel(NoteLabel label)
        {
            return letterOffset[label.PitchClass.Letter] + accidentalOffset[label.PitchClass.Accidental] + label.Octave * 12;
        }
    }
}

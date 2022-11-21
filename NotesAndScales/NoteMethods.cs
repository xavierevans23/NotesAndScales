namespace NotesAndScales
{
    public enum Letter
    {
        C,
        D,
        E,
        F,
        G,
        A,
        B,
    }

    public enum Accidental
    {
        Flat,
        Natural,
        Sharp
    }

    public static class NoteMethods
    {
        public static Letter StepBy(this Letter letter, int steps)
        {
            int letterInt = (((int)letter) + steps) % 7;

            if (letterInt < 0)
            {
                letterInt += 7;
            }

            return (Letter)letterInt;
        }

        public static bool TryParseLetter(string text, out Letter letter)
        {
            text = text.Trim();

            if (Enum.TryParse<Letter>(text, true, out letter))
            {
                if (letter < 0 || (int)letter > 7)
                {
                    letter = Letter.C;
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryParseAccidental(string text, out Accidental accidental)
        {
            text = text.Trim();

            if (Enum.TryParse<Accidental>(text, true, out accidental))
            {
                if (accidental < 0 || (int)accidental > 2)
                {
                    accidental = Accidental.Natural;
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public readonly record struct PitchClass(Letter Letter, Accidental Accidental)
    {
        public override string ToString()
        {
            return $"{Letter} {Accidental}";
        }

        public static bool TryParse(string text, out PitchClass pitchClass)
        {
            string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1 && NoteMethods.TryParseLetter(parts[0], out Letter letter1))
            {
                pitchClass = new(letter1, Accidental.Natural);
                return true;
            }
            if (parts.Length == 2 && NoteMethods.TryParseLetter(parts[0], out Letter letter2) &&
                NoteMethods.TryParseAccidental(parts[1], out Accidental accidental))
            {
                pitchClass = new(letter2, accidental);
                return true;
            }

            pitchClass = new(Letter.C, Accidental.Natural);
            return false;
        }
    }
    public readonly record struct NoteLabel(PitchClass PitchClass, int Octave)
    {
        public NoteLabel(Letter Letter, Accidental Accidental, int Octave) : this(new(Letter, Accidental), Octave) { }

        public override string ToString()
        {
            return $"{PitchClass.Letter} {PitchClass.Accidental} {Octave}";
        }

        public static bool TryParse(string text, out NoteLabel noteLabel)
        {
            string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1 && NoteMethods.TryParseLetter(parts[0], out Letter letter1))
            {
                noteLabel = new(letter1, Accidental.Natural, 4);
                return true;
            }
            if (parts.Length == 2 && NoteMethods.TryParseLetter(parts[0], out Letter letter2) &&
                NoteMethods.TryParseAccidental(parts[1], out Accidental accidental1))
            {
                noteLabel = new(letter2, accidental1, 4);
                return true;
            }
            if (parts.Length == 3 && NoteMethods.TryParseLetter(parts[0], out Letter letter3) &&
                NoteMethods.TryParseAccidental(parts[1], out Accidental accidental2) &&
                int.TryParse(parts[2], out int octave1))
            {
                noteLabel = new(letter3, accidental2, octave1);
                return true;
            }

            noteLabel = new(Letter.C, Accidental.Natural, 4);
            return false;
        }
    }
}
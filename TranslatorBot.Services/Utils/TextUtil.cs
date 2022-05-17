namespace TranslatorBot.Services.Utils;

public static class TextUtil {
    public static List<string> SplitText(string text, int maxLength) {
        var result = new List<string>();

        var sentences = SplitIntoSentences(text);
        sentences = ValidateSentences(sentences, maxLength);

        var resultText = string.Empty;
        foreach (var sentence in sentences) {
            if (resultText.Length + sentence.Length > maxLength) {
                result.Add(resultText.Trim());
                resultText = string.Empty;
            }

            resultText += sentence + " ";
        }

        if (!string.IsNullOrEmpty(resultText)) {
            result.Add(resultText.Trim());
        }

        return result;
    }

    private static List<string> SplitIntoSentences(string text) {
        var sentences = new List<string>();

        int start = 0;
        int position;

        do {
            position = text.IndexOfAny(new char[] { '.', '!', '?' }, start);
            if (position == -1) {
                position = text.Length - 1;
            }

            if (position >= 0) {
                sentences.Add(text.Substring(start, position - start + 1).Trim());
                start = position + 1;
            }
        } while (start < text.Length);

        return sentences;
    }

    private static List<string> ValidateSentences(List<string> sentences, int maxLength) {
        var result = new List<string>();

        foreach (var sentence in sentences) {
            if (sentence.Length >= maxLength) {
                result.AddRange(ChunkStr(sentence, maxLength));
            } else {
                result.Add(sentence);
            }
        }

        return result;
    }

    private static IEnumerable<string> ChunkStr(string str, int chunkSize) {
        if (string.IsNullOrEmpty(str) || chunkSize < 1) {
            throw new ArgumentException(string.Empty);
        }

        return Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));
    }
}
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Relay join Codeと日本語コードを相互変換するクラス
/// </summary>
public class NetTest_JoinCodeConverter
{
    //Relay側の文字 
    private const string RelayChars = "6789BCDFGHJKLMNPQRTW";

    //日本語側の文字
    private const string JapaneseChars = "あいうえおかきくけこさしすせそたちつてと";

    /// <summary>
    /// Relay Join Codeを日本語に変換する
    /// </summary>
    public static string ToJapanese(string joinCode)
    {
        string result = "";

        foreach (char c in joinCode.ToUpperInvariant())
        {
            int index = RelayChars.IndexOf(c);

            if (index < 0)
            {
                Debug.LogError($"変換できない文字です: {c}");
                return "";
            }

            result += JapaneseChars[index];
        }

        return result;
    }

    /// <summary>
    /// 日本語コードをRelay Join Codeに戻す
    /// </summary>
    public static string ToRelayCode(string japaneseCode)
    {
        string result = "";

        foreach (char c in japaneseCode)
        {
            int index = JapaneseChars.IndexOf(c);

            if (index < 0)
            {
                Debug.LogError($"変換できない文字です: {c}");
                return "";
            }

            result += RelayChars[index];
        }

        return result;
    }
}

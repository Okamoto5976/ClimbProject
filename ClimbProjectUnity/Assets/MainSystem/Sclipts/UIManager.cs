using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Clientごとに表示は違う
    //Hostではない場合はHostから情報を貰うことになる
    //
    //キャラクターがどこにいるかの表示
    //PlayerManagerかRendereManagerから各キャラクターの位置情報を貰う

    //全体のスタートからゴールまでのUI
    //貰った各キャラクターのY軸を渡す
    //UI自体で見やすさを変える（重ならないようにしたり）

    //順位の表示
    //
    //取得したアイテムの説明
    //ItemManagerから取得した時にEventを呼ぶようにして
    //TextDataから表示
}



# v0.2.0


**MainUpdates**

Gym環境に get_state と get_info を追加し、reset/step からstateとinfoの情報を分離しました。
  - setup中にstateを取得できるようにし、observation_spaceが定義されていない場合にある程度自動で定義できるように変更
  - 画像のみの場合等はstateの取得がなくなるので少し処理が軽くなります

1. [change] IGameController: resetとstepの戻り値からstate/infoを削除し、get_stateとget_infoを追加
1. [change] env_client.py  : 初回resetをsetup中に実行し、1stの通信中にstateを取得できるように変更
1. [new] env_client.py     : default_observation_spaceをオプションで追加し、observation_spaceが定義されていない場合、stateから予測しobservation_spaceを定義。
1. [change] doc: 更新に合わせてドキュメント更新


**OtherUpdates**

1. [change] env_server.py : 1st通信中のtimeoutを60から無限に変更
1. [update] bizhawk_api.py: set_keyでNoneや""を指定できるように変更

# v0.1.1

1. [add] UiMenu.cs  : デバッグログの出力を切り替える `Enable Debug Logging` をメニューに追加
1. [fix] PyBridge.cs: client.unpauseでたまにフリーズするので実行をupdate外に変更
    - reboot_coreも多分同じ現象が起こりそうなので同じ処理に変更
    - [rename] PyBridge.csのUpdateCrossingStateの LoadSlot を WaitingNextUpdate に名前変更
1. [add] bizhawk_main.py : pprintを追加
1. [ref] bizhawk_api.py  : set_key関数をに直して修正
1. [ref] bizhawk_main.py : print周りを微修正
1. [ref] BIzhawkApi      : CmdReturnの戻り値がvoidの場合、"None"を返していたがnullに統一
1. [del] release から BizHawk-2.10 を除外

# v0.1.0

1st upload


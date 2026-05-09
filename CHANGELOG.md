
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


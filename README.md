# BizHawkPy

BizHawkPy は、BizHawk 用の外部ツール（External Tool）として動作し、Pythonからエミュレータの操作や情報取得を可能にするツールです。
既存の Lua Console の Python 版として機能します。

また、強化学習用の標準APIを提供する [Gymnasium](https://github.com/Farama-Foundation/Gymnasium/tree/main) との連携コードも提供しています。


# 1. インストール

本ツールは BizHawk の ExternalTools として導入します。
導入にあたり、BizHawkのバージョンとビルド成果物は厳密に対応しており、異なるバージョン間では互換性がないので注意してください。

## 配布DLLを直接追加する方法

### ビルド環境
- windows11
- BizHawk v2.10.0 / v2.9.1

### 手順

1. BizHawk を任意の場所にダウンロードする
2. 配布Zipを解凍し、ExternalToolsフォルダを以下の場所に配置する
```text
BizHawk/
 ├─ EmuHawk.exe
 ├─ ExternalTools/
 │    │  # ExternalToolsフォルダ配下に以下を置く
 │    ├─ BizHawkPy.dll
 │    ├─ BizHawkPy/
 │         ├─ *.py
```
3. BizHawk を起動し、以下から起動
```
Tools → External Tools → BizHawkPy
```


## ソースコードからビルドする方法

### 開発時の環境

* Visual Studio 2026
  * .NET デスクトップ開発
  * .NET Framework 4.8.1 開発ツール

### ビルド手順

1. 本プロジェクトをクローン

```bash
git clone https://github.com/pocokhc/BizHawkPy.git
cd BizHawkPy
```

2. 以下でビルド

```bash
dotnet build -c Release /p:BIZHAWK_HOME="対象のBizHawkディレクトリパス"
```

ビルドが成功するとBizHawkのディレクトリに "ExternalTools/BizhawkPy.dll" "ExternalTools/BizhawkPy/*.py" が作成されます。

※BizHawkのバージョンとビルド成果物は厳密に対応しており、異なるバージョン間では互換性がありません。


# 2. 使い方

## 開発時の環境

Pythonは別途インストールしてください。

* Python 3.13.13

## 起動

BizHawk を起動後、メニューバーにBizHawkPyが追加されているので選択して起動します。

```
Tools → External Tools → BizHawkPy
```

## 図

interpreter


# 3. Pythonコードについて

BizHawkPyはBizHawkで実装されている [Lua functions](https://tasvideos.org/BizHawk/LuaFunctions) とほぼ同じ使い方を想定しています。
サンプルコードは以下です。

```python
# 動作について：
# - スクリプトは開始すると一度だけ実行されます
# - emu.frameadvance() は一時停止し、1フレーム後に再開します
#
# 注意：
# - 無限ループを使用する場合は、必ず emu.frameadvance() を呼び出す必要があります
# - 呼び出さない場合、このスクリプトは無限ループに陥り、エミュレータがフリーズします
#
# importについて：
# - from bizhawk_api import * にて、Lua と同じ API を使用できます
# - 例：emu、joypad、memory、gui など

from bizhawk_api import emu

while True:
    print(f"frame: {emu.framecount()}")
    emu.frameadvance()

```

## 型ヒントの利用について

* 本プロジェクトの `py/bizhawk_api.py` まはた `ExternalTools/BizHawkPy/bizhawk_api.py` は型定義として利用できます（IDE補完・型チェック用）
* 実行時には内部的に自動で読み込まれるため、`sys.path` の追加等で実行時のpathを追加する必要はありません


## Gymnasiumについて

作成例は [examples/BizHawkEnv](examples/BizHawkEnv) のREADMEを参考にしてください。


# 3. 進捗サマリ

| カテゴリ   | 総関数数 | 実装済み | 実装見送り | 進捗 | 備考|
| --------- | ---: | ---: | ---: | ---: | -: |
| bit       |    - |    - | - |    - | python側で実装可能と判断|
| bizstring |    - |    - | - |    - | python側で実装可能と判断|
| client    |   59 |   48 | 0 |  81% | |
| comm      |   33 |   30 | 1 |  94% | |
| console   |    5 |    4 | 1 | 100% | BizHawkPy側の出力窓で代替 |
| emu       |   19 |   17 | 1 |  95% | emu.yieldは実装なし |
| event     |   19 |    0 | 0 |   0% | |
| forms     |   44 |    0 | 0 |   0% | |
| gameinfo  |    7 |    7 | 0 | 100% | |
| gensis    |    8 |    0 | 0 |   0% | |
| gui       |   28 |   25 | 0 |  89% | |
| input     |    3 |    - | - |    - | ユーザ入力はpython側の実装と判断|
| joypad    |    6 |    6 | 0 | 100% | |
| LuaCanvas |   26 |    - | 0 |    - | guiで十分と判断し実装せず　|
|mainmemory |   40 |    - | 0  |   - | memoryで十分と判断し実装せず |
| memory    |   44 |   43 | 1 | 100% | 非推奨は実装せず |
|memorysavestate|4 |    4 | 0 | 100% | |
| movie     |   21 |   21 | 0 | 100% | |
| nds       |   10 |    0 | 0 |   0% | |
| nes       |   11 |    0 | 0 |   0% | |
| savestate |    4 |    4 | 0 | 100% | |
| snes      |   16 |    0 | 0 |   0% | |
| SQL       |    - |    - | - |    - | python側で実装可能と判断|
| tastudio  |   34 |    0 | 0 |   0% | |
| userdata  |    6 |    6 | 0 | 100% | |

※各実装の詳細は `BizHawkPy/BizHawkApi/*.cs` を参照してください。


## 外部リンク
- [Bizhawk/Lua Functions - TASVideos](https://tasvideos.org/BizHawk/LuaFunctions)
- [TASEmulators/BizHawk - ExternalTools Wiki](https://github.com/TASEmulators/BizHawk-ExternalTools/wiki)
- [Gymnasium](https://gymnasium.farama.org/index.html)


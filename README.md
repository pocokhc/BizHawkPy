[README in English](README-en.md)

[![(latest) release | GitHub](https://img.shields.io/github/release/pocokhc/BizHawkPy.svg?logo=github&style=popout)](https://github.com/pocokhc/BizHawkPy/releases/latest)

# BizHawkPy

BizHawkPy は、[BizHawk](https://github.com/TASEmulators/BizHawk) 用の外部ツール（External Tool）として動作し、Pythonからエミュレータの操作や情報取得を可能にするツールです。
既存の Lua Console の Python 版として機能します。

また、強化学習用の標準APIを提供する [Gymnasium](https://github.com/Farama-Foundation/Gymnasium/tree/main) との連携コードも提供しています。


# 1. インストール

本ツールは BizHawk の ExternalTools として導入します。  
導入方法は以下の2通りです。

- 配布済み DLL を利用する方法
- ソースコードからビルドする方法

> **注意**  
> BizHawk のバージョンとビルド成果物は厳密に対応しています。  
> 異なるバージョン間では互換性がないため、必ず一致させてください。

## 1.1 配布DLLを直接追加する方法

### ビルド時の環境

- windows11
- BizHawk v2.10.0 / v2.9.1

### 手順

1. BizHawk を任意の場所にダウンロード
2. [配布Zip](https://github.com/pocokhc/BizHawkPy/releases)を解凍し、BizHawk 本体の `ExternalTools` フォルダ配下に配置
```text
BizHawk/
 ├─ EmuHawk.exe
 ├─ ExternalTools/  # ExternalToolsフォルダ配下に置く
 │    ├─ BizHawkPy.dll
 │    ├─ BizHawkPy/
 │         ├─ *.py
```
3. BizHawk を起動し、以下のメニューから起動
```
Tools → External Tools → BizHawkPy
```


## 1.2 ソースコードからビルドする方法

### 開発時の環境

* Visual Studio 2026
  * .NET デスクトップ開発
  * .NET Framework 4.8.1 開発ツール

### ビルド手順

1. リポジトリをクローン

```bash
git clone https://github.com/pocokhc/BizHawkPy.git
cd BizHawkPy
```

2. ビルドを実行

```bash
dotnet build -c Release /p:BIZHAWK_HOME="BizHawk のディレクトリパス"
```
  
ルド成功後、BizHawk の `ExternalTools` ディレクトリに関連ファイルが出力されます。


# 2. 使い方

## 2.1 開発時の環境

Pythonは別途インストールしてください。

* Python 3.13.13

## 2.2 起動

BizHawk を起動後、以下のメニューから BizHawkPy を選択します。

```
Tools → External Tools → BizHawkPy
```

## 2.3 コンソール画面について

以下は実際の画面例です。

![スクリーンショット](diagrams/ss.png)

- `PythonCommand` で使用するインタプリタが指定できます（デフォルトは `py` ランチャー）
- 各 `.py` スクリプトは個別に実行・停止が可能です
- ログ出力はエミュレータ本体と独立しており、エミュレータが停止した場合でも可能な限り継続して動作します


# 3. Pythonコードについて

BizHawkPy は、BizHawk に実装されている [Lua functions](https://tasvideos.org/BizHawk/LuaFunctions) とほぼ同様の使用方法を想定しています。  
以下は基本的なサンプルコードです。

```python
# 動作について：
# - スクリプトは開始すると一度だけ実行されます
# - emu.frameadvance() は一時停止し、1フレーム後に再開されます
#
# 注意：
# - 無限ループを使用する場合は、必ず emu.frameadvance() を呼び出してください
# - 呼び出さない場合、スクリプトは無限ループに陥り、エミュレータがフリーズします
#
# importについて：
# - from bizhawk_api import * にて、Lua と同じ API を使用できます
# - 例：emu、joypad、memory、gui など

from bizhawk_api import emu

while True:
    print(f"frame: {emu.framecount()}")
    emu.frameadvance()

```

## 3.1 型ヒントの利用について

* `py/bizhawk_api.py` まはた `ExternalTools/BizHawkPy/bizhawk_api.py` は型定義として利用できます（IDE補完・型チェック用）
* 実行時には内部的に自動ロードされるため、`sys.path` の追加などは不要です


## 3.2 Gymnasium との連携

使用例については、以下を参照してください。

- [examples/BizHawkEnv](examples/BizHawkEnv/)


# 4. 進捗サマリ

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


# 外部リンク
- [Bizhawk/Lua Functions - TASVideos](https://tasvideos.org/BizHawk/LuaFunctions)
- [TASEmulators/BizHawk - ExternalTools Wiki](https://github.com/TASEmulators/BizHawk-ExternalTools/wiki)
- [Gymnasium](https://gymnasium.farama.org/index.html)


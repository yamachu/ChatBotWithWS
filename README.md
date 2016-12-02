# ChatBotWithWS
ASP.NET Core上で動く簡易ChatBot．テンプレートはyo aspnetで作成．

## コマンドの追加
1. `Models.ChatCommands.Commands` に `IChatCommand` を実装したクラスを追加
2. `Models.ChatCommands.CommandRunner.cs` 内の `_Command enum`にコマンドの名称を追加し，順番通りに`_CommandExt`の`CommandList`に名称とクラスの型を追加

## Chatルーム機能
HOST/Chat/Room/{任意文字列} でルームを作成できます．

## ToDo
* 認証系
* 面白いコマンドの追加
* エラーハンドリング
* Viewをデフォルトじゃないものに変える
* etc...

## 小技
[wscat](https://www.npmjs.com/package/wscat)使えばデバッグそこそこ楽な気がする．

## 小言
* https://github.com/aspnet/WebSockets/issues/63 で詰まることもあるのでWebSocket使う人は注意
* テストケースを走らせた時の一回目は必ず失敗する（ライブラリの問題？）
* Roslyn for Sciptingを使用しているが， .NETCoreAppが1.0.1なら動くけど1.1.0だと動かない...

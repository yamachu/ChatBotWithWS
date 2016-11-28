# ChatBotWithWS
ASP.NET Core上で動く簡易ChatBot．テンプレートはyo aspnetで作成．

## コマンドの追加
1. `Models.ChatCommands.Commands` に `IChatCommand` を実装したクラスを追加
2. `Models.ChatCommands.CommandRunner.cs` 内の `_Command enum`にコマンドの名称を追加し，順番通りに`_CommandExt`のDisplayNameとCommandInterfacesにクラスを追加

## ToDo
* 認証系
* 面白いコマンドの追加
* エラーハンドリング
* Viewをデフォルトじゃないものに変える
* etc...
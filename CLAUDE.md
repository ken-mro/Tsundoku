# Tsundoku

積読（買ったまま読んでいない本）を記録する .NET MAUI アプリ。ISBN をスキャン・入力して本を「積読」に登録し、読み終えたら「読了証明書」を発行する。対象は Android / iOS / MacCatalyst。

## 構成

- `Models/` — `BookInfo`（SQLite のテーブル）と `Book`（画面用。積読日数・背表紙色などの計算プロパティ）
- `Repository/` — SQLite へのアクセス（`IBookInfoRepository`）、設定、定数
- `ViewModels/` — CommunityToolkit.Mvvm（`[ObservableProperty]`、`[RelayCommand]`）
- `Views/` — XAML のページとポップアップ（CommunityToolkit.Maui の `Popup`）
- `Resources/Styles/` — `Colors.xaml`（デザイントークン）と `Styles.xaml`（既定スタイル・キー付きスタイル）
- `Resources/AppResources*.resx` — 文言（英語・日本語）
- `Tsundoku.xUnitTest/` — xUnit テスト。アプリの `net10.0` ヘッドを参照する
- `tools/verify/` — デバイス用ツールチェーンなしで動く検証ハーネス

## 検証

変更したら、コミット前に必ず実行する:

```bash
tools/verify/verify.sh
```

次を順に実行し、1つでも失敗すると非ゼロで終了する:

1. `net10.0` ヘッドのビルド（C# と XAML のコンパイル）
2. `check_xaml.py`：StaticResource キー、`x:DataType` に対するバインディングパス、画像参照の存在チェック。XamlC は存在しないバインディングパスを検出しないので、このチェックで補う
3. スモークテスト：全ページ・ポップアップと ItemTemplate をサンプルデータで実際に生成する
4. ユニットテスト（`Category=Network` は除外）
5. Android の Resizetizer：アイコン・スプラッシュ・画像を `obj/Debug/net10.0-android/resizetizer/` に書き出す。見た目を変えたら PNG を開いて目視確認する

補足:

- Linux には iOS/MacCatalyst のワークロードがないので、`-p:TsundokuNeutralOnly=true` で `net10.0` のみをビルドする。`-p:TargetFrameworks=...` で上書きすると、テストプロジェクトの xunit ランナーが読み込まれなくなるので使わない。
- Android SDK がない環境ではアプリ全体の Android ビルドはできない。Resizetizer だけはダミーの `AndroidSdkDirectory` で実行できる。
- `AsinUtilityTests` は実際の Amazon URL にアクセスするため `[Trait("Category", "Network")]` を付けている。
- クラウドセッションでは `.claude/hooks/session-start.sh` が .NET 10 SDK と MAUI ワークロードを入れる。

## デザインシステム

- 色は必ず `Colors.xaml` のトークンを使い、XAML に色を直接書かない。ライト／ダークは `AppThemeBinding` で両方指定する。
  - `Ink` #1F2D4A（主色）、`Paper` #F2F3F1（背景）、`Shu` #C8432B（朱：読了・警告だけに使う）、`Kincha` #C99A2E（証明書の枠）
  - `Surface`/`SurfaceDark`、`TextPrimary`/`TextPrimaryDark`、`TextSecondary`/`TextSecondaryDark`、`Line`/`LineDark`
  - 背表紙パレット `Spine1`〜`Spine5`。`Book.Color` と値を揃える
- 文字のスタイル：`PageTitle`・`DisplayNumber`・`Headline`（明朝体 `craftmincho`。見出しと数字だけに使う）、`Caption`、`Data`（ISBN・日付用の等幅）。本文は OS 標準の日本語フォントのまま（`FontFamily` を指定しない）。
- 部品のスタイル：カードは `Style="{StaticResource Card}"`、副ボタンは `SecondaryButton`。
- アイコン・スプラッシュ・画像は SVG。`Resources/AppIcon/appicon.svg`（背景）と `appiconfg.svg`（前景）、`Resources/Splash/splash.svg`、`Resources/Images/*.svg`。XAML からは `name.png` で参照する。
- タブとスワイプのアイコンは 24px の線画で、タブの色は Shell が塗る。
- 日付は `yyyy.MM.dd`（InvariantCulture）で表示する。

## コーディング規約

- XAML はページ・`DataTemplate` ともに `x:DataType` を指定する（コンパイル済みバインディング）。
- 文言を追加するときは `AppResources.resx`・`AppResources.ja.resx`・`AppResources.Designer.cs` の 3 ファイルを手で揃える。Designer.cs は IDE 以外では再生成されない。
- 既存ファイルの BOM と改行コードを維持する（BOM 付きのファイルと BOM なしのファイルが混在している）。
- `obj/`・`bin/` はコミットしない。

## コミット

- 1 コミットは 1 つの変更単位にし、そのコミット単体で `tools/verify/verify.sh` が通る状態にする。
- メッセージは英語。1 行目は命令形の要約、本文に理由を書く。

---
name: store-release-notes
description: Write the Google Play / App Store "what's new" release notes for an app version in English and Japanese, in the <en-US>/<ja-JP> block the Play Console accepts. Use when the user asks for release notes, "what's new" text, store update text or リリースノート, and from the android-play-release skill.
---

# Store release notes

The readers are app users, not developers. Say what got better for them, not how it was fixed.

1. **Find what changed** since the previous release:
   - The version's section of `CHANGELOG.md`, if the project has one.
   - `git log --oneline <previous bump>..HEAD`, where the previous bump is
     `git log -1 --format=%h -G'android:versionName' -- Platforms/Android/AndroidManifest.xml` run on the commit *before* the current bump (`<current bump>^`). Use `-G`, not `-S`: a bump changes the value without changing how often the attribute appears.
   - Read the commit bodies (`git log --format='%h %s%n%b'`) when a subject is unclear.
   - Skip internal changes: refactors, tests, docs, build scripts, Claude skills/hooks, version bumps.
2. **Group** related fixes into one line each; about 2–4 lines in total.
3. **Write each line from the user's side, as an outcome:**
   - Good: "The scanner no longer opens the same book twice."
   - Too detailed: "The barcode callback fired again while the popup was opening, so a second popup was pushed."
   - Leave out causes, device conditions, internal screen or class names, and examples.
   - No heading such as "Bug fixes", and no version number.
   - No user-facing change at all (a rebuild with updated libraries)? Say so in one short line, e.g. "Performance and stability improvements."
4. **Japanese:** natural, polite UI wording (〜ようになりました／〜しました), each line starting with `・`. **English:** each line starting with `- `.
5. **Length:** each language under 500 characters (the Play Console limit).

Return it in a code block in exactly this form, so it can be pasted into the Play Console or saved to a file for upload:

```
<en-US>
- The map stays where you left it when you come back to the app.
</en-US>
<ja-JP>
・アプリに戻ったときに、地図が前に見ていた場所のままになりました。
</ja-JP>
```

Example approved by the user (FlagsRally 1.1.17):

```
<en-US>
- Custom board tiles now line up neatly.
- The map stays steady and is easier to use while cards are showing.
- The map stays where you left it when you come back to the app.
</en-US>
<ja-JP>
・カスタムボードのタイルがきれいに並ぶようになりました。
・カードを表示しているときも、地図が動かず操作しやすくなりました。
・アプリに戻ったときに、地図が前に見ていた場所のままになりました。
</ja-JP>
```

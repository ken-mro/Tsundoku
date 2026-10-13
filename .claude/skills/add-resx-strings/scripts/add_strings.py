#!/usr/bin/env python3
"""Add localized strings to AppResources.resx, its culture files (AppResources.ja.resx, ...) and
AppResources.Designer.cs together.

    python add_strings.py strings.json [--resources <dir>]

strings.json maps each key to its texts, either by culture or as a list:
    {"Key": {"": "English text", "ja": "日本語"}, ...}   # "" is the neutral AppResources.resx
    {"Key": ["English text", "日本語"], ...}             # neutral first, then cultures sorted by name

--resources defaults to the folder holding AppResources.resx under the current directory (bin/
and obj/ are skipped). Keys that already exist are left untouched. Each file keeps its BOM and
line endings (CRLF or LF), so the diff shows only the new entries. Every culture file must get a
text for every new key; the script stops before writing anything otherwise.
"""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path
from xml.sax.saxutils import escape


def find_resources(root: Path) -> Path:
    hits = [p.parent for p in root.rglob("AppResources.resx")
            if not {"bin", "obj"} & {part.lower() for part in p.parts}]
    if len(hits) != 1:
        sys.exit(f"found {len(hits)} AppResources.resx folders under {root}; pass --resources")
    return hits[0]


def read(path: Path) -> tuple[str, bool, str]:
    raw = path.read_bytes()
    bom = raw.startswith(b"\xef\xbb\xbf")
    text = raw.decode("utf-8-sig")
    return text, bom, "\r\n" if "\r\n" in text else "\n"


def write(path: Path, text: str, bom: bool) -> None:
    path.write_bytes((b"\xef\xbb\xbf" if bom else b"") + text.encode("utf-8"))


def add_to_resx(path: Path, texts: dict[str, str]) -> list[str]:
    text, bom, nl = read(path)
    block, added = "", []
    for key, value in texts.items():
        if f'name="{key}"' in text:
            continue
        block += f'  <data name="{key}" xml:space="preserve">{nl}    <value>{escape(value)}</value>{nl}  </data>{nl}'
        added.append(key)
    if block:
        write(path, text.replace("</root>", block + "</root>"), bom)
    return added


def add_to_designer(path: Path, texts: dict[str, str]) -> list[str]:
    text, bom, nl = read(path)
    end = text.rstrip().rfind("    }")  # closing brace of the class
    block, added = "", []
    for key, value in texts.items():
        if f"string {key} " in text:
            continue
        summary = escape(value).replace("\r", " ").replace("\n", " ")
        block += (f"        {nl}        /// <summary>{nl}        ///   Looks up a localized string similar to {summary}.{nl}"
                  f"        /// </summary>{nl}        internal static string {key} {{{nl}            get {{{nl}"
                  f"                return ResourceManager.GetString(\"{key}\", resourceCulture);{nl}            }}{nl}        }}{nl}")
        added.append(key)
    if block:
        write(path, text[:end] + block + text[end:], bom)
    return added


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("strings", type=Path)
    parser.add_argument("--resources", type=Path)
    args = parser.parse_args()

    resources = args.resources or find_resources(Path.cwd())
    neutral = resources / "AppResources.resx"
    designer = resources / "AppResources.Designer.cs"
    cultures = sorted(p.name[len("AppResources."):-len(".resx")] for p in resources.glob("AppResources.*.resx"))
    for path in (neutral, designer):
        if not path.is_file():
            sys.exit(f"{path} not found")

    data = json.loads(args.strings.read_text(encoding="utf-8"))
    order = ["", *cultures]
    by_culture: dict[str, dict[str, str]] = {c: {} for c in order}
    for key, values in data.items():
        if isinstance(values, list):
            if len(values) != len(order):
                sys.exit(f"{key}: {len(values)} texts, but there are {len(order)} files (neutral, {', '.join(cultures)})")
            values = dict(zip(order, values))
        missing = [c or "neutral" for c in order if not values.get(c)]
        if missing:
            sys.exit(f"{key}: no text for {', '.join(missing)}")
        for c in order:
            by_culture[c][key] = values[c]

    added = add_to_resx(neutral, by_culture[""])
    for c in cultures:
        add_to_resx(resources / f"AppResources.{c}.resx", by_culture[c])
    add_to_designer(designer, by_culture[""])
    skipped = [k for k in data if k not in added]
    print("added:", ", ".join(added) or "(none)")
    if skipped:
        print("already there, left as is:", ", ".join(skipped))
    return 0


if __name__ == "__main__":
    sys.exit(main())

"""Static XAML checks that the MAUI build does not catch.

1. Every {StaticResource Key} used in a XAML file is defined in Colors.xaml / Styles.xaml.
2. Every {Binding Path} in Views/*.xaml and AppShell.xaml resolves to a member of the
   nearest x:DataType (public properties, [ObservableProperty] fields, [RelayCommand]s).
3. Every image referenced as "name.png" exists in Resources/Images as .png or .svg.

Exit code 0 when everything resolves, 1 otherwise.
"""
import glob
import os
import re
import sys
import xml.etree.ElementTree as ET

root=os.path.abspath(os.path.join(os.path.dirname(__file__),'..','..'))+'/'
X='{http://schemas.microsoft.com/winfx/2009/xaml}'

def xaml_files():
    for f in glob.glob(root+'**/*.xaml',recursive=True):
        # glob returns backslash paths (and case-insensitive matches such as
        # obj/.../Microsoft.Maui.Controls.Xaml directories) on Windows.
        p=f.replace('\\','/')
        if '/obj/' not in p and '/bin/' not in p and os.path.isfile(f):
            yield f

# 1. StaticResource keys
keys=set()
for f in ['Resources/Styles/Colors.xaml','Resources/Styles/Styles.xaml']:
    keys|=set(re.findall(r'x:Key="([^"]+)"',open(root+f,encoding='utf-8-sig').read()))
used=set()
for f in xaml_files():
    used|=set(re.findall(r'StaticResource ([A-Za-z0-9]+)',open(f,encoding='utf-8-sig').read()))
if used-keys:
    print('Missing StaticResource keys:',sorted(used-keys)); sys.exit(1)
print('StaticResource keys OK')

# 2. Bindings
members={}
def cls_members(name):
    if name in members: return members[name]
    src=''
    for f in glob.glob(root+'**/*.cs',recursive=True):
        if '/obj/' in f or '/bin/' in f or '/tools/' in f: continue
        t=open(f,encoding='utf-8-sig').read()
        if re.search(r'\bclass\s+'+name+r'\b',t): src+=t
    if not src: return None
    m=set(re.findall(r'public\s+(?:required\s+|static\s+|override\s+|virtual\s+)*[\w<>\[\]?,. ]+?\s+(\w+)\s*(?:\{|=>)',src))
    for f in re.findall(r'\[ObservableProperty\][^;]*?\s(_?\w+)\s*(?:=|;)',src,re.S):
        n=f.lstrip('_'); m.add(n[0].upper()+n[1:])
    for f in re.findall(r'\[RelayCommand[^\]]*\][^(]*?\s(\w+)\s*\(',src,re.S):
        n=f[:-5] if f.endswith('Async') else f; m.add(n+'Command')
    base=re.search(r'\bclass\s+'+name+r'\b[^:{]*:\s*(\w+)',src)
    members[name]=m
    if base:
        b=cls_members(base.group(1))
        if b: m|=b
    return m
def typename(v):
    v=v.strip()
    mm=re.match(r'\{x:Type\s+\w+:(\w+)\}',v)
    if mm: return mm.group(1)
    return v.split(':')[-1]
errors=[]
for f in glob.glob(root+'Views/*.xaml')+[root+'AppShell.xaml']:
    tree=ET.parse(f)
    def walk(el,dt):
        if el.get(X+'DataType'): dt=typename(el.get(X+'DataType'))
        for k,v in el.attrib.items():
            for b in re.findall(r'\{Binding([^{}]*(?:\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}[^{}]*)*)\}',v):
                b=b.strip()
                local=dt
                inline=re.search(r'x:DataType=(\w+:)?(\w+)',b)
                if inline: local=inline.group(2)
                path=None
                pm=re.search(r'Path=([\w.]+)',b)
                if pm: path=pm.group(1)
                elif b and not b.startswith(('Source','Converter','StringFormat','Mode')):
                    path=re.split(r'[,\s]',b)[0]
                if not path or path=='.': continue
                if 'Source=' in b and not inline: continue
                first=path.split('.')[0]
                ms=cls_members(local) if local else None
                if ms is None: errors.append(f'{f}: no type for {local} ({path})')
                elif first not in ms: errors.append(f'{f.replace(root,"")}: {local}.{first} not found (attr {k.split("}")[-1]})')
        for c in el: walk(c,dt)
    walk(tree.getroot(),None)
if errors:
    print('\n'.join(errors)); sys.exit(1)
print('Bindings OK')
# 3. Images
missing=[]
for f in xaml_files():
    for n in re.findall(r'"([\w-]+)\.png"',open(f,encoding='utf-8-sig').read()):
        if not any(glob.glob(root+f'Resources/Images/{n}.{e}') for e in ('png','svg')): missing.append(f'{f.replace(root,"")}: {n}.png')
if missing: print('Missing images:',missing); sys.exit(1)
print('Images OK')

#!/usr/bin/env python3
# RAS v1.1 (DONATE) - kilixkilik

import os, shutil, hashlib, random

igd = {'__pycache__', 'GameAssets', 'Assets', 'obj', 'bin'}
igf = {'script.py', 'archive.tar'}
igx = {'.log', '.tmp', '.pyc'}

# check and rename existing ras dir to avoid conflict
for i in os.listdir('.'):
    if os.path.isdir(i) and i.lower() == 'ras':
        os.rename(i, f'ras_{random.randint(1000,9999)}')

def h(p):
    try:
        with open(p, 'rb') as f:
            return hashlib.sha256(f.read()).hexdigest()[:8]
    except:
        return 'CORRUPT'

os.makedirs('ras', exist_ok=1)

# stats counter
st = {'f':0, 'd':0, 'b':0}

# root files
with open('ras/root.txt', 'w', encoding='utf-8') as o:
    for f in os.listdir('.'):
        if not os.path.isfile(f):
            continue
        if f in igf or f.lower().endswith(tuple(igx)):
            continue
        o.write(f'F: {f}\nH: {h(f)}\n')
        try:
            c = open(f, encoding='utf-8').read()
            st['b'] += len(c.encode('utf-8'))
            o.write(c)
        except:
            o.write('[BIN]')
        o.write('\n\n')
        st['f'] += 1

# dirs
for d in os.listdir('.'):
    if not os.path.isdir(d):
        continue
    if d in igd or d.lower() == 'ras':
        continue
    st['d'] += 1
    with open(f'ras/{d}.txt', 'w', encoding='utf-8') as o:
        o.write(f'D: {d}\n')
        for dp, dns, fs in os.walk(d):
            dns[:] = [x for x in dns if x not in igd and x.lower() != 'ras']
            for f in fs:
                if f.lower().endswith(tuple(igx)):
                    continue
                p = os.path.join(dp, f)
                rp = os.path.relpath(p, d)
                o.write(f'F: {rp}\nH: {h(p)}\n')
                try:
                    c = open(p, encoding='utf-8').read()
                    st['b'] += len(c.encode('utf-8'))
                    o.write(c)
                except:
                    o.write('[BIN]')
                o.write('\n\n')
                st['f'] += 1

# stats output (new feature v1.1)
print(f'files: {st["f"]} | dirs: {st["d"]} | size: {st["b"]//1024} KB')
print('done. no archive.tar created.')


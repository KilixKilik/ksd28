import os

# get this file path
cur_file = os.path.abspath(__file__)

# inputs
old = input("Старое название: ").strip()
new = input("Новое название: ").strip()

root = os.getcwd()

for dirp, dirs, files in os.walk(root, topdown=False):
    # files
    for f in files:
        old_p = os.path.join(dirp, f)
        # skip self
        if os.path.abspath(old_p) == cur_file:
            continue
            
        new_f = f.replace(old, new)
        new_p = os.path.join(dirp, new_f)
        
        # rename file
        if f != new_f:
            print(f"[FILE] {old_p} -> {new_p}")
            os.rename(old_p, new_p)
            old_p = new_p
        
        # replace content
        try:
            with open(old_p, 'r', encoding='utf-8') as fd:
                data = fd.read()
            if old in data:
                ndata = data.replace(old, new)
                with open(old_p, 'w', encoding='utf-8') as fd:
                    fd.write(ndata)
                print(f"[CONTENT] {old_p}")
        except:
            pass
    
    # dirs
    for d in dirs:
        old_dp = os.path.join(dirp, d)
        new_d = d.replace(old, new)
        new_dp = os.path.join(dirp, new_d)
        
        if d != new_d:
            print(f"[DIR] {old_dp} -> {new_dp}")
            os.rename(old_dp, new_dp)

print("Done.")

# credits by kilixkilik (@k2rkusha)

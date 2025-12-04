import os

old = "ClientHome"
new = input("Введите новое название: ").strip()

root = os.getcwd()

for dirp, dirs, files in os.walk(root, topdown=False):
    # филесы
    for f in files:
        old_path = os.path.join(dirp, f)
        new_f = f.replace(old, new)
        new_path = os.path.join(dirp, new_f)
        
        # изменениэ имэны ыыыы
        if f != new_f:
            print(f"[FILE] {old_path} -> {new_path}")
            os.rename(old_path, new_path)
            old_path = new_path
        
        # заменаыыыы
        try:
            with open(old_path, 'r', encoding='utf-8') as fd:
                data = fd.read()
            if old in data:
                ndata = data.replace(old, new)
                with open(old_path, 'w', encoding='utf-8') as fd:
                    fd.write(ndata)
                print(f"[CONTENT] {old_path}")
        except:
            continue
    
    # дирикториээ
    for d in dirs:
        old_dp = os.path.join(dirp, d)
        new_d = d.replace(old, new)
        new_dp = os.path.join(dirp, new_d)
        
        if d != new_d:
            print(f"[DIR] {old_dp} -> {new_dp}")
            os.rename(old_dp, new_dp)

print("Готово. Все замены выполнены.")

# credits by kilixkilik (@k2rkusha)

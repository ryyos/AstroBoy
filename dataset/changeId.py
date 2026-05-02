import sqlite3

conn = sqlite3.connect("astroboy.sqlite")
cursor = conn.cursor()

# Ambil berdasarkan urutan insert (ROWID)
cursor.execute("SELECT id FROM users ORDER BY rowid")
rows = cursor.fetchall()

# Mapping id lama → id baru
mapping = {}
for index, (old_id,) in enumerate(rows, start=1):
    mapping[old_id] = index

TEMP_OFFSET = 1000000

# Step A: geser dulu (hindari konflik PK)
for old_id in mapping:
    cursor.execute(
        "UPDATE users SET id = ? WHERE id = ?",
        (mapping[old_id] + TEMP_OFFSET, old_id)
    )

# Step B: set ke final id
for old_id in mapping:
    cursor.execute(
        "UPDATE users SET id = ? WHERE id = ?",
        (mapping[old_id], mapping[old_id] + TEMP_OFFSET)
    )

conn.commit()
conn.close()

print("ID berhasil di-reset berdasarkan urutan insert (ROWID).")
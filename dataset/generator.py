import sqlite3
import uuid
import random
import json

# 1. Load names.json
with open("names.json", "r") as f:
    names_data = json.load(f)

def get_random_name():
    person = random.choice(names_data)
    first = person.get("First Name", "").strip()
    last = person.get("Last Name", "").strip()
    return f"{first} {last}".strip()

def generate_email(name):
    base = name.lower().replace(" ", ".")
    return f"{base}{random.randint(1,999)}@mail.com"

# 2. Connect DB
conn = sqlite3.connect("astroboy.sqlite")
cursor = conn.cursor()

# 3. Create table
cursor.execute("""
CREATE TABLE IF NOT EXISTS users (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    role TEXT NOT NULL,
    balance REAL NOT NULL
)
""")

# 4. Generate user
def generate_user(role):
    name = get_random_name()
    return (
        str(uuid.uuid4()),
        name,
        generate_email(name),
        name.lower().replace(" ", "")+"123",
        role,
        round(random.uniform(10000, 1000000), 2)
    )

# 5. Create dummy data
customers = [generate_user("customer") for _ in range(25)]
owners = [generate_user("owner") for _ in range(11)]
admins = [(str(uuid.uuid4()), "Red Ranger", "redranger@gmail.com", "qwertyuiop", "admin", 1000000)]

all_users = customers + owners + admins

# 6. Insert (hindari duplicate email crash)
for user in all_users:
    try:
        cursor.execute("""
        INSERT INTO users (Id, Name, Email, Password, Role, Balance)
        VALUES (?, ?, ?, ?, ?, ?)
        """, user)
    except sqlite3.IntegrityError:
        pass  # skip kalau email duplikat

# 7. Commit & close
conn.commit()
conn.close()

print("Database + dummy users (random names) berhasil dibuat.")
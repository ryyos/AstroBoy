import sqlite3
import json
import uuid
import random
from datetime import datetime
from cathd import CathD

# =========================
# CONFIG
# =========================
DB_PATH = "astroboy.sqlite"
JSON_PATH = "items.json"

OWNER_IDS = list(range(26, 37))  # 26 - 36

STORE_NAMES = [
  "Culinary Crest","NoshNibble Kitchen","TreatTrail Treats","Paletta",
  "Rustic Roots Kitchen","Spoonla","The Hunger Hub","Tastilo",
  "Flavorful Fusion Feasts","SavorySpice Culinary","Savor Select Gourmet",
  "ForkFête","Tantalize Tastes","Bistrona","Eateria","Crumblet",
  "Wholesome Delights Diner","Savor & Spice Café","Charma","Artisanal Bites",
  "Harvest Haven Gourmet","Tasty Twist Kitchen","Epic Eatery Experience",
  "Crispa","Spindlea","Blissful Palate Catering","Peckino","Flavorra",
  "Bite Bounty Bazaar","Delightful Dishes Depot","HungryHunter Kitchen",
  "SimplySavory Kitchen","Yum Yum Foods & Co.","Epicurean Express Solutions",
  "Culino","GourmetGusto Pantry","Culinary Craft Connections",
  "Tasty Treats Kitchen","Flavorsome Fare Foundry","TasteBoulevard",
  "JuicyJunction Foods","TasteTricks","SavorSquad Cuisine",
  "TastyCrave Co.","Urban Tastebuds","Forkio","Flavorful Feast Foods",
  "EpicureanEdge Eateries","PalateParadise"
]

# =========================
# DB SETUP
# =========================
def init_db(conn):
    cursor = conn.cursor()

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS stores (
        store_id TEXT PRIMARY KEY,
        owner_id TEXT,
        name TEXT,
        address TEXT,
        phone TEXT
    );
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS items (
        id TEXT PRIMARY KEY,
        name TEXT,
        price REAL,
        stock INTEGER,
        category TEXT,
        store_id TEXT
    );
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS orders (
        id TEXT PRIMARY KEY,
        customer_id TEXT,
        store_id TEXT,
        created_at TEXT,
        status TEXT
    );
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS order_items (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        order_id TEXT,
        item_id TEXT,
        item_name TEXT,
        unit_price INTEGER,
        quantity INTEGER
    );
    """)

    conn.commit()

# =========================
# STORE GENERATION
# =========================
def generate_store_name(store_id):
    index = abs(hash(store_id)) % len(STORE_NAMES)
    return STORE_NAMES[index]

def generate_stores():
    stores = []

    for owner_id in OWNER_IDS:
        num_stores = random.randint(1, 5)

        for i in range(num_stores):
            store_id = f"{owner_id}-{i}"

            stores.append({
                "StoreId": store_id,
                "OwnerId": str(owner_id),
                "Name": generate_store_name(store_id),
                "Address": "Indonesia",
                "Phone": random.randint(111111111, 999999999)
            })

    return stores

# =========================
# INSERT FUNCTIONS
# =========================
def insert_store(cursor, store):
    cursor.execute("""
        INSERT OR IGNORE INTO stores (store_id, owner_id, name, address, phone)
        VALUES (?, ?, ?, ?, ?)
    """, (
        store["StoreId"],
        store["OwnerId"],
        store["Name"],
        store["Address"],
        store["Phone"]
    ))

def insert_item(cursor, item):
    cursor.execute("""
        INSERT OR REPLACE INTO items (id, name, price, stock, category, store_id)
        VALUES (?, ?, ?, ?, ?, ?)
    """, (
        item["Id"],
        item["Name"],
        item["Price"],
        item["Stock"],
        item["Category"],
        item["StoreId"]
    ))

def insert_order(cursor, order):
    cursor.execute("""
        INSERT INTO orders (id, customer_id, store_id, created_at, status)
        VALUES (?, ?, ?, ?, ?)
    """, (
        order["Id"],
        order["CustomerId"],
        order["StoreId"],
        order["CreatedAt"],
        order["Status"]
    ))

def insert_order_items(cursor, order_id, items):
    for i in items:
        cursor.execute("""
            INSERT INTO order_items (order_id, item_id, item_name, unit_price, quantity)
            VALUES (?, ?, ?, ?, ?)
        """, (
            order_id,
            i["ItemId"],
            i["ItemName"],
            i["UnitPrice"],
            i["Quantity"]
        ))

# =========================
# PROCESS TOKOPEDIA DATA
# =========================
def process_products(products, stores, cursor):
    total_stores = len(stores)

    for product in products:
        # assign store deterministically
        index = abs(hash(product["product_id"])) % total_stores
        store = stores[index]
        # CathD.download(
        #     product["product_image_url"], 
        #     path="D:/UPH/Pemrogaman Berorientasi Object/AstroBoy/Resources/Images/items/{}.jpg".format(
        #         product["product_id"]
        #     ),
        #     save=True
        # )
        # map ke Item
        item = {
            "Id": product["product_id"],
            "Name": product["product_name"],
            "Price": product["price"],
            "Stock": random.randint(10, 100),
            "Category": product.get("category"),
            "StoreId": store["StoreId"]
        }

        insert_item(cursor, item)

        # generate order (30% chance)
        if random.random() < 0.3:
            order_id = str(uuid.uuid4())

            order = {
                "Id": order_id,
                "CustomerId": f"{random.randint(1,25)}",
                "StoreId": store["StoreId"],
                "CreatedAt": datetime.now().isoformat(),
                "Status": random.choice(["Pending", "Completed"])
            }

            insert_order(cursor, order)

            order_items = [{
                "ItemId": item["Id"],
                "ItemName": item["Name"],
                "UnitPrice": int(item["Price"]),
                "Quantity": random.randint(1, 3)
            }]

            insert_order_items(cursor, order_id, order_items)

# =========================
# MAIN
# =========================
def main():
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()

    # init tables
    init_db(conn)

    # load data tokopedia
    with open(JSON_PATH, encoding="utf-8") as f:
        products = json.load(f)

    # generate stores
    stores = generate_stores()

    # insert stores
    for s in stores:
        insert_store(cursor, s)

    # process products → items + orders
    process_products(products, stores, cursor)

    conn.commit()
    conn.close()

    print("✅ Data berhasil dimasukkan ke SQLite")

if __name__ == "__main__":
    main()
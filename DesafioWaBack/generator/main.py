import datetime
import random
import pymssql

# parameters
order_count = 500
min_items_per_order = 1
max_items_per_order = 20
product_count_on_db = 77
max_quantity_per_order_item = 10

# connect to SQL Server
connection_string = open("../connectionString.txt", "r").read().strip()

# split parts
parts = connection_string.split(";")
server = None
database = None
user = None
password = None

for part in parts:
    if "Server=" in part:
        server = part.split("=")[1]
    if "Database=" in part:
        database = part.split("=")[1]
    if "User Id=" in part:
        user = part.split("=")[1]
    if "Password=" in part:
        password = part.split("=")[1]

conn = pymssql.connect(server, user, password, database)
cursor = conn.cursor()

# clear tables
# noinspection SqlWithoutWhere
cursor.execute("DELETE FROM [dbo].[OrderItems]")
# noinspection SqlWithoutWhere
cursor.execute("DELETE FROM [dbo].[Orders]")

# create order_count random dates
dates = []
for i in range(order_count):
    dates.append(datetime.datetime(
        year=2021,
        month=random.randint(1, 12),
        day=random.randint(1, 28),
        hour=random.randint(0, 23),
        minute=random.randint(0, 59),
        second=random.randint(0, 59)
    ))

# sort dates
dates.sort()

# count to order_count
for i in range(order_count):
    # create a random order
    order = {
        'order_id': i,
        'order_date': dates[i],
        'items': [],
        'delivery_date': None,
        'address': None,
        'delivery_team_id': None
    }

    # randomly decide if was delivered
    if random.randint(0, 1) == 1:
        # create a random delivery date after order date
        order['delivery_date'] = order['order_date'] + datetime.timedelta(
            days=random.randint(1, 10)
        )

        # random team between 1 and 5
        order['delivery_team_id'] = random.randint(1, 5)

    # create random city name
    city = ''

    # random syllable count
    syllable_count = random.randint(3, 6)

    # create random syllables
    for x in range(syllable_count):
        consonants = 'bcdfghjklmnpqrstvwxyz'
        vowels = 'aeiou'
        city += consonants[random.randint(0, len(consonants) - 1)]
        city += vowels[random.randint(0, len(vowels) - 1)]

    # capitalize first letter
    city = city[0].upper() + city[1:]

    # create random address
    address = {
        'street': random.randint(1, 1000),
        'number': random.randint(1, 10000),
        'neighborhood': chr(random.randint(65, 90)),
        'city': city,
        'state': chr(random.randint(65, 90)) + chr(random.randint(65, 90)),
        'zip_code': '{:08d}'.format(random.randint(0, 99999999))
    }

    # convert address to string
    order['address'] = 'Rua {}, {} - Bairro {}, {} - {} | CEP {}'.format(
        address['street'],
        address['number'],
        address['neighborhood'],
        address['city'],
        address['state'],
        address['zip_code']
    )

    # insert the order and get the id

    cursor.execute(
        "INSERT INTO Orders (CreationDate, DeliveryDate, Address, DeliveryTeamId) VALUES (%s, %s, %s, %s);",
        (
            order['order_date'],
            order['delivery_date'],
            order['address'],
            order['delivery_team_id']
        )
    )

    order_number = cursor.lastrowid

    # count to random quantity
    for j in range(random.randint(min_items_per_order, max_items_per_order)):
        # create a random item
        item = {
            'item_id': random.randint(1, product_count_on_db),
            'quantity': random.randint(1, max_quantity_per_order_item),
        }

        # insert the item
        cursor.execute(
            "INSERT INTO OrderItems (OrderNumber, ProductId, Quantity) VALUES (%s, %s, %s)",
            (order_number, item['item_id'], item['quantity'])
        )

# commit the changes
conn.commit()

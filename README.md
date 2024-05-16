# Seasonal Product Discounter

## Objectives:
  - Accommodate various types of discounts, and apply the relevant ones for a specific product on a specific day.
  - Generate thousands of randomized Product objects, providing much more data (simulates real-world applications which may work with millions of products).
  - Create user interfaces that provide user-level access.
  - Simulate the process of buyers coming in, browsing products, and making purchases (log and save to DB).

## Simulation steps:

  - Create the required tables in a SQLite database.
  - Generate randomized products and store them in a table in the SQLite database.
  - Register & authenticate users (buyers of products).
  - Simulate transactions happening in the store.
  - At the end of a simulation round, a report about the status of the store is created & displayed.

## Discount types:

  - Monthly discounts:
    - Summer Kick-off, 10% discount on every product during June and July.
    - Winter Sale, 10% discount on every product during February.

  - Colour discounts:
    - Blue Winter, 5% discount on every blue-coloured product during winter.
    - Green Spring, 5% discount on every green-coloured product during spring.
    - Yellow Summer, 5% discount on every yellow-coloured product during summer.
    - Brown Autumn, 5% discount on every brown-coloured product during autumn.

  - Seasonal discounts:
    - Sale Discount, 10% discount if the item is bought right before or right after the favoured season. (Such as buying winter gloves during spring.)
    - Outlet Discount, 20% discount if the item is bought two seasons after the favored season. (Such as buying winter gloves during summer.)

## Technologies: 
  - C#
  - SQLite

# datalib
Library API homework for Datapac.

# Prerequisities
## MSSQL LocalDB
- Make sure you have [Microsoft SQL LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver16) installed.

## Code-first database migration
- No need to create initial migrations. DB and tables will get created upon first application launch.

# Functionality overview
Online documentation is available at `.../swagger/` and `.../swagger/v1/swagger.json`.

## Users
Use `.../api/users` endpoint for CRUD operations. There is no authorization implemented.

## Books
Use `.../api/books` endpoint for CRUD operations. Multiple books with indentical `Author` and `Title` can be created.

## Checking out books
Use `POST` method on `.../api/checkouts` endpoint to check out one or multiple books. If successful, API responds with `Checkout` details. Use `.../api/checkouts?userId={userId}` to get all user's checkouts.

## Returing books
Use `POST` method `.../api/checkouts/{checkoutId}/return` endpoint to return one or multiple books. Each book can be returned only once. Application supports returning only some of the books from the checkout.

## Receipt for returned books
Use `GET` method on `.../api/checkouts/{checkoutId}` to get details of given checkout with status of respective books.

## Due date reminder emails
Application sends emails (via fake email service) to remind users that due date is coming soon. Application detects such checkouts and sends emails on each application start and then once a day periodically. App doesn't keep track of already sent emails, hence duplicate emails might get send.

## Unit tests
You can find several Test classes, however not all code is covered by tests. These tests serves just like a demonstration of ability to write unit tests.

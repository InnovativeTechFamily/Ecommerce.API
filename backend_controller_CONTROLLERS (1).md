# Controllers — Route Reference

Generated from backend/controller (ref 3a8dd50). This file lists every route, HTTP method, a short description, and the controller file that defines it.

| S.No. | Method | Route | Detail | Controller |
|---|---:|---|---|---|
| 1 | POST | /upload-image | Upload image to Cloudinary | cloudinary.js |
| 2 | POST | /create-new-conversation | Create a new conversation | conversation.js |
| 3 | GET | /get-all-conversation-seller/:id | Get seller conversations (by seller id) | conversation.js |
| 4 | GET | /get-all-conversation-user/:id | Get user conversations (by user id) | conversation.js |
| 5 | PUT | /update-last-message/:id | Update conversation last message by conversation id | conversation.js |
| 6 | POST | /create-coupon-code | Create coupon code (seller) | coupounCode.js |
| 7 | GET | /get-coupon/:id | Get all coupons of a shop (seller) | coupounCode.js |
| 8 | DELETE | /delete-coupon/:id | Delete coupon code of a shop (seller) | coupounCode.js |
| 9 | GET | /get-coupon-value/:name | Get coupon code value by name | coupounCode.js |
| 10 | POST | /create-event | Create event (uploads images to Cloudinary) | event.js |
| 11 | GET | /get-all-events/:id | Get all events of a shop (by shop id) | event.js |
| 12 | GET | /get-all-events | Get all events | event.js |
| 13 | DELETE | /delete-shop-event/:id | Delete event of a shop (by event id) | event.js |
| 14 | GET | /admin-all-events | Admin: list all events | event.js |
| 15 | POST | /import-products/:id | Import multiple products for a shop (seller) | importData.js |
| 16 | GET | /import-data/:id | Get import data for a shop (seller) | importData.js |
| 17 | POST | /create-new-message | Create new message (optionally upload image) | message.js |
| 18 | GET | /get-all-messages/:id | Get all messages for a conversation (by conversation id) | message.js |
| 19 | POST | /create-order | Create new order (splits per shop, sends email) | order.js |
| 20 | GET | /get-all-orders/:userId | Get all orders of a user | order.js |
| 21 | GET | /get-seller-all-orders/:shopId | Get all orders for a seller (by shop id) | order.js |
| 22 | PUT | /update-order-status/:id | Seller: update order status | order.js |
| 23 | PUT | /order-refund/:id | User: request a refund / update order refund status | order.js |
| 24 | PUT | /order-refund-success/:id | Seller: accept refund (refund success) | order.js |
| 25 | GET | /admin-all-orders | Admin: list all orders | order.js |
| 26 | POST | /process | Stripe: create payment intent (process payment) | payment.js |
| 27 | GET | /stripeapikey | Get Stripe API key | payment.js |
| 28 | POST | /create-product | Create product (uploads images to Cloudinary) | product.js |
| 29 | POST | /update-product | Update product (uploads images, merges existing images) | product.js |
| 30 | GET | /get-all-product/:id | Get product by id | product.js |
| 31 | GET | /get-all-products-shop/:id | Get all products of a shop (by shop id) | product.js |
| 32 | DELETE | /delete-shop-product/:id | Delete a shop product (remove images from Cloudinary) | product.js |
| 33 | DELETE | /delete-shop-products | Multi-delete products for a shop (seller) | product.js |
| 34 | GET | /get-all-products | Get all published/active products (pagination) | product.js |
| 35 | PUT | /create-new-review | Create or update a review for a product (user) | product.js |
| 36 | POST | /create-category | Create category for shop (seller) | product.js |
| 37 | GET | /get-categories/:id | Get categories for shop (by shop id) (seller) | product.js |
| 38 | DELETE | /delete-category/:id | Delete a category for shop (seller) | product.js |
| 39 | GET | /admin-all-products | Admin: list all products | product.js |
| 40 | POST | /create-shop | Create a new shop (sends activation email) | shop.js |
| 41 | POST | /activation | Activate shop account using activation token | shop.js |
| 42 | POST | /login-shop | Login shop (seller) | shop.js |
| 43 | GET | /getSeller | Load authenticated seller info | shop.js |
| 44 | GET | /logout | Logout seller (clear seller cookie) | shop.js |
| 45 | GET | /get-shop-info/:id | Get shop info by id | shop.js |
| 46 | PUT | /update-shop-avatar | Update shop avatar (seller) | shop.js |
| 47 | PUT | /update-seller-info | Update seller profile info | shop.js |
| 48 | GET | /admin-all-sellers | Admin: list all sellers | shop.js |
| 49 | DELETE | /delete-seller/:id | Admin: delete seller by id | shop.js |
| 50 | PUT | /update-payment-methods | Update seller withdraw/payment methods | shop.js |
| 51 | DELETE | /delete-withdraw-method/ | Delete seller withdraw methods (seller) | shop.js |
| 52 | POST | /create-user | Create user (sends activation email) | user.js |
| 53 | POST | /activation | Activate user using activation token | user.js |
| 54 | POST | /login-user | Login user | user.js |
| 55 | GET | /getUser | Load authenticated user info | user.js |
| 56 | GET | /logout | Logout user (clear token cookie) | user.js |
| 57 | PUT | /update-user-info | Update user info (requires password) | user.js |
| 58 | PUT | /update-avatar | Update user avatar (authenticated user) | user.js |
| 59 | PUT | /update-user-addresses | Update or add user addresses | user.js |
| 60 | DELETE | /delete-user-address/:id | Delete user address by address id | user.js |
| 61 | PUT | /update-user-password | Update user password (authenticated) | user.js |
| 62 | GET | /user-info/:id | Get user information by id | user.js |
| 63 | GET | /admin-all-users | Admin: list all users | user.js |
| 64 | DELETE | /delete-user/:id | Admin: delete user by id | user.js |
| 65 | POST | /create-withdraw-request | Seller: create withdraw request | withdraw.js |
| 66 | GET | /get-all-withdraw-request | Admin: get all withdraw requests | withdraw.js |
| 67 | PUT | /update-withdraw-request/:id | Admin: update withdraw request (mark succeed) | withdraw.js |

Notes:
- Some routes require middleware (isSeller, isAuthenticated, isAdmin). See controller files for exact middleware usage.
- Paths are taken verbatim from the controller router definitions.
- If you want the file added directly to the repository, I can create a commit/PR (I’ll need repository push permissions or you can accept the patch). Also can export this as CSV/JSON if preferred.

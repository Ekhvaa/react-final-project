# Tour API Endpoint Documentation

This file documents the HTTP endpoints exposed by the Tour API.

Base local URL:

```text
https://localhost:54937
```

Swagger UI:

```text
https://localhost:54937/swagger
```

OpenAPI JSON:

```text
https://localhost:54937/swagger/v1/swagger.json
```

---

## 1. Authentication rules

The API uses JWT Bearer authentication.

For protected endpoints, send this header:

```http
Authorization: Bearer {accessToken}
```

The login/register/social-login endpoints return both:

```text
access token = short-lived JWT used for normal API calls
refresh token = longer-lived token used to get a new access token
```

Supported roles:

```text
Tourist
TravelAgent
TourGuide
Admin
```

Public registration and Google/Facebook social login create a `Tourist` account by default.
`TravelAgent`, `TourGuide`, and `Admin` accounts must be created by an existing admin through the employee endpoint or seeded through configuration.

---

## 2. Common response models

### AuthResponse

Returned by register, login, Google login, Facebook login, and refresh.

```json
{
  "token": "jwt-access-token",
  "expiresAtUtc": "2026-07-09T12:00:00Z",
  "refreshToken": "refresh-token-value",
  "refreshTokenExpiresAtUtc": "2026-07-23T12:00:00Z",
  "userId": 1,
  "username": "lado123",
  "role": "Tourist",
  "emailConfirmed": true
}
```

### MessageResponse

```json
{
  "message": "Operation completed successfully."
}
```

### PagedResult<T>

Used by paginated search endpoints.

```json
{
  "items": [],
  "totalCount": 0,
  "page": 1,
  "pageSize": 20
}
```

### ProblemDetails error response

The global exception middleware returns `application/problem+json` for handled exceptions.

```json
{
  "type": "about:blank",
  "title": "Bad request",
  "status": 400,
  "detail": "Error message here.",
  "instance": "/api/example"
}
```

Typical status codes:

```text
400 Bad Request       Validation or business rule error
401 Unauthorized      Invalid credentials or missing/invalid token
403 Forbidden         Authenticated user has no permission
404 Not Found         Resource not found
409 Conflict          Duplicate resource
500 Server Error      Unexpected backend error
```

---

## 3. Default and health endpoints

### GET /

Redirects to Swagger.

Authorization: public.

Response:

```text
302 redirect to /swagger
```

---

### GET /api

Returns basic API info.

Authorization: public.

Example response:

```json
{
  "name": "Tour API",
  "status": "running",
  "documentation": "/swagger",
  "health": "/api/health"
}
```

---

### GET /api/health

Checks API/database health.

Authorization: public.

Example response:

```json
{
  "status": "ok",
  "databaseConnected": true
}
```

---

# 4. Auth endpoints

Base route:

```text
/api/auth
```

---

## POST /api/auth/register

Registers a new public user as a `Tourist`.

Authorization: public.

Request body:

```json
{
  "username": "lado123",
  "password": "Test123!",
  "firstName": "Lado",
  "lastName": "Maisuradze",
  "email": "lado.test@example.com",
  "contactPhone": "+995599123456",
  "dateOfBirth": "2002-05-14T00:00:00.000Z",
  "gender": "M",
  "nationalId": "01017012345"
}
```

Important:

```text
gender is a char. Use "M", "F", or another single character your backend accepts. Do not send "Male".
```

Success response: `200 OK`

```json
{
  "token": "jwt-access-token",
  "expiresAtUtc": "2026-07-09T12:00:00Z",
  "refreshToken": "refresh-token-value",
  "refreshTokenExpiresAtUtc": "2026-07-23T12:00:00Z",
  "userId": 1,
  "username": "lado123",
  "role": "Tourist",
  "emailConfirmed": false
}
```

Possible errors:

```text
400 invalid request body
409 duplicate username/email/nationalId
```

---

## POST /api/auth/login

Logs in with username/password.

Authorization: public.

Request body:

```json
{
  "username": "lado123",
  "password": "Test123!"
}
```

Success response: `200 OK`

```json
{
  "token": "jwt-access-token",
  "expiresAtUtc": "2026-07-09T12:00:00Z",
  "refreshToken": "refresh-token-value",
  "refreshTokenExpiresAtUtc": "2026-07-23T12:00:00Z",
  "userId": 1,
  "username": "lado123",
  "role": "Tourist",
  "emailConfirmed": true
}
```

Invalid login response: `401 Unauthorized`

```json
{
  "message": "Invalid username or password."
}
```

---

## POST /api/auth/google

Logs in/registers user through Google.

Authorization: public.

Frontend must first get a Google ID token, then send it to this endpoint.

Request body:

```json
{
  "idToken": "google-id-token-here"
}
```

Success response: `200 OK`

```json
{
  "token": "jwt-access-token",
  "expiresAtUtc": "2026-07-09T12:00:00Z",
  "refreshToken": "refresh-token-value",
  "refreshTokenExpiresAtUtc": "2026-07-23T12:00:00Z",
  "userId": 1,
  "username": "lado",
  "role": "Tourist",
  "emailConfirmed": true
}
```

Notes:

```text
Google/Facebook social users are created as Tourist by default.
The backend verifies the provider token and then issues its own Tour API JWT.
```

---

## POST /api/auth/facebook

Logs in/registers user through Facebook.

Authorization: public.

Frontend must first get a Facebook access token, then send it to this endpoint.

Request body:

```json
{
  "accessToken": "facebook-access-token-here"
}
```

Success response: `200 OK`

```json
{
  "token": "jwt-access-token",
  "expiresAtUtc": "2026-07-09T12:00:00Z",
  "refreshToken": "refresh-token-value",
  "refreshTokenExpiresAtUtc": "2026-07-23T12:00:00Z",
  "userId": 1,
  "username": "lado",
  "role": "Tourist",
  "emailConfirmed": true
}
```

---

## POST /api/auth/refresh

Uses a refresh token to issue a new access token and refresh token.

Authorization: public.

Request body:

```json
{
  "refreshToken": "refresh-token-value"
}
```

Success response: `200 OK`

```json
{
  "token": "new-jwt-access-token",
  "expiresAtUtc": "2026-07-09T13:00:00Z",
  "refreshToken": "new-refresh-token-value",
  "refreshTokenExpiresAtUtc": "2026-07-23T13:00:00Z",
  "userId": 1,
  "username": "lado123",
  "role": "Tourist",
  "emailConfirmed": true
}
```

Possible errors:

```text
400/401 invalid, expired, or revoked refresh token
```

---

## POST /api/auth/logout

Revokes a refresh token.

Authorization: authenticated user.

Headers:

```http
Authorization: Bearer {accessToken}
```

Request body:

```json
{
  "refreshToken": "refresh-token-value"
}
```

Success response: `200 OK`

```json
{
  "message": "Logged out successfully."
}
```

---

## POST /api/auth/confirm-email

Confirms a user's email address using an email confirmation token.

Authorization: public.

Request body:

```json
{
  "token": "email-confirmation-token"
}
```

Success response: `200 OK`

```json
{
  "message": "Email confirmed successfully."
}
```

---

## POST /api/auth/forgot-password

Starts password reset flow.

Authorization: public.

Request body:

```json
{
  "email": "lado.test@example.com"
}
```

Success response: `200 OK`

```json
{
  "message": "If the email exists, a password reset link was sent."
}
```

Security note:

```text
The response is intentionally the same whether the email exists or not.
This prevents account enumeration.
```

---

## POST /api/auth/reset-password

Resets password using a reset token.

Authorization: public.

Request body:

```json
{
  "token": "password-reset-token",
  "newPassword": "NewTest123!"
}
```

Success response: `200 OK`

```json
{
  "message": "Password reset successfully."
}
```

---

## POST /api/auth/resend-confirmation

Resends email confirmation link.

Authorization: public.

Request body:

```json
{
  "email": "lado.test@example.com"
}
```

Success response: `200 OK`

```json
{
  "message": "If the email exists, a confirmation link was sent."
}
```

---

# 5. Countries endpoints

Base route:

```text
/api/countries
```

---

## GET /api/countries

Gets all countries.

Authorization: public.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "name": "Georgia",
    "isoName": "GE",
    "flagUrl": "/uploads/countries/georgia-flag.png"
  }
]
```

---

## GET /api/countries/{id}

Gets a country by ID.

Authorization: public.

Route parameters:

```text
id: integer
```

Success response: `200 OK`

```json
{
  "id": 1,
  "name": "Georgia",
  "isoName": "GE",
  "flagUrl": "/uploads/countries/georgia-flag.png"
}
```

Not found response: `404 Not Found`

---

## POST /api/countries

Creates a country.

Authorization: `TravelAgent` or `Admin`.

Headers:

```http
Authorization: Bearer {accessToken}
```

Request body:

```json
{
  "name": "Georgia",
  "isoName": "GE"
}
```

Validation:

```text
name: required, max length 30
isoName: required, exactly 2 characters
```

Success response: `201 Created`

```json
{
  "id": 1,
  "name": "Georgia",
  "isoName": "GE",
  "flagUrl": null
}
```

---

## POST /api/countries/{id}/flag

Uploads or replaces a country flag image. The API stores the file under the configured upload folder and saves only the public file URL in the database.

Authorization: `TravelAgent` or `Admin`.

Headers:

```http
Authorization: Bearer {accessToken}
Content-Type: multipart/form-data
```

Route parameters:

```text
id: integer
```

Form fields:

```text
file: image file
```

Allowed image content types:

```text
image/jpeg
image/png
image/webp
image/gif
```

Success response: `200 OK`

```json
{
  "id": 1,
  "name": "Georgia",
  "isoName": "GE",
  "flagUrl": "/uploads/countries/abc123.png"
}
```

---

# 6. Cities endpoints

Base route:

```text
/api/cities
```

---

## GET /api/cities

Gets cities. Can optionally filter by country.

Authorization: public.

Query parameters:

```text
countryId: integer, optional
```

Example:

```http
GET /api/cities?countryId=1
```

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "name": "Tbilisi",
    "countryId": 1,
    "countryName": "Georgia"
  }
]
```

---

## GET /api/cities/{id}

Gets a city by ID.

Authorization: public.

Route parameters:

```text
id: integer
```

Success response: `200 OK`

```json
{
  "id": 1,
  "name": "Tbilisi",
  "countryId": 1,
  "countryName": "Georgia"
}
```

Not found response: `404 Not Found`

---

## POST /api/cities

Creates a city.

Authorization: `TravelAgent` or `Admin`.

Request body:

```json
{
  "name": "Tbilisi",
  "countryId": 1
}
```

Validation:

```text
name: required, max length 50
countryId: existing country ID
```

Success response: `201 Created`

```json
{
  "id": 1,
  "name": "Tbilisi",
  "countryId": 1,
  "countryName": "Georgia"
}
```

---

# 7. Hotels endpoints

Base route:

```text
/api/hotels
```

---

## GET /api/hotels

Gets hotels. Can optionally filter by city.

Authorization: public.

Query parameters:

```text
cityId: integer, optional
```

Example:

```http
GET /api/hotels?cityId=1
```

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "name": "Tbilisi Grand Hotel",
    "starRating": 5,
    "cityId": 1,
    "cityName": "Tbilisi",
    "countryName": "Georgia",
    "services": [
      {
        "id": 1,
        "name": "Wi-Fi"
      }
    ],
    "images": [
      {
        "id": 1,
        "url": "/uploads/hotels/image.jpg",
        "fileName": "image.jpg",
        "contentType": "image/jpeg",
        "length": 123456,
        "isCover": true
      }
    ]
  }
]
```

---

## GET /api/hotels/{id}

Gets hotel by ID.

Authorization: public.

Success response: `200 OK`

```json
{
  "id": 1,
  "name": "Tbilisi Grand Hotel",
  "starRating": 5,
  "cityId": 1,
  "cityName": "Tbilisi",
  "countryName": "Georgia",
  "services": [
    {
      "id": 1,
      "name": "Wi-Fi"
    }
  ],
  "images": []
}
```

Not found response: `404 Not Found`

---

## GET /api/hotels/services

Gets available hotel services.

Authorization: public.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "name": "Wi-Fi"
  },
  {
    "id": 2,
    "name": "Breakfast"
  }
]
```

---

## POST /api/hotels

Creates a hotel.

Authorization: `TravelAgent` or `Admin`.

Request body:

```json
{
  "name": "Tbilisi Grand Hotel",
  "starRating": 5,
  "cityId": 1,
  "hotelServiceIds": [1, 2, 3]
}
```

Validation:

```text
name: required, max length 50
starRating: 1 to 5
cityId: existing city ID
hotelServiceIds: existing hotel service IDs
```

Success response: `201 Created`

```json
{
  "id": 1,
  "name": "Tbilisi Grand Hotel",
  "starRating": 5,
  "cityId": 1,
  "cityName": "Tbilisi",
  "countryName": "Georgia",
  "services": [
    {
      "id": 1,
      "name": "Wi-Fi"
    }
  ],
  "images": []
}
```

---

## PUT /api/hotels/{id}

Updates a hotel.

Authorization: `TravelAgent` or `Admin`.

Request body:

```json
{
  "name": "Updated Hotel Name",
  "starRating": 4,
  "cityId": 1,
  "hotelServiceIds": [1, 2]
}
```

Success response: `204 No Content`

Not found response: `404 Not Found`

---

## GET /api/hotels/{id}/images

Gets images for a hotel.

Authorization: public.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "url": "/uploads/hotels/image.jpg",
    "fileName": "image.jpg",
    "contentType": "image/jpeg",
    "length": 123456,
    "isCover": true
  }
]
```

---

## POST /api/hotels/{id}/images?isCover=true

Uploads an image for a hotel.

Authorization: `TravelAgent` or `Admin`.

Content type:

```http
multipart/form-data
```

Query parameters:

```text
isCover: boolean, optional
```

Form fields:

```text
file: image file
```

Example form-data:

```text
file = hotel.jpg
```

Success response: `201 Created`

```json
{
  "id": 1,
  "url": "/uploads/hotels/hotel.jpg",
  "fileName": "hotel.jpg",
  "contentType": "image/jpeg",
  "length": 123456,
  "isCover": true
}
```

Possible errors:

```text
400 invalid file / file too large / unsupported content type
404 hotel not found
```

---

# 8. Tours endpoints

Base route:

```text
/api/tours
```

---

## GET /api/tours

Searches tours.

Authorization: public.

Query parameters:

```text
keyword: string, optional
cityId: integer, optional
countryId: integer, optional
minPrice: decimal, optional
maxPrice: decimal, optional
departingAfter: datetime, optional
departingBefore: datetime, optional
sortBy: string, optional
sortDirection: string, optional
page: integer, default 1
pageSize: integer, default 20
```

Example:

```http
GET /api/tours?keyword=Tbilisi&minPrice=100&maxPrice=1000&page=1&pageSize=10&sortBy=price&sortDirection=asc
```

Success response: `200 OK`

```json
{
  "items": [
    {
      "id": 1,
      "code": "GEO-001",
      "name": "Tbilisi and Kazbegi Tour",
      "currentPrice": 799.99,
      "startingCity": "Tbilisi",
      "startingCountry": "Georgia",
      "earliestDeparture": "2026-08-01T10:00:00Z",
      "durationDays": 4,
      "assignedTourGuideId": 3,
      "assignedTourGuideFullName": "Giorgi Beridze"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10
}
```

---

## GET /api/tours/{id}

Gets tour details by ID.

Authorization: public.

Success response: `200 OK`

```json
{
  "id": 1,
  "code": "GEO-001",
  "name": "Tbilisi and Kazbegi Tour",
  "currentPrice": 799.99,
  "assignedTourGuideId": 3,
  "assignedTourGuideFullName": "Giorgi Beridze",
  "itinerary": [
    {
      "id": 1,
      "sequence": 1,
      "cityId": 1,
      "cityName": "Tbilisi",
      "hotelId": 1,
      "hotelName": "Tbilisi Grand Hotel",
      "estimatedArrivalDate": "2026-08-01T10:00:00Z",
      "estimatedDepartureDate": "2026-08-03T10:00:00Z"
    }
  ],
  "images": [
    {
      "id": 1,
      "url": "/uploads/tours/tour.jpg",
      "fileName": "tour.jpg",
      "contentType": "image/jpeg",
      "length": 123456,
      "isCover": true
    }
  ]
}
```

Not found response: `404 Not Found`

---

## POST /api/tours

Creates a tour.

Authorization: `TravelAgent` or `Admin`.

Request body:

```json
{
  "code": "GEO-001",
  "name": "Tbilisi and Kazbegi Tour",
  "currentPrice": 799.99,
  "itinerary": [
    {
      "sequence": 1,
      "cityId": 1,
      "hotelId": 1,
      "estimatedArrivalDate": "2026-08-01T10:00:00.000Z",
      "estimatedDepartureDate": "2026-08-03T10:00:00.000Z"
    },
    {
      "sequence": 2,
      "cityId": 2,
      "hotelId": null,
      "estimatedArrivalDate": "2026-08-03T14:00:00.000Z",
      "estimatedDepartureDate": "2026-08-05T10:00:00.000Z"
    }
  ]
}
```

Validation:

```text
code: required, max length 50, unique
name: required
currentPrice: 0.01 to 999999999
itinerary: at least one leg
itinerary cityId values must exist
hotelId values must exist when provided
arrival/departure dates should be valid business dates
```

Success response: `201 Created`

```json
{
  "id": 1,
  "code": "GEO-001",
  "name": "Tbilisi and Kazbegi Tour",
  "currentPrice": 799.99,
  "assignedTourGuideId": null,
  "assignedTourGuideFullName": null,
  "itinerary": [],
  "images": []
}
```

---

## PUT /api/tours/{id}

Updates a tour.

Authorization: `TravelAgent` or `Admin`.

Ownership rule:

```text
Admin can update any tour.
TravelAgent can update tours according to backend ownership rules.
```

Request body: same shape as create tour.

```json
{
  "code": "GEO-001-UPD",
  "name": "Updated Tbilisi Tour",
  "currentPrice": 899.99,
  "itinerary": [
    {
      "sequence": 1,
      "cityId": 1,
      "hotelId": 1,
      "estimatedArrivalDate": "2026-08-01T10:00:00.000Z",
      "estimatedDepartureDate": "2026-08-03T10:00:00.000Z"
    }
  ]
}
```

Success response: `204 No Content`

Possible errors:

```text
403 forbidden by ownership rule
404 tour not found
409 duplicate tour code
```

---

## PUT /api/tours/{id}/guide

Assigns a tour guide to a tour.

Authorization: `TravelAgent` or `Admin`.

Request body:

```json
{
  "tourGuideId": 3
}
```

Success response: `204 No Content`

Possible errors:

```text
403 forbidden by role/ownership
404 tour or tour guide not found
```

---

## GET /api/tours/{id}/images

Gets images for a tour.

Authorization: public.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "url": "/uploads/tours/tour.jpg",
    "fileName": "tour.jpg",
    "contentType": "image/jpeg",
    "length": 123456,
    "isCover": true
  }
]
```

---

## POST /api/tours/{id}/images?isCover=true

Uploads image for a tour.

Authorization: `TravelAgent` or `Admin`.

Content type:

```http
multipart/form-data
```

Query parameters:

```text
isCover: boolean, optional
```

Form fields:

```text
file: image file
```

Success response: `201 Created`

```json
{
  "id": 1,
  "url": "/uploads/tours/tour.jpg",
  "fileName": "tour.jpg",
  "contentType": "image/jpeg",
  "length": 123456,
  "isCover": true
}
```

---

# 9. Bookings endpoints

Base route:

```text
/api/bookings
```

All booking endpoints require authentication.

Booking status enum:

```text
Pending = 0
Confirmed = 1
Cancelled = 2
Completed = 3
```

JSON can usually send enum numeric values:

```json
{
  "status": 1
}
```

---

## POST /api/bookings

Creates a booking for the currently logged-in tourist.

Authorization: `Tourist`.

Request body:

```json
{
  "tourId": 1,
  "travelAgentId": 2
}
```

Important:

```text
touristId is not sent by frontend.
Backend gets tourist identity from the JWT token.
```

Success response: `201 Created`

```json
{
  "id": 1,
  "tourId": 1,
  "tourCode": "GEO-001",
  "tourName": "Tbilisi and Kazbegi Tour",
  "touristId": 1,
  "touristFullName": "Lado Maisuradze",
  "travelAgentId": 2,
  "travelAgentFullName": "Nino Agent",
  "dateOfBooking": "2026-07-09T12:00:00Z",
  "pricePaid": 799.99,
  "status": 0
}
```

Business rules:

```text
Only tourists can create bookings.
Tourist cannot book a deleted/inactive/past tour.
Duplicate booking should be rejected.
Booking price is locked at creation time in pricePaid.
```

---

## GET /api/bookings/mine

Gets bookings for the currently logged-in tourist.

Authorization: `Tourist`.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "tourId": 1,
    "tourCode": "GEO-001",
    "tourName": "Tbilisi and Kazbegi Tour",
    "touristId": 1,
    "touristFullName": "Lado Maisuradze",
    "travelAgentId": 2,
    "travelAgentFullName": "Nino Agent",
    "dateOfBooking": "2026-07-09T12:00:00Z",
    "pricePaid": 799.99,
    "status": 0
  }
]
```

---

## GET /api/bookings/agent/mine

Gets bookings assigned to the currently logged-in travel agent.

Authorization: `TravelAgent`.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "tourId": 1,
    "tourCode": "GEO-001",
    "tourName": "Tbilisi and Kazbegi Tour",
    "touristId": 1,
    "touristFullName": "Lado Maisuradze",
    "travelAgentId": 2,
    "travelAgentFullName": "Nino Agent",
    "dateOfBooking": "2026-07-09T12:00:00Z",
    "pricePaid": 799.99,
    "status": 0
  }
]
```

---

## GET /api/bookings/agent/{travelAgentId}

Gets bookings for a specific travel agent.

Authorization: `Admin`.

Route parameters:

```text
travelAgentId: integer
```

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "tourId": 1,
    "tourCode": "GEO-001",
    "tourName": "Tbilisi and Kazbegi Tour",
    "touristId": 1,
    "touristFullName": "Lado Maisuradze",
    "travelAgentId": 2,
    "travelAgentFullName": "Nino Agent",
    "dateOfBooking": "2026-07-09T12:00:00Z",
    "pricePaid": 799.99,
    "status": 0
  }
]
```

---

## GET /api/bookings/{id}

Gets a booking by ID for the current user.

Authorization: any authenticated user, but backend applies ownership/role checks.

Rules:

```text
Tourist can see own booking only.
TravelAgent can see assigned bookings.
Admin can see any booking.
```

Success response: `200 OK`

```json
{
  "id": 1,
  "tourId": 1,
  "tourCode": "GEO-001",
  "tourName": "Tbilisi and Kazbegi Tour",
  "touristId": 1,
  "touristFullName": "Lado Maisuradze",
  "travelAgentId": 2,
  "travelAgentFullName": "Nino Agent",
  "dateOfBooking": "2026-07-09T12:00:00Z",
  "pricePaid": 799.99,
  "status": 0
}
```

Possible responses:

```text
403 forbidden if user has no ownership/access
404 not found if booking does not exist
```

---

## PUT /api/bookings/{id}/status

Updates booking status.

Authorization: `TravelAgent` or `Admin`.

Request body:

```json
{
  "status": 1
}
```

Status values:

```text
0 = Pending
1 = Confirmed
2 = Cancelled
3 = Completed
```

Success response: `204 No Content`

Business rules:

```text
TravelAgent can update assigned bookings according to backend ownership rules.
Admin can update any booking.
Invalid status transitions are rejected.
```

Example valid transitions:

```text
Pending -> Confirmed
Pending -> Cancelled
Confirmed -> Cancelled
Confirmed -> Completed
```

Example invalid transitions:

```text
Cancelled -> Confirmed
Completed -> Pending
Completed -> Cancelled
```

---

## PUT /api/bookings/{id}/cancel

Cancels the current tourist's own booking.

Authorization: `Tourist`.

Success response: `204 No Content`

Rules:

```text
Tourist can cancel only own booking.
Cancelled/completed bookings should not be cancelled again.
```

---

# 10. Users / tourist profile endpoints

Base route:

```text
/api/users/me
```

All endpoints require `Tourist` role.

---

## GET /api/users/me

Gets the current tourist profile.

Authorization: `Tourist`.

Success response: `200 OK`

```json
{
  "id": 1,
  "username": "lado123",
  "firstName": "Lado",
  "lastName": "Maisuradze",
  "email": "lado.test@example.com",
  "contactPhone": "+995599123456",
  "dateOfBirth": "2002-05-14T00:00:00Z",
  "gender": "M",
  "nationalId": "01017012345",
  "emailConfirmed": true
}
```

---

## PUT /api/users/me

Updates the current tourist profile.

Authorization: `Tourist`.

Request body:

```json
{
  "firstName": "Lado",
  "lastName": "Maisuradze",
  "email": "new.email@example.com",
  "contactPhone": "+995599999999"
}
```

Validation:

```text
firstName: required, max length 50
lastName: required, max length 50
email: required, valid email, max length 50
contactPhone: required, max length 20
```

Success response: `204 No Content`

---

## GET /api/users/me/history

Gets the current tourist's tour history.

Authorization: `Tourist`.

Success response: `200 OK`

```json
[
  {
    "tourId": 1,
    "tourCode": "GEO-001",
    "tourName": "Tbilisi and Kazbegi Tour",
    "departureDate": "2026-08-01T10:00:00Z",
    "returnDate": "2026-08-05T10:00:00Z"
  }
]
```

---

# 11. Employees endpoints

Base route:

```text
/api/employees
```

All employee endpoints require `Admin` role.

---

## GET /api/employees

Gets all employees.

Authorization: `Admin`.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "username": "agent1",
    "role": "TravelAgent",
    "firstName": "Nino",
    "lastName": "Agent",
    "email": "agent@example.com",
    "contactPhone": "+995599123456",
    "experience": "5 years in tourism"
  }
]
```

---

## GET /api/employees/{id}

Gets employee by ID.

Authorization: `Admin`.

Success response: `200 OK`

```json
{
  "id": 1,
  "username": "agent1",
  "role": "TravelAgent",
  "firstName": "Nino",
  "lastName": "Agent",
  "email": "agent@example.com",
  "contactPhone": "+995599123456",
  "experience": "5 years in tourism"
}
```

Not found response: `404 Not Found`

---

## POST /api/employees

Creates an employee account.

Authorization: `Admin`.

Allowed roles:

```text
TravelAgent
TourGuide
Admin
```

Request body:

```json
{
  "username": "guide1",
  "password": "Test123!",
  "role": "TourGuide",
  "firstName": "Giorgi",
  "lastName": "Beridze",
  "email": "guide@example.com",
  "contactPhone": "+995599123456",
  "dateOfBirth": "1995-01-01T00:00:00.000Z",
  "gender": "M",
  "nationalId": "01017055555",
  "experience": "5 years of tour guiding experience"
}
```

Validation:

```text
username: required, 3-30 chars
password: required, 6-100 chars
role: required, max length 20
firstName: required, max length 50
lastName: required, max length 50
email: required, valid email, max length 50
contactPhone: required, max length 20
gender: char, for example "M" or "F"
nationalId: required, max length 20
experience: optional, max length 200
```

Success response: `201 Created`

```json
{
  "id": 1,
  "username": "guide1",
  "role": "TourGuide",
  "firstName": "Giorgi",
  "lastName": "Beridze",
  "email": "guide@example.com",
  "contactPhone": "+995599123456",
  "experience": "5 years of tour guiding experience"
}
```

Possible errors:

```text
400 invalid role / invalid request
409 duplicate username/email/nationalId
```

---

# 12. Tour guide endpoints

Base route:

```text
/api/tour-guides
```

All endpoints require `TourGuide` role.

---

## GET /api/tour-guides/assigned-tours

Gets tours assigned to the current tour guide.

Authorization: `TourGuide`.

Success response: `200 OK`

```json
[
  {
    "id": 1,
    "code": "GEO-001",
    "name": "Tbilisi and Kazbegi Tour",
    "currentPrice": 799.99,
    "startingCity": "Tbilisi",
    "startingCountry": "Georgia",
    "earliestDeparture": "2026-08-01T10:00:00Z",
    "durationDays": 4,
    "assignedTourGuideId": 3,
    "assignedTourGuideFullName": "Giorgi Beridze"
  }
]
```

---

# 13. Frontend role-based page access

Recommended frontend access rules:

```text
Public:
- home page
- tours list
- tour details
- hotels/cities/countries browsing
- register
- login
- forgot/reset password

Tourist:
- profile
- my bookings
- create booking
- cancel own booking
- tour history

TravelAgent:
- create/update tours
- assign tour guide
- upload tour/hotel images
- create/update hotels
- create countries/cities
- manage assigned bookings

TourGuide:
- view assigned tours

Admin:
- create employees
- view employees
- view agent bookings
- manage everything allowed for TravelAgent
```

---

# 14. Frontend token handling

After login/register/social login, save:

```text
token
refreshToken
expiresAtUtc
refreshTokenExpiresAtUtc
userId
username
role
emailConfirmed
```

For every protected request:

```http
Authorization: Bearer {token}
```

When API returns `401` because the access token expired:

```text
1. call POST /api/auth/refresh with refreshToken
2. store returned new token and refresh token
3. retry original request
4. if refresh fails, redirect to login
```

When API returns `403`:

```text
show "You do not have permission."
```

---

# 15. Example Axios setup

```ts
import axios from "axios";

export const api = axios.create({
  baseURL: "https://localhost:54937"
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});
```

---

# 16. Notes for Swagger multipart upload issue

If Swagger shows:

```text
Failed to load API definition
response status is 500 /swagger/v1/swagger.json
```

and the backend exception points to file upload endpoints, check image upload methods in `ToursController` and `HotelsController`.

For `IFormFile`, prefer this method signature:

```csharp
[HttpPost("{id:int}/images")]
[Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
[RequestSizeLimit(10_000_000)]
[Consumes("multipart/form-data")]
public async Task<ActionResult<ImageDto>> UploadImage(
    int id,
    IFormFile file,
    [FromQuery] bool isCover,
    CancellationToken cancellationToken)
```

Instead of:

```csharp
[FromForm] IFormFile file
```

Also add a null/empty check before reading the stream:

```csharp
if (file is null || file.Length == 0)
{
    return BadRequest("Image file is required.");
}
```

---

# 17. Quick endpoint table

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | Public | Redirects to Swagger |
| GET | `/api` | Public | API info |
| GET | `/api/health` | Public | Health check |
| POST | `/api/auth/register` | Public | Register tourist |
| POST | `/api/auth/login` | Public | Login with username/password |
| POST | `/api/auth/google` | Public | Login/register with Google |
| POST | `/api/auth/facebook` | Public | Login/register with Facebook |
| POST | `/api/auth/refresh` | Public | Refresh JWT |
| POST | `/api/auth/logout` | Authenticated | Revoke refresh token |
| POST | `/api/auth/confirm-email` | Public | Confirm email |
| POST | `/api/auth/forgot-password` | Public | Request password reset |
| POST | `/api/auth/reset-password` | Public | Reset password |
| POST | `/api/auth/resend-confirmation` | Public | Resend confirmation email |
| GET | `/api/countries` | Public | Get countries |
| GET | `/api/countries/{id}` | Public | Get country |
| POST | `/api/countries` | TravelAgent/Admin | Create country |
| GET | `/api/cities` | Public | Get cities |
| GET | `/api/cities/{id}` | Public | Get city |
| POST | `/api/cities` | TravelAgent/Admin | Create city |
| GET | `/api/hotels` | Public | Get hotels |
| GET | `/api/hotels/{id}` | Public | Get hotel |
| GET | `/api/hotels/services` | Public | Get hotel services |
| POST | `/api/hotels` | TravelAgent/Admin | Create hotel |
| PUT | `/api/hotels/{id}` | TravelAgent/Admin | Update hotel |
| GET | `/api/hotels/{id}/images` | Public | Get hotel images |
| POST | `/api/hotels/{id}/images` | TravelAgent/Admin | Upload hotel image |
| GET | `/api/tours` | Public | Search tours |
| GET | `/api/tours/{id}` | Public | Get tour |
| POST | `/api/tours` | TravelAgent/Admin | Create tour |
| PUT | `/api/tours/{id}` | TravelAgent/Admin | Update tour |
| PUT | `/api/tours/{id}/guide` | TravelAgent/Admin | Assign guide |
| GET | `/api/tours/{id}/images` | Public | Get tour images |
| POST | `/api/tours/{id}/images` | TravelAgent/Admin | Upload tour image |
| POST | `/api/bookings` | Tourist | Create booking |
| GET | `/api/bookings/mine` | Tourist | Get own bookings |
| GET | `/api/bookings/agent/mine` | TravelAgent | Get own assigned bookings |
| GET | `/api/bookings/agent/{travelAgentId}` | Admin | Get bookings for agent |
| GET | `/api/bookings/{id}` | Authenticated | Get booking with ownership checks |
| PUT | `/api/bookings/{id}/status` | TravelAgent/Admin | Update booking status |
| PUT | `/api/bookings/{id}/cancel` | Tourist | Cancel own booking |
| GET | `/api/users/me` | Tourist | Get own profile |
| PUT | `/api/users/me` | Tourist | Update own profile |
| GET | `/api/users/me/history` | Tourist | Get own tour history |
| GET | `/api/employees` | Admin | Get employees |
| GET | `/api/employees/{id}` | Admin | Get employee |
| POST | `/api/employees` | Admin | Create employee |
| GET | `/api/tour-guides/assigned-tours` | TourGuide | Get assigned tours |

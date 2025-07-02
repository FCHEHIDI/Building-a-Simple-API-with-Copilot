# API Testing Examples

Below are example commands used to test the API endpoints during development:

## Create a User
```
curl -X POST http://localhost:5247/api/users -H "Authorization: Bearer demo-token-123" -H "Content-Type: application/json" -d '{ "firstName": "Test", "lastName": "User", "email": "test.user@techhive.com", "department": "QA" }'
```

## Get All Users (Authentication Middleware Test)
```
# Without token (should fail):
curl -i http://localhost:5247/api/users
# With invalid token (should fail):
curl -i -H "Authorization: Bearer wrong-token" http://localhost:5247/api/users
# With valid token (should succeed):
curl -i -H "Authorization: Bearer demo-token-123" http://localhost:5247/api/users
```

## Get User by ID
```
curl -H "Authorization: Bearer demo-token-123" http://localhost:5247/api/users/1
```

## Update a User
```
curl -X PUT http://localhost:5247/api/users/1 -H "Authorization: Bearer demo-token-123" -H "Content-Type: application/json" -d '{ "firstName": "TestUpdated", "lastName": "UserUpdated", "email": "test.user@techhive.com", "department": "QA" }'
```

## Delete a User
```
curl -X DELETE http://localhost:5247/api/users/1 -H "Authorization: Bearer demo-token-123"
```

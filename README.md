
# Azure Functions Example - .NET

This repository contains a simple example of Azure Functions built with C# that provides two endpoints:
- `/login`: A mock authentication endpoint that returns a token with a user role.
- `/tasks`: Returns a hardcoded list of tasks, filtered based on the role from the token.

This guide will walk you through setting up and running the solution locally using .NET CLI and the required dependencies.

## Prerequisites

Before you can run the project locally, make sure you have the following installed:

1. **.NET SDK (6.0 or later)**: [Download .NET SDK](https://dotnet.microsoft.com/download)
2. **Azure Functions Tools for Visual Studio Code (VS Code)** or **Azure Functions Core Tools**:
   - For running the functions locally: [Install Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
3. **Visual Studio Code** (Optional but recommended for editing): [Download VS Code](https://code.visualstudio.com/)

---

## Clone the Repository

1. Clone the repository to your local machine:
   ```bash
   git clone https://github.com/your-username/azure-functions-example.git
   cd azure-functions-example
   ```

---

## Set Up the Project Locally

### 1. **Install Required Packages**

Run the following command to install any necessary NuGet packages for the project:
```bash
dotnet restore
```

### 2. **Configure CORS (For Local Testing)**

If you plan to make cross-origin requests, make sure to set up CORS in `local.settings.json`. Here's an example configuration:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "Host": {
    "CORS": "*"
  }
}
```
You can customize the CORS domains by replacing `*` with your allowed origins.

---

## Function Endpoints

### 1. **POST /login**

This endpoint mimics a login system and returns a mock JWT token with a user role. The roles could be `Admin`, `User`

#### Request

- **URL**: `/api/login`
- **Method**: `POST`
- **Body**: A JSON object with `username` and `password`.
  
  Example body:
  ```json
  {
    "username": "user",
    "password": "user123"
  }

    {
    "username": "admin",
    "password": "admin123"
  }
  ```

#### Response

- **Success**: Returns a JSON object with a token and role.

  Example:
  ```json
  {
    "token": "<JWT-TOKEN-HERE>",
    "role": "User"
  }
  ```

- **Failure**: If the username or password is incorrect, a `401 Unauthorized` response is returned.

### 2. **GET /tasks**

This endpoint returns a hardcoded list of tasks, filtered based on the role specified in the JWT token passed in the `Authorization` header.

#### Request

- **URL**: `/api/tasks`
- **Method**: `GET`
- **Headers**:
  - `Authorization: Bearer <JWT-TOKEN-HERE>`

#### Response

- **Admin**: Returns all tasks.
- **User**: Returns a filtered list of tasks based on the user role.


---

## Running the Azure Function Locally

### 1. **Build the Project**

Navigate to the root of your function project and build it using the .NET CLI:
```bash
dotnet build
```
Might need to add this Package 
```bash
dotnet add package System.IdentityModel.Tokens.Jwt
```


### 2. **Run the Function Locally**

Run the function app locally with the following command:
```bash
func start
```

This will start the Azure Functions runtime and the endpoints will be available locally. 
By default, the local environment will be available at `http://localhost:7297`. 

------------------------------------------------------
DONT FORGET TO UPDATE THIS URL IN THE FRONTEND .env if the PORT changes
------------------------------------------------------

### 3. **Accessing Endpoints**

- Open a browser or use a tool like **Postman** to test the endpoints:
  - **Login**: `POST http://localhost:7297/api/login`
  - **Tasks**: `GET http://localhost:7297/api/tasks` (Ensure you pass a valid JWT token in the `Authorization` header)

### 4. **View Logs**

While the function is running, logs will be output to the terminal. You can monitor the logs for debugging.

---

## Troubleshooting

- **Missing Package Errors**: Ensure all required NuGet packages are installed by running `dotnet restore`.
- **CORS Issues**: If CORS issues occur, ensure the `local.settings.json` and `host.json` are properly configured for your environment.
- **JWT Token Errors**: If the JWT token is invalid or expired, regenerate it using valid credentials from the `/login` endpoint.

---

## Conclusion

This Azure Functions example shows how to build a simple authentication system with role-based access and demonstrate API usage in a serverless environment. You can extend this by integrating real authentication systems (e.g., Azure AD, Auth0) and connecting to a database for dynamic task management.

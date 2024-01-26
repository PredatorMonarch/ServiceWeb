# Tasky

## Overview
Provide a brief overview of your project here.

## Microservices

### UserService
This service is responsible for managing user data. It provides endpoints for creating, updating, retrieving, and deleting users. It also handles user authentication.
This service uses Argon2d for hashing password and a random salt for added complexity.
The users are stored in a sqlite database file (see image below).

![image](https://github.com/PredatorMonarch/ServiceWeb/assets/111632331/05423f0a-004b-4923-8a35-f38f05e6a647)

The token column is unused for now.

#### Endpoints
- `GET /api/Users`: Retrieves all users.
- `GET /api/Users/{id}`: Retrieves a specific user by ID.
- `PUT /api/Users/{id}`: Updates a specific user.
- `POST /api/Users/register`: Registers a new user.
- `POST /api/Users/login`: Authenticates a user.
- `DELETE /api/Users/{id}`: Deletes a specific user.

### TaskService
This service is responsible for managing tasks. Each user can create a TaskList, a Tasklist is a list of Todos that the user should do.
It provides endpoints for creating, updating, retrieving, and deleting tasks and individual todos.
This service uses the Neo4j database hosted remotely at neo4j+s://9055bc95.databases.neo4j.io and connect to it via the Neo4j Driver to handle relations between users, tasks lists, and todos. (see image below)

List of users (Limited to 25 in the visualisation) and there relation to task lists: 

![visualisation](https://github.com/PredatorMonarch/ServiceWeb/assets/111632331/bf287a7c-4392-4e91-9f29-6499c8682138)

List of all tasks of all users and their todo childs:

![visualisation (1)](https://github.com/PredatorMonarch/ServiceWeb/assets/111632331/29b4709a-f2e4-42e9-a49c-cbe8f078401a)

The complete view:

![visualisation (2)](https://github.com/PredatorMonarch/ServiceWeb/assets/111632331/e666b3e1-3c76-4232-916e-2916cebfb532)


#### Endpoints
- `GET /api/Tasks/list/{userId}` : Retrieves all tasks lists of the corresponding user
- `POST  /api/Tasks/create/{userId}` : Creates a task list
- `PUT /api/Tasks/update/{userId}/{taskId}` : Updates a task list
- `DELETE /api/Tasks/delete/{userId}/{taskId}` : Deletes a task list and its todos childs
- `GET /api/Tasks/list/{userId}/{taskId}` : Retrieves all todos of the corresponding task and user
- `POST /api/Tasks/create/{userId}/{taskId}` : Creates a todo inside the task
- `PUT /api/Tasks/update/{userId}/{taskId}/{todoId}` : Updates a todo
- `DELETE /api/Tasks/delete/{userId}/{taskId}/{todoId}` : Deletes a todo
- `POST /api/Tasks/{userId}` : Creates the user node in the database
- `DELETE /api/Tasks/{userId}` : Deletes the user node from the database


### GatewayService
This service acts as a gateway for the other services. It routes incoming requests to the appropriate service and aggregates the responses. 
It also handles request authentication and authorization via JWT Tokens.
It also filters the user registration request from invalid usernames and emails.

####Endpoints
- `GET /api/Task` : Retrieves all tasks lists
- `POST /api/Task/create` : Creates a task list
- `PUT /api/Task/update/{taskId}` : Updates a task list
- `DELETE /api/Task/delete/{taskId}` : Deletes a task list and its todos childs
- `GET /api/Task/list/{taskId}` : Retrieves all todos
- `POST /api/Task/create/{taskId}` : Creates a todo
- `PUT /api/Task/update/{taskId}/{todoId}` : Updates a todo
- `DELETE /api/Task/delete/{taskId}/{todoId}` : Deletes a todo


- `POST /api/User/login` : Authentificates a user
- `POST /api/User/register` : Registers a new user
- `PUT /api/User/update` : Updates the user info
- `DELETE /api/User/delete` : Deletes the user
- `GET /api/User/jwt` : Get the client JWT Token, for debug purposes

### FrontService
This service is responsible for the frontend of the application. It is built using a combination of C# and JavaScript, and it interacts with the backend services (UserService, TaskService, GatewayService) to fetch and display data to the user. 
It also handles user inputs and actions, such as form submissions and button clicks, and sends appropriate requests to the backend services.  
This service use a user-friendly interface with a Glassmorphic Design.
See this video for a demo :



#### Technologies Used
- C# dotnet 8.0: Used for server-side logic.
- JavaScript: Used for client-side scripting.
- Blazor: A framework for building interactive client-side web UI with .NET.
- SignalR: Used for real-time web functionality in the application.

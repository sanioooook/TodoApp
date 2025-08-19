# TodoApp

A simple Todo application built with ASP.NET Core and PostgreSQL.

The project includes Swagger for API documentation.  
Once the app is running, you can access Swagger UI at:  
[http://localhost:5000/swagger](http://localhost:5000/swagger)

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/download/) (local installation or Docker)
- [Docker](https://www.docker.com/get-started) (optional, for containerized run)
- [Docker Compose](https://docs.docker.com/compose/) (optional)

---

## 1. Clone and Run with `dotnet`

### Steps

1. **Clone the repository**:
   ```bash
   git clone https://github.com/sanioooook/TodoApp.git
   cd TodoApp/backend/
   ```

2. **Run the application**:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project TodoApp
   ```

3. The application will be available at:
   ```
   http://localhost:5000
   ```

---

## 2. Run in Docker

### Steps

1. **Build the Docker image**:
   ```bash
   docker build -t todo-app .
   ```
   
2. **Run the container**:
   ```bash
   docker run -p 5000:80 todo-app
   ```

---

## 3. Pull from GitHub Packages and Run with Docker Compose

You don't need to clone the entire repository.

Just create a `docker-compose.yml` file anywhere of youre filesystem with the following content:


```yaml
version: '3.9'
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: todo
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    volumes:
      - db_data:/var/lib/postgresql/data
    networks:
      - app_network
  todoapp:
    image: ghcr.io/sanioooook/todo-app:latest
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=todo;Username=postgres;Password=postgres
    depends_on:
      - postgres
    networks:
      - app_network
volumes:
  db_data:
networks:
  app_network:
    driver: bridge
```

Then run:
```bash
docker compose up
```

The application will be available at:  
[http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## Environment Variables

You may set the following environment variables:

- `ConnectionStrings__DefaultConnection` — Database connection string
- `ASPNETCORE_ENVIRONMENT` — Application environment (`Development` or `Production`)

---

## License

This project is licensed under the MIT License.

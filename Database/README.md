# Database Setup Guide

### 1. Extract the Zip File
If the setup comes as a zip file, start by extracting it to a convenient location.

### 2. Locate the Database Script in the Docker Compose File
In your `docker-compose.yml` file, there should be a volume pointing to the database script. This script contains the SQL code for setting up the database.

---

## How to Use the Code

1. **Open Terminal in Project Folder**
   - Navigate to the project folder.
   - Ensure that **Docker Desktop** is running.
   - Run the following command to start the containers:

     ```bash
     docker compose up -d
     ```

   - You should now see the network and container setup in Docker Desktop.

2. **Access the Container Bash Shell**
   - Open a bash shell in the database container by running:

     ```bash
     docker exec -it *container_name_or_id_from_env_file* bash
     ```

     - Example:

       ```bash
       docker exec -it postgres bash
       ```

3. **Connect to the Database in psql**
   - Once inside the container's bash shell, connect to the database with the following command:

     ```bash
     psql -d *db_name_from_env_file* -U *user_from_env_file*
     ```

     - Example:

       ```bash
       psql -d postgres -U postgres
       ```

4. **Verify Tables**
   - To confirm that your tables have been created, type:

     ```sql
     \d
     ```

   - If tables are missing, you may need to restart the container:
     - Go to **Docker Desktop** and fully remove the container.
     - Then, re-add the container to restart the process.

---

With these steps, you should have the database set up and ready for use.

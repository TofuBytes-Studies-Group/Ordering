# Use the official PostgreSQL base image
FROM postgres:latest

# Set environment variables for PostgreSQL
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=postgres
ENV POSTGRES_DB=postgres

# Copy the database dump into the container (if applicable)
COPY order-dump.sql /docker-entrypoint-initdb.d/backup.dump

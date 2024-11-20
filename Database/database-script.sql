CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "Order" (
    "Id" UUID primary key DEFAULT uuid_generate_v4() NOT NULL UNIQUE,
    "RestaurantName" VARCHAR(255) NOT NULL,
    "CustomerName" VARCHAR(255) NOT NULL,
    "CustomerEmail" VARCHAR(255) NOT NULL,
    "CustomerPhoneNumber" INT NOT NULL,
    "CustomerAddress" VARCHAR(255) NOT NULL
);

CREATE TABLE "Orderline" (
    "Id" UUID primary key DEFAULT uuid_generate_v4() NOT NULL UNIQUE,
    "Dish_Id" UUID NOT NULL,
    "Quantity" INT NOT NULL,  
    "Price" INT NOT NULL, 
    "Order_Id" UUID NOT NULL
);

CREATE TABLE "Dish" (
    "Id" UUID primary key NOT NULL UNIQUE,
    "Name" VARCHAR(255) NOT NULL,
    "Price" INT NOT NULL
);

ALTER TABLE "Orderline" ADD CONSTRAINT fk_orderline_order FOREIGN KEY ("Order_Id") REFERENCES "Order"("Id");
ALTER TABLE "Orderline" ADD CONSTRAINT fk_orderline_dish FOREIGN KEY ("Dish_Id") REFERENCES "Dish"("Id");

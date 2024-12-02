CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "Order" (
    "Id" UUID primary key DEFAULT uuid_generate_v4() NOT NULL UNIQUE,
    "CustomerId" UUID NOT NULL,
    "RestaurantId" UUID NOT NULL,
    "CustomerUserName" VARCHAR(255) NOT NULL,
    "TotalPrice" INT NOT NULL 
);

CREATE TABLE "Orderline" (
    "Id" UUID primary key DEFAULT uuid_generate_v4() NOT NULL UNIQUE,
    "DishId" UUID NOT NULL,
    "Quantity" INT NOT NULL,  
    "Price" INT NOT NULL, 
    "OrderId" UUID NOT NULL
);

ALTER TABLE "Orderline" ADD CONSTRAINT fk_orderline_order FOREIGN KEY ("OrderId") REFERENCES "Order"("Id");
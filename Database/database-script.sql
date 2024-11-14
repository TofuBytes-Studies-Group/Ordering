CREATE TABLE "Order" (
    id UUID primary key NOT NULL,
    total_price INT NOT NULL,
    restaurant_name VARCHAR(255) NOT NULL,
    customer_username VARCHAR(255) NOT NULL,
    customer_email VARCHAR(255) NOT NULL,
    customer_phone_number INT NOT NULL,
    customer_address VARCHAR(255) NOT NULL
);

CREATE TABLE "Orderline" (
    id UUID primary key NOT NULL,
    dish_id UUID NOT NULL,
    price INT NOT NULL, 
    order_id UUID NOT NULL
);

CREATE TABLE "Dish" (
    id UUID primary key NOT NULL,
    dish_name VARCHAR(255) NOT NULL,
    price INT NOT NULL
);

ALTER TABLE "Orderline" ADD CONSTRAINT fk_orderline_order FOREIGN KEY (order_id) REFERENCES "Order"(id);
ALTER TABLE "Orderline" ADD CONSTRAINT fk_orderline_dish FOREIGN KEY (dish_id) REFERENCES "Dish"(id);

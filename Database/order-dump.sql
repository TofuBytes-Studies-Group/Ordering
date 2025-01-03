--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1 (Debian 16.1-1.pgdg120+1)
-- Dumped by pg_dump version 16.1 (Debian 16.1-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Order; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Order" (
    "Id" uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    "CustomerId" uuid NOT NULL,
    "RestaurantId" uuid NOT NULL,
    "CustomerUsername" character varying(255) NOT NULL,
    "TotalPrice" integer NOT NULL
);


ALTER TABLE public."Order" OWNER TO postgres;

--
-- Name: Orderline; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Orderline" (
    "Id" uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    "DishId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "Price" integer NOT NULL,
    "OrderId" uuid NOT NULL
);


ALTER TABLE public."Orderline" OWNER TO postgres;

--
-- Data for Name: Order; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Order" ("Id", "CustomerId", "RestaurantId", "CustomerUsername", "TotalPrice") FROM stdin;
24910c81-b9f9-4c52-a08d-d513810ca7e9	06720cec-95f6-4758-b625-f119ecfb53f6	06720cec-95f6-4758-b625-f119ecfb53f6	petezarr	2
\.


--
-- Data for Name: Orderline; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Orderline" ("Id", "DishId", "Quantity", "Price", "OrderId") FROM stdin;
\.


--
-- Name: Order Order_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Order"
    ADD CONSTRAINT "Order_pkey" PRIMARY KEY ("Id");


--
-- Name: Orderline Orderline_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Orderline"
    ADD CONSTRAINT "Orderline_pkey" PRIMARY KEY ("Id");


--
-- Name: Orderline fk_orderline_order; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Orderline"
    ADD CONSTRAINT fk_orderline_order FOREIGN KEY ("OrderId") REFERENCES public."Order"("Id");


--
-- PostgreSQL database dump complete
--



Use Case
Develop the services mentioned below and implement the inter-services communication as per the requirement. 
1.	Product Service
a.	The product will have the following features:
i.	Place order (call order service to place the order. Use gRPC)
ii.	Update order (call order service to update the order. Use gRPC)

Products can be maintained in a JSON file (avoid database) having attributes as id, color, description, price etc.

2.	Order Service
a.	The order service will have a place order feature:
i.	Place order (to be used by product service)
ii.	Publish events for order creation. These events will be consumed by both notification service 1 and notification service 2 (Rabbit MQ use fanout exchange)
b.	The order service will have an update order feature:
i.	Update order (to be used by product service)
ii.	Publish events for order updating. These events will be consumed by notification service 2 only (Rabbit MQ use topic exchange).
3.	Notification service 1
a.	Consume the events raised by the order service for order creation only. The consumer needs to be connected to the rabbit MQ broker.

4.	Notification service 2
a.	Consume the events raised by the order service for order creation and updating. The consumer needs to be connected to the rabbit MQ broker.



How to setup the code?
1. Clone the repository.
2. Make sure the following nuget packages are installed correctly. 
   ![image](https://github.com/LouhanParitosh/GRPCServer/assets/9478372/b93512ce-cad0-491b-b649-6e8abfe424b9)
3. Build the server and client code individually.
4. Run the server code. 3 Command windows will open. 
a. First command window is gRPC server.
b. Second command window is Notification service 1
c. Third command window is Notification service 2
5. Run the client code and order the item. 

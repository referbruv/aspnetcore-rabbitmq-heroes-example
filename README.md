# aspnetcore-rabbitmq-heroes-example
A sample project to explain the integration of RabbitMQ with an ASP.NET Core API.

# What is RabbitMQ?
RabbitMQ is a message-broker that offers multiple messaging protocols and features that help components asynchronously pass data (in the form of messages) among one another. In this article, let's look at how do we setup a RabbitMQ and utilize its message queuing capabilities for queueing data created by a remote producer server and consume it and store it to a database via an AspNetCore API. 

# What the Application comes with
1. Onion Architecture with layers for Contracts, Infra and API separated
2. Swagger UI
3. EF Core implementation with migrations
4. Dapper implementation
5. Repository pattern 
6. SQLite database integration
7. Entire solution is built on the latest .NET 5 project types
8. RabbitMQ ReceiverService implemented as a BackgroundService

# Learn More
The complete explanation and usage of this project is at https://referbruv.com/blog/posts/integrating-rabbitmq-with-aspnet-core-quickstart-with-an-example
# TODO Application

Application is composed of two ASP.NET Core micro-services:

* `Todo.Api` - API where clients can create TODO lists and add items
  to them
* `Todo.Notifications` - SignalR server for push notifications when
  items are added to lists

When new item is added to a list, Todo.Api sends a message to RabbitMQ
queue. This message is then received by Todo.Notifications and pushed
to clients via SignalR.

There is a third .NET project in the solution, the `Todo.Shared`. It
contains the message type used by both Todo.Api and
Todo.Notifications. I used `Rebus` library to isolate RabbitMQ (a
choice I regret but more on that later).

For automated tests there's a `Tests.Integration` project containing
examples of integration tests for some of the API part.

Finally there's a dummy notifications client stub in
`SampleNotificationsClient` project.

## Running It

To demonstrate how the whole system works, there is
`docker-compose.yml` script which makes it possible to build and run
the app. In the solution directory run

```
docker-compose up
```

(docker-compose needs to be installed on the system). This may take a
while first time it's executed to build images for Todo.Api and
Todo.Notifications services, and download RabbitMQ image.

When that's done you can open the browser on
[http://localhost:5000/swagger](http://localhost:5000/swagger) to
access the OpenAPI client for Todo.Api.

Use `POST /Lists` endpoint to create a new TODO list. The response
will contain a Guid which is id of the new list,
e.g. `6bd3226e-afb5-47b2-8666-253be694cd56`.

Build and run SampleNotificationsClient manually (this is a simple
console app and is not built into a Docker image), then run it
providing list id as parameter, e.g.

```
./SampleNotificationsClient 6bd3226e-afb5-47b2-8666-253be694cd56
```

Note that for simplicity I just hardcoded URLs in the notifications
client app.

Next, use the list id to add an item with `POST /Items` (using Swagger
UI). The  SampleNotificationsClient should print out the new item's
contents to the screen.

Since I used `rabbitmq:management-alpine` image, there is also a
management page for RabbitMQ available at
[http://localhost:15672/](http://localhost:15672) (user: guest,
password: guest).

When done run

```
docker-compose down --rmi local -v
```

There's a Makefile containing shortcuts for these commands: `make up`,
`make down` and `make clean` but it only works on environment where
Maketools are available (i.e. Linux).


## Technologies

I used the following technologies:

* **SQLite** for database - for simplicity. The database schema is
  included in `Todo.Api/Data/Scripts`.
* **DBUp** library to create the database schema.
* **Entity Framework** with database first approach to communicate
  with SQLite when serving client requests. Since this example is so
  small, rolling out patterns like CQRS did not make much sense at
  this stage. For larger project, I would consider using Dapper on the
  read side and EF on the write side.
* **Rebus** with **RabbitMQ** for messaging between services.
* **SignalR** for push notifications.
* **JetBrains.Annotations** for `UsedImplicitly` attributes to
  suppress ReSharper / Rider warnings.
* **CSharpFunctionalExtensions** library to minimal extend (using the
  `Result` class to avoid relying on exceptions or nulls).

In tests I additionally used:

* **Dapper** instead of EF, to avoid using production code or setting
  up EF as I consider this to be an overkill.
* **XUnit** testing framework
* **FluentAssertions**
  

## Things Left Undone

For lack of time I consciously ignored some aspects, such as security
of the application. Out of things mentioned in the assignment there
are some left _to do_ (pun intended) as well.

* Swagger documentation and annotations for endpoints and DTOs are
  incomplete. It would require to add attributes and write XML
  documentation that I would definitely go for if this was a real
  project.
* Logging is pretty much the default console output. I think this is
  not a trivial aspect of development and in a real project a more
  involved solution should be used.

Other simplifications and shortcuts are the following:

* Because Authentication and Authorization were not required I
  completely ignored User accounts. They simply don't exist, the API
  is free-for-all. Even with hard-coded users I would need to
  implement some way of binding requests to specific users, e.g. via
  stored username and password and JWT tokens and Identity. I decided
  it's too much work for this example.
* Because of the above (and lack of time), the Notification service
  has a following simplification: client's subscriptions to TODO lists
  are ephermal, i.e. when connection is lost or closed, client needs
  to re-subscribe to a list by invoking a method on SignalR
  hub. Preserving subscriptions would require implementing a
  persistance layer in this project as well, and I skipped this part.
* Exceptions are generally not handled so the application is not
  bullet proof - I assumed a happy path in most cases.

## Tests

The included tests follow patterns from Vladimir Khorikov's book [Unit
Testing Principles, Practices, and
Patterns](https://www.manning.com/books/unit-testing). Specifically,
since at its current state this is essentially a CRUD application,
with almost no Domain/Business logic, I decided that usage of
integration tests would bring more value to development than unit
tests with mocked dependencies. Generally if this was a real project,
I would introduce unit tests as soon as decision making logic appeared
in the Domain layer, striving to achieve _Imperative Shell, Functional
Core_ where Mocking dependencies is not pervasive.

A somewhat controversial decision was to use "real" database engine to
instantiate testing instance of SQLite. Again this is a pattern from
the book I mentioned, where _private_ database (i.e. not accessed by
other services as in this case) should be a real instance for
integration tests. Also, SQLite made this very easy to implement,
however again the implementation is quick and naive, e.g. if more
testing classes were introduced, running tests in parallel would
require smarter implementation of database cleanup than just deleting
the file (or enforcing running them sequentially).


## Problems and Challenges

Since my objective was to provide a solution that can actually be
executed I've spent plenty of time dealing with services being
independent of each other. Namely, I did not want the Todo.Api to
depend on RabbitMQ being available all the time. This problem surfaced
when running the solution in `docker-compose`. At the time the
Todo.Api and Todo.Notifications were started, RabbitMQ container was
not ready to accept connections. This proved to be difficult to do
with Rebus, and I would evaluate other frameworks such as MassTransit
for a future project. Using `BackgroundService` implementations I
achieved independence from starting order, i.e. Todo.Api can handle
requests even when RabbitMQ is not yet available. The solution is
however naive and not production ready, because as soon as connection
between Todo.Api and RabbitMQ is established, the `BackgroundService`
stops. So if at some point later RabbitMQ goes down, I expect
exceptions, although I haven't tested that scenario. There must be a
better way to do this with Rebus but it will have to wait for another
day.

Note that there is some intentional code duplication between Todo.Api
and Todo.Notifications in the `BackgroundService` implementations
(i.e. `MessageBrokerServicesSetup` and related classes). I assumed
that the two services should not share much code so they potentially
could be developed independently by separate teams, but of course the
code could easily be extracted to a shared library.

If you have any questions or feedback, please let me know.

# Member Portal of Students' Club U Kachničky (Kachna Online)

Is Kachna open? That is the question.

Kachna Online is an application aiming to unify information about the
Student's Club at Brno University of Technology called U Kachničky.
The most crucial element of the system is informing about opening hours of the club,
which are dynamic, since opening the club is pointless without the target
audience knowing about the opening.
Aside from opening hours, the system also maintains a calendar of events
of the Student Union, and allows students to borrow board games from the wide
range available.

The production version is deployed on the server of
[FIT BUT Student Union](https://www.su.fit.vutbr.cz/kachna/).
Documentation of the REST API is available as an OpenAPI document and Swagger UI
[there as well](https://www.su.fit.vutbr.cz/kachna/api/swagger/index.html).
Authentication to the system is done using the Student Union's internal system
*KIS* which makes uses of academic identity federation
[eduID.cz](https://www.eduid.cz/).

## Contributing

Thank you for your interest in contributing to the project!
This section gives some advice on how to navigate the project
and develop it locally.

### Containers

The preferred way of running the project locally for development is using
containers, since it removes the hassle of installing dependencies locally.

Prerequisites:
 - docker
 - docker-compose
 - \[OPTIONAL\] GNU make - the rest of this section will assume its use, however docker-compose commands can be run directly without it.
 - \[OPTIONAL\] Use rootless podman containers with docker-compose for better security. [Setup instructions](https://fedoramagazine.org/use-docker-compose-with-podman-to-orchestrate-containers-on-fedora/)

Most common docker-compose commands useful for development are wrapped
in a Makefile in case you are not familiar with docker-compose.
Running the project for the first time requires setting up the
database:

```
make compose-setup-db
```

Following runs can be performed by:

```
make compose-up
```

This starts the ASP .NET6 API at
[https://localhost:5001/kachna/api](https://localhost:5001/kachna/api),
the interactive Swagger UI can be accessed at
[https://localhost:5001/kachna/api/swagger/index.html](https://localhost:5001/kachna/api/swagger/index.html).
The Angular 12 frontend starts at
[https://localhost:4200/kachna/](https://localhost:4200/kachna/).

The changes in the frontend are automatically applied and the frontend is restarted.
To apply backend changes, the `api` docker-compose service must be restarted:

```
make compose-restart-api
```

Some functionality of the project, e.g. Discord webhooks for notifying about
club state changes or email reminders, require additional configuration, e.g.
mail account credentials. All the available configuration fields are listed
in `KachnaOnline.App/appsettings.json`. The semantics of each field is explained
in `KachnaOnline.Business/Configuration`. To override the configuration in
a container, add your configuration as environment variables to `docker/api.env`
and restart the `api` service. The file already contains an example of overriding
`ConnectionStrings:AppDb`.

The frontend uses Angular's service worker for handling push notifications.
The service worker must be served through an HTTP server to the browser
(`ng serve` does not suffice). To simplify the setup, the backend facilitates
serving the frontend as static content. It must, however, first be built:

```
make compose-build-fe
```

The compiled frontend is then available at
[https://localhost:5001/kachna/](https://localhost:5001/kachna/).
The backend requires VAPID keys to be configured for delivering push notifications.
They can be obtained using the
[npm web-push package](https://www.npmjs.com/package/web-push):

```
web-push generate-vapid-keys
```

### Project Structure

The application consists of a REST API backend built using **.NET 6**
with a **PostgreSQL** database and a PWA frontend built using **Angular 12**.

The backend consists of 5 projects:

- **KachnaOnline.App**: The main project containing implementation of controllers (API endpoints).
- **KachnaOnline.Dto**: Declarations of data structures which are used during communication with the client application.
- **KachnaOnline.Business**: Business layer implementing logic of the application.
- **KachnaOnline.Business.Data**, **KachnaOnline.Data**: Layers providing the business layer with access to data.

The frontend is located at `ClientApp/KachnaOnline` and consists of multiple separated modules,
each corresponding to a logical element of the system (e.g. board games or events).

### Local Setup (not recommended)

In some cases, it may be useful to run the project directly on your local machine,
however it is not recommended due to the difficulty of the setup.
For completeness, this section gives a brief overview of the setup process.

#### Dependencies

- [PostgreSQL](https://www.postgresql.org/download/) with a user and a database setup.
- Runtime environment of [.NET 6](https://dotnet.microsoft.com/download)
- [Angular](https://angular.io/guide/setup-local) ([Node.js](https://nodejs.org/en/download/) environment is required for its functionality along with the *npm* package manager)

#### Running the backend

Before running the backend, it is necessary to adjust the configuration in
`KachnaOnline.App/appsettings.json`. The semantics of each configuration option
is explained in `KachnaOnline.Business/Configuration`. At minimum, the database
connection string must be configured under `ConnectionStrings:AppDb`, for example:

```
Host=127.0.0.1;Database=kachna-online;Username=[username];Password=[password]
```

The application must be run inside the main project, `KachnaOnline.App`.

Before running application for the first time, it is necessary to perform database
migrations. The database can also be filled with sample data:

```
dotnet run - --migrate-db --bootstrap-db
```

Afterwards, the application can be run in regular mode:

```
dotnet run
```

#### Running the frontend

The frontend must be run from the `ClientApp/KachnaOnline` directory.

Before running frontend for the first time, it is necessary to install the
dependencies:

```
npm install
```

By default, the application is configured to run with SSL (making use of
the development certificate from ASP.NET backend). It may be necessary to
set the certificate up:

```
node aspnetcore-https
```

The development server can now be run:

```
npm run start
```

Alternatively, the development server can be run without SSL enabled:

```
npm run start-http
```

Refer to section about Container setup for URLs to the locally hosted application
(they are identical). An application set up this way won't support push
notifications since Angular service worker requires a fully-featured HTTP server.
The npm *http-server* package can be used for this purpose, instructions for
running it can be found in
[Angular documentation](https://angular.io/guide/service-worker-getting-started#service-worker-in-action-a-tour).

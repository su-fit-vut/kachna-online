# Member Portal of Students' Club U Kachničky (Kachna Online)

Is Kachna open? That is the question.

Kachna Online is an application aiming to unify information about the
students' club at Brno University of Technology called U Kachničky.
The most crucial element of the system is informing about opening hours of the club,
which are dynamic, since opening the club is pointless without the target
audience knowing about it.
Aside from opening hours, the system also maintains a calendar of events
of the Students' Union, and allows students to borrow board games from the club.

The production version is deployed on the
[FIT BUT Students' Union](https://www.su.fit.vut.cz/kachna/) server.
Documentation of the REST API is available as an OpenAPI document and Swagger UI
[there as well](https://su.fit.vut.cz/kachna/api/swagger/index.html).
Authentication to the system is done using the Student Union's internal system
*KIS* which makes use of an academic identity federation
[eduID.cz](https://www.eduid.cz/).

## Contributing

Thank you for your interest in contributing to the project!
This section gives some advice on how to navigate the project
and develop it locally.

### Containers

The preferred way of running the project locally for development is using
containers, since it removes the hassle of installing dependencies locally.

Two Dockerfile flavours are included: the development files expect you to bind the source code as a volume and allow for hot-reloading of the frontend and backend. The production files are optimised for deployment.

Prerequisites:
 - Docker and Docker Compose
 - \[OPTIONAL\] GNU make - the rest of this section will assume its use, however docker compose commands can be run directly without it.
 - \[OPTIONAL\] Use rootless podman containers with docker compose for better security. [Setup instructions](https://fedoramagazine.org/use-docker-compose-with-podman-to-orchestrate-containers-on-fedora/)
     - When using podman, networking plugins ([_containernetworking-plugins_](https://github.com/containernetworking/plugins);
       [_dnsname_](https://github.com/containers/dnsname), which may be included in a plugins package like `podman-plugins`) must be installed.

For development, it's recommended to pass your local UID and GID to Compose. The default values are 1000:1000.

Most common Compose commands useful for development are wrapped
in a Makefile in case you are not familiar with Compose.
Running the project for the first time requires setting up the
database:

```
make compose-setup-db
```

Following runs can be performed by:

```
make compose-up
```

This starts the ASP.NET API at
[https://localhost:5001/kachna/api](https://localhost:5001/kachna/api),
the interactive Swagger UI can be accessed at
[https://localhost:5001/kachna/api/swagger/index.html](https://localhost:5001/kachna/api/swagger/index.html).
The Angular 12 frontend starts at
[https://localhost:4200/kachna/](https://localhost:4200/kachna/).

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

The application consists of a REST API backend built using **.NET 8**
with a **PostgreSQL** database and a PWA frontend built using **Angular 12**.

The backend consists of 5 projects:

- **KachnaOnline.App**: The main project containing implementation of controllers (API endpoints).
- **KachnaOnline.Dto**: Declarations of data structures which are used during communication with the client application.
- **KachnaOnline.Business**: Business layer implementing logic of the application.
- **KachnaOnline.Business.Data**, **KachnaOnline.Data**: Layers providing the business layer with access to data.

The frontend is located at `ClientApp/KachnaOnline` and consists of multiple separated modules,
each corresponding to a logical element of the system (e.g. board games or events).

### Local Setup

In some cases, it may be useful to run the project directly on your local machine (however it is not recommended due to the difficulty of the setup).
For completeness, this section gives a brief overview of the setup process.

#### Dependencies

- [PostgreSQL](https://www.postgresql.org/download/) with a user and a database setup.
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
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
dotnet run -- --migrate-db --bootstrap-db
```

Afterwards, the application can be run in regular mode:

```
dotnet run
```

You can also use `dotnet watch` instead to enable hot-reloading.

A `wwwroot` directory must be present in the `KachnaOnline.App` directory. In this repository, this is a symlink
to `ClientApp/KachnaOnline/dist/KachnaOnline`, so either build the client app (`npm build`) or simply create
the target directory. On Windows, you'll have to remove the 'symlink file' and replace it with an actual directory.

#### Running the frontend

The frontend must be run from the `ClientApp/KachnaOnline` directory.

Before running frontend for the first time, it is necessary to install the
dependencies:

```
npm install
```

The project uses quite old version of Angular and stuff. These days, you may need to use this:

```
export NODE_OPTIONS=--openssl-legacy-provider
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


## Handling dates and times

[In timezones lies madness.](https://www.youtube.com/watch?v=-5wpm-gesOY) The API will
always return correct UTC times in the ISO 8601 format (e.g. `2024-08-01T13:45:00Z`, mind the trailing Z).
Similarly, it will expect all the datetime inputs to be in UTC. The frontend must handle the conversion
between the user's timezone and UTC.

Internally, the dates are always converted to system local time. They are also stored in the database
in local time using PostgreSQL's `timestamp without time zone` type. The database should use the same
timezone as the application (using the `TimeZone` connnection string parameter should suffice). See
[Npgsql's](https://www.npgsql.org/doc/types/datetime.html) and
[PostgreSQL's](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-TIMEZONES) docs
for more information on how timezones are handled.

# BestCard

## Unique Feature: Gifting packages

Added endpoint "/gift" which expects a UserId as parameter. { "Username": "admin"}

Adds a package to this user and substracts 5 credits from session user.

## Protocol

### 17.09.2022: GameLogic Setup

 -Working Interface ICard

- Working All Monster types
- Working Spell type
- Working simple test script to try out battle
- Working CardDeck class
  - Missing:	A way of knowing which deck ( =Player) won,
    so cards can be exchanged
- Approx. 3h

### 3-5.10.2022: Networking und DB Setup

- DB Setup => can connect to databse
- Networking: decided to use HttpListener and HttpClient instead of sockets
- inital classes for HttpClientController and HttpServerController
- Approx. 6h

### 10-11.10.2022: Networking ...

- Added working TCP func, then realized we need to use HTTP
- created HTTPRequestHeader from string
- working HTTPResponseHeader from string
- used Interface IHttpHeader for both
- Added some Enums
- Adapted Server to not keep connection with client after successfull req => res
- Swapped Names for pretty much everything
- Decided to implement Routes in RoutingController (Route.cs missing)
- Approx. 10h

### 12.10.2022: Finally working httpServer

- restructure whole project

  - now logic is called Framework
  - Networking is now sub of Framework
  - Card Logic now in Framework under Battle
  - Added Data for Database access/models
  - Added HTTPComponents for structure
- fixed all bugs of HttpMessage Components
- added ToString() to Messages for sending
- working now with postman!
- started adding routes
- a lot of refactoring
- Approx. 5h

### 17.12.2022: Docker DB Setup

- pg_Container and pgAdmin4
- web Interface to connect to localhost:5050
- finally docker user has permissions

  - init.sql creates User and sets Grants
- Access via CShap!

  - Implemented DatabaseController as singleton
- Approx. 14h

### 19.12.2022: Server Refinements and DBAccess cleanup

- Func for Non-Read queries to reduce duplicate code
- Created User class
- Set up /Users endpoint
- Added UserService class
- Approx. 5h

### 31.12.2022: /users and /sessions endpoint

* Now also multi-threaded! Added Async functions
  * also added async keyword to routes
* Json Is now Stored Into actual objects located In data.Models
* Session management now working
* Approx. 15h

### 09.1.2023: Unit tests

* Started working on requirement
* Some tests implemented
* Approx. 5h

### 13.1.2023: Cleanup and CardService

* Cleanup session management and Admin sessions (no login required)
* Added CardService Class
  * No singleton needed?
  * AddPackage from Cards[ ]
  * AcquirePackage
* Approx. 20h

### 17.1.2023: New Routes

* added GET cards
* added GET deck
* added PUT deck
* started working on BattleService Class
* Approx. 10h

### 22.1.2023: Working Battle

* Now working battle.
* Added last unit tests
* fixed a lot of bugs
* Approx. 10h

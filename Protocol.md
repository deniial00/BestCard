# BestCard

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
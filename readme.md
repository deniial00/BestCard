# BestCard: A Monster Card Game

## Structure

* Server.cs Class adds Routes by calling AddRoute() function (passes function to be executed when client connects to Endpoints).
* UserService.cs, CardService.cs, BattleService.cs are used to handle Database functions
* BattleController.cs responsible for lobby and Initating battle when two players match
* CardModel, BattleModel and UserModel are representations of database tables

## Unit Tests

1. BattleTest()
2. AddPlayerToLobbyTest()
3. DatabaseAccessTest()
4. NoSessionTest()
5. CreateUserTest()
6. LoginUserTest()
7. AddPackagesTest()
8. AcquirePackageTest()
9. CheckAdminTest()
10. HashPasswordTest()
11. GenerateTokenTest()
12. GetUsernameTest()
13. UpdateCreditsTest()
14. GetCardsByUserIdTest()
15. SetDeckByUserIdFailTest()
16. SetDeckByUserIdTest()
17. GetDeckByUserIdTest()
18. UserModelConstructorTest()
19. UserCredentialsConstructorTest()
20. ResetDatabaseTest()
21. InitDatabase()

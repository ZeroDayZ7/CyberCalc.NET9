`dotnet run --project CyberCalc.NET9/WPF-CALC-NET-9.csproj`

`dotnet watch run --project CyberCalc.NET9/WPF-CALC-NET-9.csproj`

`dotnet format`

| Komenda                     | Opis                                                         | Przykład                                                    |
| --------------------------- | ------------------------------------------------------------ | ----------------------------------------------------------- |
| `dotnet new`                | Tworzy nowy projekt/rozwiązanie                              | `dotnet new wpf -n MyApp`                                   |
| `dotnet build`              | Kompiluje projekt                                            | `dotnet build MyApp/MyApp.csproj`                           |
| `dotnet run`                | Buduje i uruchamia projekt                                   | `dotnet run --project MyApp/MyApp.csproj`                   |
| `dotnet watch run`          | Uruchamia projekt i automatycznie przebudowuje przy zmianach | `dotnet watch run --project MyApp/MyApp.csproj`             |
| `dotnet clean`              | Usuwa pliki tymczasowe kompilacji                            | `dotnet clean MyApp/MyApp.csproj`                           |
| `dotnet restore`            | Pobiera wszystkie zależności NuGet                           | `dotnet restore`                                            |
| `dotnet test`               | Uruchamia testy jednostkowe                                  | `dotnet test MyApp.Tests/MyApp.Tests.csproj`                |
| `dotnet add package`        | Dodaje paczkę NuGet                                          | `dotnet add package Newtonsoft.Json`                        |
| `dotnet remove package`     | Usuwa paczkę NuGet                                           | `dotnet remove package Newtonsoft.Json`                     |
| `dotnet list package`       | Pokazuje zainstalowane paczki                                | `dotnet list package`                                       |
| `dotnet sln add`            | Dodaje projekt do solution                                   | `dotnet sln MySolution.sln add MyApp/MyApp.csproj`          |
| `dotnet sln remove`         | Usuwa projekt z solution                                     | `dotnet sln MySolution.sln remove MyApp/MyApp.csproj`       |
| `dotnet publish`            | Publikuje projekt gotowy do wdrożenia                        | `dotnet publish MyApp/MyApp.csproj -c Release -o ./publish` |
| `dotnet --info`             | Pokazuje informacje o SDK i środowisku                       | `dotnet --info`                                             |
| `dotnet restore --no-cache` | Wymusza pobranie paczek od nowa                              | `dotnet restore --no-cache`                                 |

---

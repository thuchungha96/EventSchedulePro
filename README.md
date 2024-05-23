
+ Register new account from https://render.com/
+ Create new PosgreSQL
![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/0fe50b6f-3677-48b8-a24c-23b9c6e76032)
![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/e51ae8fb-af0f-4d52-a98e-24a55cf66b53)
+ Copy
 ![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/ea2fe458-1f65-43ba-b78a-bc009f9623a1)
postgres://root:hd72s7wRDeEnOuGTNtiuWi4If1LtY1yI@dpg-cp71886v3ddc73fs8qqg-a.oregon-postgres.render.com/db_bsdb
Example:  connect string C# format postgres//{User Id}:{Password}@{Server}/{Database}
+ Open appsettings.json in source code, find "EventDB", and change it follow prev step
 ![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/a96224d8-45cc-4f30-b102-a684a5b6f2d9)
 "EventDB": "Server={Server};Database={Database};Port=5432;User Id={User Id};Password={Password};"
Example: "Server=dpg-cp71886v3ddc73fs8qqg-a.oregon-postgres.render.com;Database=db_bsdb;Port=5432;User Id=root;Password=hd72s7wRDeEnOuGTNtiuWi4If1LtY1yI;"
+ Visual Studio -> Tools -> Nuget package manager -> Pakage manager console.
  Add-Migration InitialCreate
  ![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/1b1e0cea-e111-458f-8593-11b7cc44a9bb)
  Update-Database
  ![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/e94b5c71-c1a4-4329-833b-afc3789df13d)
=> Done setup Database Posgre.

Setup key api TinyMCE:
+ Open _Layout.cshtml, find
  <script src="https://cdn.tiny.cloud/1/hq5vs4y7vejecx3frt0fsoxiqw9gparwxycvye5luungp5br/tinymce/7/tinymce.min.js" referrerpolicy="origin"></script>
  It format like <script src="https://cdn.tiny.cloud/1/{apikey}/tinymce/7/tinymce.min.js" referrerpolicy="origin"></script>
+ Register new account from https://www.tiny.cloud/get-tiny/
+ Find APIKEY
  ![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/871e5513-4522-44da-834b-bc9cb1cff669)
replace this apikey into <script src="https://cdn.tiny.cloud/1/{apikey}/tinymce/7/tinymce.min.js" referrerpolicy="origin"></script>, and save sourcecode.

+ Upload sourcecode into github.
+ Back to https://render.com/, create new Webservice
 ![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/9501c4a0-b5a0-4c57-a42a-7564e80d30db)
![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/cd1e25ca-82df-4bc9-bbb2-6a28af9684c2)
![image](https://github.com/thuchungha96/SimpleEventSchedule/assets/168333817/0f4ab532-ef85-4834-bd6f-cf01b6f303ee)

Connect to git project from prev step.


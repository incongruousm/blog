
Create a new Class Library `MyProject.Data`.

`PM> Install-Package EntityFramework.SqlServerCompact`

Spin up DbContext class...

```cs
using System.Data.Entity;

public class MyDbContext : DbContext
{
	public MyDbContext() : base("MyDb.Data")
	{
         
	}
}
```
























This comes later...
`PM> Enable-Migrations`
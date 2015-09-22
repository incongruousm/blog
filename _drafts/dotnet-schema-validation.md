# .NET Schema Validation
- Jason Deabill
- incongruousm
- 2014-06-26
- Tech
- published

I ran across a problem the other day the reason for which took me somewhat by suprise. A class in the codebase I work on has the responsibility for validating incoming XML messages against a schema. We noticed that a bad message was failing _after_ the schema check, and it wasn't just a slightly incorrect message, it was a totally incorrect message intended for another part of the system.

Using the awesome [Linqpad](http://www.linqpad.net) I pulled together a trivial example to investigate.

```xml
<?xml version="1.0" encoding="utf-8"?>
<xs:schema targetNamespace="schemas.deabill.net"
           elementFormDefault="qualified"
           xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xs:element name="MyRootElement" type="xs:string" />

</xs:schema>
```

My schema expected a single _MyRootElement_ node (of type string) in the _schemas.deabill.net_ namespace. The validation code looked much like this:

```csharp
var schemaSet = new XmlSchemaSet();
schemaSet.Add(XmlSchema.Read(new StringReader(MySchema), (sender, args) => Console.WriteLine(args.Message)));

var settings = new XmlReaderSettings
   {
      ValidationType = ValidationType.Schema,
      Schemas = schemaSet
   };
settings.ValidationEventHandler += (sender, args) => Console.WriteLine(args.Message);

var reader = XmlReader.Create(new StringReader(MyXml), settings);
while (reader.Read()) {}
```

I've simplified and tweaked a few things but the important function is as-was. Then I validated the following XML...

```xml
<IncorrectRootElement />
```

Despite being the wrong element name and having no namespace at all the validation succeeded. I decided to see how the validator responded is the element was at least in the correct namespace...

```xml
<IncorrectRootElement xmlns="schemas.deabill.net" />
```

> The 'schemas.deabill.net:IncorrectRootElement' element is not declared.

This was more like what I was expecting. A validation failure highlighting the undefined element. Some further digging revealed that the schema validating XML reader in .NET does not consider the bad namespace an error, merely a warning. The upshot being that slightly incorrect messages would fail validation, but totally incorrect messages would fall through.

Personally, I think an unexpected element in an unexpected namespace is very much an error. The default behaviour I'd expect would be to reject anything not in a given schema namespace, but who am I to argue? A quick tweak to the _XMLReaderSettings_ solves the problem.

```csharp
ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
```

Now my spurious XML fails validation as expected.

```xml
<IncorrectRootElement />
```

> Could not find schema information for the element 'IncorrectRootElement'.

You do need to be a bit careful raising validation warnings as it may lead to other (unforeseen) failures, but it's a small gotcha that got me.
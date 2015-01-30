# OrderByString
Extension methods to use strings in LINQ OrderBy and ThenBy methods
Enable the extensions methods by including a reference and a "using OrderByExtensions;" statement

Supports the following formats:
```
    ().OrderBy("Property").ThenBy("OtherProperty");
    ().OrderBy("Property desc").ThenBy("OtherProperty desc");
    ().OrderByDescending("Property").ThenByDescending("OtherProperty");
    ().OrderBy("Property desc, OtherProperty asc");
    ().OrderBy("Property, OtherProperty desc");
 ```
 .OrderBy and .ThenBy default to ascending, OrderByDescending and ThenByDescending default to descending
 
 "asc" or "desc" in the text takes precedence over the default from the command so that
 ```   ().OrderBy("Property asc")```
 does the same thing as
```    ().OrderByDescending("Property asc")```
 
 
